namespace Apta.Application.Validators.Administrators;

public sealed class UpdateAdministratorSchemeValidator : AbstractValidator<UpdateAdministratorScheme>
{
    public UpdateAdministratorSchemeValidator()
    {
        RuleFor(administrator => administrator.Name)
            .NotEmpty()
            .WithMessage("name is required.")
            .MaximumLength(200)
            .WithMessage("name must be at most 200 characters long.");

        RuleFor(administrator => administrator.Code)
            .NotEmpty()
            .WithMessage("code is required.")
            .Length(8)
            .WithMessage("code must be exactly 8 characters long.")
            .Matches("^[0-9]{8}$")
            .WithMessage("code must contain only digits.");
    }
}
