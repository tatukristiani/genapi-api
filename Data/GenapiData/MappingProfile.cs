using AutoMapper;
using genapi_api.Data.GenapiData.Entities;
using genapi_api.Data.GenapiData.Models;

namespace genapi_api.Data.GenapiData
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity → DTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Organizations,
                           opt => opt.MapFrom(src => src.Organizations.Select(u => u.Name)))
                .ForMember(dest => dest.OrganizationsAsEditor,
                           opt => opt.MapFrom(src => src.OrganizationsAsEditor.Select(u => u.Name)));

            // DTO → Entity
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Organizations,
                           opt => opt.Ignore())
                .ForMember(dest => dest.OrganizationsAsEditor,
                           opt => opt.Ignore());

            // DTO → Entity
            CreateMap<UserCreateDTO, User>()
                .ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.PasswordHash, opt => opt.Ignore())
                .ForMember(u => u.PasswordSalt, opt => opt.Ignore())
                .ForMember(u => u.Created, opt => opt.Ignore());

            // Entity → DTO
            CreateMap<Organization, OrganizationCreateDTO>()
                .ForMember(dest => dest.Users,
                           opt => opt.MapFrom(src => src.Users.Select(u => u.Username)))
                .ForMember(dest => dest.Editors,
                           opt => opt.MapFrom(src => src.Editors.Select(u => u.Username)));

            // DTO → Entity
            CreateMap<OrganizationCreateDTO, Organization>()
                .ForMember(dest => dest.Users,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Editors,
                           opt => opt.Ignore());

            // Entity → DTO
            CreateMap<Organization, OrganizationDTO>()
                .ForMember(dest => dest.Users,
                           opt => opt.MapFrom(src => src.Users.Select(u => u.Username)))
                .ForMember(dest => dest.Editors,
                           opt => opt.MapFrom(src => src.Editors.Select(u => u.Username)));

            // DTO → Entity
            CreateMap<OrganizationDTO, Organization>()
                .ForMember(dest => dest.Users,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Editors,
                           opt => opt.Ignore());
        }
    }
}

