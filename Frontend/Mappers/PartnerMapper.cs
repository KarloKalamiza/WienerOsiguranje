using Frontend.DTO;
using Frontend.Models;
using Frontend.Request;

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

    public static PartnerRequest MapToRequest(EditPartnerDTO partner)
    {
        return new PartnerRequest()
        {
            FirstName = partner.FirstName,
            LastName = partner.LastName,
            Address = partner.Address,
            CreatedByUser = partner.CreatedByUser,
            ExternalCode = partner.ExternalCode,
            CroatianPIN = partner.CroatianPIN,
            Gender = int.Parse(partner.Gender),
            IsForeign = partner.IsForeign,
            PartnerNumber = partner.PartnerNumber,
            PartnerTypeId = int.Parse(partner.PartnerTypeId)
        };
    }

    public static EditPartnerDTO MapToEditPartnerDTO(Partner partner)
    {
        var (firstname, lastname) = SplitFullName(partner.FullName);

        return new EditPartnerDTO
        {
            FirstName = firstname,
            LastName= lastname,
            Address = partner.Address,
            CreatedAtUtc = partner.CreatedAtUtc,
            CreatedByUser = partner.CreatedByUser,
            CroatianPIN= partner.CroatianPIN,
            ExternalCode= partner.ExternalCode,
            Gender = partner.Gender,
            IsForeign = partner.IsForeign,
            PartnerId = partner.PartnerId,
            PartnerNumber = partner.PartnerNumber,
            PartnerTypeId = partner.PartnerTypeId
        };
    }

    public static (string FirstName, string LastName) SplitFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (string.Empty, string.Empty);
        }

        var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : string.Empty;

        return (firstName, lastName);
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
