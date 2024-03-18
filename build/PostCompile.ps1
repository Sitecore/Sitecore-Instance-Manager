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
    [Parameter(Mandatory=$True)] [string]$AzureSignToolFile
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
$MageToolFile = ".\tools\mage.exe"
$TimestampSha2 = "http://timestamp.globalsign.com/?signature=sha2"
$TimestampRfc3161 = "http://rfc3161timestamp.globalsign.com/advanced"

# create parent folder of $ReleaseFolder
MKDIR $ReleaseFolder
RMDIR $ReleaseFolder -Force

# move
Write-Host "Move-Item $BuildFolder -Destination $ReleaseFolder"
Move-Item $BuildFolder -Destination $ReleaseFolder

# clean up files
"" | Set-Content "stderr.txt"
"" | Set-Content "stdout.txt"

# create manifest
Write-Host "> $MageToolFile -New Application -IconFile `"$Icon`" -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -Processor msil"
Start-Process -FilePath $MageToolFile -ArgumentList "-New Application -IconFile `"$Icon`" -ToFile `"$ManifestFile`" -FromDirectory `"$ReleaseFolder`" -Name `"$Name`" -Version `"$Version`" -Processor msil" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create application file
Write-Host "> $MageToolFile -New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor msil -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`""
Start-Process -FilePath $MageToolFile -ArgumentList "-New Deployment -AppManifest `"$ManifestFile`" -ToFile `"$ApplicationFile`" -Processor msil -Install true -Publisher `"$PublisherName`" -ProviderUrl `"$URL`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# workaround to replace some wrong values that I have no idea they came from and why + add a couple things mage doesn't seem to support (SupportUrl)
(Get-Content $ApplicationFile).Replace("asmv2:product=`"SIM`"", "asmv2:product=`"$Title`" asmv2:supportUrl=`"$SupportUrl`"").Replace("name=`"SIM.app`" version=`"1.0.0.0`"", "name=`"$ApplicationFileName`" version=`"$Version`"").Replace("<expiration maximumAge=`"0`" unit=`"days`" />", "<expiration maximumAge=`"0`" unit=`"days`" />") | Set-Content $ApplicationFile

# sign executable file, manifest and application file
Write-Host "> $AzureSignToolFile -path=$OutputFolder -mageToolPath=$MageToolFile -azure-key-vault-url=$env:AzureKeyvaultUrl -azure-key-vault-client-id=$env:AzureSigningClientId -azure-key-vault-client-secret=$env:AzureSigningSecret -azure-key-vault-tenant-id=$env:AzureSigningTenantId -azure-key-vault-certificate=$env:SignCertificateName -timestamp-sha2=$TimestampSha2 -timestamp-rfc3161=$TimestampRfc3161 -description=$Title"
Start-Process -FilePath $AzureSignToolFile -ArgumentList "-path=$OutputFolder -mageToolPath=$MageToolFile -azure-key-vault-url=$env:AzureKeyvaultUrl -azure-key-vault-client-id=$env:AzureSigningClientId -azure-key-vault-client-secret=$env:AzureSigningSecret -azure-key-vault-tenant-id=$env:AzureSigningTenantId -azure-key-vault-certificate=$env:SignCertificateName -timestamp-sha2=$TimestampSha2 -timestamp-rfc3161=$TimestampRfc3161 -description=$Title" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host