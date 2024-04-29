using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace ServiceA.Controller;

[ApiController]
[Route("api/[controller]")]
public class ServiceAController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Tracer _tracer;
    private readonly string _serviceBUrl;
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public ServiceAController(IHttpClientFactory clientFactory, TracerProvider tracerProvider,  IConfiguration configuration)
    {
        _httpClientFactory = clientFactory;
        _tracer = tracerProvider.GetTracer("ServiceATracer");
        // _serviceBUrl = configuration["SERVICEB_URL"] ?? throw new ArgumentException("ServiceB URL must be set");
        _serviceBUrl = "http://localhost:5217";
    }
    
    [HttpGet("call-serviceb")]
    public async Task<IActionResult> CallServiceB()
    {
        var client = _httpClientFactory.CreateClient("ServiceBClient");
        var response = await client.GetAsync("api/serviceb");
        var content = await response.Content.ReadAsStringAsync();
        return Ok(content);
    }
    

    // [HttpGet]
    // public async Task<ActionResult<JsonContent>> GetServiceA()
    // {
    //     using (var span = _tracer.StartActiveSpan("ServiceA-GetServiceA"))
    //     {
    //         try
    //         {
    //             using var requestMessage = new HttpRequestMessage(HttpMethod.Get, _serviceBUrl + "/weatherforecast");
    //                 
    //             // Inject the current trace context into the request headers
    //             Propagator.Inject(new PropagationContext(span.Context, Baggage.Current), requestMessage.Headers, (headers, name, value) => headers.Add(name, value));
// 
    //             var response = await _httpClient.SendAsync(requestMessage);
    //             var content = await response.Content.ReadAsStringAsync();
    //             
    //             if (response.IsSuccessStatusCode)
    //             {
    //                 return Content(content, "application/json");
    //             }
    //             else
    //             {
    //                 return StatusCode((int)response.StatusCode, content);
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             span.RecordException(ex);
    //             span.SetStatus(OpenTelemetry.Trace.Status.Error.WithDescription(ex.Message));
    //             return StatusCode(500, ex.Message);
    //         }
    //         finally
    //         {
    //             span.End();
    //         }
    //     }
    // }
}