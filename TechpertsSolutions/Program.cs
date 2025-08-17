using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Extensions;
using TechpertsSolutions.Repository.Data;
using TechpertsSolutions.Utilities;

namespace TechpertsSolutions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(opt => {
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechpertsSolutions", Version = "v1" });
                c.CustomSchemaIds(type => type.FullName); 
                c.SchemaFilter<EnumSchemaFilter>(); 
                c.OperationFilter<FormDataOperationFilter>();
                c.OperationFilter<EnumOperationFilter>();
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //add scope for DI
            builder.Services.AddApplicationServices();

            builder.Services.AddDbContext<TechpertsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b =>
                b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            builder.Services.AddIdentity<AppUser, AppRole>()
                            .AddRoleManager<RoleManager<AppRole>>()
                            .AddEntityFrameworkStores<TechpertsContext>()
                            .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.Configure<DeliveryAssignmentSettings>(builder.Configuration.GetSection("DeliveryAssignmentSettings"));
            builder.Services.Configure<StripeSettings>(
            builder.Configuration.GetSection("Stripe"));


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRoles.SeedRolesAsync(services);
                await SeedServiceUsages.SeedServiceUsagesAsync(services);
                await SeedCategories.SeedCategoriesAsync(services);
                await SeedSubCategories.SeedSubCategoriesAsync(services);
                await SeedCategorySubCategoryLink.SeedLinksAsync(services);
                await SeedAdminUser.SeedAdminUserAsync(services);
                SeedEnums.LogEnumValues();
                Console.WriteLine("Seeding completed");
            }
            if (app.Environment.IsDevelopment())
            {
               app.UseSwagger();
               app.UseSwaggerUI();
            }
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets");

            if (Directory.Exists(assetsPath))
            {
                var subFolders = Directory.GetDirectories(assetsPath);

                foreach (var folder in subFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(folder),
                        RequestPath = "/" + folderName
                    });
                }
            }
            app.MapHub<NotificationsHub>("/notifications");
            app.MapHub<ChatHub>("/chat");

            app.MapControllers();

            app.Run();
        }
    }
}

