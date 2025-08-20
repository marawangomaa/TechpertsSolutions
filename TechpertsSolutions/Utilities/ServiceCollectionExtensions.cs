using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Service;

namespace TechpertsSolutions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
            services.AddScoped<IDeliveryClusterService, DeliveryClusterService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<ISpecificationService, SpecificationService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderHistoryService, OrderHistoryService>();
            services.AddScoped<IServiceUsageService, ServiceUsageService>();
            services.AddScoped<ITechCompanyService, TechCompanyService>();
            services.AddScoped<IWarrantyService, WarrantyService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IAdminUserManagementService, AdminUserManagementService>();
            services.AddScoped<IPCAssemblyCompatibilityService, PCAssemblyCompatibilityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPCAssemblyService, PCAssemblyService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IProfileService, ProfileService>();

            return services;
        }
    }
}
