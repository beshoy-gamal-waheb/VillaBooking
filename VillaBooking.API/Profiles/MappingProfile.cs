using AutoMapper;
using VillaBooking.API.DTOs.Auth;
using VillaBooking.API.DTOs.Villa;
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
        }
    }
}
