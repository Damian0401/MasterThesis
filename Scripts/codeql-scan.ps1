param(
    [string]$configPath
)

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
$config = Import-PowerShellDataFile -Path $configPath
if (Test-Path $config.CODEQL_DATABASE_NAME)
{
    rm -r $config.CODEQL_DATABASE_NAME
}
dotnet clean
codeql database create $config.CODEQL_DATABASE_NAME --command "dotnet build" --language=csharp
codeql database analyze $config.CODEQL_DATABASE_NAME --format=sarif-latest --output=.sarif/codeql-result.sarif