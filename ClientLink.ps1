param (
    [string]$ClientDir,
    [string]$ServerDir
)

# Check if the script is running on Windows
if ($IsWindows -eq $false) {
    Write-Host "This script must be run on Windows."
    exit 1
}

# Validate if the arguments are provided
if (-not $ClientDir -or -not $ServerDir) {
    Write-Host "Usage: .\create_virtual_directory.ps1 -ClientDir 'C:\path\to\client\directory' -ServerDir 'C:\path\to\server\directory'"
    exit 1
}

# Define the Source directory within the client directory
$sourceDir = Join-Path -Path $ClientDir -ChildPath "Source"

# Check if the Source directory exists
if (-not (Test-Path $sourceDir -PathType Container)) {
    Write-Host "Source directory does not exist in the client directory: $sourceDir"
    exit 1
}

# Define the Shared directory within the server directory
$sharedDir = Join-Path -Path $ServerDir -ChildPath "Shared"

# If the Shared directory already exists, remove it
if (Test-Path $sharedDir -PathType Container) {
    Write-Host "Removing existing Shared directory: $sharedDir"
    Remove-Item -Path $sharedDir -Recurse -Force
}

# Create the junction (link) so that Shared points to Source
Write-Host "Creating a junction so that $sharedDir points to $sourceDir"
$junctionCommand = "cmd /c mklink /J `"$sharedDir`" `"$sourceDir`""
Invoke-Expression $junctionCommand

# Check if the junction was created successfully
if ($LASTEXITCODE -eq 0) {
    Write-Host "Directory junction created successfully."
    Write-Host "$sharedDir now mirrors $sourceDir."
} else {
    Write-Host "Failed to create directory junction."
    exit 1
}
