using AutoMapper;
using Microsoft.Extensions.Options;
using VillaBooking.DTO.Auth;
using VillaBooking.DTO.Villa;
using VillaBooking.DTO.VillaAmenity;
using VillaBooking.API.Models;

namespace VillaBooking.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VillaUpsertDTO, Villa>();
            CreateMap<Villa, VillaDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<VillaAmenityCreateDTO, VillaAmenity>();
            CreateMap<VillaAmenityUpdateDTO, VillaAmenity>();
            CreateMap<VillaAmenity, VillaAmenityDTO>()
                .ForMember(dest => dest.VillaName, options => options.MapFrom(src => src.Villa != null ? src.Villa.Name : null));
        }
    }
}
