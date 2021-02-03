Import-Module SitecoreDockerTools

$SqlAdminPassword = Get-SitecoreRandomString -Length 20 -DisAllowSpecial -EnforceComplexity

return $SqlAdminPassword