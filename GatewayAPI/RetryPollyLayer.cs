using System.Net;
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

    public async Task<HttpResponseMessage> GetAsyncWithRetry(Uri url, params HttpStatusCode[] permittedStatusCodes)
    {
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.GetAsync(url));
    }

    public async Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value, params HttpStatusCode[] permittedStatusCodes)
    {
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.PutAsync(url, value));
    }

    public async Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url, params HttpStatusCode[] permittedStatusCodes)
    {
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.DeleteAsync(url));
    }

    public async Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value, params HttpStatusCode[] permittedStatusCodes)
    {
        return await CreateRetryPolicy(permittedStatusCodes).ExecuteAsync(() => _httpClient.PostAsync(url, value));
    }
}