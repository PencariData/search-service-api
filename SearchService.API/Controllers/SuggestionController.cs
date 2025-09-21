using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SuggestionController
    (ISearchSuggestionService suggestionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetSuggestionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Suggestion([FromQuery] GetSuggestionRequest request)
    {
        var result = await suggestionService.GetSuggestionsAsync(request);
        
        return Ok(ApiResponse<GetSuggestionResponse>.Ok(result));
    }
    
    [HttpPost("click")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult AccommodationClick([FromBody] PostAccommodationClickRequest request)
    {
        throw new NotImplementedException();
    }
}