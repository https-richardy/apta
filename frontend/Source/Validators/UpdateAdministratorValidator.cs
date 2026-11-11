namespace Apta.WebUI.Validators;

public sealed class UpdateAdministratorValidator : AbstractValidator<UpdateAdministratorScheme>
{
    public UpdateAdministratorValidator()
    {
        RuleFor(administrator => administrator.Name)
            .NotEmpty()
            .WithMessage("Informe o nome da administradora.");

        RuleFor(administrator => administrator.Code)
            .NotEmpty()
            .WithMessage("Informe o código BACEN.")
            .Matches(@"^\d{8}$")
            .WithMessage("O código BACEN deve conter 8 dígitos.");
    }
}
