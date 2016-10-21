# params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$Name,
    [Parameter(Mandatory=$True)] [string]$Title,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$OutputFolder,
    [Parameter(Mandatory=$True)] [string]$Icon,
    [Parameter(Mandatory=$True)] [string]$BaseApplicationURL,
    [Parameter(Mandatory=$True)] [string]$SupportURL,
    [Parameter(Mandatory=$True)] [string]$PublisherName,
    [Parameter(Mandatory=$True)] [string]$CertificatePath,
    [Parameter(Mandatory=$True)] [string]$CertificatePassword
)

# get version
Write-Host "tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`""
$Version = Invoke-Expression ".\tools\GetVersion.ps1 -BaseVersion `"$BaseVersion`""

# normalize paths
$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)

# cleanup useless files
DIR "$BuildFolder" -Filter *.pdb -Recurse | Remove-Item

# rename unsupported by http://dl.sitecore.net/updater extensions
DIR "$BuildFolder" -Filter *.xml -Recurse | Rename-Item -NewName {$_.FullName + ".deploy.txt"}
DIR "$BuildFolder" -Filter *.config -Recurse | Rename-Item -NewName {$_.FullName + ".deploy.txt"}

# vars
$ReleaseFolder = "$OutputFolder\Application Files\$Name.$Version"
$ApplicationFileName = "$Name.application"
$ApplicationFile = "$OutputFolder\$ApplicationFileName"
$URL = "$BaseApplicationUrl/$ApplicationFileName"
$ExecutableFile = "$ReleaseFolder\$Name.exe"
$ManifestFile = "$ExecutableFile.manifest"

# create parent folder of $ReleaseFolder
MKDIR $ReleaseFolder
RMDIR $ReleaseFolder -Force

# move
Write-Host "Move-Item $BuildFolder -Destination $ReleaseFolder"
Move-Item $BuildFolder -Destination $ReleaseFolder

# clean up files
"" | Set-Content "stderr.txt"
"" | Set-Content "stdout.txt"

# sign executable file
Write-Host "> tools\signtool.exe sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`""
Start-Process -FilePath "tools\signtool.exe" -ArgumentList "sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create manifest
Write-Host "> tools\mage.exe -New Application -IconFile `"$Icon`" -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -Processor x86"
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-New Application -IconFile `"$Icon`" -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -Processor x86" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# sign manifest
Write-Host "> tools\mage.exe -Sign `"$ManifestFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-Sign `"$ManifestFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create application file
Write-Host "> tools\mage.exe -New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor x86 -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor x86 -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# workaround to replace some wrong values that I have no idea they came from and why + add a couple things mage doesn't seem to support (SupportUrl)
(Get-Content $ApplicationFile).Replace("asmv2:product=`"SIM`"", "asmv2:product=`"$Title`" asmv2:supportUrl=`"$SupportUrl`"").Replace("name=`"SIM.app`" version=`"1.0.0.0`"", "name=`"$ApplicationFileName`" version=`"$Version`"").Replace("<expiration maximumAge=`"0`" unit=`"days`" />", "<expiration maximumAge=`"0`" unit=`"days`" />") | Set-Content $ApplicationFile

# sign application file
Write-Host "> tools\mage.exe -Sign `"$ApplicationFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`""
Start-Process -FilePath "tools\mage.exe" -ArgumentList "-Sign `"$ApplicationFile`" -CertFile `"$CertificatePath`" -Password `"$CertificatePassword`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host
