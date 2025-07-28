using Microsoft.EntityFrameworkCore;
using Rapide.Entities;

namespace Rapide.Repositories.DBContext
{
    public class RapideDbContext (DbContextOptions<RapideDbContext> options) : DbContext(options)
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<ParameterGroup> ParameterGroup { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductGroup> ProductGroup { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<VehicleMake> VehicleMake { get; set; }
        public virtual DbSet<VehicleModel> VehicleModel { get; set; }
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<Membership> Membership { get; set; }
        public virtual DbSet<ServiceGroup> ServiceGroup { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategory { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<Package> Package { get; set; }
        public virtual DbSet<PackageProduct> PackageProduct { get; set; }
        public virtual DbSet<PackageService> PackageService { get; set; }
        public virtual DbSet<JobStatus> JobStatus { get; set; }
        public virtual DbSet<Estimate> Estimate { get; set; }
        public virtual DbSet<EstimatePackage> EstimatePackage { get; set; }
        public virtual DbSet<EstimateProduct> EstimateProduct { get; set; }
        public virtual DbSet<EstimateService> EstimateService { get; set; }
        public virtual DbSet<EstimateTechnician> EstimateTechnician { get; set; }
        public virtual DbSet<JobOrder> JobOrder { get; set; }
        public virtual DbSet<JobOrderPackage> JobOrderPackage { get; set; }
        public virtual DbSet<JobOrderProduct> JobOrderProduct { get; set; }
        public virtual DbSet<JobOrderService> JobOrderService { get; set; }
        public virtual DbSet<JobOrderTechnician> JobOrderTechnician { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoicePackage> InvoicePackage { get; set; }
        public virtual DbSet<QuickSales> QuickSales { get; set; }
        public virtual DbSet<QuickSalesProduct> QuickSalesProduct { get; set; }
        public virtual DbSet<Expenses> Expenses { get; set; }
        public virtual DbSet<Deposit> Deposit { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<Inspection> Inspection { get; set; }
        public virtual DbSet<InspectionTechnician> InspectionTechnician { get; set; }
        public virtual DbSet<CompanyInfo> CompanyInfo { get; set; }
        public virtual DbSet<PettyCash> PettyCash { get; set; }
        public virtual DbSet<PettyCashDetails> PettyCashDetails { get; set; }
    }
}
