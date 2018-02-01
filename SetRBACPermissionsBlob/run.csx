
#r "D:\home\site\wwwroot\SetRBACPermissionsBlob\bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
using System;
using System.Security.Cryptography;
using System.Net;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

public class Properties
{
    public string roleDefinitionId { get; set; }
    public string principalId { get; set; }
}

public class RoleAssignment
{
    public Properties properties = new Properties();
}

public static void Run(Stream myBlob, string name, TraceWriter log)
{
    log.Info("ResourceId: " + ResourceId);
    if (name== "done")
    {
        RunAsync(log).GetAwaiter().GetResult();
    }
    
}

static string ResourceId = System.Environment.GetEnvironmentVariable("ResourceId");
static string AuthEndpoint = System.Environment.GetEnvironmentVariable("AuthEndpoint");
static string TenantId = System.Environment.GetEnvironmentVariable("TenantId");
static string SubscriptionId = System.Environment.GetEnvironmentVariable("SubscriptionId");
static string ResourceGroup = System.Environment.GetEnvironmentVariable("ResourceGroup");
static string Resource = System.Environment.GetEnvironmentVariable("Resource");
static string Container = System.Environment.GetEnvironmentVariable("Container");
static string clientID = System.Environment.GetEnvironmentVariable("clientID");
static string clientSecret = System.Environment.GetEnvironmentVariable("clientSecret");
static string principalId = System.Environment.GetEnvironmentVariable("principalId");
static string RoleDefinitionId = System.Environment.GetEnvironmentVariable("RoleDefinitionId");

        
static HttpClient client = new HttpClient();

public static async Task<string> GetAccessTokenAsync()
{
    string authority = string.Format(CultureInfo.InvariantCulture, AuthEndpoint, TenantId);
    AuthenticationContext authContext = new AuthenticationContext(authority);

    ClientCredential clientCredential = new ClientCredential(clientID, clientSecret);

    AuthenticationResult authenticationResult = null;
    authenticationResult = await authContext.AcquireTokenAsync(ResourceId, clientCredential);

    return authenticationResult.AccessToken;
}

static async Task<RoleAssignment> SetRoleAssignment(RoleAssignment roleAssignment, TraceWriter log)
{
    Guid newGuid = Guid.NewGuid();
    string scope = String.Format("/subscriptions/{0}/resourceGroups/{1}", SubscriptionId, ResourceGroup);
    string requestUri = String.Format("{0}/providers/Microsoft.Authorization/roleAssignments/{1}?api-version=2015-07-01", scope, newGuid.ToString());
    
    log.Info("Execute REST API call to set access role");
    HttpResponseMessage response = await client.PutAsJsonAsync(requestUri, roleAssignment);
    log.Info("Response: " + response.ToString());

    response.EnsureSuccessStatusCode();

    // Deserialize the updated role assignment from the response body.
    roleAssignment = await response.Content.ReadAsAsync<RoleAssignment>();
    return roleAssignment;
}

static async Task RunAsync(TraceWriter log)
{
    //Get access token from Azure AD and set in request header
    string accessToken = await GetAccessTokenAsync();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
    
    // Update port # in the following line.
    client.BaseAddress = new Uri(ResourceId);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    try
    {
        // Create a new role assignment
        RoleAssignment roleAssignment = new RoleAssignment();
        //roleAssignment.properties.roleDefinitionId = String.Format("/subscriptions/{0}/providers/Microsoft.Authorization/roleDefinitions/{1}", SubscriptionId, RoleDefinitionId);
        roleAssignment.properties.roleDefinitionId = String.Format("/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Storage/storageAccounts/{2}/providers/Microsoft.Authorization/roleDefinitions/{3}", SubscriptionId, ResourceGroup, Resource, RoleDefinitionId);
        
        roleAssignment.properties.principalId = principalId;

        await SetRoleAssignment(roleAssignment, log);              
    }
    catch (Exception e)
    {
        log.Error(e.Message);
    }
}


