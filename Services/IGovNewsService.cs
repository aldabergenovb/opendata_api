using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using opendata_api.Exceptions;
using opendata_api.Models;
using opendata_api.Services.Policies;

namespace opendata_api.Services;

public interface IGovHttpService
{
    Task<NewsModel.RssFeed> SendHttpRequest<TResponse>(string _api, HttpMethod _httpMethod, string _token = null, object _requestData = null) where TResponse : class;
}

public class GovHttpService : IGovHttpService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientPolicy _clientPolicy;

    public GovHttpService(HttpClient httpClient, HttpClientPolicy clientPolicy)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
                (_, __, ___, ____) => true
        };
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = httpClient.BaseAddress
        };
        _clientPolicy = clientPolicy;
    }

    public async Task<NewsModel.RssFeed> SendHttpRequest<TResponse>(string _api, HttpMethod _httpMethod,
        string _token = null, object _requestData = null) where TResponse : class
    {
        var request = new HttpRequestMessage(_httpMethod, _api);
        if (_token != null)
        {
            _token = _token.Replace("Bearer ", "");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        if (_requestData != null)
        {
            var json = JsonConvert.SerializeObject(_requestData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await _clientPolicy.ExponentialHttpRetry.ExecuteAsync(() =>
                _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead));
        }
        catch (System.Net.Http.HttpRequestException)
        {
            throw new HttpRequestException();
        }
        catch (System.TimeoutException)
        {
            throw new TimeoutException();
        }

        if (response.IsSuccessStatusCode)
        {
            var contentStream = await response.Content.ReadAsStringAsync();
            XmlSerializer serializer = new XmlSerializer(typeof(NewsModel.RssFeed));

            using StringReader reader = new StringReader(contentStream);
            var rssFeed = (NewsModel.RssFeed)serializer.Deserialize(reader)!;
            // var result = JsonConvert.DeserializeObject(contentStream, typeof(TResponse), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return rssFeed;
        }
        else
        {
            throw new Exception();
        }
    }
}