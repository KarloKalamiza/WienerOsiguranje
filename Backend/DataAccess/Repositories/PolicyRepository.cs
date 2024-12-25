using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Mappers;
using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.DataAccess.Repositories
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

            InsertIntoPartnerResponse(filteredPartner, insuranceResponse);
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

        private void InsertIntoPartnerResponse(PartnerResponse partnerResponse, InsurancePolicyResponse insuranceResponse)
        {
            if (partnerResponse.Policies == null)
                partnerResponse.Policies = [];

            partnerResponse.Policies.Add(insuranceResponse);
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
