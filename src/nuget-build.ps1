# 确保脚本抛出错误时停止执行
$ErrorActionPreference = "Stop"

# 获取脚本所在文件夹
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 确保 packages 文件夹存在
$packagesDir = Join-Path -Path $scriptDir -ChildPath "packages"
if (-not (Test-Path -Path $packagesDir)) {
    New-Item -ItemType Directory -Path $packagesDir
}

# 获取当前文件夹中所有的 .csproj 文件
$projectFiles = Get-ChildItem -Recurse -Filter *.csproj

if (-not $projectFiles) {
    Write-Error "未找到任何 .csproj 文件。"
    exit 1
}

foreach ($projectFile in $projectFiles) {
    # 排除 Samples 文件夹和 Libs 文件夹下的 .csproj 文件
    if ($projectFile.FullName -notmatch "\\Samples\\") {
        Write-Host "正在处理项目：$($projectFile.FullName)"

        # 清理项目
        dotnet clean $projectFile.FullName -c Release

        # 删除 bin 和 obj 文件夹
        $binDir = Join-Path -Path $projectFile.DirectoryName -ChildPath "bin"
        $objDir = Join-Path -Path $projectFile.DirectoryName -ChildPath "obj"

        if (Test-Path -Path $binDir) {
            Remove-Item -Recurse -Force -Path $binDir
            Write-Host "已删除文件夹：$binDir"
        }

        if (Test-Path -Path $objDir) {
            Remove-Item -Recurse -Force -Path $objDir
            Write-Host "已删除文件夹：$objDir"
        }

        # 恢复 NuGet 包
        dotnet restore $projectFile.FullName

        # 打包 NuGet 包
        dotnet pack $projectFile.FullName -c Release

        # 获取打包后的 NuGet 包文件（包括 .nupkg 和 .snupkg 文件）
        $nugetPackages = Get-ChildItem -Path "$($projectFile.DirectoryName)\bin\Release" | Where-Object { $_.Extension -eq ".nupkg" -or $_.Extension -eq ".snupkg" }

        if (-not $nugetPackages) {
            Write-Error "未找到任何 NuGet 包文件。"
            continue
        }

        foreach ($nugetPackage in $nugetPackages) {
            # 复制 NuGet 包到 packages 文件夹
            $destinationPath = Join-Path -Path $packagesDir -ChildPath $nugetPackage.Name
            Copy-Item -Path $nugetPackage.FullName -Destination $destinationPath -Force
        }
    } else {
        Write-Host "跳过项目：$($projectFile.FullName)"
    }
}

Write-Host "所有项目处理完毕。"