using Frontend.DTO;
using Frontend.Errors;
using Frontend.Mappers;
using Frontend.Models;
using Frontend.Request;
using Frontend.Responses;
using Newtonsoft.Json;
using System.Text;

namespace Frontend.Services;

public class CrmService
{
    private readonly Uri _baseUri = new Uri("http://localhost:5229/");
    private readonly HttpClient _httpClient;
    public CrmService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = _baseUri;
    }

    public async Task<List<PartnerDTO>> GetPartners()
    {
        try
        {
            List<PartnerDTO> partnerDTOs = new List<PartnerDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync("api/Partner/PartnersWithPolicies");
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<Partner> partners = JsonConvert.DeserializeObject<List<Partner>>(data) ?? throw new Exception();
                foreach (var partner in partners)
                {
                    PartnerDTO partnerDTO = PartnerMapper.MapToDTO(partner);
                    partnerDTOs.Add(partnerDTO);
                }
                partnerDTOs.Sort((x, y) => y.CreatedAtUtc.CompareTo(x.CreatedAtUtc));
                return partnerDTOs;
            }
            else
            {
                return [];
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ServiceResponse> CreatePartner(PartnerRequest partner)
    {
        try
        {
            string serializedPartner = JsonConvert.SerializeObject(partner);
            var content = new StringContent(serializedPartner, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("api/Partner/CreatePartner", content);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                PartnerDTO partnerDTO = JsonConvert.DeserializeObject<PartnerDTO>(data) ?? throw new Exception();

                return new ServiceResponse
                {
                    Success = true,
                    Data = partnerDTO
                };
            }
            else
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                ServiceResponse formatedResponse = ErrorHandler.HandleUniqueError(errorDetails); ;
                return formatedResponse;
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse
            {
                Success = false,
                ErrorMessage = $"Failed to add partner: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse> GetById(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Partner/PartnersWithPoliciesByID/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var partner = JsonConvert.DeserializeObject<PartnerDTO>(data) ?? new();

                return new ServiceResponse
                {
                    Success = true,
                    Data = partner,
                    ErrorMessage = ""
                };
            }
            else
            {
                return new ServiceResponse { Success = false, ErrorMessage = $"Failed to load information for partner with ID: {id}" };
            }

        }
        catch (Exception ex)
        {
            return new ServiceResponse
            {
                Success = false,
                ErrorMessage = $"Failed to add partner: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse> CreatePolicyForPartner(string externalCode, InsurancePolicyRequest policy)
    {
        try
        {
            string serializedObject = JsonConvert.SerializeObject(policy);
            StringContent content = new(serializedObject, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await _httpClient.PostAsync($"api/Policy/CreatePolicyForPartner?externalCode={externalCode}", content);
            if (httpResponse.IsSuccessStatusCode)
            {
                string data = httpResponse.Content.ReadAsStringAsync().Result;
                InsurancePolicy createdPolicy = JsonConvert.DeserializeObject<InsurancePolicy>(data) ?? throw new Exception();

                return new ServiceResponse() { Success = true, Data = createdPolicy, ErrorMessage = "" };
            }
            else
            {
                string errorDetails = await httpResponse.Content.ReadAsStringAsync();
                ServiceResponse formatedResponse = ErrorHandler.HandleUniqueError(errorDetails);
                return formatedResponse;
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse() { Success = false, ErrorMessage = ex.Message, };
        }
    }

    public async Task<ServiceResponse> FindPolicyByPolicyNumber(string policyNumber)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Policy/Policies/{policyNumber}");
            if (httpResponse.IsSuccessStatusCode)
            {
                string data = httpResponse.Content.ReadAsStringAsync().Result;
                InsurancePolicy insurancePolicy = JsonConvert.DeserializeObject<InsurancePolicy>(data) ?? new InsurancePolicy();
                return new ServiceResponse()
                {
                    Success = true,
                    Data = insurancePolicy,
                    ErrorMessage = ""
                };
            }
            else
            {
                string errorDetails = await httpResponse.Content.ReadAsStringAsync();
                return new ServiceResponse
                {
                    Success = false,
                    ErrorMessage = errorDetails,
                };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse() { Success = false, ErrorMessage = ex.Message, };
        }
    }

    public async Task<ServiceResponse> FindPartnerByID(int id)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Partner/PartnersWithPoliciesByID/{id}");
            if (httpResponse.IsSuccessStatusCode)
            {
                string data = httpResponse.Content.ReadAsStringAsync().Result;
                Partner? partner = JsonConvert.DeserializeObject<Partner>(data);
                return new ServiceResponse
                {
                    Data = partner,
                    Success = true,
                };
            }
            else
            {
                string errorDetails = await httpResponse.Content.ReadAsStringAsync();
                return new ServiceResponse
                {
                    Success = false,
                    ErrorMessage = errorDetails,
                };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse() { Success = false, ErrorMessage = ex.Message, };
        }
    }

    public async Task<ServiceResponse> UpdatePolicy(int id, InsurancePolicyRequest policy)
    {
        try
        {
            string serializedObject = JsonConvert.SerializeObject(policy);
            StringContent content = new(serializedObject, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await _httpClient.PutAsync($"api/Policy/UpdatePolicy/{id}", content);
            if (httpResponse.IsSuccessStatusCode)
            {
                return new ServiceResponse()
                {
                    Success = true,
                    ErrorMessage = ""
                };
            }
            else
            {
                string errorDetails = await httpResponse.Content.ReadAsStringAsync();
                return new ServiceResponse
                {
                    Success = false,
                    ErrorMessage = errorDetails,
                };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse() { Success = false, ErrorMessage = ex.Message, };
        }
    }

    public async Task<ServiceResponse> UpdatePartner(int id, EditPartnerDTO partner)
    {
        try
        {
            PartnerRequest? request = PartnerMapper.MapToRequest(partner);
            string serializedObject = JsonConvert.SerializeObject(request);
            StringContent content = new(serializedObject, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await _httpClient.PutAsync($"api/Partner/UpdatePartner/{id}", content);
            if (httpResponse.IsSuccessStatusCode)
            {
                return new ServiceResponse()
                {
                    Success = true,
                    ErrorMessage = ""
                };
            }
            else
            {
                string errorDetails = await httpResponse.Content.ReadAsStringAsync();
                ServiceResponse formatedResponse = ErrorHandler.HandleUniqueError(errorDetails);

                return formatedResponse;
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse() { Success = false, ErrorMessage = ex.Message, };
        }
    }
}
