param(
    [Parameter(Mandatory=$true)][Int32]$increment,
    [Parameter(Mandatory=$true)][string]$upn
)

$baseName = $increment.ToString("xstorelab0000")
$user = Get-AzureRmADUser -Mail $upn
New-AzureRmRoleAssignment -ObjectId $user.Id -ResourceGroupName $baseName -RoleDefinitionName Contributor
New-AzureRmRoleAssignment -ObjectId $user.Id -ResourceGroupName xstoreilldb -ResourceName xstoreill -ResourceType "Microsoft.Databricks/workspaces" -RoleDefinitionName Contributor
