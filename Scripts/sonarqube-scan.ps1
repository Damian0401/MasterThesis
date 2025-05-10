param(
    [string]$configPath
)

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
$config = Import-PowerShellDataFile -Path $configPath
dotnet tool restore
dotnet sonarscanner begin `
    /k:"$($config.SONARQUBE_PROJECT_NAME)" `
    /d:sonar.host.url="$($config.SONARQUBE_HOST_URL)" `
    /d:sonar.token="$($config.SONARQUBE_ANALYSE_TOKEN)"
dotnet build
dotnet sonarscanner end /d:sonar.token="$($config.SONARQUBE_ANALYSE_TOKEN)"
SonarQubeToSarif -h $config.SONARQUBE_HOST_URL -p demo -t $config.SONARQUBE_API_TOKEN -o ./.sarif/sonarqube-result.sarif