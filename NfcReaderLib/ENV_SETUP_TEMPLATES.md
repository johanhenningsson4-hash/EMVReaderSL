# Environment Variable Setup Templates

## ?? IMPORTANT: Never commit this file with actual API keys!

This file provides templates for setting environment variables. Copy the commands you need and paste them in your terminal.

---

## Windows PowerShell

### Temporary (Current Session Only)
```powershell
# Set API key for current PowerShell session
$env:NUGET_API_KEY = "paste-your-actual-api-key-here"

# Verify it's set
echo $env:NUGET_API_KEY
```

### Permanent (Current User)
```powershell
# Set permanently for current user
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'paste-your-actual-api-key-here', 'User')

# Restart terminal, then verify
echo $env:NUGET_API_KEY
```

### Permanent (All Users - Requires Admin)
```powershell
# Run PowerShell as Administrator, then:
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'paste-your-actual-api-key-here', 'Machine')

# Restart terminal, then verify
echo $env:NUGET_API_KEY
```

---

## Windows Command Prompt (CMD)

### Temporary (Current Session Only)
```cmd
rem Set API key for current CMD session
set NUGET_API_KEY=paste-your-actual-api-key-here

rem Verify it's set
echo %NUGET_API_KEY%
```

### Permanent (Current User)
```cmd
rem Set permanently for current user
setx NUGET_API_KEY "paste-your-actual-api-key-here"

rem Close and reopen CMD, then verify
echo %NUGET_API_KEY%
```

### Permanent (All Users - Requires Admin CMD)
```cmd
rem Run CMD as Administrator, then:
setx NUGET_API_KEY "paste-your-actual-api-key-here" /M

rem Close and reopen CMD, then verify
echo %NUGET_API_KEY%
```

---

## Linux / macOS (Bash)

### Temporary (Current Session Only)
```bash
# Set API key for current terminal session
export NUGET_API_KEY="paste-your-actual-api-key-here"

# Verify it's set
echo $NUGET_API_KEY
```

### Permanent (Current User - Bash)
```bash
# Add to ~/.bashrc
echo 'export NUGET_API_KEY="paste-your-actual-api-key-here"' >> ~/.bashrc

# Reload configuration
source ~/.bashrc

# Verify
echo $NUGET_API_KEY
```

### Permanent (Current User - Zsh)
```bash
# Add to ~/.zshrc
echo 'export NUGET_API_KEY="paste-your-actual-api-key-here"' >> ~/.zshrc

# Reload configuration
source ~/.zshrc

# Verify
echo $NUGET_API_KEY
```

### Permanent (Current User - Fish)
```fish
# Set universal variable (Fish shell)
set -Ux NUGET_API_KEY "paste-your-actual-api-key-here"

# Verify
echo $NUGET_API_KEY
```

---

## GitHub Actions (Repository Secret)

1. **Navigate to Repository Settings**:
   ```
   https://github.com/johanhenningsson4-hash/EMVReaderSL/settings/secrets/actions
   ```

2. **Click "New repository secret"**

3. **Enter Details**:
   - Name: `NUGET_API_KEY`
   - Value: `paste-your-actual-api-key-here`

4. **Click "Add secret"**

5. **Verify** in workflow file (`.github/workflows/publish-nuget.yml`):
   ```yaml
   env:
     NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
   ```

---

## Visual Studio / VS Code

### Using .env File (Local Development Only)

1. **Create `.env` file** in project root:
   ```env
   NUGET_API_KEY=paste-your-actual-api-key-here
   ```

2. **Add to .gitignore**:
   ```gitignore
   .env
   .env.local
   ```

3. **Load in PowerShell** (if using .env file):
   ```powershell
   # Load .env file into environment
   Get-Content .env | ForEach-Object {
       if ($_ -match '^([^=]+)=(.*)$') {
           [System.Environment]::SetEnvironmentVariable($Matches[1], $Matches[2], 'Process')
       }
   }
   ```

---

## Docker / Container Environments

### Docker Run
```bash
docker run -e NUGET_API_KEY="paste-your-api-key" your-image
```

### Docker Compose
```yaml
# docker-compose.yml
version: '3.8'
services:
  builder:
    image: mcr.microsoft.com/dotnet/sdk:6.0
    environment:
      - NUGET_API_KEY=${NUGET_API_KEY}
```

### Kubernetes Secret
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: nuget-credentials
type: Opaque
data:
  api-key: <base64-encoded-api-key>
```

---

## Verification Commands

### Check if Variable is Set
```powershell
# PowerShell
if ($env:NUGET_API_KEY) { 
    Write-Host "? NUGET_API_KEY is set" 
} else { 
    Write-Host "? NUGET_API_KEY is NOT set" 
}
```

```bash
# Bash
if [ -n "$NUGET_API_KEY" ]; then 
    echo "? NUGET_API_KEY is set"
else 
    echo "? NUGET_API_KEY is NOT set"
fi
```

### View Current Value (Be Careful!)
```powershell
# PowerShell - shows first/last 4 chars only
$key = $env:NUGET_API_KEY
"$($key.Substring(0,4))...$($key.Substring($key.Length-4))"
```

```bash
# Bash - shows first/last 4 chars only
echo "${NUGET_API_KEY:0:4}...${NUGET_API_KEY: -4}"
```

---

## Security Reminders

? **DO**:
- Use environment variables
- Store in GitHub Secrets for CI/CD
- Set appropriate permissions (Push only)
- Set expiration dates
- Rotate keys regularly

? **DON'T**:
- Commit API keys to Git
- Share keys in email/chat
- Use overly broad permissions
- Store in code files
- Echo full key value in logs

---

## Quick Reference

| Platform | Command | Scope |
|----------|---------|-------|
| PowerShell | `$env:NUGET_API_KEY = "key"` | Session |
| PowerShell | `SetEnvironmentVariable(..., 'User')` | Permanent |
| CMD | `set NUGET_API_KEY=key` | Session |
| CMD | `setx NUGET_API_KEY "key"` | Permanent |
| Bash | `export NUGET_API_KEY="key"` | Session |
| Bash | Add to `~/.bashrc` | Permanent |
| GitHub | Repository Secrets | CI/CD |

---

## Getting Your NuGet API Key

1. Visit: https://www.nuget.org/account/apikeys
2. Click "Create"
3. Set name, expiration, and scopes
4. **Copy immediately** - you won't see it again!
5. Use one of the methods above to set it

---

**Remember**: Never commit actual API keys to version control!
