namespace Backend.DataAccess.Data.Responses;

public class InsurancePolicyResponse
{
    public string PolicyNumber { get; set; } = string.Empty;
    public Decimal PolicyAmount { get; set; } = decimal.Zero;
}
