param(
    [string]$configPath
)

$startTime = Get-Date

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
$config = Import-PowerShellDataFile -Path $configPath
dotnet tool install dotnet-sonarscanner
dotnet tool restore
dotnet sonarscanner begin `
    /k:"$($config.SONARQUBE_PROJECT_NAME)" `
    /d:sonar.host.url="$($config.SONARQUBE_HOST_URL)" `
    /d:sonar.token="$($config.SONARQUBE_ANALYSE_TOKEN)" `
    /d:sonar.scanner.scanAll=false
dotnet build
dotnet sonarscanner end /d:sonar.token="$($config.SONARQUBE_ANALYSE_TOKEN)"
SonarQubeToSarif -h $config.SONARQUBE_HOST_URL `
    -p $config.SONARQUBE_PROJECT_NAME `
    -t $config.SONARQUBE_API_TOKEN `
    -i false `
    -o ./.sarif/sonarqube-result.sarif

$endTime = Get-Date
$duration = $endTime - $startTime
$logEntry = "SonarQube - Duration: $($duration.ToString())"
Add-Content -Path "logs.txt" -Value $logEntry