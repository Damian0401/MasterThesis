import argparse
import pandas as pd
import numpy as np

def calculate_metrics(count, tp, total_vuln):
    try:
        if pd.isna(total_vuln) or pd.isna(count) or pd.isna(tp):
            return np.nan, np.nan, np.nan, np.nan, np.nan

        fp = count - tp
        fn = total_vuln - tp
        precision = tp / count if count > 0 else 0
        recall = tp / total_vuln if total_vuln > 0 else 0
        f1 = 2 * precision * recall / (precision + recall) if (precision + recall) > 0 else 0

        return fp, fn, precision, recall, f1

    except Exception as e:
        print(f"Error calculating metrics: {e}")
        return np.nan, np.nan, np.nan, np.nan, np.nan

def main():
    parser = argparse.ArgumentParser(description="Fill missing metric fields in CSV.")
    parser.add_argument('--path', required=True, help='Path to input CSV file.')
    args = parser.parse_args()

    df = pd.read_csv(args.path, header=[0,1,2])

    total_vuln_col = df.columns[2]

    count_cols = [col for col in df.columns if col[2] == 'Count']
    tp_cols = [col for col in df.columns if col[2] == 'TP']
    tools = set(c[:2] for c in count_cols) & set(c[:2] for c in tp_cols)

    for tool in tools:
        count_col = (*tool, 'Count')
        tp_col = (*tool, 'TP')
        fp_col = (*tool, 'FP')
        fn_col = (*tool, 'FN')
        prec_col = (*tool, 'Precision')
        recall_col = (*tool, 'Recall')
        f1_col = (*tool, 'F1')

        required_cols = [count_col, tp_col, fp_col, fn_col, prec_col, recall_col, f1_col]
        if not all(col in df.columns for col in [count_col, tp_col]):
            print(f"Skipping tool {tool} due to missing Count or TP columns.")
            continue

        existing_metric_cols = [col for col in [fp_col, fn_col, prec_col, recall_col, f1_col] if col in df.columns]

        for idx, row in df.iterrows():
            count = row[count_col]
            tp = row[tp_col]
            total_vuln = row[total_vuln_col]
            fp, fn, prec, recall, f1 = calculate_metrics(count, tp, total_vuln)

            if fp_col in existing_metric_cols and (pd.isna(row[fp_col]) or row[fp_col] == ''):
                df.at[idx, fp_col] = fp
            if fn_col in existing_metric_cols and (pd.isna(row[fn_col]) or row[fn_col] == ''):
                df.at[idx, fn_col] = fn
            if prec_col in existing_metric_cols and (pd.isna(row[prec_col]) or row[prec_col] == ''):
                df.at[idx, prec_col] = prec
            if recall_col in existing_metric_cols and (pd.isna(row[recall_col]) or row[recall_col] == ''):
                df.at[idx, recall_col] = recall
            if f1_col in existing_metric_cols and (pd.isna(row[f1_col]) or row[f1_col] == ''):
                df.at[idx, f1_col] = f1

    df.columns = pd.MultiIndex.from_tuples(
        [(col[0], "", "") if i < 3 else col for i, col in enumerate(df.columns)]
    )

    df.to_csv(args.path, index=False)
    print(f"Filled missing fields and saved back to {args.path}")

if __name__ == "__main__":
    main()
