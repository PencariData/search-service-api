using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccommodationController(
    IAccommodationService accommodationService,
    ISearchSuggestionService searchSuggestionService) : ControllerBase
{
    [HttpPost("/accommodations")]
    [ProducesResponseType(typeof(ApiResponse<List<GetAccommodationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Accommodations([FromBody] GetAccommodationRequest request)
    {
        var result = await accommodationService.SearchAccommodationsAsync(request);
    
        return Ok(ApiResponse<List<GetAccommodationResponse>>.Ok(result));
    }

    [HttpPost("/accommodations/suggestion")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AccommodationSuggestion([FromQuery] GetAccommodationSuggestionRequest request)
    {
        var result = await searchSuggestionService.SearchAccommodationSuggestionsAsync(request);
        
        return  Ok(ApiResponse<List<string>>.Ok(result));
    }
}