namespace Apta.Application.Mapping;

public sealed class CustomerMapping : Profile
{
    public CustomerMapping()
    {
        CreateMap<CustomerCreationScheme, Customer>()
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name.Trim()))
            .ForMember(destination => destination.Phone,
                options => options.MapFrom(source => (source.Phone ?? string.Empty).SanitizeNumbers().OrNotSpecified()))
            .ForMember(destination => destination.Document,
                options => options.MapFrom(source => string.IsNullOrWhiteSpace(source.Document) ? null : source.Document.SanitizeNumbers()))
            .ForMember(destination => destination.Id,
                options => options.Ignore())
            .ForMember(destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.IsDeleted,
                options => options.Ignore());

        CreateMap<UpdateCustomerScheme, Customer>()
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name.Trim()))
            .ForMember(destination => destination.Phone,
                options => options.MapFrom(source => (source.Phone ?? string.Empty).SanitizeNumbers().OrNotSpecified()))
            .ForMember(destination => destination.Document,
                options => options.MapFrom(source => string.IsNullOrWhiteSpace(source.Document) ? null : source.Document.SanitizeNumbers()))
            .ForMember(destination => destination.Id,
                options => options.Ignore())
            .ForMember(destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.IsDeleted,
                options => options.Ignore());

        CreateMap<Customer, CustomerScheme>()
            .ForMember(destination => destination.Id,
                options => options.MapFrom(source => source.Id))
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name))
            .ForMember(destination => destination.Phone,
                options => options.MapFrom(source => (source.Phone ?? string.Empty).OrNotSpecified()))
            .ForMember(destination => destination.Document,
                options => options.MapFrom(source => (source.Document ?? string.Empty).OrNotSpecified()))
            .ForMember(destination => destination.CreatedAt,
                options => options.MapFrom(source => source.CreatedAt))
            .ForMember(destination => destination.UpdatedAt,
                options => options.MapFrom(source => source.UpdatedAt));
    }
}
