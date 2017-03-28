# if code signing not required - remove this $CertificatePassword param and in PostCompile.ps1 all sections related to signing
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$CertificatePassword
)

$Name = "SIM.Tool.DEV"
$Title = "Sitecore Instance Manager DEV (SIM)"
$BaseVersion = "1.4"
$PublisherName = "Sitecore"
$Icon = [System.IO.Path]::GetFullPath("..\src\SIM.Tool.Windows\Images\window.ico")
$BaseApplicationURL = "http://dl.sitecore.net/updater/dev/sim"
$SupportURL = "https://github.com/sitecore/sitecore-instance-manager/issues/new"
$CertificatePath = "C:\Sitecore\Keys\certificate.pfx"

$BuildFolder = [System.IO.Path]::GetFullPath("build\bin")
$OutputFolder = [System.IO.Path]::GetFullPath("publish")
$ProjectFile =  [System.IO.Path]::GetFullPath("..\src\SIM.Tool\SIM.Tool.csproj")

Write-Host "> Build.ps1 -Name `"$Name`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -ProjectFile `"$ProjectFile`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""
Invoke-Expression ".\Build.ps1 -Name `"$Name`" -Title `"$Title`" -BaseVersion `"$BaseVersion`" -BuildFolder `"$BuildFolder`" -OutputFolder `"$OutputFolder`" -ProjectFile `"$ProjectFile`" -Icon `"$Icon`" -BaseApplicationURL `"$BaseApplicationURL`" -SupportURL `"$SupportURL`" -PublisherName `"$PublisherName`" -CertificatePath `"$CertificatePath`" -CertificatePassword `"$CertificatePassword`""