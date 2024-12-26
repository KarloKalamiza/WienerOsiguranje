using Backend.Core.Domain.Models;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Data.Responses;

namespace Backend.Application.Mappers;

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
