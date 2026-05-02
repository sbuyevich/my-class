@echo off
pushd "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0publish.ps1" %*
set "EXIT_CODE=%ERRORLEVEL%"
popd

if not "%EXIT_CODE%"=="0" (
    exit /b %EXIT_CODE%
)

powershell -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference = 'Stop'; $repoRoot = Resolve-Path '%~dp0..'; $distPath = Join-Path $repoRoot 'dist'; $zipPath = Join-Path $repoRoot 'dist.zip'; if (Test-Path -LiteralPath $zipPath) { Remove-Item -LiteralPath $zipPath -Force }; Compress-Archive -LiteralPath $distPath -DestinationPath $zipPath -Force; Write-Host \"Created $zipPath\""
set "EXIT_CODE=%ERRORLEVEL%"
exit /b %EXIT_CODE%
