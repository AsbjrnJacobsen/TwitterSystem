using System.Net;
using System.Net.Http.Headers;
using Polly;
using Polly.Retry;

namespace GatewayAPI;

public class RetryPollyLayer : IRetryPollyLayer
{
    private readonly HttpClient _httpClient;
    public RetryPollyLayer()
    {
        _httpClient = new HttpClient();
    }

    public AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(params HttpStatusCode[] permittedStatusCodes)
    {
        return Policy           // On all other status codes than the ones supplied, the retry policy will trigger.
            .HandleResult<HttpResponseMessage>(r => !permittedStatusCodes.Contains(r.StatusCode))
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                onRetry: (response, delay, retryCount, context) => {
                    Console.WriteLine($"Retry {retryCount} after {delay.TotalSeconds} seconds with status code {response.Result.StatusCode}");
                });
    }

    public async Task<HttpResponseMessage> GetAsyncWithRetry(Uri url, AuthenticationHeaderValue authHeader,
        params HttpStatusCode[] permittedStatusCodes)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, url);
        requestMsg.Headers.Authorization = authHeader;
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.SendAsync(requestMsg));
    }

    public async Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value, AuthenticationHeaderValue authHeader, params HttpStatusCode[] permittedStatusCodes)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Put, url);
        requestMsg.Headers.Authorization = authHeader;
        requestMsg.Content = value;
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.SendAsync(requestMsg));
    }

    public async Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url, AuthenticationHeaderValue authHeader, params HttpStatusCode[] permittedStatusCodes)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Delete, url);
        requestMsg.Headers.Authorization = authHeader;
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.SendAsync(requestMsg));
    }

    public async Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value, AuthenticationHeaderValue authHeader, params HttpStatusCode[] permittedStatusCodes)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Post, url);
        requestMsg.Headers.Authorization = authHeader;
        requestMsg.Content = value;
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.SendAsync(requestMsg));
    }
}