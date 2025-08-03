using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Enums;
using Core.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;
using TechpertsSolutions.Utilities;
using TechpertsSolutions.Hubs;
using TechpertsSolutions.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            
            // Add SignalR
            builder.Services.AddSignalR();

            
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

            
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICustomerService,CustomerService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthService>(provider =>
            {
                return new AuthService(
                    provider.GetRequiredService<UserManager<AppUser>>(),
                    provider.GetRequiredService<RoleManager<AppRole>>(),
                    provider.GetRequiredService<IRepository<Admin>>(),
                    provider.GetRequiredService<IRepository<Customer>>(),
                    provider.GetRequiredService<IRepository<Cart>>(),
                    provider.GetRequiredService<IRepository<TechCompany>>(),
                    provider.GetRequiredService<IRepository<DeliveryPerson>>(),
                    provider.GetRequiredService<ICustomerService>(),
                    provider.GetRequiredService<IWishListService>(),
                    provider.GetRequiredService<IPCAssemblyService>(),
                    provider.GetRequiredService<IEmailService>(),
                    provider.GetRequiredService<IConfiguration>(),
                    provider.GetRequiredService<TechpertsContext>(),
                    provider.GetRequiredService<IFileService>()
                );
            });
            builder.Services.AddScoped<IRoleService>(provider =>
            {
                return new RoleService(
                    provider.GetRequiredService<RoleManager<AppRole>>(),
                    provider.GetRequiredService<UserManager<AppUser>>(),
                    provider.GetRequiredService<IRepository<Admin>>(),
                    provider.GetRequiredService<IRepository<Customer>>(),
                    provider.GetRequiredService<IRepository<TechCompany>>(),
                    provider.GetRequiredService<IRepository<DeliveryPerson>>(),
                    provider.GetRequiredService<IRepository<Cart>>(),
                    provider.GetRequiredService<IRepository<TechpertsSolutions.Core.Entities.WishList>>(),
                    provider.GetRequiredService<TechpertsContext>(),
                    provider.GetRequiredService<ICustomerService>()
                );
            });
            builder.Services.AddScoped<ICartService>(provider =>
            {
                return new CartService(
                    provider.GetRequiredService<IRepository<Cart>>(),
                    provider.GetRequiredService<IRepository<CartItem>>(),
                    provider.GetRequiredService<IRepository<Product>>(),
                    provider.GetRequiredService<IRepository<Customer>>(),
                    provider.GetRequiredService<IRepository<Order>>(),
                    provider.GetRequiredService<IRepository<OrderItem>>(),
                    provider.GetRequiredService<TechpertsContext>()
                );
            });
            builder.Services.AddScoped<IProductService>(provider =>
            {
                return new ProductService(
                    provider.GetRequiredService<IRepository<Product>>(),
                    provider.GetRequiredService<IRepository<Specification>>(),
                    provider.GetRequiredService<IRepository<Warranty>>(),
                    provider.GetRequiredService<IRepository<Category>>(),
                    provider.GetRequiredService<IRepository<SubCategory>>(),
                    provider.GetRequiredService<IRepository<TechCompany>>(),
                    provider.GetRequiredService<IFileService>(),
                    provider.GetRequiredService<INotificationService>()
                );
            });
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
            builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
            builder.Services.AddScoped<ISpecificationService, SpecificationService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderHistoryService, OrderHistoryService>();
            builder.Services.AddScoped<IPCAssemblyService>(provider =>
            {
                return new PCAssemblyService(
                    provider.GetRequiredService<IRepository<PCAssembly>>(),
                    provider.GetRequiredService<IRepository<PCAssemblyItem>>(),
                    provider.GetRequiredService<IRepository<Product>>(),
                    provider.GetRequiredService<IRepository<Customer>>(),
                    provider.GetRequiredService<IRepository<ServiceUsage>>(),
                    provider.GetRequiredService<INotificationService>()
                );
            });
            builder.Services.AddScoped<IServiceUsageService, ServiceUsageService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ITechCompanyService, TechCompanyService>();
            builder.Services.AddScoped<IWarrantyService, WarrantyService>();
            builder.Services.AddScoped<IWishListService, WishListService>();
            builder.Services.AddScoped<IFileService, FileService>();
            
            // Register new services
            builder.Services.AddScoped<IUserManagementService, UserManagementService>();
            builder.Services.AddScoped<IAdminUserManagementService, AdminUserManagementService>();
            builder.Services.AddScoped<IPCAssemblyCompatibilityService, PCAssemblyCompatibilityService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INotificationHub, NotificationHubService>();
            
            // Register new commission and location services
            builder.Services.AddScoped<ICommissionService, CommissionService>();
            builder.Services.AddScoped<ILocationService, LocationService>();



            
            builder.Services.AddDbContext<TechpertsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
                await SeedCategories.SeedCategoriesAsync(services);
                await SeedSubCategories.SeedSubCategoriesAsync(services);
                await SeedAdminUser.SeedAdminUserAsync(services);
                SeedEnums.LogEnumValues();
            }
            if (app.Environment.IsDevelopment())
            {
               app.UseSwagger();
               app.UseSwaggerUI();
            }
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
     
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();
            
            // Add SignalR hubs
            app.MapHub<NotificationHub>("/notificationHub");
            app.MapHub<ChatHub>("/chatHub");

            app.MapControllers();

            app.Run();
        }
    }
}

