using System;
using Pulumi;
using Pulumi.AzureNative.Resources;
using RG = Pulumi.AzureNative.Resources.ResourceGroup;

namespace CloudEng.InvoiceBuilder.Infrastructure {
  public static class ResourceGroup {
    public static Output<string> Name => rg.Value.Name;

    public static Output<string> Location => rg.Value.Location;

    private static readonly Lazy<RG> rg = new Lazy<RG>(() => new RG("cloud-eng-invoice-builder", new ResourceGroupArgs {
      Location = Constants.Location
    }));
  }
}