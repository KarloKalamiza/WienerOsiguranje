using Frontend.Responses;

namespace Frontend.Errors;

public static class ErrorHandler
{
    private static readonly string _uniqueKey = "UQ__Partner__A93D26343E93CBEC";

    public static ServiceResponse HandleUniqueError(string errorDetails)
    {
        if (errorDetails.Contains(_uniqueKey))
        {
            errorDetails = "External number should be unique field. Partner with this external code already exists and please enter new value.";
        }

        return new ServiceResponse 
        { 
            Success = false,
            ErrorMessage = errorDetails
        };
    }
}
