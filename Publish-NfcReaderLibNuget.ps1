# Path to the .nupkg file (adjust if needed)
$nupkg = Get-ChildItem -Path ".\NfcReaderLib\bin\Release" -Filter "NfcReaderLib.*.nupkg" | Select-Object -First 1

if (-not $nupkg) {
    Write-Error "No NfcReaderLib .nupkg file found in .\bin\Release"
    exit 1
}

# Get API key from environment variable
$apiKey = $env:NUGET_API_KEY

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Error "NUGET_API_KEY environment variable is not set."
    exit 1
}

# Push to NuGet.org
.\nuget push $nupkg.FullName -Source https://api.nuget.org/v3/index.json -ApiKey $apiKey -SkipDuplicate