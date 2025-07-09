using Rapide.Contracts.Repositories;
using Rapide.Repositories.Repos;

namespace Rapide.Web.DI
{
    public static class RepositoryConfiguration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient<IPettyCashRepo, PettyCashRepo>()
                .AddTransient<ICompanyInfoRepo, CompanyInfoRepo>()
                .AddTransient<IInspectionTechnicianRepo, InspectionTechnicianRepo>()
                .AddTransient<IInspectionRepo, InspectionRepo>()
                .AddTransient<IPaymentDetailsRepo, PaymentDetailsRepo>()
                .AddTransient<IPaymentRepo, PaymentRepo>()
                .AddTransient<IDepositRepo, DepositRepo>()
                .AddTransient<IExpensesRepo, ExpensesRepo>()
                .AddTransient<IQuickSalesRepo, QuickSalesRepo>()
                .AddTransient<IQuickSalesProductRepo, QuickSalesProductRepo>()
                .AddTransient<IInvoiceRepo, InvoiceRepo>()
                .AddTransient<IInvoicePackageRepo, InvoicePackageRepo>()
                .AddTransient<IPackageServiceRepo, PackageServiceRepo>()
                .AddTransient<IPackageProductRepo, PackageProductRepo>()
                .AddTransient<IPackageRepo, PackageRepo>()
                .AddTransient<IJobOrderTechnicianRepo, JobOrderTechnicianRepo>()
                .AddTransient<IJobOrderServiceRepo, JobOrderServiceRepo>()
                .AddTransient<IJobOrderProductRepo, JobOrderProductRepo>()
                .AddTransient<IJobOrderPackageRepo, JobOrderPackageRepo>()
                .AddTransient<IJobOrderRepo, JobOrderRepo>()
                .AddTransient<IEstimateServiceRepo, EstimateServiceRepo>()
                .AddTransient<IEstimateProductRepo, EstimateProductRepo>()
                .AddTransient<IEstimateTechnicianRepo, EstimateTechnicianRepo>()
                .AddTransient<IEstimatePackageRepo, EstimatePackageRepo>()
                .AddTransient<IEstimateRepo, EstimateRepo>()
                .AddTransient<IJobStatusRepo, JobStatusRepo>()
                .AddTransient<ISupplierRepo, SupplierRepo>()
                .AddTransient<IVehicleRepo, VehicleRepo>()
                .AddTransient<ICustomerRepo, CustomerRepo>()
                .AddTransient<IServiceRepo, ServiceRepo>()
                .AddTransient<IServiceCategoryRepo, ServiceCategoryRepo>()
                .AddTransient<IServiceGroupRepo, ServiceGroupRepo>()
                .AddTransient<IManufacturerRepo, ManufacturerRepo>()
                .AddTransient<IProductCategoryRepo, ProductCategoryRepo>()
                .AddTransient<IProductGroupRepo, ProductGroupRepo>()
                .AddTransient<IProductRepo, ProductRepo>()
                .AddTransient<IVehicleMakeRepo, VehicleMakeRepo>()
                .AddTransient<IVehicleModelRepo, VehicleModelRepo>()
                .AddTransient<IUnitOfMeasureRepo, UnitOfMeasureRepo>()
                .AddTransient<IParameterRepo, ParameterRepo>()
                .AddTransient<IParameterGroupRepo, ParameterGroupRepo>()
                .AddTransient<IUserRolesRepo, UserRolesRepo>()
                .AddTransient<IUserRepo, UserRepo>()
                .AddTransient<IRoleRepo, RoleRepo>();
        }
    }
}
