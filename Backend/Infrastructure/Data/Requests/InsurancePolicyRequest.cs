namespace Backend.Infrastructure.Data.Requests;

public class InsurancePolicyRequest
{
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal PolicyAmount { get; set; } = decimal.Zero;
}
