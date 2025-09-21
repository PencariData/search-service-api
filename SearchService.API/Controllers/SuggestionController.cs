using Microsoft.AspNetCore.Mvc;
using SearchService.API.Responses;
using SearchService.Application.Dto;
using SearchService.Application.Interfaces.Services;

namespace SearchService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SuggestionController (
    ISuggestionSearchService suggestionSearchService,
    ISuggestionInteractionService suggestionInteractionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetSuggestionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Suggestion([FromQuery] GetSuggestionRequest request)
    {
        var result = await suggestionSearchService.GetSuggestionsAsync(request);
        
        return Ok(ApiResponse<GetSuggestionResponse>.Ok(result));
    }
    
    [HttpPost("click")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult AccommodationClick([FromBody] PostSuggestionClickRequest request)
    { 
        //fire-and-forget: enqueue the click event
        _ = suggestionInteractionService.RegisterClickAsync(request); 

        // return immediately
        return Accepted();
    }
}