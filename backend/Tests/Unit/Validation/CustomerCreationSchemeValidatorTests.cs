namespace Apta.Tests.Unit.Validation;

public sealed class CustomerCreationSchemeValidatorTests
{
    private readonly CustomerCreationSchemeValidator _validator = new();
    private readonly Fixture _fixture = new();

    [Fact(DisplayName = "when customer creation scheme is valid, validation succeeds")]
    public void When_Validate_IsCalled_WithValidScheme_Then_ValidationSucceeds()
    {
        /* arrange: create a valid customer creation scheme */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify validation succeeded */
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "when only the name is provided, validation succeeds because contact data is optional")]
    public void When_Validate_IsCalled_WithOnlyName_Then_ValidationSucceeds()
    {
        /* arrange: create a customer creation scheme with the optional contact data omitted */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .Without(customer => customer.Email)
            .Without(customer => customer.Document)
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify validation succeeded */
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "when name is empty, name validation error is returned")]
    public void When_Validate_IsCalled_WithEmptyName_Then_NameValidationErrorIsReturned()
    {
        /* arrange: create a customer creation scheme with an empty name */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, string.Empty)
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomerCreationScheme.Name));
    }

    [Fact(DisplayName = "when name exceeds maximum length, name validation error is returned")]
    public void When_Validate_IsCalled_WithNameLongerThan200Characters_Then_NameValidationErrorIsReturned()
    {
        /* arrange: create a customer creation scheme with a name exceeding the maximum length */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, new string('A', 201))
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomerCreationScheme.Name));
    }

    [Fact(DisplayName = "when phone is not a valid brazilian phone number, phone validation error is returned")]
    public void When_Validate_IsCalled_WithInvalidPhone_Then_PhoneValidationErrorIsReturned()
    {
        /* arrange: create a customer creation scheme with an invalid phone number */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "invalid-phone")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify phone validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomerCreationScheme.Phone));
    }

    [Fact(DisplayName = "when email is not a valid email address, email validation error is returned")]
    public void When_Validate_IsCalled_WithInvalidEmail_Then_EmailValidationErrorIsReturned()
    {
        /* arrange: create a customer creation scheme with an invalid email address */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "not-an-email")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify email validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomerCreationScheme.Email));
    }

    [Fact(DisplayName = "when document is not a valid cpf, document validation error is returned")]
    public void When_Validate_IsCalled_WithInvalidDocument_Then_DocumentValidationErrorIsReturned()
    {
        /* arrange: create a customer creation scheme with an invalid cpf */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123")
            .Create();

        /* act: validate customer creation scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify document validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomerCreationScheme.Document));
    }
}
