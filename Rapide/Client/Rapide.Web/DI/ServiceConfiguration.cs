using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Services;

namespace Rapide.Web.DI
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                    .AddMemoryCache()
                    .AddTransient<IPettyCashService, PettyCashService>()
                    .AddTransient<ICompanyInfoService, CompanyInfoService>()
                    .AddTransient<IInspectionTechnicianService, InspectionTechnicianService>()
                    .AddTransient<IInspectionService, InspectionService>()
                    .AddTransient<IPaymentDetailsService, PaymentDetailsService>()
                    .AddTransient<IPaymentService, PaymentService>()
                    .AddTransient<IDepositService, DepositService>()
                    .AddTransient<IExpensesService, ExpensesService>()
                    .AddTransient<IQuickSalesService, QuickSalesService>()
                    .AddTransient<IQuickSalesProductService, QuickSalesProductService>()
                    .AddTransient<IInvoiceService, InvoiceService>()
                    .AddTransient<IInvoicePackageService, InvoicePackageService>()
                    .AddTransient<IPackageServiceService, PackageServiceService>()
                    .AddTransient<IPackageProductService, PackageProductService>()
                    .AddTransient<IPackageService, PackageService>()
                    .AddTransient<IJobOrderTechnicianService, JobOrderTechnicianService>()
                    .AddTransient<IJobOrderServiceService, JobOrderServiceService>()
                    .AddTransient<IJobOrderProductService, JobOrderProductService>()
                    .AddTransient<IJobOrderPackageService, JobOrderPackageService>()
                    .AddTransient<IJobOrderService, JobOrderService>()
                    .AddTransient<IEstimateService, EstimateService>()
                    .AddTransient<IEstimatePackageService, EstimatePackageService>()
                    .AddTransient<IEstimateTechnicianService, EstimateTechnicianService>()
                    .AddTransient<IEstimateProductService, EstimateProductService>()
                    .AddTransient<IEstimateServiceService, EstimateServiceService>()
                    .AddTransient<IJobStatusService, JobStatusService>()
                    .AddTransient<ISupplierService, SupplierService>()
                    .AddTransient<IVehicleService, VehicleService>()
                    .AddTransient<ICustomerService, CustomerService>()
                    .AddTransient<IServiceService, ServiceService>()
                    .AddTransient<IServiceCategoryService, ServiceCategoryService>()
                    .AddTransient<IServiceGroupService, ServiceGroupService>()
                    .AddTransient<IManufacturerService, ManufacturerService>()
                    .AddTransient<IProductCategoryService, ProductCategoryService>()
                    .AddTransient<IProductGroupService, ProductGroupService>()
                    .AddTransient<IProductService, ProductService>()
                    .AddTransient<IVehicleMakeService, VehicleMakeService>()
                    .AddTransient<IVehicleModelService, VehicleModelService>()
                    .AddTransient<IUnitOfMeasureService, UnitOfMeasureService>()
                    .AddTransient<IParameterService, ParameterService>()
                    .AddTransient<IParameterGroupService, ParameterGroupService>()
                    .AddTransient<IUserRolesService, UserRolesService>()
                    .AddTransient<IUserService, UserService>()
                    .AddTransient<IRoleService, RoleService>();
        }
    }
}
