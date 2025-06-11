namespace Rapide.Web.Helpers
{
    public enum ReportFilterTypes
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
    }

    public enum DialogTypes
    {
        Payment,
        User,
        Customer,
        JobStatus
    }

    public enum VehicleModelDialogType
    {
        Make,
        Body,
        Classification,
    }

    public enum ProductDialogType
    {
        ProductGroup,
        ProductCategory,
        UnitOfMeasure,
        Manufacturer,
        Supplier,
    }

    public enum JobOrderDialogType
    {
        Customer,
        Vehicle,
        ServiceAdvisor,
        ServiceGroup,
        JobStatus,
        User,
    }

    public enum EstimateDialogType
    {
        Customer,
        Vehicle,
        User,
        ServiceGroup,
        JobStatus,
    }

    public enum VehicleDialogType
    {
        Customer,
        Parameter,
        VehicleModel,
    }

    public enum ServiceDialogType
    {
        Category,
        Group,
    }
}
