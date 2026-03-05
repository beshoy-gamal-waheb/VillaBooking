using VillaBooking.DTO.Villa;

namespace VillaBooking.Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>();
        Task<T?> GetAsync<T>(int id);
        Task<T?> CreateAsync<T>(VillaUpsertDTO dto);
        Task<T?> UpdateAsync<T>(int id, VillaUpsertDTO dto);
        Task<T?> DeleteAsync<T>(int id);
    }
}
