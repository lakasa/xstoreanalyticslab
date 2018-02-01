param(
    [Parameter(Mandatory=$true)][Int32]$increment,
    [Parameter(Mandatory=$true)][string]$upn,
    [Parameter(Mandatory=$true)][string]$dataBricksToken,
    [string]$location = "eastus"
)

$baseName = $increment.ToString("xstorelab0000")
$user = Get-AzureRmADUser -Mail $upn
New-AzureRmResourceGroup -Name $baseName -Location $location
New-AzureRmResourceGroupDeployment -Name xstoreilldeploy1 -ResourceGroupName $baseName -Mode Incremental -TemplateFile .\azuredeploy.json -baseName $baseName -userPrincipalId $user.Id -dataBricksUsername jamesbak@microsoft.com -dataBricksToken $dataBricksToken
New-AzureRmRoleAssignment -ObjectId $user.Id -ResourceGroupName xstoreilldb -ResourceName xstoreill -ResourceType "Microsoft.Databricks/workspaces" -RoleDefinitionName Contributor
$account = Get-AzureRmStorageAccount -ResourceGroupName $baseName -Name $baseName
New-AzureStorageContainer -Context $account.Context -Name sourcedata
New-AzureStorageContainer -Context $account.Context -Name outputdata
New-AzureStorageContainer -Context $account.Context -Name archivaldata
