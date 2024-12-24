using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.DataAccess.Repositories
{
    public class PolicyRepository : IPolicy
    {
        private readonly SqlConnection _sqlConnection;
        private readonly IConfiguration _configuration;

        public PolicyRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public Task<InsurancePolicy> Add(InsurancePolicy model)
        {
            throw new NotImplementedException();
        }

        public Task<InsurancePolicy> Find(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InsurancePolicy>> Get()
        {
            try
            {
                var query = "SELECT * FROM InsurancePolicy";
                IEnumerable<InsurancePolicy> policies = await _sqlConnection.QueryAsync<InsurancePolicy>(query) ?? [];
                return policies;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<InsurancePolicy> GetPolicyByPolicyNumber(string policyNumber)
        {
            try
            {
                var query = "SELECT * FROM InsurancePolicy WHERE PolicyNumber = @PolicyNumber";
                InsurancePolicy? policy = await _sqlConnection.QuerySingleAsync<InsurancePolicy>(query, new { PolicyNumber = policyNumber });
                if (policy is null)
                    throw new KeyNotFoundException($"Policy with policy number: {policyNumber} does not exist.");
                return policy;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<int> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<InsurancePolicy> Update(InsurancePolicy model)
        {
            throw new NotImplementedException();
        }
    }
}
