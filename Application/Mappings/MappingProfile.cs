using APIUsuarios.Application.DTOs;
using APIUsuarios.Domain.Entities;
using AutoMapper;

namespace APIUsuarios.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Usuario -> UsuarioReadDto
        CreateMap<Usuario, UsuarioReadDto>();

        // UsuarioCreateDto -> Usuario
        CreateMap<UsuarioCreateDto, Usuario>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLowerInvariant()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Ativo, opt => opt.Ignore());

        // UsuarioUpdateDto -> Usuario
        CreateMap<UsuarioUpdateDto, Usuario>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLowerInvariant()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.Senha, opt => opt.Ignore());
    }
}
