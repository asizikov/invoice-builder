using System.Threading.Tasks;
using Pulumi;

namespace CloudEng.InvoiceBuilder.Infrastructure.Tests {
  public static class TestingExtensions {
    public static Task<T> GetValueAsync<T>(this Output<T> output) {
      var tcs = new TaskCompletionSource<T>();
      output.Apply(v => {
        tcs.SetResult(v);
        return v;
      });
      return tcs.Task;
    }
  }
}