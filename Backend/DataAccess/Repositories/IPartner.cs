using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Models;

namespace Backend.DataAccess.Repositories;

public interface IPartner : IGenericRepository<Partner>
{
    Task<IEnumerable<PartnerResponse>> GetPartnerWithPolicies();
    Task<PartnerResponse> GetPartnerWithPoliciesById(int id);
    Task<IEnumerable<PartnerResponse>> GetPartnersForPolicy(string policyNumber);
    Task<Partner> InsertPartner(PartnerRequest partnerRequest);
    Task<int> SoftDeletePartner(string externalCode, string deletedByUser);
    Task<int> UpdatePartner(int id, PartnerRequest request);
}
