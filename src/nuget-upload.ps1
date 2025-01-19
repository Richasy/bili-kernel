# 获取环境变量 NUGET_KEY
$nugetKey = $env:NUGET_KEY

if (-not $nugetKey) {
    Write-Error "NUGET_KEY 环境变量未设置"
    exit 1
}

# 设置包目录
$packageDir = Join-Path -Path (Get-Location) -ChildPath "packages"

# 检查包目录是否存在
if (-not (Test-Path -Path $packageDir)) {
    Write-Error "packages 目录不存在"
    exit 1
}

# 获取所有 nupkg 文件
$packageFiles = Get-ChildItem -Path $packageDir -Filter *.nupkg

if ($packageFiles.Count -eq 0) {
    Write-Error "packages 目录中没有 nupkg 文件"
    exit 1
}

# 上传每个 nupkg 文件
foreach ($packageFile in $packageFiles) {
    Write-Host "上传包: $($packageFile.FullName)"
    nuget push $packageFile.FullName -ApiKey $nugetKey -Source https://api.nuget.org/v3/index.json -SkipDuplicate
}

Write-Host "所有包已上传"