using Frontend.Responses;

namespace Frontend.Errors;

public static class ErrorHandler
{
    private static readonly string _uniqueKeyPartnerTable = "UQ__Partner__A93D26343E93CBEC";
    private static readonly string _uniqueKeyPolicyTable = "UQ__Insuranc__46DA0157D58AC148";

    public static ServiceResponse HandleUniqueError(string errorDetails)
    {
        if (errorDetails.Contains(_uniqueKeyPartnerTable))
        {
            errorDetails = "External number or PIN should be unique field. Partner with this external or PIN code already exists and please enter new value.";
        }

        if (errorDetails.Contains(_uniqueKeyPolicyTable))
        {
            errorDetails = "Policy number should be unique field. Policy with this policy number already exists and please enter new value.";
        }

        return new ServiceResponse 
        { 
            Success = false,
            ErrorMessage = errorDetails
        };
    }
}
