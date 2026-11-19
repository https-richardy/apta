namespace Apta.WebUI.Validators;

public sealed class CustomersImportSchemeValidator : AbstractValidator<CustomersImportScheme>
{
    public CustomersImportSchemeValidator()
    {
        RuleFor(import => import.Content)
            .NotNull()
            .WithMessage("selecione um arquivo para importar.")
            .Must(content => content.Length <= 5 * 1024 * 1024)
            .WithMessage("o arquivo deve ter no máximo 5 MB.")
            .When(import => import.Content is not null, ApplyConditionTo.CurrentValidator);

        RuleFor(import => import.FileName)
            .Must(fileName => fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("o arquivo deve ser um .csv.")
            .When(import => !string.IsNullOrEmpty(import.FileName));
    }
}
