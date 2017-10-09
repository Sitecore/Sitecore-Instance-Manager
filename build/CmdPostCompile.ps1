# params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$Name,
    [Parameter(Mandatory=$True)] [string]$BaseVersion,
    [Parameter(Mandatory=$True)] [string]$BuildFolder,
    [Parameter(Mandatory=$True)] [string]$OutputFolder,
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

# vars
$ReleaseFolder = "$OutputFolder"
$ApplicationFileName = "$Name.zip"
$ApplicationFile = "$OutputFolder\$ApplicationFileName"
$ExecutableFileName = "SIM.exe"
$ExecutableFile = "$BuildFolder\$ExecutableFileName"

# create parent folder of $ReleaseFolder
MKDIR $ReleaseFolder
RMDIR $ReleaseFolder -Force -Recurse
MKDIR $ReleaseFolder

# sign executable file
Write-Host "> tools\signtool.exe sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`""
Start-Process -FilePath "tools\signtool.exe" -ArgumentList "sign /f `"$CertificatePath`" /p `"$CertificatePassword`" /t `"http://timestamp.verisign.com/scripts/timstamp.dll`" `"$ExecutableFile`"" -Wait -RedirectStandardOutput stdout.txt -RedirectStandardError stderr.txt
Get-Content stdout.txt | Write-Host
Get-Content stderr.txt | Write-Host

# create zip
function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory("$sourcedir", "$zipfilename", "$compressionLevel", $false)
}

ZipFiles "$ApplicationFile" "$BuildFolder"

# create latest-version.txt
"$Version" | Set-Content "$OutputFolder\latest-version.txt"