using System.ComponentModel.DataAnnotations;

namespace Frontend.DTO;

public class EditInsurancePolicyDTO
{
    public int InsurancePolicyId { get; set; }

    [Required(ErrorMessage = "Please enter policy number")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Policy number must be numeric")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Policy number must be between 10 and 15 characters")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter policy amount")]
    [Range(0, double.MaxValue, ErrorMessage = "Policy amount must be a valid positive number")]
    public decimal PolicyAmount { get; set; }
}
