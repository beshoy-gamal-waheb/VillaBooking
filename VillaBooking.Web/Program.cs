using VillaBooking.Web.Services;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.
            
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient("VillaBookingAPI", options =>
            {
                var villaAPIUrl = builder.Configuration["ServiceUrls:VillaAPI"];
                options.BaseAddress = new Uri(villaAPIUrl);
                options.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddScoped<IVillaService, VillaService>();

            #endregion

            var app = builder.Build();


            #region Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            #endregion

            app.Run();
        }
    }
}
