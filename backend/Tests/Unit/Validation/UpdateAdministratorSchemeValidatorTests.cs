namespace Apta.Tests.Unit.Validation;

public sealed class UpdateAdministratorSchemeValidatorTests
{
    private readonly UpdateAdministratorSchemeValidator _validator = new();
    private readonly Fixture _fixture = new();

    [Fact(DisplayName = "when update administrator scheme is valid, validation succeeds")]
    public void When_Validate_IsCalled_WithValidScheme_Then_ValidationSucceeds()
    {
        /* arrange: create a valid update administrator scheme */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify validation succeeded */
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "when name is empty, name validation error is returned")]
    public void When_Validate_IsCalled_WithEmptyName_Then_NameValidationErrorIsReturned()
    {
        /* arrange: create update administrator scheme with an empty name */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, string.Empty)
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAdministratorScheme.Name));
    }

    [Fact(DisplayName = "when name exceeds maximum length, name validation error is returned")]
    public void When_Validate_IsCalled_WithNameLongerThan200Characters_Then_NameValidationErrorIsReturned()
    {
        /* arrange: create update administrator scheme with a name exceeding the maximum length */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, new string('A', 201))
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAdministratorScheme.Name));
    }

    [Fact(DisplayName = "when code is empty, code validation error is returned")]
    public void When_Validate_IsCalled_WithEmptyCode_Then_CodeValidationErrorIsReturned()
    {
        /* arrange: create update administrator scheme with an empty code */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, string.Empty)
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify code validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAdministratorScheme.Code));
    }

    [Fact(DisplayName = "when code is not exactly eight characters, code validation error is returned")]
    public void When_Validate_IsCalled_WithCodeShorterThanEightCharacters_Then_CodeValidationErrorIsReturned()
    {
        /* arrange: create update administrator scheme with a code shorter than eight characters */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "1234567")
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify code validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAdministratorScheme.Code));
    }

    [Fact(DisplayName = "when code contains non-digit characters, code validation error is returned")]
    public void When_Validate_IsCalled_WithNonDigitCode_Then_CodeValidationErrorIsReturned()
    {
        /* arrange: create update administrator scheme with a non-digit code */
        var scheme = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "ABCD1234")
            .Create();

        /* act: validate update administrator scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify code validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateAdministratorScheme.Code));
    }
}
