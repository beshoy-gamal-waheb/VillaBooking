using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace VillaBooking.Web.Extensions
{
    public static class MultipartFormDataExtensions
    {
        public static MultipartFormDataContent ToMultipartFormData(this object obj)
        {
            var formData = new MultipartFormDataContent();
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value is null)
                    continue;

                var propertyName = property.Name;

                if (value is IFormFile file && file.Length > 0)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    formData.Add(streamContent, propertyName, file.FileName);
                }
                else
                {
                    var stringValue = value.ToString();
                    formData.Add(new StringContent(stringValue!), propertyName);
                }
            }
            return formData;
        }
    }
}
