using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Toggl.Api.DataObjects;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace CloudEng.InvoiceBuilder.Toggl.Tests {
  public class TimeEntriesCollectorTests {
    [Theory]
    [MemberData(nameof(GroupingTestCases))]
    public async Task TimeSeries_Matched_Grouped_By_Day(GroupingTestCase testCase) {
      var timeEntries = testCase.TimeEntries;

      var counter = 0;
      var expectedStrings = testCase.ExpectedResults;
      await foreach (var str in timeEntries.ToAsyncEnumerable().ToDaysAsync()) {
        expectedStrings[counter++].ShouldBe(str);
      }
      counter.ShouldBe(testCase.ExpectedResults.Length);
    }

    public static TheoryData<GroupingTestCase> GroupingTestCases => new() {
      new() {
        TimeEntries = new List<ReportTimeEntry> {
          new() {
            Start = "2021-07-01",
            Description = "1-0"
          },
          new() {
            Start = "2021-07-01",
            Description = "1-1"
          },
          new() {
            Start = "2021-07-02",
            Description = "2-0"
          }
        },
        ExpectedResults = new[] {(1, "1-0,1-1"), (2, "2-0")}
      },
      new() {
        TimeEntries = new List<ReportTimeEntry> {
          new() {
            Start = "2021-07-03",
            Description = "3-0"
          },
          new() {
            Start = "2021-07-28",
            Description = "28-0"
          },
          new() {
            Start = "2021-07-31",
            Description = "31-0"
          }
        },
        ExpectedResults = new[] {(3, "3-0"), (28, "28-0"), (31, "31-0")}
      },
      new() {
        TimeEntries = new List<ReportTimeEntry> {
          new() {
            Start = "2021-07-03",
            Description = "3-0"
          },
        },
        ExpectedResults = new[] {(3, "3-0")}
      }
    };

    public class GroupingTestCase {
      public List<ReportTimeEntry> TimeEntries { get; set; }
      public (int, string)[] ExpectedResults { get; set; }
    }
  }
}