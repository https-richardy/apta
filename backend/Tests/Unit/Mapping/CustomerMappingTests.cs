namespace Apta.Tests.Unit.Mapping;

public sealed class CustomerMappingTests
{
    private readonly IMapper _mapper;
    private readonly Fixture _fixture = new();

    public CustomerMappingTests()
    {
        var configuration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<CustomerMapping>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "when mapper configuration is created, mapping configuration is valid")]
    public void When_Configuration_IsCreated_Then_MappingConfigurationIsValid()
    {
        /* arrange: create mapper configuration with customer profile */
        var configuration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<CustomerMapping>();
        });

        /* act and assert: verify mapper configuration is valid */
        configuration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "when customer creation scheme is mapped, phone and document are stored as digits only")]
    public void When_Map_IsCalled_WithCustomerCreationScheme_Then_ContactDataIsSanitized()
    {
        /* arrange: create customer creation scheme with formatted contact values */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "  João da Silva  ")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Email, "joao.silva@gmail.com")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: map customer creation scheme into customer entity */
        var customer = _mapper.Map<Customer>(scheme);

        /* assert: verify name and email are normalized */
        Assert.Equal("João da Silva", customer.Name);
        Assert.Equal("joao.silva@gmail.com", customer.Email);

        /* assert: verify phone and document keep digits only */
        Assert.Equal("11988887777", customer.Phone);
        Assert.Equal("12345678909", customer.Document);
    }

    [Fact(DisplayName = "when customer creation scheme omits contact data, phone falls back and unique fields stay null")]
    public void When_Map_IsCalled_WithoutContactData_Then_PhoneFallsBackAndUniqueFieldsStayNull()
    {
        /* arrange: create customer creation scheme with the optional contact data omitted */
        var scheme = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .Without(customer => customer.Email)
            .Without(customer => customer.Document)
            .Create();

        /* act: map customer creation scheme into customer entity */
        var customer = _mapper.Map<Customer>(scheme);

        /* assert: verify phone uses the unspecified fallback */
        Assert.Equal("João da Silva", customer.Name);
        Assert.Equal("NÃO ESPECIFICADO", customer.Phone);

        /* assert: verify the unique fields stay null to preserve uniqueness */
        Assert.Null(customer.Email);
        Assert.Null(customer.Document);
    }

    [Fact(DisplayName = "when customer is mapped, customer scheme fields are copied")]
    public void When_Map_IsCalled_WithCustomer_Then_CustomerSchemeFieldsAreMapped()
    {
        /* arrange: create customer entity with representative values */
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Id, "customer_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(entity => entity.Name, "João da Silva")
            .With(entity => entity.Phone, "11988887777")
            .With(entity => entity.Email, "joao.silva@gmail.com")
            .With(entity => entity.Document, "12345678909")
            .With(entity => entity.CreatedAt, DateTime.UtcNow.AddDays(-1))
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .Create();

        /* act: map customer entity into customer scheme */
        var scheme = _mapper.Map<CustomerScheme>(customer);

        /* assert: verify identity and contact fields */
        Assert.Equal(customer.Id, scheme.Id);
        Assert.Equal(customer.Name, scheme.Name);

        /* assert: verify contact and document fields */
        Assert.Equal(customer.Phone, scheme.Phone);
        Assert.Equal(customer.Email, scheme.Email);
        Assert.Equal(customer.Document, scheme.Document);

        /* assert: verify audit fields */
        Assert.Equal(customer.CreatedAt, scheme.CreatedAt);
        Assert.Equal(customer.UpdatedAt, scheme.UpdatedAt);
    }

    [Fact(DisplayName = "when customer has no email or document, the scheme returns the unspecified marker instead of null")]
    public void When_Map_IsCalled_WithCustomerMissingContactData_Then_SchemeReturnsUnspecifiedMarker()
    {
        /* arrange: create customer entity without email and document */
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Id, "customer_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(entity => entity.Name, "João da Silva")
            .With(entity => entity.Phone, "11988887777")
            .Without(entity => entity.Email)
            .Without(entity => entity.Document)
            .Create();

        /* act: map customer entity into customer scheme */
        var scheme = _mapper.Map<CustomerScheme>(customer);

        /* assert: verify missing fields fall back to the unspecified marker */
        Assert.Equal("NÃO ESPECIFICADO", scheme.Email);
        Assert.Equal("NÃO ESPECIFICADO", scheme.Document);

        /* assert: verify provided phone is preserved */
        Assert.Equal("11988887777", scheme.Phone);
    }
}
