using System.Threading.Tasks;
using CloudEng.InvoiceBuilder.Toggl.Configuration;
using CloudEng.InvoiceBuilder.Workflow.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CloudEng.InvoiceBuilder.Host {
  public class Program {
    public static async Task Main() {
      var host = new HostBuilder()
        .ConfigureFunctionsWorkerDefaults()
        .ConfigureServices(ConfigureServices)
        .Build();

      await host.RunAsync().ConfigureAwait(false);
    }


    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection serviceCollection) {
      var togglConfiguration = new TogglConfiguration {
        ApiKey = hostContext.Configuration.GetSection("TogglClient.ApiKey").Value,
        UserAgent = hostContext.Configuration.GetSection("TogglClient.UserAgent").Value,
        WorkspaceId = int.Parse(hostContext.Configuration.GetSection("TogglClient.WorkspaceId").Value)
      };
      serviceCollection.AddSingleton(Options.Create(togglConfiguration));
      serviceCollection.UseInvoiceBuilderWorkflow();
    }
  }
}