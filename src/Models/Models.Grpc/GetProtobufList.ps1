$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$protoFiles = Get-ChildItem -Path $scriptPath -Recurse -Filter "*.proto"

foreach ($file in $protoFiles) {
    $relativePath = $file.FullName.Replace($scriptPath, "").TrimStart("\")
    $includeStatement = '<Protobuf Include="{0}" />' -f $relativePath
    Write-Output $includeStatement
}