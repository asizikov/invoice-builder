using System.Collections.Immutable;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.Testing;

namespace CloudEng.InvoiceBuilder.Infrastructure.Tests {
  public static class Testing {
    public static Task<ImmutableArray<Resource>> RunAsync<T>() where T : Stack, new()
      => Deployment.TestAsync<T>(new Mocks(), new TestOptions {IsPreview = false});
  }
}