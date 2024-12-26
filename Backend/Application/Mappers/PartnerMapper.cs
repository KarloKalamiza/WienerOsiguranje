using Backend.Core.Domain.Enums;
using Backend.Core.Domain.Models;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Data.Responses;

namespace Backend.Application.Mappers;

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
            Gender = partnerRequest.Gender.ToString(),
        };
    }

    public static DeletedPartner MapToDeletedPartner(Partner partner, string deletedByUser)
    {
        return new DeletedPartner
        {
            PartnerId = partner.PartnerId,
            FirstName = partner.FirstName,
            LastName = partner.LastName,
            Address = partner.Address,
            PartnerNumber = partner.PartnerNumber,
            CroatianPIN = partner.CroatianPIN,
            PartnerTypeId = partner.PartnerTypeId,
            CreatedAtUtc = partner.CreatedAtUtc,
            CreatedByUser = partner.CreatedByUser,
            IsForeign = partner.IsForeign,
            ExternalCode = partner.ExternalCode,
            Gender = partner.Gender.ToString(),
            DeletedByUser = deletedByUser,
            DeletedAtUtc = DateTime.UtcNow,
        };
    }

    private static string FormatGenderDatabaseSaving(GenderType gender)
    {
        return gender switch
        {
            GenderType.M => "M",
            GenderType.F => "F",
            GenderType.N => "N",
            _ => "Unknown"
        };
    }

    public static PartnerResponse MapToPartnerResponse(Partner partner, List<InsurancePolicy?>? policies)
    {
        return new PartnerResponse
        {
            PartnerId = partner.PartnerId,
            FullName = $"{partner.FirstName} {partner.LastName}",
            Address = partner.Address,
            PartnerNumber = partner.PartnerNumber,
            CroatianPIN = partner.CroatianPIN,
            PartnerTypeId = FormatPartnerType(partner.PartnerTypeId),
            CreatedAtUtc = partner.CreatedAtUtc,
            CreatedByUser = partner.CreatedByUser,
            IsForeign = partner.IsForeign,
            ExternalCode = partner.ExternalCode,
            Gender = FormatGender(partner.Gender),
            Policies = policies?.Select(policy => new InsurancePolicy
            {
                InsurancePolicyId = policy!.InsurancePolicyId,
                PolicyNumber = policy!.PolicyNumber,
                PolicyAmount = policy.PolicyAmount
            }).ToList()
        };
    }

    private static string FormatPartnerType(PartnerType partnerTypeId)
    {
        return partnerTypeId switch
        {
            PartnerType.Legal => "Legal",
            PartnerType.Personal => "Personal",
            _ => "Unknown"
        };
    }

    private static string FormatGender(string? gender)
    {
        return gender switch
        {
            "M" => "Male",
            "F" => "Female",
            "N" => "Other",
            _ => "Unknown"
        };
    }


}