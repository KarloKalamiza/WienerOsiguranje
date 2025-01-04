using System.ComponentModel.DataAnnotations;

namespace Frontend.DTO;

public class EditPartnerDTO
{
    public int PartnerId { get; set; }

    [Required(ErrorMessage = "Please enter partner firstname")]
    [MinLength(2, ErrorMessage = "Partner's first name minimum length is 2 characters.")]
    [MaxLength(255, ErrorMessage = "Partner's first name maximum length is 255 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter partner lastname")]
    [MinLength(2, ErrorMessage = "Partner's last name minimum length is 2 characters.")]
    [MaxLength(255, ErrorMessage = "Partner's last name maximum length is 255 characters.")]
    public string LastName { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9 čćđšžČĆĐŠŽ]*$", ErrorMessage = "Please enter partner address.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter partner number")]
    [RegularExpression("^[0-9]{20}$", ErrorMessage = "PartnerNumber must be exactly 20 digits.")]
    public string PartnerNumber { get; set; } = string.Empty;

    [RegularExpression("^[0-9]{11}$", ErrorMessage = "CroatianPIN must be exactly 11 digits.")]
    public string CroatianPIN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Partner type ID (legal or personal) is required")]
    public string PartnerTypeId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    [Required(ErrorMessage = "Created user email is required.")]
    [EmailAddress(ErrorMessage = "Must be valid email address.")]
    [MaxLength(255, ErrorMessage = "Email (CreatedyUser field) cannot be longer than 255 characters.")]
    public string CreatedByUser { get; set; } = string.Empty;

    [Required(ErrorMessage = "The IsForeign field must be either true or false.")]
    public bool IsForeign { get; set; }

    [Required(ErrorMessage = "Please enter partner external code.")]
    [MinLength(10, ErrorMessage = "External code must be minimum 10 characters long.")]
    [MaxLength(20, ErrorMessage = "External code must be maximum 20 characters long.")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "ExternalCode must be alphanumeric.")]
    public string ExternalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender field is required.")]
    public string Gender { get; set; } = string.Empty;
}
