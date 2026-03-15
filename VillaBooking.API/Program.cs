using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar;
using Scalar.AspNetCore;
using System.Text;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Models;
using VillaBooking.DTO.Responses;
using VillaBooking.API.Profiles;
using VillaBooking.API.Services.Auth;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
namespace VillaBooking.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings")["Secret"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


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
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            builder.Services.AddCors();

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            var builderProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in builderProvider.ApiVersionDescriptions)
            {
                var versionName = description.GroupName;
                var versionNumber = description.ApiVersion.ToString();
                var displayName = $"Demo API -- {versionNumber}";


                builder.Services.AddOpenApi(versionName, options =>
                {
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        document.Info = new OpenApiInfo
                        {
                            Title = "Demo Villa Booking",
                            Version = versionName,
                            Description = displayName,
                            Contact = new OpenApiContact
                            {
                                Name = "Beshoy Gamal",
                                Email = "beshoygamall12@gmail.com" 
                            }
                        };

                        document.Components ??= new();
                        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                        {
                            ["Bearer"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                                Description = "Enter JWT Bearer token"
                            }
                        };

                        document.Security =
                        [
                            new OpenApiSecurityRequirement
                        {
                            { new OpenApiSecuritySchemeReference("Bearer"), new List<string>() }
                        }
                        ];

                        return Task.CompletedTask;
                    });
                });

            }
     

            #endregion

            var app = builder.Build();
            await ApplyMigrations(app);

            #region Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi("/openapi/{documentName}.json");

                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                app.MapScalarApiReference(options =>
                {
                    options.Title = "Demo - Villa Booking API";

                    var sortedVersion = provider.ApiVersionDescriptions.OrderBy(v => v.ApiVersion).ToList();

                    foreach (var description in sortedVersion)
                    {
                        var versionName = description.GroupName;
                        var versionNumber = description.ApiVersion.ToString();
                        var versionDisplay = $"Demo API -- {versionNumber}";

                        var isDefault = description.ApiVersion.Equals(new ApiVersion(2, 0));

                        options.AddDocument(versionName, versionDisplay, $"/openapi/{versionName}.json", isDefault);
                    }
                });
            }

            app.UseCors(option => option.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*"));

            app.UseHttpsRedirection();

            app.UseAuthentication();
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
