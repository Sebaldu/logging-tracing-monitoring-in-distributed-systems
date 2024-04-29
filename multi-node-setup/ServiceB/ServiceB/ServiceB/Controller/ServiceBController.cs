using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace ServiceB.Controller;

[ApiController]
[Route("api/[controller]")]
public class ServiceBController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(500);
        return Ok("Data from ServiceB");
    }
}