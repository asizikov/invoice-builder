using System;
using MediatR;

namespace CloudEng.InvoiceBuilder.Workflow.Messages {
  public class DownloadReportDataCommand : IRequest<ReportData> {
    public DateTime Date { get; set; }
  }
}