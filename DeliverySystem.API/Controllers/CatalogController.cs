using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Composite;
using DeliverySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly CatalogProvider _catalogProvider;

    public CatalogController(CatalogProvider catalogProvider)
    {
        _catalogProvider = catalogProvider ?? throw new ArgumentNullException(nameof(catalogProvider));
    }

    [HttpGet]
    public ActionResult<CatalogNodeDto> GetCatalog()
    {
        var root = _catalogProvider.GetRootCatalog();
        var dto = MapToDto(root);
        return Ok(dto);
    }

    private static CatalogNodeDto MapToDto(IProductCatalogComponent component)
    {
        var children = component.GetChildren()
            .Select(MapToDto)
            .ToList();
        return new CatalogNodeDto(
            component.Name,
            component.GetTotalPrice(),
            component.GetTotalWeight(),
            children);
    }
}
