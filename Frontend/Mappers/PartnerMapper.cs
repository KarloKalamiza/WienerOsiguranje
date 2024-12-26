using Frontend.DTO;
using Frontend.Models;

namespace Frontend.Mappers;

public static class PartnerMapper
{
    public static PartnerDTO MapToDTO(Partner partner)
    {
        return new PartnerDTO()
        {
            PartnerId = partner.PartnerId,
            Address = partner.Address,
            CreatedAtUtc = partner.CreatedAtUtc,
            CreatedByUser = partner.CreatedByUser,
            CroatianPIN = FormatCroatianPIN(partner.CroatianPIN),
            ExternalCode = partner.ExternalCode,
            FullName = partner.FullName,
            Gender = partner.Gender,
            IsForeign = partner.IsForeign,
            PartnerNumber = partner.PartnerNumber,
            PartnerTypeId = partner.PartnerTypeId,
            Policies = partner.Policies,
            SpecialPartnerSign = IsSpecialUser(partner.Policies, partner.FullName)
        };
    }

    private static string FormatCroatianPIN(string croatianPIN)
    {
        if (string.IsNullOrEmpty(croatianPIN))
            return "N/A";
        return croatianPIN;
    }

    private static string IsSpecialUser(List<InsurancePolicy> policies, string fullName)
    {
        bool policyCountCondition = policies.Count > 5;
        bool policyAmountCondition = policies.Sum(policy => policy.PolicyAmount) > 5000;
        if (policyCountCondition || policyAmountCondition)
        {
            return $"* {fullName}";
        }
        return fullName;
    }
}
