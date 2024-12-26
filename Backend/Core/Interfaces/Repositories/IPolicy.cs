using Backend.Core.Domain.Models;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Data.Responses;

namespace Backend.Core.Interfaces.Repositories;

public interface IPolicy : IGenericRepository<InsurancePolicy>
{
    Task<InsurancePolicy> GetPolicyByPolicyNumber(string policyNumber);
    Task<InsurancePolicy> InsertPolicy(InsurancePolicyRequest insurancePolicyRequest);
    Task<PartnerResponse> CreatePolicyForPartner(InsurancePolicyRequest insurancePolicyRequest, string externalCode);
    Task<int> RemovePolicyByPolicyNumber(string policyNumber);
    Task<int> UpdatePolicy(int id, InsurancePolicyRequest request);
}
