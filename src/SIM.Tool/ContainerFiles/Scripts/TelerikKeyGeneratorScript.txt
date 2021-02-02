Import-Module SitecoreDockerTools

$idData = @()

# TELERIK_ENCRYPTION_KEY
$idData += Get-SitecoreRandomString 128

return $idData;