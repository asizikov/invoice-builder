using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudEng.InvoiceBuilder.Host {
  public class Program {
    public static async Task Main() {
      var host = new HostBuilder()
        .ConfigureFunctionsWorkerDefaults()
        .ConfigureServices(s => ConfigureServices(s))
        .Build();

      await host.RunAsync().ConfigureAwait(false);
    }

    private static void ConfigureServices(IServiceCollection serviceCollection) {

    }
  }
}