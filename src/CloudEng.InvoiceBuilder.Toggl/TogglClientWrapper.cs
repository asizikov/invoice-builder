﻿using System;
using System.Collections.Generic;
using CloudEng.InvoiceBuilder.Toggl.Configuration;
using Microsoft.Extensions.Options;
using Toggl.Api;
using Toggl.Api.DataObjects;
using Toggl.Api.QueryObjects;

namespace CloudEng.InvoiceBuilder.Toggl {
  public class TogglClientWrapper {
    private readonly IOptions<TogglConfiguration> _options;

    public TogglClientWrapper(IOptions<TogglConfiguration> options) {
      _options = options;
    }

    public async IAsyncEnumerable<ReportTimeEntry> GetDetailedReportAsync() {
      var togglClient = new TogglClient(_options.Value.ApiKey);
      var completed = false;
      var enumerated = 0;

      for (var page = 1; !completed; page++) {
        var detailedReport = await togglClient.Reports.GetFullDetailedReportAsync(BuildRequestParameters(page)).ConfigureAwait(false);
        foreach (var entry in detailedReport.Data) {
          enumerated++;
          yield return entry;
        }

        if (enumerated >= detailedReport.TotalCount) {
          completed = true;
        }
      }
    }

    private DetailedReportParams BuildRequestParameters(int page) {
      var today = DateTime.Today;
      var currentMonths = new DateTime(today.Year, today.Month, 1);

      return new DetailedReportParams {
        WorkspaceId = _options.Value.WorkspaceId,
        Since = currentMonths.AddMonths(-1).Date.ToString("yyyy-MM-dd"),
        Until = currentMonths.AddDays(-1).Date.ToString("yyyy-MM-dd"),
        UserAgent = "report-converter",
        Page = page
      };
    }
  }
}