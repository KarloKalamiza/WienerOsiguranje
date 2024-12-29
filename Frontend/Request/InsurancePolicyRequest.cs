using System.ComponentModel.DataAnnotations;

namespace Frontend.Request;

public class InsurancePolicyRequest
{
    [Required(ErrorMessage = "Please enter policy number")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Policy number must be numeric")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter policy amount")]
    [Range(0, double.MaxValue, ErrorMessage = "Policy amount must be a valid positive number")]
    public decimal PolicyAmount { get; set; }
}
