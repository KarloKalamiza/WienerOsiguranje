namespace Backend.Core.Domain.Models;

public class InsurancePolicy
{
    public int InsurancePolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal PolicyAmount { get; set; } = decimal.Zero;
}
