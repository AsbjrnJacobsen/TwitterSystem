using System.Net;
using System.Net.Http.Headers;
using Polly.Retry;

namespace GatewayAPI;

public interface IRetryPollyLayer
{
    AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(params HttpStatusCode[] permittedStatusCodes);

    Task<HttpResponseMessage> GetAsyncWithRetry(Uri url, AuthenticationHeaderValue authHeader,
        params HttpStatusCode[] permittedStatusCodes);

    Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value, AuthenticationHeaderValue authHeader,
        params HttpStatusCode[] permittedStatusCodes);

    Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url, AuthenticationHeaderValue authHeader,
        params HttpStatusCode[] permittedStatusCodes);

    Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value, AuthenticationHeaderValue authHeader,
        params HttpStatusCode[] permittedStatusCodes);
}