namespace Backend.DataAccess.Data.Requests;

public class InsurancePolicyRequest
{
    public string PolicyNumber { get; set; } = string.Empty;
    public Decimal PolicyAmount { get; set; } = decimal.Zero;
}
