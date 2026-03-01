using AutoMapper;
using VillaBooking.DTO.Villa;

namespace VillaBooking.Web.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VillaUpsertDTO, VillaDTO>().ReverseMap();
        }
    }
}
