using System.Collections.Generic;

namespace CloudEng.InvoiceBuilder.Workflow.Messages {
  public class ReportData {
    public List<DayEntry> DayEntries { get; } = new List<DayEntry>();
  }
}