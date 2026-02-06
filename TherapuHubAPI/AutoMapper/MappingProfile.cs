using AutoMapper;
using TherapuHubAPI.DTOs.Requests.UserTypes;
using TherapuHubAPI.DTOs.Requests.Users;
using TherapuHubAPI.DTOs.Requests.Events;
using TherapuHubAPI.DTOs.Requests.Companies;
using TherapuHubAPI.DTOs.Requests.Menus;
using TherapuHubAPI.DTOs.Responses.UserTypes;
using TherapuHubAPI.DTOs.Responses.Users;
using TherapuHubAPI.DTOs.Responses.Menus;
using TherapuHubAPI.DTOs.Responses.Events;
using TherapuHubAPI.DTOs.Responses.Companies;
using TherapuHubAPI.Models;

namespace TherapuHubAPI.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UserTypes: Model (EN) <-> DTO (ES)
        CreateMap<UserTypes, TipoUsuarioResponseDto>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description));
        CreateMap<CreateTipoUsuarioRequestDto, UserTypes>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion));
        CreateMap<UpdateTipoUsuarioRequestDto, UserTypes>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Users: Model (EN) <-> DTO (ES)
        CreateMap<Users, UsuarioResponseDto>()
            .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.TipoUsuarioNombre, opt => opt.Ignore());
        CreateMap<CreateUsuarioRequestDto, Users>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Correo))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.MustResetPassword, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore());
        CreateMap<UpdateUsuarioRequestDto, Users>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Correo))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Menus: Model (EN) <-> DTO (ES). Children set manually when building tree.
        CreateMap<Menus, MenuResponseDto>()
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Ruta, opt => opt.MapFrom(src => src.Route))
            .ForMember(dest => dest.Icono, opt => opt.MapFrom(src => src.Icon))
            .ForMember(dest => dest.Orden, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.Children, opt => opt.Ignore());
        CreateMap<CreateMenuRequestDto, Menus>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Ruta))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icono))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.Orden));
        CreateMap<UpdateMenuRequestDto, Menus>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Ruta))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icono))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.Orden));

        // EventTypes: Model (EN) <-> DTO (ES)
        CreateMap<EventTypes, TipoEventoResponseDto>()
            .ForMember(dest => dest.IdTipoEvento, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Icono, opt => opt.MapFrom(src => src.Icon));

        // Events: Model (EN) <-> DTO (ES)
        CreateMap<Events, EventoResponseDto>()
            .ForMember(dest => dest.IdEvento, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.IdTipoEvento, opt => opt.MapFrom(src => src.EventTypeId))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.TipoEventoNombre, opt => opt.Ignore())
            .ForMember(dest => dest.TipoEventoColor, opt => opt.Ignore());
        CreateMap<CreateEventoRequestDto, Events>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.FechaInicio))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.FechaFin))
            .ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => src.IdTipoEvento))
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore());
        CreateMap<UpdateEventoRequestDto, Events>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.FechaInicio))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.FechaFin))
            .ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => src.IdTipoEvento))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Estado));

        // Companies: Model (EN) <-> DTO (ES)
        CreateMap<Companies, CompaniaResponseDto>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Nit, opt => opt.MapFrom(src => src.TaxId));
        CreateMap<CreateCompaniaRequestDto, Companies>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.Nit))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateCompaniaRequestDto, Companies>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.Nit))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
