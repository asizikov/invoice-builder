using System;
using System.Linq;
using System.Threading.Tasks;
using CloudEng.InvoiceBuilder.Toggl.Configuration;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace CloudEng.InvoiceBuilder.Toggl.Tests {
  public class TogglClientWrapperIntegrationTests {
    private readonly ITestOutputHelper _testOutputHelper;

    public TogglClientWrapperIntegrationTests(ITestOutputHelper testOutputHelper) {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Client_Iterates_Over_Each_Page() {
      var client = new TogglClientWrapper(Options.Create(new TogglConfiguration {
        ApiKey = "",
        UserAgent =  "invoice-builder:org@cloud-eng.nl",
        WorkspaceId = 0
      }));

      _testOutputHelper.WriteLine("go");
      await foreach (var dayString in client.GetDetailedReportAsync(DateTime.Today).ToDaysAsync()) {
       // _testOutputHelper.WriteLine(dayString);
      }
      // await foreach (var reportTimeEntry in client.GetDetailedReportAsync()) {
      //   _testOutputHelper.WriteLine($"{reportTimeEntry.Start}{reportTimeEntry.Description}");
      // }
    }
  }
}