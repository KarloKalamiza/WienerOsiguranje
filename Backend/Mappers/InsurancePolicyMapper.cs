using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Models;

namespace Backend.Mappers;

public static class InsurancePolicyMapper
{
    public static InsurancePolicy MapToInsurancePolicy(InsurancePolicyRequest request)
    {
        return new InsurancePolicy()
        {
            PolicyAmount = request.PolicyAmount,
            PolicyNumber = request.PolicyNumber,
        };
    }

    public static InsurancePolicyResponse MapToInsurancePolicyResponse(InsurancePolicy policy)
    {
        return new InsurancePolicyResponse()
        {
            PolicyAmount = policy.PolicyAmount,
            PolicyNumber = policy.PolicyNumber,
        };
    }
}
