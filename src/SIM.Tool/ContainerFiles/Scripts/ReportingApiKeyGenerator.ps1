Import-Module SitecoreDockerTools

# REPORTING_API_KEY
# Type: ASCII string
# Minimum legth: 32 characters. 
# https://doc.sitecore.com/developers/100/platform-administration-and-architecture/en/configure-api-authentication-keys-in-a-scaled-environment.html
$ReportingApiKey = Get-SitecoreRandomString -Length 40 -DisallowSpecial

return $ReportingApiKey