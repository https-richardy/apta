namespace Apta.Tests.Unit.Mapping;

public sealed class AdministratorMappingTests
{
    private readonly IMapper _mapper;
    private readonly Fixture _fixture = new();

    public AdministratorMappingTests()
    {
        var configuration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<AdministratorMapping>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "when mapper configuration is created, mapping configuration is valid")]
    public void When_Configuration_IsCreated_Then_MappingConfigurationIsValid()
    {
        /* arrange: create mapper configuration with administrator profile */
        var configuration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<AdministratorMapping>();
        });

        /* act and assert: verify mapper configuration is valid */
        configuration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "when administrator creation scheme is mapped, administrator fields are copied")]
    public void When_Map_IsCalled_WithAdministratorCreationScheme_Then_AdministratorFieldsAreMapped()
    {
        /* arrange: create administrator creation scheme with representative values */
        var scheme = _fixture.Build<AdministratorCreationScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: map administrator creation scheme into administrator entity */
        var administrator = _mapper.Map<Administrator>(scheme);

        /* assert: verify all mapped properties */
        Assert.Equal(scheme.Name, administrator.Name);
        Assert.Equal(scheme.Code, administrator.Code);
    }

    [Fact(DisplayName = "when administrator is mapped, administrator scheme fields are copied")]
    public void When_Map_IsCalled_WithAdministrator_Then_AdministratorSchemeFieldsAreMapped()
    {
        /* arrange: create administrator entity with representative values */
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Id, "administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
            .With(entity => entity.Name, "EMBRACON")
            .With(entity => entity.Code, "58113812")
            .With(entity => entity.CreatedAt, DateTime.UtcNow.AddDays(-1))
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .Create();

        /* act: map administrator entity into administrator scheme */
        var scheme = _mapper.Map<AdministratorScheme>(administrator);

        /* assert: verify all mapped properties */
        Assert.Equal(administrator.Id, scheme.Id);
        Assert.Equal(administrator.Name, scheme.Name);
        Assert.Equal(administrator.Code, scheme.Code);

        Assert.Equal(administrator.CreatedAt, scheme.CreatedAt);
        Assert.Equal(administrator.UpdatedAt, scheme.UpdatedAt);
    }
}
