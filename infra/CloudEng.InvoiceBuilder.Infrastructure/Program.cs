using System.Threading.Tasks;
using Pulumi;

namespace CloudEng.InvoiceBuilder.Infrastructure {
  public class Program {
    static Task<int> Main() => Deployment.RunAsync<InvoiceBuilderStack>();
  }
}