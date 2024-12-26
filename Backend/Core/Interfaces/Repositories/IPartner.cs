using Backend.Core.Domain.Models;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Data.Responses;
using FluentResults;

namespace Backend.Core.Interfaces.Repositories;

public interface IPartner : IGenericRepository<Partner>
{
    Task<IEnumerable<PartnerResponse>> GetPartnerWithPolicies();
    Task<PartnerResponse> GetPartnerWithPoliciesById(int id);
    Task<IEnumerable<PartnerResponse>> GetPartnersForPolicy(string policyNumber);
    Task<Partner> InsertPartner(PartnerRequest partnerRequest);
    Task<int> SoftDeletePartner(string externalCode, string deletedByUser);
    Task<int> UpdatePartner(int id, PartnerRequest request);
    Task<IEnumerable<PartnerResponse>> FindPartners(int numOfPolicies, Decimal entirePolicyAmount);
}
