# 用 Visual Studio 的 MSBuild 編譯 Release 版 Thunder.exe（單一 exe，不需要額外 DLL）
# 完成後複製為 Thunder\Thunder_v{主版本}{次版本}.exe，版本號取自 Properties\AssemblyInfo.cs
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repo 'Thunder\Thunder.csproj'

# 1. 找 MSBuild（Visual Studio 或 Build Tools 2022 / 2019）
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = $null
if (Test-Path $vswhere) {
    $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
}
if (-not $msbuild) {
    Write-Host '找不到 MSBuild。請安裝「Visual Studio 2022 Build Tools」（免費）：' -ForegroundColor Yellow
    Write-Host '  https://visualstudio.microsoft.com/zh-hant/downloads/ → 「Visual Studio 2022 的建置工具」'
    Write-Host '  安裝時勾選工作負載「.NET 桌面建置工具」，並確認個別元件含「.NET Framework 4.8 目標套件」'
    exit 1
}
Write-Host "MSBuild：$msbuild"

# 2. 編譯 Release（不簽署 ClickOnce 資訊清單：repo 裡沒有 Thunder_TemporaryKey.pfx）
& $msbuild $project /t:Rebuild /p:Configuration=Release /p:SignManifests=false /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw '編譯失敗' }

# 3. 依版本號複製成 Thunder_vXX.exe
$output = Join-Path $repo 'Thunder\bin\Release\Thunder.exe'
$version = (Get-Item $output).VersionInfo
$target = Join-Path $repo ("Thunder\Thunder_v{0}{1}.exe" -f $version.FileMajorPart, $version.FileMinorPart)
Copy-Item $output $target -Force
Write-Host "完成：$target（版本 $($version.FileVersion)）" -ForegroundColor Green

# 3.5 有程式碼簽章憑證時自動簽章（優先用 IT / 正式憑證，其次是測試用自簽憑證）
$signing = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Where-Object { $_.NotAfter -gt (Get-Date) } |
    Sort-Object { $_.Subject -like '*PatternMagic Test Signing*' } | Select-Object -First 1
if ($signing) {
    $result = Set-AuthenticodeSignature -FilePath $target -Certificate $signing -HashAlgorithm SHA256
    $trusted = if ($result.Status -eq 'Valid') { '受信任' } else { '未受信任（自簽憑證在其他電腦上不被信任）' }
    Write-Host "已簽章：$($signing.Subject)，$trusted"
}
else {
    Write-Host '沒有程式碼簽章憑證，exe 未簽章' -ForegroundColor Yellow
}

# 4. 檢查是否為單一 exe（不應參考額外的 DLL）
$assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom($target)
$extra = $assembly.GetReferencedAssemblies() | Where-Object { $_.Name -notin 'mscorlib', 'System', 'System.Core', 'System.Drawing', 'System.Windows.Forms', 'System.Deployment', 'System.Data', 'System.Xml', 'System.Xml.Linq', 'Microsoft.CSharp', 'System.Net.Http', 'System.Data.DataSetExtensions' }
if ($extra) { Write-Host ('注意：參考了額外組件 ' + (($extra | ForEach-Object Name) -join ', ') + '，發佈時需一併附上') -ForegroundColor Yellow }
else { Write-Host '單一 exe，可直接複製到其他電腦執行（需 .NET Framework 4.8，Windows 10/11 內建）' }
