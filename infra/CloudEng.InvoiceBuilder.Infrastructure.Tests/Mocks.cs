using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.Testing;

namespace CloudEng.InvoiceBuilder.Infrastructure.Tests {
  internal class Mocks : IMocks {
    public Task<(string? id, object state)> NewResourceAsync(MockResourceArgs args) {
      var outputs = ImmutableDictionary.CreateBuilder<string, object>();
      outputs.AddRange(args.Inputs);

      if (!args.Inputs.ContainsKey("name")) outputs.Add("name", args.Name);

      if (args.Type == "azure-native:storage:Blob") {
        outputs.Remove("source");
      }

      if (args.Type == "azure-native:storage:StorageAccount") {
        outputs.Add("primaryEndpoints", new Dictionary<string, string> {
          {"web", $"https://{args.Name}.web.core.windows.net"},
        }.ToImmutableDictionary());
      }

      // Default the resource ID to `{name}_id`.
      // We could also format it as `/subscription/abc/resourceGroups/xyz/...` if that was important for tests.
      args.Id ??= $"{args.Name}_id";
      return Task.FromResult((args.Id, (object) outputs));
    }

#pragma warning disable 1998
    public async Task<object> CallAsync(MockCallArgs args) {
#pragma warning restore 1998
      if (args.Token == "azure-native:authorization:getClientConfig") {
        return new Dictionary<string, object> {
          {"ClientId", Guid.NewGuid().ToString()},
          {"ObjectId", Guid.NewGuid().ToString()},
          {"SubscriptionId", Guid.NewGuid().ToString()},
          {"TenantId", Guid.NewGuid().ToString()}
        };
      }

      if (args.Token == "azure-native:storage:listStorageAccountKeys") {
        return new Dictionary<string, object> {
          {"Keys", new[] {"primaryKey"}}
        };
      }

      return args.Args;
    }
  }
}