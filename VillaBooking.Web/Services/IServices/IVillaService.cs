using VillaBooking.DTO.Villa;
using VillaBooking.Web.Models;

namespace VillaBooking.Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>();
        Task<T?> GetAllAsync<T>(VillaQueryParameters parameters);
        Task<T?> GetAsync<T>(int id);
        Task<T?> CreateAsync<T>(VillaUpsertDTO dto);
        Task<T?> UpdateAsync<T>(int id, VillaUpsertDTO dto);
        Task<T?> DeleteAsync<T>(int id);
    }
}
