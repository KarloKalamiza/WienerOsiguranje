namespace Backend.Infrastructure.Data.Responses;

public class InsurancePolicyResponse
{
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal PolicyAmount { get; set; } = decimal.Zero;
}
