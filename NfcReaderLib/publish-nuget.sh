#!/bin/bash
# NuGet Publishing Script for Linux/macOS
# Uses NUGET_API_KEY environment variable

set -e  # Exit on error

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  NfcReaderLib NuGet Publishing Script${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Check if API key is set
if [ -z "$NUGET_API_KEY" ]; then
    echo -e "${RED}ERROR: NUGET_API_KEY environment variable is not set!${NC}"
    echo ""
    echo "Please set it with:"
    echo "  export NUGET_API_KEY=\"your-api-key-here\""
    echo ""
    echo "Or add to ~/.bashrc for persistence:"
    echo "  echo 'export NUGET_API_KEY=\"your-api-key-here\"' >> ~/.bashrc"
    echo "  source ~/.bashrc"
    exit 1
fi

echo -e "${GREEN}? API key found in environment variable${NC}"
echo ""

# Set variables
PROJECT="NfcReaderLib.csproj"
CONFIG="Release"
OUTPUT="nupkg"

# Clean output directory
if [ -d "$OUTPUT" ]; then
    echo "Cleaning output directory..."
    rm -rf "$OUTPUT"
fi
mkdir -p "$OUTPUT"

# Build
echo "Building project..."
dotnet build "$PROJECT" --configuration "$CONFIG"
echo -e "${GREEN}? Build completed${NC}"
echo ""

# Pack
echo "Packing NuGet package..."
dotnet pack "$PROJECT" --configuration "$CONFIG" --no-build --output "$OUTPUT"
echo -e "${GREEN}? Package created${NC}"
echo ""

# Find package file
PACKAGE=$(ls $OUTPUT/*.nupkg | head -n 1)
echo "Package: $PACKAGE"
echo ""

# Confirm
echo -e "${YELLOW}Ready to publish to NuGet.org${NC}"
read -p "Continue? (Y/N): " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Publishing cancelled."
    exit 0
fi
echo ""

# Push to NuGet
echo "Publishing to NuGet.org..."
dotnet nuget push "$OUTPUT"/*.nupkg \
    --api-key "$NUGET_API_KEY" \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  ? Successfully published to NuGet!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${BLUE}It may take a few minutes to appear in search results.${NC}"
echo -e "${BLUE}Visit: https://www.nuget.org/packages/NfcReaderLib${NC}"
echo ""
