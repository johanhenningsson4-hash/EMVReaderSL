# CI/CD Setup - Quick Start Guide

**Get your CI/CD pipeline running in 5 minutes!**

---

## ? Prerequisites

- [x] GitHub repository created
- [x] Code pushed to GitHub
- [x] NuGet.org account (for package publishing)

---

## ?? Setup Steps

### Step 1: Enable GitHub Actions (1 minute)

GitHub Actions is automatically enabled for public repositories. For private repositories:

1. Go to repository **Settings**
2. Click **Actions** ? **General**
3. Select **Allow all actions and reusable workflows**
4. Click **Save**

? **Done!** GitHub Actions is now enabled.

---

### Step 2: Add NuGet API Key (2 minutes)

1. **Get your NuGet API Key:**
   - Go to [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
   - Click **Create**
   - Name: `GitHub Actions`
   - Select **Push** permission
   - Click **Create**
   - Copy the key immediately (it won't be shown again)

2. **Add to GitHub:**
   - Go to repository **Settings**
   - Navigate to **Secrets and variables** ? **Actions**
   - Click **New repository secret**
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click **Add secret**

? **Done!** NuGet publishing is configured.

---

### Step 3: Verify Workflows (1 minute)

1. Go to **Actions** tab in your repository
2. You should see these workflows:
   - ? CI/CD Pipeline
   - ? Publish NuGet Packages
   - ? Create Release
   - ? Pull Request Checks

3. Click on any workflow to see its configuration

? **Done!** All workflows are ready.

---

### Step 4: Test the Pipeline (1 minute)

**Option A: Push a commit**
```bash
# Make a small change
echo "# Test" >> README.md

# Commit and push
git add README.md
git commit -m "test: Verify CI/CD pipeline"
git push origin master
```

**Option B: Create a pull request**
```bash
# Create a branch
git checkout -b test/ci-cd

# Make changes and push
git add .
git commit -m "test: CI/CD verification"
git push origin test/ci-cd

# Create PR on GitHub
```

**Watch the workflow:**
1. Go to **Actions** tab
2. You'll see the workflow running
3. Wait for it to complete (2-3 minutes)

? **Done!** Your CI/CD pipeline is working!

---

## ?? What Happens Next?

### On Every Push/PR:
- ? Code is built automatically
- ? Tests are run
- ? Results are reported
- ? Build artifacts are saved

### On Version Tag (e.g., v2.0.0):
- ? NuGet packages are created
- ? Published to NuGet.org
- ? GitHub release is created
- ? Binaries are attached to release

---

## ?? Creating Your First Release

### Method 1: Manual Release (Recommended)

1. Go to **Actions** tab
2. Click **Create Release** workflow
3. Click **Run workflow** button
4. Enter version (e.g., `2.0.0`)
5. Select if pre-release
6. Click **Run workflow**

**That's it!** The workflow will:
- Build everything
- Run tests
- Create packages
- Create GitHub release
- Publish to NuGet

### Method 2: Tag Release

```bash
# Create version tag
git tag -a v2.0.0 -m "Release v2.0.0"

# Push tag
git push origin v2.0.0

# Automatic publishing starts immediately
```

---

## ?? Add Status Badges to README

Copy these to your README.md:

```markdown
## Status

[![Build Status](https://github.com/johanhenningsson4-hash/EMVReaderSL/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/johanhenningsson4-hash/EMVReaderSL/actions)
[![NuGet](https://img.shields.io/nuget/v/EMVCard.Core.svg)](https://www.nuget.org/packages/EMVCard.Core/)
[![License](https://img.shields.io/github/license/johanhenningsson4-hash/EMVReaderSL)](LICENSE)
[![Latest Release](https://img.shields.io/github/v/release/johanhenningsson4-hash/EMVReaderSL)](https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/latest)
```

---

## ? Verification Checklist

After setup, verify these work:

### CI/CD Pipeline
- [ ] Push triggers build
- [ ] Tests run automatically
- [ ] Build artifacts are created
- [ ] Test results are published

### Pull Request Checks
- [ ] PR triggers build
- [ ] PR title is validated
- [ ] Test results are commented
- [ ] Large PR warning works

### Release Pipeline
- [ ] Manual release works
- [ ] Binaries are created
- [ ] NuGet packages are published
- [ ] GitHub release is created

---

## ?? Common Issues

### Issue: Build fails with "MSBuild not found"

**Solution:**
- Ensure `runs-on: windows-latest` in workflow
- Check .NET Framework version is correct

### Issue: Tests don't run

**Solution:**
```bash
# Verify test project exists
cd EMVCard.Tests
dotnet test

# If successful, tests should run in CI
```

### Issue: NuGet push fails

**Solution:**
- Check API key is correct
- Verify key has push permission
- Ensure version doesn't already exist on NuGet.org

### Issue: Workflow not triggering

**Solution:**
- Check workflow file is in `.github/workflows/`
- Verify file syntax is correct
- Check branch name matches trigger

---

## ?? Next Steps

1. **Read the full documentation:** `.github/CI-CD-DOCUMENTATION.md`
2. **Customize workflows** to your needs
3. **Add more tests** for better coverage
4. **Set up branch protection** rules
5. **Configure notifications** for workflow failures

---

## ?? Support

**Having issues?**
- Check [GitHub Actions documentation](https://docs.github.com/en/actions)
- Review workflow logs in Actions tab
- Open an issue on GitHub

---

**Setup Time:** 5 minutes  
**Difficulty:** Easy  
**Value:** Huge! ??

**Congratulations! Your CI/CD pipeline is live! ??**
