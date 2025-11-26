param(
    [string]$Endpoint,
    [string]$ApiKey,
    [string]$Model
)

# 1. 获取 Git 提交记录
try {
    $lastTag = git describe --tags --abbrev=0 2>$null
    if (-not $lastTag) {
        Write-Host "No tags found, using last 30 commits."
        $commits = git log -n 20 --pretty=format:"- %s"
    } else {
        Write-Host "Last tag: $lastTag"
        $commits = git log "$lastTag..HEAD" --pretty=format:"- %s"
    }
} catch {
    Write-Warning "Failed to get git history: $_"
    $commits = "No commit history available."
}

if (-not $commits) {
    $commits = "No new commits."
}

# 2. 构建 Prompt
$systemPrompt = @"
You are a release note generator assistant.
Your task is to analyze the provided git commit messages and generate a structured release note in JSON format.
The JSON should have one key: 'zh' (Chinese).
The language section should be formatted as a Markdown string with the following structure:

🚀 功能
- Feature 1
- Feature 2

⚡ 优化
- Improvement 1
- Improvement 2

🐛 修复
- Fix 1
- Fix 2

Note: 'Improvements' refers to performance and user experience optimizations, not new features or bug fixes.
Ignore purely technical commits like 'chore:', 'ci:', 'docs:', 'build:' unless they are significant to the user.
If there are no features, improvements or fixes, just output a friendly 'Maintenance update' message in Chinese.
Return ONLY the JSON string, no markdown code blocks.
"@

$userPrompt = "Commits:`n$commits"

Write-Host $userPrompt

# 3. 调用 OpenAI 兼容接口
$headers = @{
    "Content-Type"  = "application/json"
    "Authorization" = "Bearer $ApiKey"
}

$body = @{
    model = $Model
    messages = @(
        @{ role = "system"; content = $systemPrompt },
        @{ role = "user"; content = $userPrompt }
    )
    temperature = 0.7
    max_tokens = 800
} | ConvertTo-Json -Depth 10

$url = "$Endpoint/v1/chat/completions"

try {
    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $body
    $content = $response.choices[0].message.content
    
    # 清理可能的 Markdown 代码块标记
    $jsonString = $content -replace '^```json', '' -replace '^```', '' -replace '```$', ''
    $json = $jsonString | ConvertFrom-Json

    # 4. 设置 GitHub Output
    # 需要处理多行字符串，使用 EOF 定界符
    $zhNotes = $json.zh
    
    # 输出到 GitHub Actions
    if ($env:GITHUB_OUTPUT) {
        "release_notes_zh<<EOF" | Out-File -FilePath $env:GITHUB_OUTPUT -Append -Encoding utf8
        $zhNotes | Out-File -FilePath $env:GITHUB_OUTPUT -Append -Encoding utf8
        "EOF" | Out-File -FilePath $env:GITHUB_OUTPUT -Append -Encoding utf8
    }

    Write-Host "Generated Release Notes (ZH):"
    Write-Host $zhNotes

} catch {
    Write-Error "Failed to generate release notes: $_"
    exit 1
}
