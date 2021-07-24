using CloudEng.InvoiceBuilder.Toggl.DependencyInjection;
using CloudEng.InvoiceBuilder.Workflow.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CloudEng.InvoiceBuilder.Workflow.DependencyInjection {
  public static class WorkflowServiceCollectionExtensions {
    public static IServiceCollection UseInvoiceBuilderWorkflow(this IServiceCollection services) {
      services.UseTogglService()
        .AddMediatR(typeof(BuildInvoiceCommand));
      return services;
    }
  }
}