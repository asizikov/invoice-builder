using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudEng.InvoiceBuilder.Host {

  public class DownloadReportFunction {
    public DownloadReportFunction(
      //IMediator mediator
      ) {

    }

    [Function(nameof(DownloadReportFunction))]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer, FunctionContext context) {
      var logger = context.GetLogger(nameof(DownloadReportFunction));
      logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }
  }
}