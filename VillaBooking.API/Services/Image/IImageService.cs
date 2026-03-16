namespace VillaBooking.API.Services.Image
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        Task<bool> DeleteImageAsync(string imageUrl);
        bool ValidateImage(IFormFile imageFile);
    }
}
