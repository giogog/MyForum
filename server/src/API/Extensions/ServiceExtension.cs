using Application;
using Application.Mapping;
using Application.Services;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Infrastructure.DataConnection;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigurePostgreSql(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<ApplicationDataContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.MigrationsAssembly("API");
                }));

        public static void AddIdentityService(this IServiceCollection services, IConfiguration config)
        {

            var issuer = config.GetValue<string>("ApiSettings:JwtOptions:Issuer")!;
            var audience = config.GetValue<string>("ApiSettings:JwtOptions:Audience");
            string TokenKey = config["ApiSettings:JwtOptions:Secret"]!;


            services.AddIdentity<User, Role>(option =>
            {
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;


            }).AddEntityFrameworkStores<ApplicationDataContext>().AddDefaultTokenProviders();




            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

        }
        public static void GeneralConfiguration(this IServiceCollection services)
        {
            services.AddTransient<ITokenGenerator, TokenGenerator>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection service) =>
            service.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureAutomapper(this IServiceCollection services) => 
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

        public static void ConfigureJwtOptions(this WebApplicationBuilder builder) => 
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Baasi API",
                    Version = "v1",
                    Description = "Baasi forum API with authentication, authorization, and core forum features",
                    Contact = new OpenApiContact
                    {
                        Name = "Baasi Support",
                        Email = "support@baasi.com"
                    }
                });

                // Enable XML comments for better Swagger documentation
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string example: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] {}
                    }
                });

                // Add custom operation filters for better documentation
                options.CustomSchemaIds(type => type.FullName);
            });
        }

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:5003")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });
    }
}
