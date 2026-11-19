namespace Apta.Application.Validators.Customers;

public sealed class CustomersImportSchemeValidator : AbstractValidator<CustomersImportScheme>
{
    public CustomersImportSchemeValidator()
    {
        RuleFor(import => import.File)
            .NotNull()
            .WithMessage("file is required.");

        RuleFor(import => import.File.FileName)
            .NotEmpty()
            .WithMessage("file name is required.")
            .Must(fileName => fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("file must be a csv.")
            .When(import => import.File is not null);

        RuleFor(import => import.File.Stream)
            .NotNull()
            .WithMessage("file content is required.")
            .When(import => import.File is not null);
    }
}
