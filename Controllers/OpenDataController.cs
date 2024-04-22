using Microsoft.AspNetCore.Mvc;
using opendata_api.Models;
using opendata_api.Services;

namespace opendata_api.Controllers;

[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("[controller]")]
public class OpenDataController(IDataEgovHttpService service, IConfiguration configuration, IGovHttpService newsService) : ControllerBase
{
    [HttpGet("FindBin")]
    public async Task<IActionResult> FindBin(string bin)
    {
        var api = $"api/v4/gbd_ul/v1?apiKey={configuration.GetValue<string>("StatGov:apiKey")}&source={{ \"query\": {{ \"match\": {{ \"bin\":\"{bin}\" }} }} }}";
        var result = await service.SendHttpRequestAsync<List<GbdUlModel>>(api, HttpMethod.Get);
        return Ok(result);
    }

    [HttpGet("GetPopulation")]
    public async Task<IActionResult> GetPopulationInfo()
    {
        var api = $"api/v4/population_at_the_beginning_of/v2?apiKey={configuration.GetValue<string>("StatGov:apiKey")}&source={{\"size\":100}}";
        var result = await service.SendHttpRequestAsync<List<PopulationModel>>(api, HttpMethod.Get);
        return Ok(result);
    }

    [HttpGet("GetUnemployedPopulation")]
    public async Task<IActionResult> GetUnemployedPopulation()
    {
        var api = $"api/v4/zhumyssyz_halyk_15_zhas_zhane/v3?apiKey={configuration.GetValue<string>("StatGov:apiKey")}&source={{\"size\":100}}";
        var result = await service.SendHttpRequestAsync<List<UnemployedPopulationModel>>(api, HttpMethod.Get);
        return Ok(result);
    }

    [HttpGet("GovNews")]
    public async Task<IActionResult> GetNewsWithRss(string lang)
    {
        var endpoint = $"api/v1/public/rss/astana/news/{lang}";
        var result = await newsService.SendHttpRequest<NewsModel.RssFeed>(endpoint, HttpMethod.Get);
        return Ok(result);
    }
}