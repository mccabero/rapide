namespace Rapide.Web.Components.Utilities
{
    public static class Constants
    {
        public static string JWTToken { get; set; } = "";
        public const string MediaType = "application/json";
        public const string Bearer = "bearer";
        public const string LocalToken = "JWT Token";
        public const string LocalUserDetails = "UserDetails";
        public const string ApiUrl = "ApiUrl";
        public const string SecretKey = "8Zz5tw0Ionm3XPZZfN0NOml3z9FMfmpgXwovR9fp6ryDIoGRM8EPHAB6iHsc0fb";
        public const string IssuerValue = "RAPIDE_POS_2025";
        public const string AudienceValue = "http://localhost/";
        public const int TokenExpiryValue = 1;

        public static class ReportType
        {
            public const string Customers = "Customers";
            public const string Vehicles = "Vehicles";
            public const string Services = "Services";
            public const string SalesReport = "Sales";
            public const string SalesSummaryReport = "Sales-Summary";
            public const string Expenses = "Expenses";
            public const string IncentivesTech = "Incentives-Tech";
            public const string IncentivesSA = "Incentives-SA";
            public const string CommissionsTech = "Commissions-Tech";
            public const string CommissionsSA = "Commissions-SA";
            public const string CreditCardPayment = "Credit-Card-Payment";
        }

        public static class ParameterType
        {
            public const string RegionParam = "REGION";
            public const string BodyTypeParam = "BODY TYPE";
            public const string ClassificationParam = "CLASSIFICATION";
            public const string TransmissionParam = "TRANSMISSION";
            public const string EngineTypeParam = "ENGINE TYPE";
            public const string EngineSizeParam = "ENGINE SIZE";
            public const string OdometerTypeParam = "ODOMETER TYPE";
            public const string PaymentTypeParam = "PAYMENT TYPE";
            public const string CustomerRegistrationTypeParam = "CUSTOMER REGISTRATION TYPE";
        }

        public static class UserRoles
        {
            public const string Owner = "OWNER";
            public const string SystemAdministrator = "SYSTEM ADMINISTRATOR";
            public const string RapideAdministrator = "RAPIDE ADMINISTRATOR";
            public const string RapideAssistantAdministrator = "RAPIDE ASSISTANT ADMINISTRATOR";
            public const string Supervisor = "SUPERVISOR";
            public const string ServiceAdvisor = "SERVICE ADVISOR";
            public const string Cashier = "CASHIER";
            public const string Estimator = "ESTIMATOR";
            public const string SeniorTechnician = "SENIOR TECHNICIAN";
            public const string JuniorTechnician = "JUNIOR TECHNICIAN";

            public const string OIC = "OIC";
            public const string HR = "HR";
            public const string Accountant = "ACCOUNTANT";
        }

        public static class JobStatus
        {
            public const string Open = "OPEN";
            public const string Converted = "CONVERTED";
            public const string InProgress = "IN PROGRESS";
            public const string Completed = "COMPLETED";
            public const string Release = "RELEASE";
            public const string Cancelled = "CANCELLED";
        }

        public static class Operations
        {
            public const string Inspections = "INSPECTION";
        }
    }
}
