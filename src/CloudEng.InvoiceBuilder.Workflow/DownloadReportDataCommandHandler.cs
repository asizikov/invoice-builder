using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.InvoiceBuilder.Toggl;
using CloudEng.InvoiceBuilder.Workflow.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudEng.InvoiceBuilder.Workflow {
  public class DownloadReportDataCommandHandler : IRequestHandler<DownloadReportDataCommand, ReportData> {
    private readonly ITogglClientWrapper _togglClient;
    private readonly ILogger<DownloadReportDataCommandHandler> _logger;

    public DownloadReportDataCommandHandler(ITogglClientWrapper togglClient, ILogger<DownloadReportDataCommandHandler> logger) {
      _togglClient = togglClient ?? throw new ArgumentNullException(nameof(togglClient));
      _logger = logger;
    }

    public async Task<ReportData> Handle(DownloadReportDataCommand request, CancellationToken cancellationToken) {
      _logger.LogInformation("Ready to download report data for {Month}", request.Date.ToString("M"));

      var result = new ReportData();
      await foreach (var dayEntry in _togglClient.GetDetailedReportAsync(request.Date).ToDaysAsync()) {
        _logger.LogInformation("{Day}: {LoggedWork}", dayEntry.Day, dayEntry.Description);
        result.DayEntries.Add(new DayEntry {
          Day = dayEntry.Day,
          Description = dayEntry.Description
        });
      }

      return result;
    }
  }
}