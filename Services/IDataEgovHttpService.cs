using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using opendata_api.Exceptions;

namespace opendata_api.Services;

public interface IDataEgovHttpService
{
    Task<TResponse> SendHttpRequestAsync<TResponse>(string endpoint, HttpMethod httpMethod, string token = null, object requestData = null)
        where TResponse : class;
}

public class DataEgovHttpService : IDataEgovHttpService
{
    private readonly HttpClient _httpClient;

    public DataEgovHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<TResponse> SendHttpRequestAsync<TResponse>(string endpoint, HttpMethod httpMethod, string token = null, object requestData = null) where TResponse : class
    {
        var request = new HttpRequestMessage(httpMethod, endpoint);
        if (token != null)
        {
            token = token.Replace("Bearer ", "");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (requestData != null)
        {
            var json = JsonConvert.SerializeObject(requestData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }
        catch (System.Net.Http.HttpRequestException)
        {
            return null;
        }
        catch (System.TimeoutException)
        {
            return null;
        }
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject(content, typeof(TResponse), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return (TResponse)result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new ServerErrorException(content);
        }
    }
}