# Installing Git Hooks for Secret Protection

## What is a Git Hook?

A Git hook is a script that runs automatically at certain points in the Git workflow. The pre-commit hook runs before each commit to check for issues.

## Pre-Commit Hook: Secret Detection

We've included a pre-commit hook that prevents you from accidentally committing API keys and other secrets.

## Installation

### Windows PowerShell

```powershell
# Navigate to repository root
cd C:\Jobb\EMVReaderSLCard

# Create hooks directory if it doesn't exist
New-Item -ItemType Directory -Force -Path .git\hooks

# Copy the hook
Copy-Item .git-hooks\pre-commit .git\hooks\pre-commit

# Make executable (Git Bash or WSL)
# If you have Git Bash installed:
& 'C:\Program Files\Git\bin\bash.exe' -c 'chmod +x .git/hooks/pre-commit'
```

### Linux / macOS

```bash
# Navigate to repository root
cd /path/to/EMVReaderSLCard

# Copy the hook
cp .git-hooks/pre-commit .git/hooks/pre-commit

# Make executable
chmod +x .git/hooks/pre-commit
```

## What It Does

The hook checks for:

1. **API Keys**: Patterns like `NUGET_API_KEY = "xxxxxxx"`
2. **Tokens**: Any token-like strings
3. **Passwords**: Password assignments
4. **Secret Files**: `.env`, `secrets.txt`, `*.key`, etc.

## How It Works

When you try to commit:

```bash
git commit -m "My changes"
```

The hook will:
- ? **Allow commit** if no secrets detected
- ? **Block commit** if potential secrets found

Example blocked commit:
```
Checking for secrets...
WARNING: Potential secret found!
.env:1:NUGET_API_KEY="abc123def456..."

================================================
Commit blocked: Potential secrets detected!
================================================

If this is a false positive, you can:
1. Review the changes carefully
2. Use: git commit --no-verify (NOT RECOMMENDED)
```

## Bypassing (Not Recommended)

If you're absolutely sure there are no secrets:

```bash
git commit --no-verify -m "My changes"
```

**Warning**: Only use `--no-verify` if you're certain the detected pattern is a false positive!

## Testing the Hook

Test that the hook is working:

```bash
# Create a test file with a fake secret
echo 'NUGET_API_KEY="test123456789012345678"' > test-secret.txt

# Try to commit it
git add test-secret.txt
git commit -m "Test secret detection"

# Should be blocked!

# Clean up
git reset HEAD test-secret.txt
rm test-secret.txt
```

## What to Do If Blocked

1. **Review the file**: Open the flagged file
2. **Check if it's actually a secret**: 
   - Real secret ? Remove it, use environment variable
   - False positive ? Consider rewording
3. **Remove from staging**:
   ```bash
   git reset HEAD <file>
   ```
4. **Fix the issue**
5. **Commit again**

## Maintenance

### Update the Hook

If we update the hook script:

```powershell
# Re-copy the updated version
Copy-Item .git-hooks\pre-commit .git\hooks\pre-commit -Force
```

### Disable Temporarily

```bash
# Rename to disable
mv .git/hooks/pre-commit .git/hooks/pre-commit.disabled

# Rename back to enable
mv .git/hooks/pre-commit.disabled .git/hooks/pre-commit
```

## Additional Protection

### Global .gitignore

Create `~/.gitignore_global`:

```gitignore
# Secret files
.env
.env.local
.env.*.local
secrets.txt
api-keys.txt
*.key
*.pem
*.pfx
*.p12

# NuGet packages
*.nupkg
*.snupkg
```

Configure Git to use it:

```bash
git config --global core.excludesfile ~/.gitignore_global
```

## Troubleshooting

### Hook Not Running

**PowerShell**:
```powershell
# Check if hook exists
Test-Path .git\hooks\pre-commit

# View hook contents
Get-Content .git\hooks\pre-commit
```

**Bash**:
```bash
# Check if hook exists and is executable
ls -la .git/hooks/pre-commit

# Should show: -rwxr-xr-x (executable)
```

### Permission Denied (Windows)

Git on Windows doesn't require executable permission, but the file must exist in `.git/hooks/`.

### Hook Seems Disabled

Check Git config:
```bash
git config --get core.hooksPath
```

If it returns a path, hooks are in that location instead of `.git/hooks/`.

## Best Practices

1. ? Always install pre-commit hooks
2. ? Use environment variables for secrets
3. ? Keep `.env` in `.gitignore`
4. ? Use GitHub Secrets for CI/CD
5. ? Review changes before committing: `git diff --cached`
6. ? Never use `--no-verify` unless absolutely necessary
7. ? Never commit secrets "temporarily"

## Resources

- [Git Hooks Documentation](https://git-scm.com/book/en/v2/Customizing-Git-Git-Hooks)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)
- [Environment Variable Best Practices](./ENV_SETUP_TEMPLATES.md)

## Summary

Installing the pre-commit hook is **highly recommended** to prevent accidental secret commits. It's a simple one-time setup that provides continuous protection.

```powershell
# Quick install (PowerShell):
Copy-Item .git-hooks\pre-commit .git\hooks\pre-commit
```

That's it! You're now protected against accidentally committing secrets. ??
