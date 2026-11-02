namespace Apta.Tests.Unit.Validation;

public sealed class AdministratorFiltersParametersValidatorTests
{
    private readonly AdministratorFiltersParametersValidator _validator = new();
    private readonly Fixture _fixture = new();

    [Fact(DisplayName = "when filter parameters are valid, validation succeeds")]
    public void When_Validate_IsCalled_WithValidParameters_Then_ValidationSucceeds()
    {
        /* arrange: create valid filter parameters */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify validation succeeded */
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "when identifier format is invalid, identifier validation error is returned")]
    public void When_Validate_IsCalled_WithInvalidIdentifier_Then_IdentifierValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with an invalid identifier format */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "invalid-identifier")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify identifier validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.Id));
    }

    [Fact(DisplayName = "when name exceeds maximum length, name validation error is returned")]
    public void When_Validate_IsCalled_WithNameLongerThan200Characters_Then_NameValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with a name exceeding the maximum length */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, new string('A', 201))
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.Name));
    }

    [Fact(DisplayName = "when code contains non-digit characters, code validation error is returned")]
    public void When_Validate_IsCalled_WithNonDigitCode_Then_CodeValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with a non-digit code */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "ABCD1234")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify code validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.Code));
    }

    [Fact(DisplayName = "when sort direction is not ascending or descending, order validation error is returned")]
    public void When_Validate_IsCalled_WithInvalidOrder_Then_OrderValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with an invalid sort direction */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Sideways")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify order validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.Order));
    }

    [Fact(DisplayName = "when page is not greater than zero, page validation error is returned")]
    public void When_Validate_IsCalled_WithPageEqualToZero_Then_PageValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with a page equal to zero */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 0)
            .With(filters => filters.PageSize, 10)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify page validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.Page));
    }

    [Fact(DisplayName = "when page size exceeds one hundred, page size validation error is returned")]
    public void When_Validate_IsCalled_WithPageSizeGreaterThan100_Then_PageSizeValidationErrorIsReturned()
    {
        /* arrange: create filter parameters with a page size above the maximum */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(filters => filters.Name, "Embracon")
            .With(filters => filters.Code, "58113812")
            .With(filters => filters.By, "Name")
            .With(filters => filters.Order, "Ascending")
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 101)
            .Create();

        /* act: validate filter parameters */
        var result = _validator.Validate(parameters);

        /* assert: verify page size validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdministratorFiltersParameters.PageSize));
    }
}
