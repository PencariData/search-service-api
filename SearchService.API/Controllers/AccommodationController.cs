using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dtos;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccommodationController : ControllerBase
{
    private readonly IAccommodationService _accommodationService;

    public AccommodationController(IAccommodationService accommodationService)
    {
        _accommodationService = accommodationService;
    }

    [HttpPost("/accommodations")]
    [ProducesResponseType(typeof(ApiResponse<List<AccommodationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Accommodations([FromBody] GetAccommodationRequest request)
    {
        var result = await _accommodationService.SearchAccommodationAsync(request);
    
        return Ok(ApiResponse<List<AccommodationDto>>.Ok(result));
    }
}