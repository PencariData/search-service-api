using FluentValidation;
using SearchService.API.Responses;
using SearchService.Application.Exceptions;

namespace SearchService.API.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed");

            var response = ApiResponse<string>.Fail(
                ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"),
                "Validation failed",
                context.TraceIdentifier
            );

            await WriteResponse(context, StatusCodes.Status400BadRequest, response);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found");

            var response = ApiResponse<string>.Fail(ex.Message, context.TraceIdentifier);
            await WriteResponse(context, StatusCodes.Status404NotFound, response);
        }
        catch (UnauthorizedException ex)
        {
            logger.LogWarning(ex, "Unauthorized request");

            var response = ApiResponse<string>.Fail(ex.Message, context.TraceIdentifier);
            await WriteResponse(context, StatusCodes.Status401Unauthorized, response);
        }
        catch (ForbiddenException ex)
        {
            logger.LogWarning(ex, "Forbidden request");

            var response = ApiResponse<string>.Fail(ex.Message, context.TraceIdentifier);
            await WriteResponse(context, StatusCodes.Status403Forbidden, response);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            logger.LogError(ex, "Unhandled exception with TraceId {TraceId}", traceId);

            var response = ApiResponse<string>.FromException(ex, traceId, "An unexpected error occurred.");
            await WriteResponse(context, StatusCodes.Status500InternalServerError, response);
        }
    }
    
    private static async Task WriteResponse<T>(HttpContext context, int statusCode, ApiResponse<T> response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}
