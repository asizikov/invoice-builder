using CloudEng.InvoiceBuilder.Infrastructure.Security;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Kind = Pulumi.AzureNative.Storage.Kind;
using ManagedServiceIdentityType = Pulumi.AzureNative.Web.ManagedServiceIdentityType;
using SkuArgs = Pulumi.AzureNative.Storage.Inputs.SkuArgs;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

namespace CloudEng.InvoiceBuilder.Infrastructure {
  // ReSharper disable once ClassNeverInstantiated.Global
  public class InvoiceBuilderStack : Stack {
    [Output] public Output<string> ResourceGroupName { get; set; }

    public InvoiceBuilderStack() {
      var resourceGroup = new ResourceGroup("cloud-eng-invoice-builder", new ResourceGroupArgs {
        Location = Constants.Location
      });

      var fileStorageAccount = CreateFileStorageAccount("files-storage", "sacloudenginvoicefiles", resourceGroup);
      var functionStorageAccount = CreateFileStorageAccount("function-storage", "sacloudenginvoicefunc", resourceGroup);

      var appServicePlan = CreateAppServicePlan(resourceGroup);
      var clientConfig = GetClientConfig.InvokeAsync().Result;

      var appInsights = new Component("invoice-builder-app-insights", new ComponentArgs {
        ApplicationType = ApplicationType.Web,
        Kind = "web",
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location
      });

      var builderFunctionApp = new WebApp("invoice-builder-app", new WebAppArgs {
        ResourceGroupName = resourceGroup.Name,
        Kind = "FunctionApp",
        ServerFarmId = appServicePlan.Id,
        Reserved = true,
        Identity = new ManagedServiceIdentityArgs {
          Type = ManagedServiceIdentityType.SystemAssigned,
        },
        SiteConfig = new SiteConfigArgs {
          AppSettings = new[] {
            new NameValuePairArgs {
              Name = "FUNCTIONS_EXTENSION_VERSION",
              Value = "~3"
            },
            new NameValuePairArgs {
              Name = "FUNCTIONS_WORKER_RUNTIME",
              Value = "dotnet-isolated"
            },
            new NameValuePairArgs {
              Name = "AzureWebJobsStorage",
              Value = GetConnectionString(resourceGroup.Name, functionStorageAccount.Name)
            },
            new NameValuePairArgs {
              Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
              Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
            },
            //app config
            new NameValuePairArgs {
              Name = "TogglClient.ApiKey",
              Value = "@Microsoft.KeyVault(SecretUri=https://invoice-builder-vault.vault.azure.net/secrets/TogglClient-ApiKey)"
            },
            new NameValuePairArgs {
              Name = "TogglClient.UserAgent",
              Value = "@Microsoft.KeyVault(SecretUri=https://invoice-builder-vault.vault.azure.net/secrets/TogglClient-UserAgent)"
            },
            new NameValuePairArgs {
              Name = "TogglClient.WorkspaceId",
              Value = "@Microsoft.KeyVault(SecretUri=https://invoice-builder-vault.vault.azure.net/secrets/TogglClient-WorkspaceId)"
            },
          }
        }
      });
      var functionPrincipalId = builderFunctionApp.Identity.Apply(response => response.PrincipalId );
      var keyVault = new KeyVault(resourceGroup, clientConfig, functionPrincipalId);
      ResourceGroupName = resourceGroup.Name;
    }

    private static AppServicePlan CreateAppServicePlan(ResourceGroup resourceGroup) {
      return new AppServicePlan("asp-invoice-builder-func", new AppServicePlanArgs {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Kind = "FunctionApp",
        Reserved = true,
        Sku = new SkuDescriptionArgs {
          Tier = "Dynamic",
          Name = "Y1"
        }
      });
    }

    private static StorageAccount CreateFileStorageAccount(string filesStorageName, string filesAccountName, ResourceGroup resourceGroup) {
      return new StorageAccount(filesStorageName, new StorageAccountArgs {
        AccountName = filesAccountName,
        Kind = Kind.StorageV2,
        Location = resourceGroup.Location,
        ResourceGroupName = resourceGroup.Name,
        Sku = new SkuArgs {
          Name = SkuName.Standard_LRS
        }
      });
    }

    private static Output<string> GetConnectionString(Input<string> resourceGroupName, Input<string> accountName) {
      // Retrieve the primary storage account key.
      var storageAccountKeys = Output.All<string>(resourceGroupName, accountName).Apply(t => {
        var resourceGroupName = t[0];
        var accountName = t[1];
        return ListStorageAccountKeys.InvokeAsync(
          new ListStorageAccountKeysArgs {
            ResourceGroupName = resourceGroupName,
            AccountName = accountName
          });
      });
      return storageAccountKeys.Apply(keys => {
        var primaryStorageKey = keys.Keys[0].Value;

        // Build the connection string to the storage account.
        return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
      });
    }
  }
}