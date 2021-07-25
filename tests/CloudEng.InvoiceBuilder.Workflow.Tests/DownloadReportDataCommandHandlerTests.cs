using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CloudEng.InvoiceBuilder.Toggl;
using CloudEng.InvoiceBuilder.Workflow.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Toggl.Api.DataObjects;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace CloudEng.InvoiceBuilder.Workflow.Tests {
  [SuppressMessage("ReSharper", "ConsiderUsingConfigureAwait")]
  public class DownloadReportDataCommandHandlerTests {
    private readonly Mock<ITogglClientWrapper> _togglClientWrapperMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    [Fact]
    public async Task ReportData_Create() {
      var reportDate = DateTime.Today;

      var report = new List<ReportTimeEntry> {
        new() {Start = "2021-01-02", Description = "10"}
      }.ToAsyncEnumerable();

      _togglClientWrapperMock.Setup(c => c.GetDetailedReportAsync(reportDate))
        .Returns(report);

      var handler = new DownloadReportDataCommandHandler(_togglClientWrapperMock.Object, NullLogger<DownloadReportDataCommandHandler>.Instance);

      var command = new DownloadReportDataCommand {
        Date = reportDate
      };

      var reportData = await handler.Handle(command, _token);

      reportData.ShouldSatisfyAllConditions(
        () => reportData.ShouldNotBeNull(),
        () => reportData.DayEntries.ShouldNotBeNull(),
        () => reportData.DayEntries.Count.ShouldBe(1),
        () => reportData.DayEntries[0].Day.ShouldBe(2),
        () => reportData.DayEntries[0].Description.ShouldBe("10")
      );
    }
  }
}