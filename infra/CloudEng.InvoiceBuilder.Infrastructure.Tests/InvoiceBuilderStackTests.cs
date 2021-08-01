using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CloudEng.InvoiceBuilder.Infrastructure.Tests {
  [SuppressMessage("ReSharper", "ConsiderUsingConfigureAwait")]
  public class InvoiceBuilderStackTests {
    private readonly ITestOutputHelper _testOutputHelper;

    public InvoiceBuilderStackTests(ITestOutputHelper testOutputHelper) {
      _testOutputHelper = testOutputHelper;
    }

    [Fact, Trait("Category", "Infrastructure")]
    public async Task Stack_Creation_Does_Not_Fail() {
      try {
        var resources = await Testing.RunAsync<InvoiceBuilderStack>();
        resources.IsEmpty.ShouldBeFalse();
      }
      catch (Exception ex) {
        _testOutputHelper.WriteLine($"{ex.Message}: {ex}");
        throw new XunitException($"Stack creation should not fail, {ex}") {
        };
      }
    }
  }
}