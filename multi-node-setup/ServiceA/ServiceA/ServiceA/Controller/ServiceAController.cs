using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controller;

[ApiController]
[Route("api/[controller]")]
public class ServiceAController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public ServiceAController(IHttpClientFactory clientFactory)
    {
        _httpClientFactory = clientFactory;
    }
    
    [HttpGet("call-serviceb")]
    public async Task<IActionResult> CallServiceB()
    {
        await Task.Delay(300);
        var client = _httpClientFactory.CreateClient("ServiceBClient");
        var response = await client.GetAsync("api/serviceb");
        var content = await response.Content.ReadAsStringAsync();
        await Task.Delay(250);
        return Ok(content);
    }
}