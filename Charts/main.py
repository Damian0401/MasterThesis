import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

desired_order = [
    'SonarQube', 'CodeQL', 'SnykCode',
    'GPT-4.1', 'Mistral-Large', 'DeepSeek-V3'
]

df = pd.read_csv("results.csv", header=[0,1,2])

if ('Project', '', '') in df.columns:
    projects = df[('Project', '', '')]
elif 'Project' in df.columns:
    projects = df['Project']
else:
    raise KeyError("Missing 'Project' column")

f1_columns = [col for col in df.columns if col[-1] == 'F1']
f1_df = df[f1_columns].copy()
f1_df.columns = [f"{col[1]}" for col in f1_columns]
f1_df.insert(0, 'Project', projects)
f1_melted = f1_df.melt(id_vars='Project', var_name='Tool', value_name='F1-score')
f1_melted['Tool'] = pd.Categorical(f1_melted['Tool'], categories=desired_order, ordered=True)

sns.set(style="whitegrid")
sns.set_context("talk")  
plt.figure(figsize=(14, 6))
sns.boxplot(data=f1_melted, x='Tool', y='F1-score')
plt.xticks(rotation=45, ha='right')
plt.title('Comparison of Tool Effectiveness (F1-score)')
plt.ylabel('F1-score')
plt.xlabel('Tool')
plt.tight_layout()
plt.savefig('./plots/f1_score_comparison.png')

precision_columns = [col for col in df.columns if col[-1] == 'Precision']
precision_df = df[precision_columns].copy()
precision_df.columns = [f"{col[1]}" for col in precision_columns]
precision_df.insert(0, 'Project', projects)
precision_melted = precision_df.melt(id_vars='Project', var_name='Tool', value_name='Precision')
precision_melted['Tool'] = pd.Categorical(precision_melted['Tool'], categories=desired_order, ordered=True)

plt.figure(figsize=(14, 6))
sns.boxplot(data=precision_melted, x='Tool', y='Precision')
plt.xticks(rotation=45, ha='right')
plt.title('Comparison of Tool Effectiveness (Precision)')
plt.ylabel('Precision')
plt.xlabel('Tool')
plt.tight_layout()
plt.savefig('./plots/precision_comparison.png')

# Recall plot
recall_columns = [col for col in df.columns if col[-1] == 'Recall']
recall_df = df[recall_columns].copy()
recall_df.columns = [f"{col[1]}" for col in recall_columns]
recall_df.insert(0, 'Project', projects)
recall_melted = recall_df.melt(id_vars='Project', var_name='Tool', value_name='Recall')
recall_melted['Tool'] = pd.Categorical(recall_melted['Tool'], categories=desired_order, ordered=True)

plt.figure(figsize=(14, 6))
sns.boxplot(data=recall_melted, x='Tool', y='Recall')
plt.xticks(rotation=45, ha='right')
plt.title('Comparison of Tool Effectiveness (Recall)')
plt.ylabel('Recall')
plt.xlabel('Tool')
plt.tight_layout()
plt.savefig('./plots/recall_comparison.png')

fp_columns = [col for col in df.columns if col[-1] == 'FP']
fp_df = df[fp_columns].copy()
fp_df.columns = [f"{col[1]}" for col in fp_columns]
fp_df.insert(0, 'Project', projects)
fp_melted = fp_df.melt(id_vars='Project', var_name='Tool', value_name='False positive')
fp_melted['Tool'] = pd.Categorical(fp_melted['Tool'], categories=desired_order, ordered=True)

plt.figure(figsize=(14, 6))
sns.boxplot(data=fp_melted, x='Tool', y='False positive')
plt.xticks(rotation=45, ha='right')
plt.title('Comparison of False Positives by Tool')
plt.ylabel('False Positives')
plt.xlabel('Tool')
plt.tight_layout()
plt.savefig('./plots/fp_comparison.png')

if ('TotalVulnerabilities', '', '') in df.columns:
    total_vuln = df[('TotalVulnerabilities', '', '')]
elif 'TotalVulnerabilities' in df.columns:
    total_vuln = df['TotalVulnerabilities']
else:
    raise KeyError("Missing 'TotalVulnerabilities' column")

fp_df['TotalVulnerabilities'] = total_vuln.values
fp_prop_melted = fp_df.melt(id_vars=['Project', 'TotalVulnerabilities'], var_name='Tool', value_name='False positive')
fp_prop_melted['Tool'] = pd.Categorical(fp_prop_melted['Tool'], categories=desired_order, ordered=True)
fp_prop_melted['FP/TotalVulnerabilities'] = fp_prop_melted['False positive'] / fp_prop_melted['TotalVulnerabilities']

fp_prop_mean = fp_prop_melted.groupby('Tool')['FP/TotalVulnerabilities'].mean().reset_index()
fp_prop_mean['Tool'] = pd.Categorical(fp_prop_mean['Tool'], categories=desired_order, ordered=True)

plt.figure(figsize=(10, 5))
sns.barplot(data=fp_prop_mean, x='Tool', y='FP/TotalVulnerabilities')
plt.xticks(rotation=45, ha='right')
plt.title('Average FP to Total Vulnerabilities Ratio by Tool')
plt.ylabel('Average FP / Total Vulnerabilities')
plt.xlabel('Tool')
plt.tight_layout()
plt.savefig('./plots/fp_proportion_comparison.png')
