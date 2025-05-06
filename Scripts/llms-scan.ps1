param(
    [string]$configPath
)

$config = Import-PowerShellDataFile -Path $configPath
ProjectAnalyzer -m $config.LLMS_MODELS -t $config.LLMS_TOKEN -o ./.sarif/llms-result.sarif -s bin,obj,.sarif,.config,.sonarqube