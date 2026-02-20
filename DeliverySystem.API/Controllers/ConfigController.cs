using DeliverySystem.API.DTOs;
using DeliverySystem.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly DeliverySystemConfiguration _config;

    public ConfigController(DeliverySystemConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public ActionResult<ConfigResponse> GetConfig()
    {
        return Ok(new ConfigResponse(
            _config.SystemName,
            _config.MaxDeliveryDistanceKm,
            _config.DefaultCurrency,
            _config.MaxOrderItems));
    }
}
