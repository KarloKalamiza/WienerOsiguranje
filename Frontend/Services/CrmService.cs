using Frontend.DTO;
using Frontend.Mappers;
using Frontend.Models;
using Newtonsoft.Json;

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
                throw new Exception($"Failed to fetch products: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
