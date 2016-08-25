# get params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$Name,
    [Parameter(Mandatory=$True)] [string]$ProjectFile,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$OutputFolder,
    [Parameter(Mandatory=$True)] [string]$BuildFolder
)

# normalize paths
$ProjectFile = [System.IO.Path]::GetFullPath($ProjectFile)
$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)

# get version
Write-Host "tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`""
$Version = Invoke-Expression ".\tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`""

# recreate $OutputFolder 
New-Item $OutputFolder -ItemType Directory -Force
Remove-Item $OutputFolder -Recurse -Force 
New-Item $OutputFolder -ItemType Directory

Write-Host "> Done. Recreated [$OutputFolder] folder"
Write-Host ""

# delete build folder
New-Item $BuildFolder -ItemType Directory -Force
Remove-Item $BuildFolder -Recurse -Force 

Write-Host "> Done. Deleted [$BuildFolder] folder"
Write-Host ""

# update csproj
Write-Host "> Updating $ProjectFile"
(Get-Content $ProjectFile).Replace("SIM.Tool.DEV", $Name) | Set-Content $ProjectFile

Write-Host "> Done. Updated project file"
Write-Host ""

# update assemblyinfo.cs
DIR ..\src AssemblyInfo.cs -Recurse | 
  ForEach-Object { $_.FullName } | 
  ForEach-Object { 
    Write-Host ".\tools\UpdateAssemblyInfo.exe --path $_ --version $Version --force false"
    (.\tools\UpdateAssemblyInfo.exe --path $_ --version $Version --force false )
  } 

Write-Host "> Done. Updated assembly info"