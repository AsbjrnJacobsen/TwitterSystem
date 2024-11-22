using System.Net;
using Polly.Retry;

namespace GatewayAPI;

public interface IRetryPollyLayer
{
    AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(params HttpStatusCode[] permittedStatusCodes);
    Task<HttpResponseMessage> GetAsyncWithRetry(Uri url, params HttpStatusCode[] permittedStatusCodes);
    Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value, params HttpStatusCode[] permittedStatusCodes);
    Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url, params HttpStatusCode[] permittedStatusCodes);
    Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value, params HttpStatusCode[] permittedStatusCodes);
}