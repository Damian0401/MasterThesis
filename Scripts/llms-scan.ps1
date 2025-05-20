param(
    [string]$configPath
)

$startTime = Get-Date

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
$config = Import-PowerShellDataFile -Path $configPath
ProjectAnalyzer -m $config.LLMS_MODELS -t $config.LLMS_TOKEN -o ./.sarif/llms-result.sarif -e cs,sln,csproj -s bin,obj,.sarif,.config,.sonarqube,.codeql

$endTime = Get-Date
$duration = $endTime - $startTime
$logEntry = "LLMs - Duration: $($duration.ToString())"
Add-Content -Path "logs.txt" -Value $logEntry