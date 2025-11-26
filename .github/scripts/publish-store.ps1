param(
    [string]$TenantId,
    [string]$ClientId,
    [string]$ClientSecret,
    [string]$AppId,
    [string]$PackagePath,
    [string]$NotesZh,
    [string]$NotesEn,
    [switch]$SkipCommit
)

$ErrorActionPreference = "Stop"

function Get-AccessToken {
    $body = @{
        grant_type    = "client_credentials"
        client_id     = $ClientId
        client_secret = $ClientSecret
        resource      = "https://manage.devcenter.microsoft.com"
    }
    $response = Invoke-RestMethod -Uri "https://login.microsoftonline.com/$TenantId/oauth2/token" -Method Post -Body $body
    return $response.access_token
}

function Invoke-StoreApi {
    param($Uri, $Method = "Get", $Body = $null, $ContentType = "application/json")
    $headers = @{
        "Authorization" = "Bearer $script:token"
        "User-Agent"    = "PowerShell-Store-Deploy"
    }
    
    $params = @{
        Uri     = $Uri
        Method  = $Method
        Headers = $headers
    }
    
    # POST 和 PUT 请求需要设置 ContentType
    if ($Method -eq "Post" -or $Method -eq "Put") {
        $params.ContentType = $ContentType
        if ($null -ne $Body) {
            $params.Body = $Body
        }
    }

    return Invoke-RestMethod @params
}

Write-Host "1. Getting Access Token..."
$script:token = Get-AccessToken

Write-Host "2. Getting Application Data..."
$app = Invoke-StoreApi -Uri "https://manage.devcenter.microsoft.com/v1.0/my/applications/$AppId"

Write-Host "3. Cleaning up pending submissions..."
# 删除现有的 pending submission，确保我们可以创建新的
if ($app.pendingApplicationSubmission) {
    $pendingId = $app.pendingApplicationSubmission.id
    Write-Host "Found pending submission $pendingId, deleting..."
    try {
        Invoke-StoreApi -Uri "https://manage.devcenter.microsoft.com/v1.0/my/applications/$AppId/submissions/$pendingId" -Method Delete
        Write-Host "Deleted pending submission."
    } catch {
        Write-Host "Failed to delete pending submission, continuing anyway: $_"
    }
}

Write-Host "4. Creating new submission..."
$submission = Invoke-StoreApi -Uri "https://manage.devcenter.microsoft.com/v1.0/my/applications/$AppId/submissions" -Method Post
$submissionId = $submission.id
$uploadUrl = $submission.fileUploadUrl
Write-Host "Submission ID: $submissionId"

Write-Host "5. Uploading Packages..."
# 获取所有 msixupload 文件
$files = Get-ChildItem $PackagePath -Recurse -Filter "*.msixupload"
if ($files.Count -eq 0) {
    throw "No .msixupload files found in $PackagePath"
}
Write-Host "Found $($files.Count) package(s) to upload."

# fileUploadUrl 指向一个 blob 容器，我们需要上传 zip 包含所有文件
# 根据 Microsoft Store API，需要将所有包打包成一个 zip 文件上传
$zipPath = Join-Path $env:TEMP "StorePackages.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Write-Host "Creating zip archive..."
Compress-Archive -Path $files.FullName -DestinationPath $zipPath -Force
Write-Host "Zip created: $zipPath"

# 上传 zip 到 Azure Blob Storage
Write-Host "Uploading to Blob Storage..."
try {
    $httpClient = [System.Net.Http.HttpClient]::new()
    $httpClient.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob")
    $fileContent = [System.IO.File]::ReadAllBytes($zipPath)
    $content = [System.Net.Http.ByteArrayContent]::new($fileContent)
    
    $response = $httpClient.PutAsync($uploadUrl, $content).Result
    if (-not $response.IsSuccessStatusCode) {
        $errorBody = $response.Content.ReadAsStringAsync().Result
        throw "Upload failed: $($response.StatusCode) - $errorBody"
    }
    Write-Host "Upload successful."
} catch {
    throw "Failed to upload package: $_"
} finally {
    if ($httpClient) { $httpClient.Dispose() }
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
}

Write-Host "6. Updating Submission Data..."
# 更新 applicationPackages
# 将所有旧包标记为 PendingDelete，添加新包
$newPackages = @()
foreach ($existingPkg in $submission.applicationPackages) {
    $existingPkg.fileStatus = "PendingDelete"
    $newPackages += $existingPkg
}
foreach ($file in $files) {
    $newPackages += @{
        fileName = $file.Name
        fileStatus = "PendingUpload"
    }
}
$submission.applicationPackages = $newPackages

# 更新 Release Notes (Listings)
Write-Host "Updating release notes..."
$listings = $submission.listings
if ($listings) {
    foreach ($prop in $listings.PSObject.Properties) {
        $lang = $prop.Name
        if ($lang -like "zh-*" -and $NotesZh) {
            Write-Host "Updating release notes for $lang"
            $prop.Value.baseListing.releaseNotes = $NotesZh
        }
        elseif ($lang -like "en-*" -and $NotesEn) {
            Write-Host "Updating release notes for $lang"
            $prop.Value.baseListing.releaseNotes = $NotesEn
        }
    }
}

# 提交更新
Write-Host "Sending update to Store..."
$updatedSubmission = Invoke-StoreApi -Uri "https://manage.devcenter.microsoft.com/v1.0/my/applications/$AppId/submissions/$submissionId" -Method Put -Body ($submission | ConvertTo-Json -Depth 20)

if ($SkipCommit) {
    Write-Host "⚠️ SkipCommit flag is set. Submission created but NOT committed."
    Write-Host "Please go to Partner Center to review and manually publish."
} else {
    Write-Host "7. Committing Submission..."
    Invoke-StoreApi -Uri "https://manage.devcenter.microsoft.com/v1.0/my/applications/$AppId/submissions/$submissionId/commit" -Method Post
    Write-Host "✅ Successfully committed to Microsoft Store! Waiting for certification..."
}
