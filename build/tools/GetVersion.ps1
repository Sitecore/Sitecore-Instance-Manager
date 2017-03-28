# params
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)] [string]$BaseVersion
)

# var
$CommitCountText = (git rev-list --count HEAD)
$CommitCount = [System.Int32]::Parse($CommitCountText)
$Revision = ($CommitCount - 423) ## +123 is initial offset that came from internal SVN during publishing to GitHub

# do
$BaseVersion + ".0." + $Revision