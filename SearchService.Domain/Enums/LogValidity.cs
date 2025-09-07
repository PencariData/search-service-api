namespace SearchService.Domain.Enums;

public enum LogValidity
{
    Valid = 0,       // trusted for analysis
    Suspect = 1,     // keep but don’t use for strict relevance metrics
    Invalid = 2      // broken, should not be analyzed
}