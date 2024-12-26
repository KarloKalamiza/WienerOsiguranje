using Frontend.Models;

namespace Frontend.DTO;

public class PartnerDTO
{
    public int PartnerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PartnerNumber { get; set; } = string.Empty;
    public string CroatianPIN { get; set; } = string.Empty;
    public string PartnerTypeId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public bool IsForeign { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public List<InsurancePolicy> Policies { get; set; } = [];
    public string SpecialPartnerSign { get; set; } = string.Empty;
}

