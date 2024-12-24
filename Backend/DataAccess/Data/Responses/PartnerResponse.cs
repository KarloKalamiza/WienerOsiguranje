using Backend.Enums;

namespace Backend.DataAccess.Data.Responses;

public class PartnerResponse
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PartnerNumber { get; set; } = string.Empty;
    public string? CroatianPIN { get; set; } = string.Empty;
    public String PartnerTypeId { get; set; } = String.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedByUser { get; set; } = string.Empty;
    public bool IsForeign { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public List<InsurancePolicyResponse>? Policies { get; set; }
}
