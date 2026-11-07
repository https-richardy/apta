namespace Apta.WebUI.Validators;

public sealed class AdministratorCreationValidator : AbstractValidator<AdministratorCreationScheme>
{
    public AdministratorCreationValidator()
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
