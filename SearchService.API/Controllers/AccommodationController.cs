using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccommodationController(
    IAccommodationService accommodationService,
    IAccommodationInteractionService accommodationInteractionService) : ControllerBase
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
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult AccommodationClick([FromBody] PostAccommodationClickRequest request)
    {
        // fire-and-forget: enqueue the click event
        _ = accommodationInteractionService.RegisterClickAsync(request); 

        // return immediately
        return Accepted();
    }
}