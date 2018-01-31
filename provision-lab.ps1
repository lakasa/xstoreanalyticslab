param(
    [Int32]$increment,
    [string]$upn,
    [string]$clientSecret
)

$baseName = $increment.ToString("xstorelab0000")
$user = Get-AzureRmADUser -Mail $upn
New-AzureRmResourceGroup -Name $baseName -Location eastus
New-AzureRmResourceGroupDeployment -Name xstoreilldeploy1 -ResourceGroupName $baseName -Mode Incremental -TemplateFile .\azuredeploy.json -baseName $baseName -userPrincipalId $user.Id -dataBricksUsername jamesbak@microsoft.com -dataBricksToken dapi2f292e228d75afb9f911a6fd9f39dc43 -clientSecret $clientSecret
New-AzureRmRoleAssignment -ObjectId $user.Id -ResourceGroupName xstoreilldb -ResourceName xstoreill -ResourceType "Microsoft.Databricks/workspaces" -RoleDefinitionName Contributor