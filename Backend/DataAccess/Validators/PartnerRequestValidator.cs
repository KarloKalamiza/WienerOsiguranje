using Backend.DataAccess.Data.Requests;
using FluentValidation;

namespace Backend.DataAccess.Validators;

public class PartnerRequestValidator : AbstractValidator<PartnerRequest>
{
    public PartnerRequestValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("Partner first name is required")
            .MinimumLength(2).WithMessage("Partner's first name minimum length is 2 characters.")
            .MaximumLength(255).WithMessage("Partner's first name maximum length is 255 characters.");
        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("Partner last name is required")
            .MinimumLength(2).WithMessage("Partner's last name minimum length is 2 characters.")
            .MaximumLength(255).WithMessage("Partner's last name maximum length is 255 characters.");
        RuleFor(p => p.Address)
            .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Address must be alphanumeric.")
            .When(x => !string.IsNullOrEmpty(x.Address));
        RuleFor(p => p.PartnerNumber)
            .NotEmpty().WithMessage("Partner number is required")
            .Matches("^[0-9]{20}$").WithMessage("PartnerNumber must be exactly 20 digits.");
        RuleFor(p => p.CroatianPIN)    
            .Matches("^[0-9]{11}$").WithMessage("CroatianPIN must be exactly 11 digits.")
            .When(x => !string.IsNullOrEmpty(x.CroatianPIN));
        RuleFor(p => p.PartnerTypeId)
            .NotEmpty().WithMessage("Partner type ID (legal or personal) is required");
        RuleFor(p => p.CreatedByUser)
            .NotEmpty().WithMessage("Created user email is required.")
            .EmailAddress().WithMessage("Must be valid email address.")
            .MaximumLength(255).WithMessage("Email (CreatedyUser field) cannot be longer than 255 characters");
        RuleFor(p => p.IsForeign)
            .Must(value => value == true || value == false)
            .WithMessage("The IsForeign field must be either true or false.");
        RuleFor(p => p.ExternalCode)
            .NotEmpty().WithMessage("External code is required.")
            .MinimumLength(10).WithMessage("External code bust be minimum 10 characters long.")
            .MaximumLength(20).WithMessage("External code must be maximum 20 characters long.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("ExternalCode must be alphanumeric.");
        //RuleFor(p => p.Gender)
        //    .NotEmpty().WithMessage("Gender field is required.");
    }
}
