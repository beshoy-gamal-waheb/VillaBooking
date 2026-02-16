using AutoMapper;
using VillaBooking.API.Models;
using VillaBooking.API.Models.DTOs;

namespace VillaBooking.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VillaUpsertDTO, Villa>();
            CreateMap<Villa, VillaDTO>();
        }
    }
}
