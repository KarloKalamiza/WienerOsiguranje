namespace Frontend.Models;

public class InsurancePolicy
{
    public int InsurancePolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public Decimal PolicyAmount { get; set; }
}
