# Build the project
msbuild .\EMVCard.Core.csproj /p:Configuration=Release

# Pack the NuGet package
msbuild .\EMVCard.Core.csproj /p:Configuration=Release /t:Pack

# Find the .nupkg file
$nupkg = Get-ChildItem -Path ".\bin\Release" -Filter "EMVCard.Core.*.nupkg" | Select-Object -First 1

if (-not $nupkg) {
    Write-Error "No EMVCard.Core .nupkg file found in .\bin\Release"
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