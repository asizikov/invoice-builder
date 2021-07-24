using Microsoft.Extensions.DependencyInjection;

namespace CloudEng.InvoiceBuilder.Toggl.DependencyInjection {
  public static class TogglServiceCollectionExtensions {
    public static IServiceCollection UseTogglService(this IServiceCollection services) {
      services.AddScoped<ITogglClientWrapper, TogglClientWrapper>();
      return services;
    }
  }
}