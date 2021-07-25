using System;
using System.Collections.Generic;
using Toggl.Api.DataObjects;

namespace CloudEng.InvoiceBuilder.Toggl {
  public static class TimeEntriesCollector {
    public static async IAsyncEnumerable<(int Day, string Description)> ToDaysAsync(this IAsyncEnumerable<ReportTimeEntry> timeEntries) {

      var days = new List<string>[32];
      var currentDay = 0;

      await foreach (var reportTimeEntry in timeEntries) {
        if (DateTime.TryParse(reportTimeEntry.Start, out var startDateTime)) {
          var day = startDateTime.Day;
          if (day == currentDay) {
            AppendDescription(day, reportTimeEntry);
          }
          else {
            if (days[currentDay] is not null && days[currentDay].Count > 0) {
              yield return (currentDay , string.Join(',', days[currentDay]));
            }
            currentDay = day;
            AppendDescription(day, reportTimeEntry);
          }
        }
      }

      if (days[currentDay] is not null && days[currentDay].Count > 0) {
        yield return (currentDay, string.Join(',', days[currentDay]));
      }

      void AppendDescription(int day, ReportTimeEntry reportTimeEntry) {
        days[day] ??= new List<string>();
        days[day].Add(reportTimeEntry.Description);
      }
    }
  }
}