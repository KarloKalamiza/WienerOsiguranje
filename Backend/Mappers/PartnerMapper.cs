using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Models;

namespace Backend.Mappers;

public class PartnerMapper
{
    public static Partner MapToPartner(PartnerRequest partnerRequest)
    {
        return new Partner
        {
            FirstName = partnerRequest.FirstName,
            LastName = partnerRequest.LastName,
            Address = partnerRequest.Address,
            PartnerNumber = partnerRequest.PartnerNumber,
            CroatianPIN = partnerRequest.CroatianPIN,
            PartnerTypeId = partnerRequest.PartnerTypeId,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUser = partnerRequest.CreatedByUser,
            IsForeign = partnerRequest.IsForeign,
            ExternalCode = partnerRequest.ExternalCode,
            Gender = partnerRequest.Gender
        };
    }

    public static PartnerResponse MapToPartnerResponse(Partner partner, List<InsurancePolicy?> policies)
    {
        return new PartnerResponse
        {
            FullName = $"{partner.FirstName} {partner.LastName}",
            Address = partner.Address,
            PartnerNumber = partner.PartnerNumber,
            CroatianPIN = partner.CroatianPIN,
            PartnerTypeId = partner.PartnerTypeId,
            CreatedAtUtc = partner.CreatedAtUtc,
            CreatedByUser = partner.CreatedByUser,
            IsForeign = partner.IsForeign,
            ExternalCode = partner.ExternalCode,
            Gender = partner.Gender,
            Policies = policies.Select(policy => new InsurancePolicyResponse
            {
                PolicyNumber = policy!.PolicyNumber,
                PolicyAmount = policy.PolicyAmount
            }).ToList()
        };
    }
}