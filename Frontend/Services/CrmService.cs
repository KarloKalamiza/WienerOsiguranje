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
                partnerDTOs.Sort((x,y) => y.CreatedAtUtc.CompareTo(x.CreatedAtUtc));
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
}
