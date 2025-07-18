import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

info_df = pd.read_csv('projects.csv')

results_df = pd.read_csv('results.csv', header=[0, 1, 2])

project_col = ('Project', 'Unnamed: 0_level_1', 'Unnamed: 0_level_2')
vuln_col = ('TotalVulnerabilities', 'Unnamed: 2_level_1', 'Unnamed: 2_level_2')

llm_tools = ['GPT-4.1', 'Mistral-Large', 'DeepSeek-V3']
fp_data = {
    'Project': results_df[project_col],
    'TotalVulnerabilities': results_df[vuln_col]
}

for tool in llm_tools:
    fp_data[tool] = results_df[('LLM', tool, 'FP')]

fp_df = pd.DataFrame(fp_data)

print("Correlation between FP and Total Vulnerabilities:")
for tool in llm_tools:
    corr = fp_df[tool].corr(fp_df['TotalVulnerabilities'])
    print(f"{tool}: {corr:.3f}")

melted = fp_df.melt(id_vars=['Project', 'TotalVulnerabilities'], 
                    value_vars=llm_tools, 
                    var_name='Tool', 
                    value_name='False Positives')

sns.set(style="whitegrid")
plt.figure(figsize=(10, 6))
sns.lmplot(
    data=melted,
    x='TotalVulnerabilities',
    y='False Positives',
    hue='Tool',
    ci=None,
    markers='o',
    aspect=1.5,
    height=6
)

plt.title('False Positives vs. Total Vulnerabilities (LLM Tools)')
plt.xlabel('Total Vulnerabilities')
plt.ylabel('False Positives')
plt.tight_layout()
plt.savefig('./plots/fp_vs_vulnerabilities_llm.png')
# plt.show()