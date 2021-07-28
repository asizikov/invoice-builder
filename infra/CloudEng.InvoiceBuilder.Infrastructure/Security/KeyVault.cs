using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;

namespace CloudEng.InvoiceBuilder.Infrastructure.Security {
  public class KeyVault {
    public KeyVault(ResourceGroup resourceGroup, GetClientConfigResult clientConfig, Output<string> functionPrincipalId) {
      var keyVault = new Vault("invoice-builder-vault", new VaultArgs {
        Location = resourceGroup.Location,
        Properties = new VaultPropertiesArgs {
          AccessPolicies = new[] {
            new AccessPolicyEntryArgs {
              TenantId = clientConfig.TenantId,
              ObjectId = clientConfig.ObjectId,
              Permissions = new PermissionsArgs {
                Secrets = new InputList<Union<string, SecretPermissions>> {"get", "set", "list"}
              }
            },
            new AccessPolicyEntryArgs {
              TenantId = clientConfig.TenantId,
              ObjectId = functionPrincipalId,
              Permissions = new PermissionsArgs {
                Secrets = new InputList<Union<string, SecretPermissions>> {"get"}
              }
            }
          },
          Sku = new SkuArgs {
            Family = SkuFamily.A,
            Name = SkuName.Standard
          },
          TenantId = clientConfig.TenantId,
        },

        ResourceGroupName = resourceGroup.Name,
        VaultName = "invoice-builder-vault",
      });
    }
  }
}