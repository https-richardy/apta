namespace Apta.Application.Validators.Customers;

public sealed class CustomerCreationSchemeValidator : AbstractValidator<CustomerCreationScheme>
{
    public CustomerCreationSchemeValidator()
    {
        RuleFor(customer => customer.Name)
            .NotEmpty()
            .WithMessage("name is required.")
            .MaximumLength(200)
            .WithMessage("name must be at most 200 characters long.");

        RuleFor(customer => customer.Phone)
            .Matches(ExpressionPatterns.BrazilianPhone)
            .When(customer => !string.IsNullOrWhiteSpace(customer.Phone))
            .WithMessage("phone must be a valid brazilian phone number.");

        RuleFor(customer => customer.Document)
            .Matches(ExpressionPatterns.Cpf)
            .When(customer => !string.IsNullOrWhiteSpace(customer.Document))
            .WithMessage("document must be a valid cpf.");
    }
}
