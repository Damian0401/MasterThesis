$startTime = Get-Date

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null

if (Test-Path .codeql)
{
    rm -r .codeql
}

dotnet clean
codeql database create .codeql --command "dotnet build" --language=csharp
codeql database analyze .codeql `
	--format=sarif-latest `
	--output=.sarif/codeql-result.sarif `
	--ram=4096 `
	csharp-security-extended.qls

$endTime = Get-Date
$duration = $endTime - $startTime
$logEntry = "CodeQL - Duration: $($duration.ToString())"
Add-Content -Path "logs.txt" -Value $logEntry