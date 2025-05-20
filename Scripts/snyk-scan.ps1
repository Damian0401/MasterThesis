$startTime = Get-Date

New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
snyk code test --sarif-file-output=./.sarif/snyk-result.sarif

$endTime = Get-Date
$duration = $endTime - $startTime
$logEntry = "Snyk - Duration: $($duration.ToString())"
Add-Content -Path "logs.txt" -Value $logEntry