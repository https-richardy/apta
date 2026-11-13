namespace Apta.Application.Validators.Customers;

public sealed class CustomerFiltersParametersValidator : AbstractValidator<CustomerFiltersParameters>
{
    public CustomerFiltersParametersValidator()
    {
        RuleFor(parameters => parameters.Id)
            .MaximumLength(80)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Id))
            .WithMessage("customer identifier is invalid.")
            .Matches(@"^customer_[A-Za-z0-9]+$")
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Id))
            .WithMessage("customer identifier format is invalid.");

        RuleFor(parameters => parameters.Name)
            .MaximumLength(200)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Name))
            .WithMessage("name must be at most 200 characters long.");

        RuleFor(parameters => parameters.Phone)
            .MaximumLength(20)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Phone))
            .WithMessage("phone must be at most 20 characters long.");

        RuleFor(parameters => parameters.Email)
            .MaximumLength(256)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Email))
            .WithMessage("email must be at most 256 characters long.");

        RuleFor(parameters => parameters.Document)
            .MaximumLength(14)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Document))
            .WithMessage("document must be at most 14 characters long.");

        RuleFor(parameters => parameters.By)
            .MaximumLength(100)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.By))
            .WithMessage("sort field must be at most 100 characters long.");

        RuleFor(parameters => parameters.Order)
            .MaximumLength(100)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Order))
            .WithMessage("sort direction must be at most 100 characters long.")
            .Must(order => string.IsNullOrWhiteSpace(order) || new[] { SortDirection.Ascending.ToString(), SortDirection.Descending.ToString() }.Contains(order, StringComparer.OrdinalIgnoreCase))
            .WithMessage("sort direction must be either 'Ascending' or 'Descending'.");

        RuleFor(parameters => parameters.Page)
            .GreaterThan(0)
            .WithMessage("page must be greater than 0.");

        RuleFor(parameters => parameters.PageSize)
            .GreaterThan(0)
            .WithMessage("page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("page size must be less than or equal to 100.");
    }
}
