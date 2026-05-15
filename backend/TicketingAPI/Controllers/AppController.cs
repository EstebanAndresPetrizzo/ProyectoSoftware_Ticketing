using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProyectoSoftware_Ticketing.DTOs.App;
using ProyectoSoftware_Ticketing.DTOs.Common;
using TicketingAPI.Configuration;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/v1/app")]
public class AppController : ControllerBase
{
    private readonly AppBrandingOptions _branding;

    public AppController(IOptions<AppBrandingOptions> branding)
    {
        _branding = branding.Value;
    }

    /// <summary>
    /// Obtiene la información de branding de la aplicación, como nombre y email de contacto.
    /// </summary>
    [HttpGet("branding")]
    public ActionResult<ApiResponse<AppBrandingDto>> GetBranding()
    {
        return Ok(new ApiResponse<AppBrandingDto>
        {
            Success = true,
            Data = new AppBrandingDto
            {
                Name = string.IsNullOrWhiteSpace(_branding.Name) ? "TicketingApp" : _branding.Name.Trim(),
                ContactEmail = _branding.ContactEmail?.Trim() ?? ""
            }
        });
    }
}
