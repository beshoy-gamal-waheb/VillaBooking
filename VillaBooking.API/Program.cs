using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar;
using Scalar.AspNetCore;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Models.Responses;
using VillaBooking.API.Profiles;
namespace VillaBooking.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Add services to the container.
            
            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .Select(x => new
                        {
                            Field = x.Key,
                            Errors = x.Value!.Errors.Select(e => e.ErrorMessage)
                        });

                    var response = APIResponse<object>.BadRequest(
                        "Validation failed",
                        errors
                    );

                    return new BadRequestObjectResult(response);
                };
            });


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            #endregion

            var app = builder.Build();
            await ApplyMigrations(app);   

            #region Configure the HTTP request pipeline.
            
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers(); 

            #endregion

            app.Run();
        }

        static async Task ApplyMigrations(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();
        } 
    }
}
