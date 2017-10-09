# get params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$Name,
    [Parameter(Mandatory=$True)] [string]$Title,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$PublisherName,
    [Parameter(Mandatory=$True)] [string]$Icon,
    [Parameter(Mandatory=$True)] [string]$BaseApplicationUrl,
    [Parameter(Mandatory=$True)] [string]$SupportURL,
    [Parameter(Mandatory=$True)] [string]$CertificatePath,
# if code signing not required - remove this $CertificatePassword param and in PostCompile.ps1 all sections related to signing
    [Parameter(Mandatory=$True)] [string]$CertificatePassword,
    [Parameter(Mandatory=$True)] [string]$ProjectFile,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$OutputFolder
)

$Icon = [System.IO.Path]::GetFullPath($Icon)
$BuildFolder = [System.IO.Path]::GetFullPath($BuildFolder)
$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
$ProjectFile =  [System.IO.Path]::GetFullPath($ProjectFile)

Write-Host "> PreCompile.ps1 -Name `"$Name`" -BaseVersion `"$BaseVersion`" -ProjectFile `"$ProjectFile`" -OutputFolder `"$OutputFolder`" -BuildFolder `"$BuildFolder`""
Invoke-Expression ".\PreCompile.ps1 -Name `"$Name`" -BaseVersion `"$BaseVersion`" -ProjectFile `"$ProjectFile`" -OutputFolder `"$OutputFolder`" -BuildFolder `"$BuildFolder`""

Write-Host "> DoCompile.ps1 -ProjectFile `"$ProjectFile`" -BuildFolder `"$BuildFolder`""
Invoke-Expression ".\DoCompile.ps1 -ProjectFile `"$ProjectFile`" -BuildFolder `"$BuildFolder`""

Write-Host "> PostCompile.ps1 -Name `"$Name`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""
Invoke-Expression ".\PostCompile.ps1 -Name `"$Name`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""