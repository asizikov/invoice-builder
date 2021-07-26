using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudEng.InvoiceBuilder.Workflow.Messages;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CloudEng.InvoiceBuilder.Host {
  public class DownloadReportFunction {
    private readonly IMediator _mediator;

    public DownloadReportFunction(IMediator mediator) {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function(nameof(DownloadReportFunction))]
    public async Task<FunctionResponse> BuildInvoiceAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "build")]
      HttpRequestData request, FunctionContext context) {
      var logger = context.GetLogger(nameof(DownloadReportFunction));
      var currentDate = DateTime.Now;
      var previousMonth = currentDate.AddMonths(-1);

      logger.LogInformation("Function triggered at {Date}, invoice will be built for {Month}", currentDate, previousMonth.ToString("M"));

      var reportData = await _mediator.Send(new BuildInvoiceCommand {Date = currentDate}).ConfigureAwait(false);

      return new FunctionResponse {
        HttpResponse = request.CreateResponse(HttpStatusCode.OK),
        FileContent = FormatCsvContent(reportData)
      };
    }

    private static string FormatCsvContent(ReportData reportData) {
      var sb = new StringBuilder();
      foreach (var dayEntry in reportData.DayEntries.OrderBy(d => d.Day)) {
        sb.AppendLine($"{dayEntry.Day},\"{dayEntry.Description}\"");
      }
      return sb.ToString();
    }
  }
}