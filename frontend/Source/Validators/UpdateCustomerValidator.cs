namespace Apta.WebUI.Validators;

public sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerScheme>
{
    public UpdateCustomerValidator()
    {
        RuleFor(customer => customer.Name)
            .NotEmpty()
            .WithMessage("Informe o nome do cliente.")
            .MaximumLength(200)
            .WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(customer => customer.Phone)
            .Matches(@"^(\+55\s?)?(\(?\d{2}\)?\s?)?\d{4,5}-?\d{4}$")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Phone))
            .WithMessage("Informe um telefone válido.");

        RuleFor(customer => customer.Document)
            .Matches(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Document))
            .WithMessage("Informe um CPF válido.");
    }
}
