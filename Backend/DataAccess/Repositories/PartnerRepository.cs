using Backend.DataAccess.Data.Responses;
using Backend.Mappers;
using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.DataAccess.Repositories;

public class PartnerRepository : IPartner
{
    //private readonly SqlConnectionFactory _sqlConnectionFactory;
    private readonly SqlConnection _sqlConnection;
    private readonly IConfiguration _configuration;
    public PartnerRepository(IConfiguration configuration)
    {
        //_sqlConnectionFactory = sqlConnectionFactory;
        _configuration = configuration;
        _sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
    public Task<Partner> Add(Partner model)
    {
        throw new NotImplementedException();
    }

    public Task<Partner> Find(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Partner>> Get()
    {
        try
        {
            string query = "SELECT * FROM Partner";
            IEnumerable<Partner> partners = await _sqlConnection.QueryAsync<Partner>(query) ?? [];
            return partners;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<PartnerResponse>> GetPartnersForPolicy(string policyNumber)
    {
        try
        {
            IEnumerable<PartnerResponse> partners = await GetPartnerWithPolicies();
            List<PartnerResponse> filteredPartners = partners.Where(partner => partner.Policies != null && partner.Policies.Any(policy => policy.PolicyNumber == policyNumber)).ToList();
            return filteredPartners;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<PartnerResponse>> GetPartnerWithPolicies()
    {
        try
        {
            // Fetch all partners
            IEnumerable<Partner> partners = await Get();

            // Query to fetch policies associated with partners
            var policiesQuery = @"
        SELECT 
            pp.PartnerId, 
            pp.InsurancePolicyId
        FROM PartnerPolicy pp";

            // Fetch Partner-Policy mappings
            var partnerPolicies = await _sqlConnection.QueryAsync(policiesQuery);

            // Get distinct InsurancePolicyIds to fetch their details
            var policyIds = partnerPolicies
                .Select(pp => (int)pp.InsurancePolicyId)
                .Distinct()
                .ToList();

            // Fetch policy details for these IDs
            var policiesDetailsQuery = @"
                SELECT 
                    ip.InsurancePolicyId, 
                    ip.PolicyNumber, 
                    ip.PolicyAmount
                FROM InsurancePolicy ip
                WHERE ip.InsurancePolicyId IN @PolicyIds";

            var policyDetails = await _sqlConnection.QueryAsync<InsurancePolicy>(policiesDetailsQuery, new { PolicyIds = policyIds });

            // Map policies to PartnerId
            var policyGroups = partnerPolicies
                .GroupBy(pp => (int)pp.PartnerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(pp => policyDetails.FirstOrDefault(p => p.InsurancePolicyId == (int)pp.InsurancePolicyId))
                          .Where(p => p != null)
                          .ToList()
                );

            // Map partners to responses, including their policies
            IEnumerable<PartnerResponse> partnerResponses = partners.Select(partner =>
            {
                // Get policies for this partner if they exist
                var partnerPolicies = policyGroups.ContainsKey(partner.PartnerId)
                    ? policyGroups[partner.PartnerId]!
                    : new List<InsurancePolicy>();

                // Map partner and their policies to a response
                return PartnerMapper.MapToPartnerResponse(partner, partnerPolicies!);
            });

            // Return the final list of responses
            return partnerResponses;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PartnerResponse> GetPartnerWithPoliciesById(int id)
    {
        try
        {
            // Fetch partners
            IEnumerable<Partner> partners = await Get();
            Partner? partner = partners.FirstOrDefault(p => p.PartnerId == id);

            if (partner is null)
            {
                throw new NotFiniteNumberException($"Partner with ID {id} does not exist");
            }

            // Query to fetch insurance policies related to the partner
            var policiesQuery = @"
                SELECT 
                    ip.InsurancePolicyId, 
                    ip.PolicyNumber, 
                    ip.PolicyAmount
                FROM PartnerPolicy pp
                INNER JOIN InsurancePolicy ip ON pp.InsurancePolicyId = ip.InsurancePolicyId
                WHERE pp.PartnerId = @PartnerId";

            // Fetch policies using Dapper
            var policies = await _sqlConnection.QueryAsync<InsurancePolicy>(policiesQuery, new { PartnerId = id }) ?? [];

            // Map the partner and their policies to the response
            var partnerResponse = PartnerMapper.MapToPartnerResponse(partner, policies.ToList()!);

            return partnerResponse;
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

    public Task<Partner> Update(Partner model)
    {
        throw new NotImplementedException();
    }
}
