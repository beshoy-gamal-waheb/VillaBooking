namespace VillaBooking.API.Services.Image
{
    public class ImageService(IWebHostEnvironment _webHostEnvironment) : IImageService
    {
        private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (!ValidateImage(imageFile))
            {
                throw new InvalidOperationException("Invalid image file. Please upload a valid image (jpg, jpeg, png) with a size less than 5 MB.");
            }

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "villas");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}"; 
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return $"/images/villas/{uniqueFileName}";
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return false;
            }

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "villas", fileName);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath)); 
                //File.Delete(filePath);
                return true;
            }
            return false;
        }

        public bool ValidateImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0)
            {
                return false;
            }

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || imageFile.Length > MaxImageSize)
            {
                return false;
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                return false;
            }

            return true;
        }
    }
}
