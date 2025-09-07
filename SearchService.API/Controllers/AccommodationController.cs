using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccommodationController(
    IAccommodationService accommodationService) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<GetAccommodationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Accommodations([FromQuery] GetAccommodationRequest request)
    {
        var result = await accommodationService.SearchAccommodationsAsync(request);
    
        return Ok(ApiResponse<GetAccommodationResponse>.Ok(result));
    }

    [HttpPost("click")]
    public Task<IActionResult> AccommodationClick([FromBody] PostAccommodationClickRequest request)
    {
        throw new NotImplementedException();
    }
}