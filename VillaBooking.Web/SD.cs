namespace VillaBooking.Web
{
    public static class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public const string SessionToken = "JWTToken";
        public const string CurrentApiVersion = "v2";

        public static string APIBaseUrl { get; set; }

        public static string GetImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return $"/images/placeholder-villa.png";
            }
            return $"{APIBaseUrl}/{imageUrl}";
        }
    }
}
