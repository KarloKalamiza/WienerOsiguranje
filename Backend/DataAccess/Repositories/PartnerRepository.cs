using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.Mappers;
using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

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

    public async Task<Partner> InsertPartner(PartnerRequest partnerRequest)
    {
        try
        {
            Partner partner = PartnerMapper.MapToPartner(partnerRequest);
            string query = @"
                INSERT INTO Partner (
                    FirstName,
                    LastName,
                    Address,
                    PartnerNumber,
                    CroatianPIN,
                    PartnerTypeId,
                    CreatedAtUtc,
                    CreatedByUser,
                    IsForeign,
                    ExternalCode,
                    Gender
                )
                OUTPUT INSERTED.PartnerId
                VALUES (
                    @FirstName,
                    @LastName,
                    @Address,
                    @PartnerNumber,
                    @CroatianPIN,
                    @PartnerTypeId,
                    @CreatedAtUtc,
                    @CreatedByUser,
                    @IsForeign,
                    @ExternalCode,
                    @Gender
                );";

            int generatedid = await _sqlConnection.QuerySingleAsync<int>(query, partner);
            if (generatedid == 0)
                throw new Exception($"Insert into database failed for partner: {partner}");
            partner.PartnerId = generatedid;

            return partner;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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

    public async Task<int> Remove(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id), "Invalid ID");

        try
        {
            string query = "DELETE FROM Partner WHERE PartnerId = @PartnerId";
            int count = await _sqlConnection.ExecuteAsync(query, new { PartnerId = id });
            if (count == 0)
                throw new Exception($"Failed while executing SQL query for id: {id}");

            return count;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<Partner> Update(Partner model)
    {
        throw new NotImplementedException();
    }

    public async Task<int> SoftDeletePartner(string externalCode, string deletedByUser)
    {
        if (string.IsNullOrEmpty(externalCode) || string.IsNullOrEmpty(deletedByUser))
            throw new ArgumentNullException(nameof(externalCode));

        try
        {
            // find partner from external code
            string findPartnerQuery = "SELECT * FROM Partner WHERE ExternalCode = @ExternalCode";
            Partner partner = await _sqlConnection.QuerySingleAsync<Partner>(findPartnerQuery, new { ExternalCode = externalCode });
            IEnumerable<PartnerResponse> partners = await GetPartnerWithPolicies();

            PartnerResponse? partnerResponse = partners.FirstOrDefault(p => p.ExternalCode == externalCode);
            if (partnerResponse == null)
                throw new Exception();

            if (partnerResponse.Policies.IsNullOrEmpty())
            {
                // insert partner into deleted partner database table
                DeletedPartner deletedPartner = PartnerMapper.MapToDeletedPartner(partner, deletedByUser);
                string insertQuery = "INSERT INTO DeletedPartner " +
                    "(PartnerId, FirstName, LastName, Address, PartnerNumber, CroatianPIN, PartnerTypeId, CreatedAtUtc, CreatedByUser, IsForeign, ExternalCode, Gender, DeletedAtUtc, DeletedByUser) " +
                    "OUTPUT INSERTED.PartnerId, INSERTED.FirstName, INSERTED.LastName, INSERTED.Address, INSERTED.PartnerNumber, INSERTED.CroatianPIN, INSERTED.PartnerTypeId, INSERTED.CreatedAtUtc, INSERTED.CreatedByUser, INSERTED.IsForeign, INSERTED.ExternalCode, INSERTED.Gender, INSERTED.DeletedAtUtc, INSERTED.DeletedByUser " +
                    "VALUES (@PartnerId, @FirstName, @LastName, @Address, @PartnerNumber, @CroatianPIN, @PartnerTypeId, @CreatedAtUtc, @CreatedByUser, @IsForeign, @ExternalCode, @Gender, @DeletedAtUtc, @DeletedByUser);";


                DeletedPartner insertedDeletedPartner = await _sqlConnection.QuerySingleAsync<DeletedPartner>(insertQuery, deletedPartner);

                // delete record from table Partners 
                int count = await Remove(partner.PartnerId);
                if (count == 0)
                    throw new Exception($"Failed to remove partner with ID: {partner.PartnerId}");
                return count; 
            }
            else
            {
                throw new Exception("Cannot delete partner with existing policies.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> UpdatePartner(int id, PartnerRequest request)
    {
        if (id < 0)
            throw new ArgumentOutOfRangeException($"Invalid ID: {id}");
        if (request == null)
            throw new ArgumentOutOfRangeException("Partner request is required.");

        try
        {
            string updateQuery = @"UPDATE Partner
                SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Address = @Address,
                    PartnerNumber = @PartnerNumber,
                    CroatianPIN = @CroatianPIN,
                    PartnerTypeId = @PartnerTypeId,
                    CreatedByUser = @CreatedByUser,
                    IsForeign = @IsForeign,
                    ExternalCode = @ExternalCode,
                    Gender = @Gender
                WHERE PartnerId = @PartnerId;";
            Partner partner = PartnerMapper.MapToPartner(request);
            partner.PartnerId = id;

            int count = await _sqlConnection.ExecuteAsync(updateQuery, partner);
            if (count == 0)
                throw new Exception($"Update failed for partner with id: {id}");

            return count;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
