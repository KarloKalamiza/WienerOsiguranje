using Backend.Application.Mappers;
using Backend.Core.Domain.Models;
using Backend.Core.Interfaces.Repositories;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Data.Responses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.Infrastructure.Repositories
{
    public class PolicyRepository : IPolicy
    {
        private readonly SqlConnection _sqlConnection;
        private readonly IConfiguration _configuration;
        private readonly IPartner _partner;

        public PolicyRepository(IConfiguration configuration, IPartner partner)
        {
            _configuration = configuration;
            _sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            _partner = partner;
        }

        public Task<InsurancePolicy> Add(InsurancePolicy model)
        {
            throw new NotImplementedException();
        }

        public async Task<PartnerResponse> CreatePolicyForPartner(InsurancePolicyRequest insurancePolicyRequest, string externalCode)
        {
            IEnumerable<Partner> partnerModels = await _partner.Get();
            Partner? partner = partnerModels.FirstOrDefault(partner => partner.ExternalCode == externalCode);
            if (partner is null)
                throw new NotImplementedException($"Partner with external code: {externalCode} does not exist");
            int partnerId = partner.PartnerId;

            IEnumerable<PartnerResponse> partners = await _partner.GetPartnerWithPolicies();
            PartnerResponse? filteredPartner = partners.FirstOrDefault(partner => partner.ExternalCode == externalCode);
            if (filteredPartner is null)
                throw new NotImplementedException($"Partner with external code: {externalCode} does not exist");

            InsurancePolicy insurancePolicy = await InsertPolicy(insurancePolicyRequest);
            int insuranceId = insurancePolicy.InsurancePolicyId;
            InsurancePolicyResponse insuranceResponse = InsurancePolicyMapper.MapToInsurancePolicyResponse(insurancePolicy);

            InsertIntoPartnerResponse(filteredPartner, insurancePolicy);
            InsertIntoTablePartnerPolicy(partnerId, insuranceId);

            return filteredPartner;
        }

        private async void InsertIntoTablePartnerPolicy(int partnerId, int insurancePolicyId)
        {
            // PROBLEM JE ŠTO JE INSURANCE ID 0
            string query = @"
                INSERT INTO PartnerPolicy (PartnerId, InsurancePolicyId)
                VALUES (@PartnerId, @InsurancePolicyId)";

            int result = await _sqlConnection.ExecuteAsync(query, new { PartnerId = partnerId, InsurancePolicyId = insurancePolicyId });

            if (result == 0)
            {
                throw new Exception($"Failed to insert PartnerPolicy relationship for PartnerId: {partnerId} and InsurancePolicyId: {insurancePolicyId}");
            }
        }

        private void InsertIntoPartnerResponse(PartnerResponse partnerResponse, InsurancePolicy insurance)
        {
            if (partnerResponse.Policies == null)
                partnerResponse.Policies = [];

            partnerResponse.Policies.Add(insurance);
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<InsurancePolicy> InsertPolicy(InsurancePolicyRequest insurancePolicyRequest)
        {
            try
            {
                InsurancePolicy insurancePolicy = InsurancePolicyMapper.MapToInsurancePolicy(insurancePolicyRequest);

                string query = @"
                    INSERT INTO InsurancePolicy
                        (PolicyNumber, PolicyAmount)
                    OUTPUT INSERTED.InsurancePolicyId 
                    VALUES
                        (@PolicyNumber, @PolicyAmount)";

                int generatedId = await _sqlConnection.QuerySingleAsync<int>(query, insurancePolicy);
                if (generatedId == 0)
                    throw new Exception($"Insert into database failed for policy: {insurancePolicy}");

                insurancePolicy.InsurancePolicyId = generatedId;

                return insurancePolicy;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> Remove(int id)
        {
            if (id == 0)
                throw new Exception($"Invalid ID: {id}");

            try
            {
                string query = "DELETE FROM InsurancePolicy WHERE InsurancePolicyId = @InsurancePolicyId";
                int count = await _sqlConnection.ExecuteAsync(query, new { InsurancePolicyId = id });
                if (count == 0)
                    throw new Exception($"Policy with id: {id} was not deleted.");
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<InsurancePolicy> Update(InsurancePolicy model)
        {
            if (model == null)
                throw new Exception("Insurance policy is required to be updated.");

            try
            {
                string updateQuery = "UPDATE InsurancePolicy SET PolicyNumber = @PolicyNumber, PolicyAmount = @PolicyAmount WHERE @InsurancePolicyId = @InsurancePolicyId";
                int count = await _sqlConnection.ExecuteAsync(updateQuery, model);
                if (count == 0)
                    throw new Exception($"Failed to update policy with ID: {model.InsurancePolicyId}");
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> RemovePolicyByPolicyNumber(string policyNumber)
        {
            if (string.IsNullOrEmpty(policyNumber))
                throw new Exception("Policy number is required");

            try
            {
                string query = "DELETE FROM InsurancePolicy WHERE PolicyNumber = @PolicyNumber";
                int count = await _sqlConnection.ExecuteAsync(query, new { PolicyNumber = policyNumber });
                if (count == 0)
                    throw new Exception($"Failed to delete policy with number: {policyNumber}");
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdatePolicy(int id, InsurancePolicyRequest request)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException($"Invalid Id: {id}");
            if (request == null)
                throw new ArgumentOutOfRangeException("Insurance request is required");

            try
            {
                string updateQuery = @"
                    UPDATE InsurancePolicy
                    SET PolicyNumber = @PolicyNumber,
                        PolicyAmount = @PolicyAmount
                    WHERE InsurancePolicyId = @InsurancePolicyId
                      AND NOT EXISTS (
                          SELECT 1
                          FROM InsurancePolicy
                          WHERE PolicyNumber = @PolicyNumber
                            AND InsurancePolicyId != @InsurancePolicyId
                      );";
                InsurancePolicy insurancePolicy = InsurancePolicyMapper.MapToInsurancePolicy(request);
                insurancePolicy.InsurancePolicyId = id;
                int count = await _sqlConnection.ExecuteAsync(updateQuery, insurancePolicy);
                if (count == 0)
                    throw new Exception($"Failed updating policy with ID: {id}");
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
