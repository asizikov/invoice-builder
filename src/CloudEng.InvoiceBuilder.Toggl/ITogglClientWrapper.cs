using System;
using System.Collections.Generic;
using Toggl.Api.DataObjects;

namespace CloudEng.InvoiceBuilder.Toggl {
  public interface ITogglClientWrapper {
    IAsyncEnumerable<ReportTimeEntry> GetDetailedReportAsync(DateTime requestDate);
  }
}