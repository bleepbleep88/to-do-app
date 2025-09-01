using Microsoft.AspNetCore.Mvc;

namespace PlannerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new 
        { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}