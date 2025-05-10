New-Item -ItemType Directory -Force -Path '.sarif' | Out-Null
snyk code test --sarif-file-output=./.sarif/snyk-result.sarif
