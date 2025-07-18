import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

info_df = pd.read_csv('projects.csv')
runtime_df = pd.read_csv('time.csv') 

def convert_time(t):
    return float(t.strip().replace('s', ''))

for col in runtime_df.columns[1:]:
    runtime_df[col] = runtime_df[col].apply(convert_time)

merged_df = pd.merge(info_df, runtime_df, on='Project')

tools = ['SonarQube', 'CodeQL', 'SnykCode', 'GPT 4.1', 'Mistral Large', 'DeepSeek V3']

correlations = {
    tool: merged_df['Number of characters'].corr(merged_df[tool])
    for tool in tools
}

print("Correlation between runtime and number of characters:")
for tool, corr in correlations.items():
    print(f"{tool}: {corr:.3f}")

sns.set(style="whitegrid")
plt.figure(figsize=(10, 6))

for tool in tools:
    data = pd.DataFrame({
        'Number of characters': merged_df['Number of characters'],
        'Runtime (s)': merged_df[tool]
    })
    sns.regplot(
        data=data,
        x='Number of characters',
        y='Runtime (s)',
        label=tool,
        scatter=False,
        ci=None
    )

plt.title('Regression: Tool Runtime vs. Number of Characters')
plt.xlabel('Number of Characters')
plt.ylabel('Runtime (seconds)')
plt.legend(title='Tool')
plt.tight_layout()
plt.savefig('./plots/runtime_regression_overlay.png')
plt.show()
