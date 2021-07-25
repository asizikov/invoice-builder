using Pulumi;
using Pulumi.AzureNative.Resources;

namespace CloudEng.InvoiceBuilder.Infrastructure {
  // ReSharper disable once ClassNeverInstantiated.Global
  public class InvoiceBuilderStack : Stack {
    public InvoiceBuilderStack() {
      var resourceGroup = new ResourceGroup("cloud-eng-invoice-builder", new ResourceGroupArgs {
        Location = Constants.Location
      });
    }
  }

  public static class Constants {
    public const string Location = "westeurope";
  }
}