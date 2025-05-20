import os
import json
import csv
import argparse

def extract_sarif_data(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        sarif_data = json.load(f)

    result_list = []

    for run in sarif_data.get('runs', []):
        driver = run.get('tool', {}).get('driver', {})
        driver_name = driver.get('name', 'Unknown Tool')
        results = run.get('results', [])

        for result in results:
            rule_id = result.get('ruleId', '')
            message = result.get('message', {}).get('text', '')
            location = result.get('locations', [{}])[0].get('physicalLocation', {}).get('region', {})

            result_list.append({
                'Driver Name': driver_name,
                'ruleId': rule_id,
                'message': message,
                'startLine': location.get('startLine', ''),
                'endLine': location.get('endLine', ''),
                'startColumn': location.get('startColumn', ''),
                'endColumn': location.get('endColumn', '')
            })

    return result_list

def generate_csv_report(sarif_dir):
    sarif_files = [f for f in os.listdir(sarif_dir) if f.endswith('.sarif')]
    if not sarif_files:
        return

    report_rows = []
    for sarif_file in sarif_files:
        sarif_path = os.path.join(sarif_dir, sarif_file)
        results = extract_sarif_data(sarif_path)

        tool_result_counts = {}
        for res in results:
            key = (sarif_file, res['Driver Name'])
            tool_result_counts[key] = tool_result_counts.get(key, 0) + 1

        for res in results:
            result_count = tool_result_counts[(sarif_file, res['Driver Name'])]

            report_rows.append({
                'SARIF File': sarif_file,
                'Driver Name': res['Driver Name'],
                'Total Results': result_count,
                'ruleId': res['ruleId'],
                'message': res['message'],
                'startLine': res['startLine'],
                'endLine': res['endLine'],
                'startColumn': res['startColumn'],
                'endColumn': res['endColumn'],
            })

    output_path = os.path.join(sarif_dir, 'sarif_report.csv')
    with open(output_path, 'w', newline='', encoding='utf-8') as csvfile:
        fieldnames = ['SARIF File', 'Driver Name', 'Total Results', 'ruleId', 'message',
                      'startLine', 'endLine', 'startColumn', 'endColumn']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(report_rows)

    print(f"CSV report generated at: {output_path}")

def process_all_subdirectories(base_dir):
    for entry in os.listdir(base_dir):
        subdir_path = os.path.join(base_dir, entry)
        sarif_dir = os.path.join(subdir_path, '.sarif')

        if os.path.isdir(sarif_dir):
            print(f"Processing SARIF directory: {sarif_dir}")
            generate_csv_report(sarif_dir)

def count_characters_in_prompt(prompt_path):
    try:
        with open(prompt_path, 'r', encoding='utf-8') as f:
            return len(f.read())
    except FileNotFoundError:
        return 0

def extract_tool_vulnerabilities(sarif_path):
    with open(sarif_path, 'r', encoding='utf-8') as f:
        sarif_data = json.load(f)

    tool_counts = {}
    for run in sarif_data.get('runs', []):
        tool_name = run.get('tool', {}).get('driver', {}).get('name', 'UnknownTool')
        count = len(run.get('results', []))
        tool_counts[tool_name] = tool_counts.get(tool_name, 0) + count

    return tool_counts

def generate_summary_report(base_dir):
    all_sast_tools = set()
    all_llm_tools = set()
    project_data = {}

    for entry in sorted(os.listdir(base_dir)):
        project_path = os.path.join(base_dir, entry)
        if not os.path.isdir(project_path):
            continue

        sarif_dir = os.path.join(project_path, '.sarif')
        prompt_path = os.path.join(project_path, 'prompt.txt')

        if not os.path.isdir(sarif_dir):
            continue

        char_count = count_characters_in_prompt(prompt_path)
        tool_counts_total = {}

        for sarif_file in os.listdir(sarif_dir):
            if not sarif_file.endswith('.sarif'):
                continue

            sarif_path = os.path.join(sarif_dir, sarif_file)
            tool_counts = extract_tool_vulnerabilities(sarif_path)

            for tool_name, count in tool_counts.items():
                tool_counts_total[tool_name] = tool_counts_total.get(tool_name, 0) + count
                if sarif_file == "llm-result.sarif":
                    all_llm_tools.add(tool_name)
                else:
                    all_sast_tools.add(tool_name)

        project_data[entry] = {
            "NumberOfCharacters": char_count,
            "tools": tool_counts_total
        }

    all_sast_tools = sorted(all_sast_tools)
    all_llm_tools = sorted(all_llm_tools)

    data_rows_full = []
    data_rows_short = []

    for project, data in project_data.items():
        row_full = [project, data["NumberOfCharacters"]]
        row_short = [project, data["NumberOfCharacters"]]

        for tool in all_sast_tools:
            count = data["tools"].get(tool, 0)
            row_full += [count, "", "", "", ""]
            row_short += [count]

        for tool in all_llm_tools:
            count = data["tools"].get(tool, 0)
            row_full += [count, "", "", "", ""]
            row_short += [count]

        row_full += [""]

        data_rows_full.append(row_full)
        data_rows_short.append(row_short)

    header_row_1_full = ["Project", "NumberOfCharacters"]
    header_row_1_full += ["NumberOfVulnerabilitiesSAST"] * (len(all_sast_tools) * 5)
    header_row_1_full += ["NumberOfVulnerabilitiesLLM"] * (len(all_llm_tools) * 5)
    header_row_1_full += ["TotalVulnerabilities"]

    header_row_2_full = ["", ""]
    for tool in all_sast_tools + all_llm_tools:
        header_row_2_full += [tool] * 5
    header_row_2_full += [""]

    header_row_3_full = ["", ""]
    for _ in all_sast_tools + all_llm_tools:
        header_row_3_full += ["Count", "TP", "FP", "FN", "TN"]
    header_row_3_full += [""]

    full_output_path = os.path.join(base_dir, 'summary_sarif_report_full.csv')
    with open(full_output_path, 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(header_row_1_full)
        writer.writerow(header_row_2_full)
        writer.writerow(header_row_3_full)
        writer.writerows(data_rows_full)

    print(f"Full summary report generated at: {full_output_path}")

    header_row_1_short = ["Project", "NumberOfCharacters"]
    header_row_1_short += ["NumberOfVulnerabilities - SAST"] * len(all_sast_tools)
    header_row_1_short += ["NumberOfVulnerabilities - LLM"] * len(all_llm_tools)

    header_row_2_short = ["", ""]
    header_row_2_short += all_sast_tools
    header_row_2_short += all_llm_tools

    short_output_path = os.path.join(base_dir, 'summary_sarif_report_short.csv')
    with open(short_output_path, 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(header_row_1_short)
        writer.writerow(header_row_2_short)
        writer.writerows(data_rows_short)

    print(f"Short summary report generated at: {short_output_path}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Process SARIF files and generate reports.")
    parser.add_argument('--path', type=str, required=True, help='Base directory path containing SARIF subdirectories')

    args = parser.parse_args()
    base_directory = args.path

    if not os.path.isdir(base_directory):
        print(f"Error: The path '{base_directory}' is not a valid directory.")
        exit(1)

    process_all_subdirectories(base_directory)
    generate_summary_report(base_directory)