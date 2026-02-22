using AutoMapper;
using Microsoft.Extensions.Options;
using VillaBooking.API.DTOs.Auth;
using VillaBooking.API.DTOs.Villa;
using VillaBooking.API.DTOs.VillaAmenity;
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
