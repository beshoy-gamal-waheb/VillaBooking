using VillaBooking.DTO.Villa;

namespace VillaBooking.Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>(string token);
        Task<T?> GetAsync<T>(int id, string token);
        Task<T?> CreateAsync<T>(VillaUpsertDTO dto, string token);
        Task<T?> UpdateAsync<T>(int id, VillaUpsertDTO dto, string token);
        Task<T?> DeleteAsync<T>(int id, string token);
    }
}
