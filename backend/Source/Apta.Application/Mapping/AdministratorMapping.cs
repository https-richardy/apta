namespace Apta.Application.Mapping;

public sealed class AdministratorMapping : Profile
{
    public AdministratorMapping()
    {
        CreateMap<AdministratorCreationScheme, Administrator>()
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name))
            .ForMember(destination => destination.Code,
                options => options.MapFrom(source => source.Code))
            .ForMember(destination => destination.Id,
                options => options.Ignore())
            .ForMember(destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.IsDeleted,
                options => options.Ignore());

        CreateMap<UpdateAdministratorScheme, Administrator>()
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name))
            .ForMember(destination => destination.Code,
                options => options.MapFrom(source => source.Code))
            .ForMember(destination => destination.Id,
                options => options.Ignore())
            .ForMember(destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(destination => destination.IsDeleted,
                options => options.Ignore());

        CreateMap<Administrator, AdministratorScheme>()
            .ForMember(destination => destination.Id,
                options => options.MapFrom(source => source.Id))
            .ForMember(destination => destination.Name,
                options => options.MapFrom(source => source.Name))
            .ForMember(destination => destination.Code,
                options => options.MapFrom(source => source.Code))
            .ForMember(destination => destination.CreatedAt,
                options => options.MapFrom(source => source.CreatedAt))
            .ForMember(destination => destination.UpdatedAt,
                options => options.MapFrom(source => source.UpdatedAt));
    }
}
