# 在沒有 Visual Studio 的電腦上用 .NET SDK 編譯並執行 Thunder（僅供測試）
# 不會修改 repo：先把 Thunder 複製到 %TEMP%\ThunderDev，在副本加上編譯設定再建置。
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$work = Join-Path $env:TEMP 'ThunderDev'

Get-Process Thunder -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "$work*" } | Stop-Process -Force
if (Test-Path $work) { Remove-Item $work -Recurse -Force }
Copy-Item (Join-Path $repo 'Thunder') $work -Recurse
Remove-Item "$work\bin", "$work\obj" -Recurse -Force -ErrorAction SilentlyContinue

# .NET SDK 的 MSBuild 需要 System.Resources.Extensions 才能嵌入圖片資源
$csproj = Join-Path $work 'Thunder.csproj'
$extra = @'
  <PropertyGroup>
    <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
    <GenerateResourceUsePreserializedResources>true</GenerateResourceUsePreserializedResources>
    <SignManifests>false</SignManifests>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="System.Resources.Extensions" Version="8.0.0" />
  </ItemGroup>
</Project>
'@
(Get-Content $csproj -Raw) -replace '</Project>\s*$', $extra | Set-Content $csproj -Encoding UTF8

dotnet restore $csproj -nologo -v:q
if ($LASTEXITCODE -ne 0) { throw 'restore 失敗' }

# 舊式 csproj 不會自動帶入套件組件，手動加上參考並複製相依 DLL
$nuget = Join-Path $env:USERPROFILE '.nuget\packages'
$sre = Join-Path $nuget 'system.resources.extensions\8.0.0\lib\net462\System.Resources.Extensions.dll'
(Get-Content $csproj -Raw) -replace '</Project>\s*$', "  <ItemGroup><Reference Include=`"System.Resources.Extensions`"><HintPath>$sre</HintPath></Reference></ItemGroup>`r`n</Project>" | Set-Content $csproj -Encoding UTF8

dotnet build $csproj -c Debug -nologo -v:q
if ($LASTEXITCODE -ne 0) { throw '建置失敗' }

$out = Join-Path $work 'bin\Debug'
foreach ($pkg in 'system.memory', 'system.buffers', 'system.numerics.vectors', 'system.runtime.compilerservices.unsafe') {
    $dir = Get-ChildItem (Join-Path $nuget $pkg) -Directory | Sort-Object Name | Select-Object -Last 1
    Get-ChildItem (Join-Path $dir.FullName 'lib') -Recurse -Filter *.dll |
        Where-Object { $_.DirectoryName -match 'net4[6-8]' } | Select-Object -First 1 |
        Copy-Item -Destination $out
}

# 專案資料夾有 config.txt 時放到 exe 旁邊，測試啟動自動載入
$config = Join-Path $repo 'Thunder\config.txt'
if (Test-Path $config) {
    Copy-Item $config $out
    Write-Host "已複製 Thunder\config.txt 到程式資料夾"
}

Write-Host "執行 $out\Thunder.exe（AutoSave 的檔案會存在這個資料夾）"
Start-Process (Join-Path $out 'Thunder.exe') -WorkingDirectory $out
