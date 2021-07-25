using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CloudEng.InvoiceBuilder.Host {
  public class FunctionResponse {
    public HttpResponseData HttpResponse { get; set; }
    [BlobOutput("invoice-data/{DateTime:yyyy}/{DateTime:M}/output-{DateTime:yyyy-MM-dd}.csv")]
    public string FileContent { get; set; }
  }
}