param(
    [string]$configPath
)

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
$config = Import-PowerShellDataFile -Path $configPath
ProjectAnalyzer -m $config.LLMS_MODELS -t $config.LLMS_TOKEN -o ./.sarif/llms-result.sarif -s bin,obj,.sarif,.config,.sonarqube,.codeql