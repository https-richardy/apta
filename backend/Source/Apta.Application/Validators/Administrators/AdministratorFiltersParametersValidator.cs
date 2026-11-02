namespace Apta.Application.Validators.Administrators;

public sealed class AdministratorFiltersParametersValidator : AbstractValidator<AdministratorFiltersParameters>
{
    public AdministratorFiltersParametersValidator()
    {
        RuleFor(parameters => parameters.Id)
            .MaximumLength(80)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Id))
            .WithMessage("administrator identifier is invalid.")
            .Matches(@"^administrator_[A-Za-z0-9]+$")
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Id))
            .WithMessage("administrator identifier format is invalid.");

        RuleFor(parameters => parameters.Name)
            .MaximumLength(200)
            .WithMessage("name must be at most 200 characters long.")
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Name));

        RuleFor(parameters => parameters.Code)
            .MaximumLength(8)
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Code))
            .WithMessage("code must be at most 8 characters long.")
            .Matches("^[0-9]+$")
            .When(parameters => !string.IsNullOrWhiteSpace(parameters.Code))
            .WithMessage("code must contain only digits.");

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
