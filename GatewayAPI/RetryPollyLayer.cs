using System.Net;
using Polly;
using Polly.Retry;

namespace GatewayAPI;

public class RetryPollyLayer : IRetryPollyLayer
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    
    public RetryPollyLayer()
    {
        _httpClient = new HttpClient();
        
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                onRetry: (response, delay, retryCount, context) => {
                    Console.WriteLine($"Retry {retryCount} after {delay.TotalSeconds} seconds with status code {response.Result.StatusCode}");
                });
    }

    public async Task<HttpResponseMessage> GetAsyncWithRetry(Uri url)
    {
        return await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
    }

    public async Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value)
    {
        return await _retryPolicy.ExecuteAsync(() => _httpClient.PutAsync(url, value));
    }

    public async Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url)
    {
        return await _retryPolicy.ExecuteAsync(() => _httpClient.DeleteAsync(url));
    }

    public async Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value)
    {
        return await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(url, value));
    }
}