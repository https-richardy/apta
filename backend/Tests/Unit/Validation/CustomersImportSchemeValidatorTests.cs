namespace Apta.Tests.Unit.Validation;

public sealed class CustomersImportSchemeValidatorTests
{
    private readonly CustomersImportSchemeValidator _validator = new();

    [Fact(DisplayName = "when the import scheme carries a csv file with content, validation succeeds")]
    public void When_Validate_IsCalled_WithValidScheme_Then_ValidationSucceeds()
    {
        /* arrange: create an import scheme with a csv file and a non-null stream */
        var scheme = new CustomersImportScheme
        {
            File = new Blob { FileName = "customers.csv", Stream = Stream.Null }
        };

        /* act: validate the import scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify validation succeeded */
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "when the file is missing, a file validation error is returned")]
    public void When_Validate_IsCalled_WithoutFile_Then_FileValidationErrorIsReturned()
    {
        /* arrange: create an import scheme without a file */
        var scheme = new CustomersImportScheme { File = null! };

        /* act: validate the import scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify a file validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CustomersImportScheme.File));
    }

    [Fact(DisplayName = "when the file name is empty, a file name validation error is returned")]
    public void When_Validate_IsCalled_WithEmptyFileName_Then_FileNameValidationErrorIsReturned()
    {
        /* arrange: create an import scheme whose file has an empty name */
        var scheme = new CustomersImportScheme
        {
            File = new Blob { FileName = string.Empty, Stream = Stream.Null }
        };

        /* act: validate the import scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify a file name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "File.FileName");
    }

    [Fact(DisplayName = "when the file is not a csv, a file name validation error is returned")]
    public void When_Validate_IsCalled_WithNonCsvFile_Then_FileNameValidationErrorIsReturned()
    {
        /* arrange: create an import scheme whose file is not a csv */
        var scheme = new CustomersImportScheme
        {
            File = new Blob { FileName = "customers.txt", Stream = Stream.Null }
        };

        /* act: validate the import scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify a file name validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "File.FileName");
    }

    [Fact(DisplayName = "when the file has no stream, a file content validation error is returned")]
    public void When_Validate_IsCalled_WithoutStream_Then_FileContentValidationErrorIsReturned()
    {
        /* arrange: create an import scheme whose file carries no stream */
        var scheme = new CustomersImportScheme
        {
            File = new Blob { FileName = "customers.csv", Stream = null! }
        };

        /* act: validate the import scheme */
        var result = _validator.Validate(scheme);

        /* assert: verify a file content validation error */
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "File.Stream");
    }
}
