using System;
using MediatR;

namespace CloudEng.InvoiceBuilder.Workflow.Messages {
  public class BuildInvoiceCommand : IRequest<ReportData> {
    public DateTime Date { get; set; }
  }
}