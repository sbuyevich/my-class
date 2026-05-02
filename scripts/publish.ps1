[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$Framework = "net10.0",
    [switch]$NoClean,
    [switch]$FrameworkDependent,
    [switch]$NoSingleFile
)

$ErrorActionPreference = "Stop"

function Add-DotNetToProcessPath {
    if ([string]::IsNullOrWhiteSpace($env:ProgramFiles)) {
        return
    }

    $dotnetPath = Join-Path $env:ProgramFiles "dotnet"

    if (-not (Test-Path -LiteralPath $dotnetPath)) {
        return
    }

    $pathParts = $env:Path -split ';' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    if ($pathParts -contains $dotnetPath) {
        return
    }

    $env:Path = "$dotnetPath;$env:Path"
}

function Get-DotNetCommand {
    Add-DotNetToProcessPath

    $dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue

    if (-not $dotnetCommand) {
        throw "dotnet was not found. Install the .NET 10 SDK from https://dotnet.microsoft.com/download/dotnet/10.0 and run this script again."
    }

    return $dotnetCommand.Source
}

function Get-InstalledDotNetSdkVersions {
    param([string]$DotNetPath)

    try {
        $sdkLines = & $DotNetPath --list-sdks 2>$null
    }
    catch {
        return @()
    }

    return @(
        $sdkLines | ForEach-Object {
            if ($_ -match '^(?<Version>\d+\.\d+\.\d+)') {
                $Matches.Version
            }
        }
    )
}

function Assert-DotNet10SdkInstalled {
    param([string]$DotNetPath)

    $sdkVersions = Get-InstalledDotNetSdkVersions -DotNetPath $DotNetPath
    $hasDotNet10 = [bool]($sdkVersions | Where-Object { $_ -like "10.*" } | Select-Object -First 1)

    if (-not $hasDotNet10) {
        throw ".NET 10 SDK was not found. Install the SDK from https://dotnet.microsoft.com/download/dotnet/10.0 and run this script again."
    }
}

function Get-NormalizedFullPath {
    param([string]$Path)

    return [System.IO.Path]::GetFullPath($Path).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
}

function Assert-SafeAppOutputPath {
    param(
        [string]$DistRoot,
        [string]$AppOutputPath
    )

    $expectedPath = Get-NormalizedFullPath -Path (Join-Path $DistRoot "app")
    $actualPath = Get-NormalizedFullPath -Path $AppOutputPath

    if ($actualPath -ne $expectedPath) {
        throw "Refusing to clean unexpected publish output path '$actualPath'. Expected '$expectedPath'."
    }
}

function Set-RepoDotNetCliHome {
    param([string]$RepoRoot)

    if (-not [string]::IsNullOrWhiteSpace($env:DOTNET_CLI_HOME)) {
        return
    }

    $dotnetCliHome = Join-Path $RepoRoot ".dotnet-home"
    New-Item -ItemType Directory -Path $dotnetCliHome -Force | Out-Null
    $env:DOTNET_CLI_HOME = $dotnetCliHome
}

function Stop-MyClassWebProcess {
    param([string]$PidPath)

    $processes = @(Get-Process -Name "MyClass.Web" -ErrorAction SilentlyContinue)

    if (-not $processes) {
        return
    }

    foreach ($process in $processes) {
        Write-Host "Stopping existing MyClass.Web process $($process.Id)."
        Stop-Process -Id $process.Id -Force
        Wait-Process -Id $process.Id -Timeout 10 -ErrorAction SilentlyContinue
    }

    if (Test-Path -LiteralPath $PidPath) {
        Remove-Item -LiteralPath $PidPath -ErrorAction SilentlyContinue
    }
}

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
$repoRoot = Split-Path -Parent $scriptRoot
$distRoot = Join-Path $repoRoot "dist"
$projectPath = Join-Path $repoRoot "src\MyClass.Web\MyClass.Web.csproj"
$appOutputPath = Join-Path $distRoot "app"
$pidPath = Join-Path $distRoot "myclass.pid"

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Project file not found at $projectPath."
}

if (-not $FrameworkDependent -and [string]::IsNullOrWhiteSpace($RuntimeIdentifier)) {
    throw "RuntimeIdentifier is required for a self-contained publish."
}

Set-RepoDotNetCliHome -RepoRoot $repoRoot
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

$dotnetPath = Get-DotNetCommand
Assert-DotNet10SdkInstalled -DotNetPath $dotnetPath
Assert-SafeAppOutputPath -DistRoot $distRoot -AppOutputPath $appOutputPath
Stop-MyClassWebProcess -PidPath $pidPath

if (-not $NoClean -and (Test-Path -LiteralPath $appOutputPath)) {
    Write-Host "Cleaning $appOutputPath"
    Remove-Item -LiteralPath $appOutputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $appOutputPath -Force | Out-Null

$publishArgs = @(
    "publish",
    $projectPath,
    "--configuration", $Configuration,
    "--framework", $Framework,
    "--output", $appOutputPath,
    "--nologo"
)

if (-not [string]::IsNullOrWhiteSpace($RuntimeIdentifier)) {
    $publishArgs += @("--runtime", $RuntimeIdentifier)
}

if ($FrameworkDependent) {
    $publishArgs += "--no-self-contained"
}
else {
    $publishArgs += @("--self-contained", "true")
}

$publishSingleFile = if ($NoSingleFile) { "false" } else { "true" }
$publishArgs += "-p:PublishSingleFile=$publishSingleFile"

Write-Host "Publishing $projectPath"
Write-Host "Output: $appOutputPath"

& $dotnetPath @publishArgs

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

$exePath = Join-Path $appOutputPath "MyClass.Web.exe"

if (-not $FrameworkDependent -and -not (Test-Path -LiteralPath $exePath)) {
    throw "Publish completed, but expected executable was not found at $exePath."
}

Write-Host "Published MyClass to $appOutputPath"
