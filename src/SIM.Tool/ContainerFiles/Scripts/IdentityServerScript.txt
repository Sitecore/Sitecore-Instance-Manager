Import-Module SitecoreDockerTools

$idData = @()

# SITECORE_IDSECRET
$idData += Get-SitecoreRandomString 64 -DisallowSpecial 

$idCertPassword = Get-SitecoreRandomString 12 -DisallowSpecial

# SITECORE_ID_CERTIFICATE
$idData += Get-SitecoreCertificateAsBase64String -DnsName "localhost" -Password (ConvertTo-SecureString -String $idCertPassword -Force -AsPlainText)

# SITECORE_ID_CERTIFICATE_PASSWORD
$idData += $idCertPassword

return $idData;