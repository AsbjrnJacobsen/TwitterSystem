namespace GatewayAPI;

public interface IRetryPollyLayer
{
    Task<HttpResponseMessage> GetAsyncWithRetry(Uri url);
    Task<HttpResponseMessage> PutAsyncWithRetry(Uri url, HttpContent value);
    Task<HttpResponseMessage> DeleteAsyncWithRetry(Uri url);
    Task<HttpResponseMessage> PostAsyncWithRetry(Uri url, HttpContent value);
}