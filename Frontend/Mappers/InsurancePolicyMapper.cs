using Frontend.DTO;
using Frontend.Models;

namespace Frontend.Mappers;

public static class InsurancePolicyMapper
{
    public static EditInsurancePolicyDTO MapToEditInsurancePolicyDTO(InsurancePolicy policy)
    {
        return new EditInsurancePolicyDTO
        {
            InsurancePolicyId = policy.InsurancePolicyId,
            PolicyAmount = policy.PolicyAmount,
            PolicyNumber = policy.PolicyNumber
        };
    }
}
