using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.InvoiceBuilder.Workflow.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudEng.InvoiceBuilder.Workflow {
  public class BuildInvoiceCommandHandler : AsyncRequestHandler<BuildInvoiceCommand> {
    private readonly ILogger<BuildInvoiceCommandHandler> _logger;
    private readonly IMediator _mediator;

    public BuildInvoiceCommandHandler(ILogger<BuildInvoiceCommandHandler> logger, IMediator mediator) {
      _logger = logger;
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task Handle(BuildInvoiceCommand request, CancellationToken cancellationToken) {
      _logger.LogInformation("Invoice creation requested for {Month}", request.Date.Date.ToString("M"));

      var reportData = await _mediator.Send(new DownloadReportDataCommand {Date = request.Date}, cancellationToken)
        .ConfigureAwait(false);

      _logger.LogInformation("Report data successfully downloaded for {Month}", request.Date.ToString("M"));
    }
  }
}