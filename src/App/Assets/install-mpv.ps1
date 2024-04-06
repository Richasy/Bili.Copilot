param(
    [Parameter(Mandatory=$true)]
    [string]$installDir
)

# Ensure the install directory exists
if (!(Test-Path $installDir)) {
    New-Item -ItemType Directory -Force -Path $installDir
}

Write-Host "安装目录：$installDir"

# Download the latest release of MPV_lazy
Write-Host "正在尝试从 hooke007/MPV_lazy 下载最新的构建..."
$latestRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/hooke007/MPV_lazy/releases/latest"
$downloadUrl = $latestRelease.assets | Where-Object { $_.name -match "mpv-lazy-.*\.exe" } | Select-Object -First 1 -ExpandProperty browser_download_url
Write-Host "准备下载 $downloadUrl"
$exePath = Join-Path $installDir "mpv-lazy.exe"
Invoke-WebRequest -Uri $downloadUrl -OutFile $exePath

# Run the downloaded exe
Write-Host "请在弹出的解压窗口中点击确认，不要修改解压目录。"
Start-Process -FilePath $exePath -Wait
Remove-Item $exePath

# Move files from mpv-lazy folder to install directory
Write-Host "正在移动解压后的文件目录..."
$mpvLazyDir = Join-Path $installDir "mpv-lazy"
if (Test-Path $mpvLazyDir) {
    Get-ChildItem -Path $mpvLazyDir -Recurse | Move-Item -Destination $installDir
    Remove-Item $mpvLazyDir -Recurse
}

# Run the mpv-install.bat script
$installerDir = Join-Path $installDir "installer"
$batPath = Join-Path $installerDir "mpv-install.bat"
Start-Process -FilePath $batPath -Wait

# Add install directory to PATH
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
[Environment]::SetEnvironmentVariable("Path", $currentPath + ";$installDir", "User")
Write-Host "已完成 MPV 安装。"

# Download the latest release of BiliBili Comments
Write-Host "正在尝试从 Richasy/MPV-Play-BiliBili-Comments 下载弹幕模块..."
$latestRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/Richasy/MPV-Play-BiliBili-Comments/releases/latest"
$downloadUrl = $latestRelease.assets | Where-Object { $_.name -eq "bilibiliAssert.zip" } | Select-Object -First 1 -ExpandProperty browser_download_url
$zipPath = Join-Path $installDir "bilibiliAssert.zip"
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath

# Extract the downloaded zip
Write-Host "下载完成，正在解压至 scripts 文件夹"
$extractDir = Join-Path $installDir "portable_config\scripts\bilibiliAssert"
if (!(Test-Path $extractDir)) {
    New-Item -ItemType Directory -Force -Path $extractDir
}
Expand-Archive -Path $zipPath -DestinationPath $extractDir
Remove-Item $zipPath

Write-Host "已加载弹幕模块，一切准备就绪，按下任意键退出安装程序"
Read-Host