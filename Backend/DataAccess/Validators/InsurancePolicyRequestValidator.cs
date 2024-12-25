using Backend.DataAccess.Data.Requests;
using FluentValidation;

namespace Backend.DataAccess.Validators;

public class InsurancePolicyRequestValidator : AbstractValidator<InsurancePolicyRequest>
{
    public InsurancePolicyRequestValidator()
    {
        RuleFor(i => i.PolicyAmount)
            .NotEmpty().WithMessage("Policy amount is required.")
            .GreaterThan(0).WithMessage("Policy amount must be greater than 0.")
            .Must(num => Decimal.TryParse(num.ToString(), out var value)).WithMessage("Policy amount must be decimal number");
        RuleFor(p => p.PolicyNumber)
            .NotEmpty().WithMessage("Policy number is required.")
            .MinimumLength(10).WithMessage("Policy number code bust be minimum 10 characters long.")
            .MaximumLength(15).WithMessage("Policy number code must be maximum 20 characters long.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Policy number must be alphanumeric.");
    }
}
