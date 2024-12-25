using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Models;

namespace Backend.DataAccess.Repositories;

public interface IPolicy : IGenericRepository<InsurancePolicy>
{
    Task<InsurancePolicy> GetPolicyByPolicyNumber(string policyNumber);
    Task<InsurancePolicy> InsertPolicy(InsurancePolicyRequest insurancePolicyRequest);
    Task<PartnerResponse> CreatePolicyForPartner(InsurancePolicyRequest insurancePolicyRequest, string externalCode);
    Task<int> RemovePolicyByPolicyNumber(string policyNumber);
}
