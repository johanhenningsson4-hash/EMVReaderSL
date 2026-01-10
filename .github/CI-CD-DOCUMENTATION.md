# CI/CD Pipeline Documentation

**Project:** EMVReaderSL  
**Last Updated:** January 10, 2026  
**Status:** ? Active

---

## ?? Overview

This project uses GitHub Actions for continuous integration, automated testing, and deployment. The CI/CD pipeline ensures code quality, automates releases, and publishes NuGet packages.

---

## ?? Workflows

### 1. **CI/CD Pipeline** (`ci-cd.yml`)

**Triggers:**
- Push to `master` or `develop` branches
- Pull requests to `master` or `develop`
- Manual workflow dispatch

**Jobs:**
- **build-and-test**: Builds solution and runs all tests
- **code-quality**: Runs static code analysis
- **security-scan**: Scans for security vulnerabilities

**Artifacts:**
- Build outputs (DLLs, EXEs)
- Test results (TRX format)

**Example:**
```bash
# Triggered automatically on push
git push origin master
```

---

### 2. **NuGet Publishing** (`publish-nuget.yml`)

**Triggers:**
- Version tags (e.g., `v1.0.0`, `v2.1.3`)
- Manual workflow dispatch with version input

**Process:**
1. Extract version from tag
2. Build all projects
3. Create NuGet packages
4. Publish to NuGet.org
5. Create GitHub release with packages

**Usage:**
```bash
# Create and push version tag
git tag -a v2.0.0 -m "Release v2.0.0"
git push origin v2.0.0

# Or manually trigger from GitHub Actions UI
```

---

### 3. **Release Creation** (`release.yml`)

**Triggers:**
- Manual workflow dispatch only

**Inputs:**
- `version`: Release version (e.g., 2.0.0)
- `prerelease`: Whether this is a pre-release

**Process:**
1. Build solution with version
2. Run all tests
3. Create release directory with binaries
4. Generate ZIP archive
5. Create NuGet packages
6. Generate release notes
7. Create Git tag
8. Create GitHub release
9. Publish to NuGet.org (if not pre-release)

**Usage:**
```yaml
# From GitHub Actions UI:
# 1. Go to Actions tab
# 2. Select "Create Release" workflow
# 3. Click "Run workflow"
# 4. Enter version (e.g., 2.0.0)
# 5. Select if pre-release
# 6. Click "Run workflow"
```

---

### 4. **Pull Request Checks** (`pr-checks.yml`)

**Triggers:**
- Pull request opened, synchronized, or reopened

**Jobs:**
- **validate-pr**: Validates PR title format (semantic)
- **build-pr**: Builds and tests PR code
- **code-review**: Automated code review checks
- **size-check**: Warns about large PRs

**PR Title Format:**
```
<type>: <description>

Types:
- feat: New feature
- fix: Bug fix
- docs: Documentation changes
- style: Code style changes
- refactor: Code refactoring
- perf: Performance improvements
- test: Test additions/changes
- chore: Maintenance tasks

Examples:
- feat: Add transaction storage feature
- fix: Resolve card reader timeout issue
- docs: Update README with examples
```

---

## ?? Secrets Configuration

### Required Secrets

Add these secrets in GitHub repository settings (`Settings` ? `Secrets and variables` ? `Actions`):

| Secret Name | Description | How to Get |
|-------------|-------------|------------|
| `NUGET_API_KEY` | NuGet.org API key | [Create at nuget.org](https://www.nuget.org/account/apikeys) |

### Adding Secrets

```bash
# Via GitHub UI:
# 1. Go to repository Settings
# 2. Navigate to Secrets and variables ? Actions
# 3. Click "New repository secret"
# 4. Name: NUGET_API_KEY
# 5. Value: Your NuGet API key
# 6. Click "Add secret"
```

---

## ?? NuGet Package Publishing

### Automatic Publishing

**Via Version Tag:**
```bash
# 1. Update version in .nuspec files
# 2. Commit changes
git add .
git commit -m "chore: Bump version to 2.0.0"

# 3. Create and push tag
git tag -a v2.0.0 -m "Release v2.0.0"
git push origin v2.0.0

# 4. GitHub Actions automatically:
#    - Builds packages
#    - Publishes to NuGet.org
#    - Creates GitHub release
```

### Manual Publishing

```bash
# Via GitHub Actions UI:
# 1. Go to Actions tab
# 2. Select "Publish NuGet Packages"
# 3. Click "Run workflow"
# 4. Enter version number
# 5. Click "Run workflow"
```

---

## ??? Version Tagging

### Semantic Versioning

Format: `v<MAJOR>.<MINOR>.<PATCH>`

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Examples

```bash
# Bug fix release
git tag -a v2.0.1 -m "Fix: Card reader timeout issue"

# New feature release
git tag -a v2.1.0 -m "Feature: Add transaction storage"

# Breaking change release
git tag -a v3.0.0 -m "Breaking: New API structure"

# Push tag
git push origin v2.0.1
```

---

## ? Status Badges

Add these badges to your README.md:

### Build Status
```markdown
[![Build Status](https://github.com/johanhenningsson4-hash/EMVReaderSL/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/johanhenningsson4-hash/EMVReaderSL/actions)
```

### NuGet Packages
```markdown
[![NuGet EMVCard.Core](https://img.shields.io/nuget/v/EMVCard.Core.svg)](https://www.nuget.org/packages/EMVCard.Core/)
[![NuGet NfcReaderLib](https://img.shields.io/nuget/v/NfcReaderLib.svg)](https://www.nuget.org/packages/NfcReaderLib/)
```

### License
```markdown
[![License](https://img.shields.io/github/license/johanhenningsson4-hash/EMVReaderSL)](LICENSE)
```

### Latest Release
```markdown
[![Latest Release](https://img.shields.io/github/v/release/johanhenningsson4-hash/EMVReaderSL)](https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/latest)
```

---

## ?? Testing

### Running Tests Locally

```bash
# Run all tests
dotnet test EMVCard.Tests\EMVCard.Tests.csproj

# Run tests with coverage
dotnet test EMVCard.Tests\EMVCard.Tests.csproj /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~CardTransactionTests"
```

### CI Test Execution

Tests run automatically on:
- Every push to master/develop
- Every pull request
- Release creation

**Test Reports:**
- Published as workflow artifacts
- Viewable in GitHub Actions UI
- TRX format for Visual Studio integration

---

## ?? Deployment Process

### Standard Release

```bash
# 1. Ensure all tests pass
dotnet test

# 2. Update version numbers
# - EMVCard.Core\EMVCard.Core.nuspec
# - NfcReaderLib\NfcReaderLib.nuspec
# - EMVReaderSL.csproj (AssemblyVersion)

# 3. Commit version changes
git add .
git commit -m "chore: Bump version to 2.0.0"
git push origin master

# 4. Create release via GitHub Actions
# Go to Actions ? Create Release ? Run workflow
```

### Hotfix Release

```bash
# 1. Create hotfix branch
git checkout -b hotfix/2.0.1

# 2. Make fixes and commit
git add .
git commit -m "fix: Critical bug fix"

# 3. Merge to master
git checkout master
git merge hotfix/2.0.1

# 4. Create tag
git tag -a v2.0.1 -m "Hotfix v2.0.1"
git push origin v2.0.1

# 5. Automatic deployment via GitHub Actions
```

---

## ?? Monitoring

### Workflow Status

Monitor workflow status at:
```
https://github.com/johanhenningsson4-hash/EMVReaderSL/actions
```

### Notifications

Configure GitHub notifications:
1. Go to repository Settings
2. Navigate to Notifications
3. Enable notifications for:
   - Workflow failures
   - Pull request reviews
   - Release published

---

## ??? Troubleshooting

### Common Issues

**1. Build Fails**
```yaml
Error: MSBuild not found

Solution:
- Ensure windows-latest runner is used
- Check .NET Framework version compatibility
```

**2. Test Failures**
```yaml
Error: Tests failed during CI

Solution:
- Run tests locally first
- Check test output in Actions artifacts
- Review test logs in workflow summary
```

**3. NuGet Push Fails**
```yaml
Error: 409 Conflict - Package already exists

Solution:
- Version already published to NuGet.org
- Increment version number
- Re-run workflow
```

**4. Tag Already Exists**
```yaml
Error: Tag v2.0.0 already exists

Solution:
# Delete local tag
git tag -d v2.0.0

# Delete remote tag
git push origin :refs/tags/v2.0.0

# Create new tag with incremented version
git tag -a v2.0.1 -m "Release v2.0.1"
```

---

## ?? Workflow Files

| File | Purpose | Trigger |
|------|---------|---------|
| `ci-cd.yml` | Build, test, quality checks | Push, PR |
| `publish-nuget.yml` | Publish NuGet packages | Version tags |
| `release.yml` | Create releases | Manual |
| `pr-checks.yml` | PR validation | Pull requests |

---

## ?? Workflow Dependencies

```
ci-cd.yml
  ?? build-and-test (required)
  ?? code-quality (needs: build-and-test)
  ?? security-scan (needs: build-and-test)

publish-nuget.yml
  ?? Build all projects
  ?? Create NuGet packages
  ?? Publish to NuGet.org
  ?? Create GitHub release

release.yml
  ?? Build solution
  ?? Run tests
  ?? Create release artifacts
  ?? Generate release notes
  ?? Create Git tag
  ?? Create GitHub release
  ?? Publish NuGet packages

pr-checks.yml
  ?? validate-pr
  ?? build-pr
  ?? code-review
  ?? size-check
```

---

## ?? Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [NuGet Publishing Guide](https://docs.microsoft.com/en-us/nuget/create-packages/publish-a-package)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## ? Checklist for Contributors

Before creating a PR:
- [ ] All tests pass locally
- [ ] Code follows project style guidelines
- [ ] PR title follows semantic format
- [ ] Documentation updated if needed
- [ ] No sensitive data in commits

---

**Status:** ? CI/CD Pipeline Configured and Active  
**Last Updated:** January 10, 2026  
**Maintained by:** GitHub Actions
