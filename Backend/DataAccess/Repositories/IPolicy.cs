using Backend.Models;

namespace Backend.DataAccess.Repositories;

public interface IPolicy : IGenericRepository<InsurancePolicy>
{
    Task<InsurancePolicy> GetPolicyByPolicyNumber(string policyNumber);
}
