# Install.ps1

# Hide the progress bar to prevent it from obscuring the output log
$ProgressPreference = 'SilentlyContinue'

# 1. Check and Request Admin Privileges
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Requesting Administrator privileges..." -ForegroundColor Yellow
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host "=== Bili Copilot Installer ===" -ForegroundColor Cyan

# 2. Install Certificate
$certFile = Get-ChildItem -Path $scriptDir -Filter "*.cer" | Select-Object -First 1

if ($certFile) {
    Write-Host "Found certificate: $($certFile.Name)"
    
    try {
        $certObj = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certFile.FullName)
        $installed = Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Thumbprint -eq $certObj.Thumbprint }

        if ($installed) {
            Write-Host "Certificate is already installed. Skipping." -ForegroundColor Gray
        } else {
            Write-Host "Installing certificate to Trusted Root Certification Authorities..."
            Import-Certificate -FilePath $certFile.FullName -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
            Write-Host "Certificate installed successfully." -ForegroundColor Green
        }
    }
    catch {
        Write-Error "Failed to process certificate. Error: $_"
        Write-Host "Press any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
} else {
    Write-Warning "No certificate (.cer) found. Skipping certificate installation."
}

# 3. Install Dependencies (WinAppSDK, VCLibs, etc.)
# We assume dependencies are any .msix/.appx files that start with "Microsoft"
$depFiles = Get-ChildItem -Path "$scriptDir\*" -Include "*.msix", "*.appx" | Where-Object { $_.Name -match "^Microsoft" }

if ($depFiles) {
    Write-Host "Found $($depFiles.Count) dependency packages."
    foreach ($dep in $depFiles) {
        Write-Host "Installing dependency: $($dep.Name)..."
        try {
            Add-AppxPackage -Path $dep.FullName -ForceApplicationShutdown -ErrorAction Stop
            Write-Host "  OK" -ForegroundColor Green
        }
        catch {
            Write-Warning "  Skipping $($dep.Name): $_"
            Write-Host "  (This is usually fine if a newer version is already installed)" -ForegroundColor Gray
        }
    }
}

# 4. Install Main App
# Assume the main app is the .msix/.appx file that does NOT start with "Microsoft"
$appFile = Get-ChildItem -Path "$scriptDir\*" -Include "*.msix", "*.appx" | Where-Object { $_.Name -notmatch "^Microsoft" } | Select-Object -First 1

if ($appFile) {
    Write-Host "Installing Application: $($appFile.Name)..."
    Write-Host "Please wait, this may take a few moments..." -ForegroundColor Cyan
    try {
        Add-AppxPackage -Path $appFile.FullName -ForceApplicationShutdown -ErrorAction Stop
        Write-Host "Installation Complete!" -ForegroundColor Green
        Write-Host "You can now find 'Bili Copilot' in your Start Menu."
    }
    catch {
        Write-Error "Failed to install application. Error: $_"
        Write-Host "Press any key to exit..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
} else {
    Write-Error "Main application package (.msix) not found!"
}

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
