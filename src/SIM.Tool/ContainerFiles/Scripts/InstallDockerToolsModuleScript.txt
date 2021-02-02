$moduleName = "SitecoreDockerTools"

if (!(Get-Module -Name $moduleName -ListAvailable))
{
$repoAddress = "https://sitecore.myget.org/F/sc-powershell/api/v2"

$tempRepositoryName = "Temp" + (New-Guid)

$repository = Get-PSRepository | Where-Object { $_.SourceLocation -eq $repoAddress}
    try
    {
       if (!$repository) {
            Register-PSRepository -Name $tempRepositoryName -SourceLocation $repoAddress -InstallationPolicy Trusted
            $repository = Get-PSRepository | Where-Object { $_.SourceLocation -eq $repoAddress }
        }

        if (!(Get-Module -Name $moduleName)) {
        Install-Module -Name $moduleName -Repository $repository.Name -AllowClobber -Force -ErrorAction "Stop"
        }
    }

   finally {
        if ($repository -and $repository.Name -eq $tempRepositoryName) {
            Unregister-PSRepository -Name $tempRepositoryName
        }
    }
}