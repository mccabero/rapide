DELETE FROM dbo.InvoicePackage
DBCC CHECKIDENT ('InvoicePackage', RESEED, 0);
DELETE FROM dbo.Invoice
DBCC CHECKIDENT ('Invoice', RESEED, 0);

DELETE FROM dbo.JobOrderProduct
DBCC CHECKIDENT ('JobOrderProduct', RESEED, 0);
DELETE FROM dbo.JobOrderService
DBCC CHECKIDENT ('JobOrderService', RESEED, 0);
DELETE FROM dbo.JobOrderTechnician
DBCC CHECKIDENT ('JobOrderTechnician', RESEED, 0);
DELETE FROM dbo.JobOrderPackage
DBCC CHECKIDENT ('JobOrderPackage', RESEED, 0);
DELETE FROM dbo.JobOrder
DBCC CHECKIDENT ('JobOrder', RESEED, 0);


DELETE FROM dbo.EstimateService
DBCC CHECKIDENT ('EstimateService', RESEED, 0);
DELETE FROM dbo.EstimateProduct
DBCC CHECKIDENT ('EstimateProduct', RESEED, 0);
DELETE FROM dbo.EstimateTechnician
DBCC CHECKIDENT ('EstimateTechnician', RESEED, 0);
DELETE FROM dbo.EstimatePackage
DBCC CHECKIDENT ('EstimatePackage', RESEED, 0);
DELETE FROM dbo.Estimate
DBCC CHECKIDENT ('Estimate', RESEED, 0);

DELETE FROM dbo.Inspection
DBCC CHECKIDENT ('Inspection', RESEED, 0);

DELETE FROM dbo.VehicleMake
DBCC CHECKIDENT ('VehicleMake', RESEED, 0);
DELETE FROM dbo.VehicleModel
DBCC CHECKIDENT ('VehicleModel', RESEED, 0);
DELETE FROM dbo.Vehicle
DBCC CHECKIDENT ('Vehicle', RESEED, 0);

DELETE FROM dbo.Customer
DBCC CHECKIDENT ('Customer', RESEED, 0);


DELETE FROM dbo.Deposit
DBCC CHECKIDENT ('Deposit', RESEED, 0);
DELETE FROM dbo.PaymentDetails
DBCC CHECKIDENT ('PaymentDetails', RESEED, 0);
DELETE FROM dbo.Payment
DBCC CHECKIDENT ('Payment', RESEED, 0);
DELETE FROM dbo.QuickSalesProduct
DBCC CHECKIDENT ('QuickSalesProduct', RESEED, 0);
DELETE FROM dbo.QuickSales
DBCC CHECKIDENT ('QuickSales', RESEED, 0);
DELETE FROM dbo.Expenses
DBCC CHECKIDENT ('Expenses', RESEED, 0);


DELETE FROM dbo.PackageProduct
DBCC CHECKIDENT ('PackageProduct', RESEED, 0);
DELETE FROM dbo.PackageService
DBCC CHECKIDENT ('PackageService', RESEED, 0);
DELETE FROM dbo.Package
DBCC CHECKIDENT ('Package', RESEED, 0);



DELETE FROM dbo.ServiceGroup
DBCC CHECKIDENT ('ServiceGroup', RESEED, 0);
DELETE FROM dbo.ServiceCategory
DBCC CHECKIDENT ('ServiceCategory', RESEED, 0);
DELETE FROM dbo.[Service]
DBCC CHECKIDENT ('Service', RESEED, 0);

DELETE FROM dbo.ProductGroup
DBCC CHECKIDENT ('ProductGroup', RESEED, 0);
DELETE FROM dbo.ProductCategory
DBCC CHECKIDENT ('ProductCategory', RESEED, 0);
DELETE FROM dbo.[Product]
DBCC CHECKIDENT ('[Product]', RESEED, 0);
DELETE FROM dbo.Supplier
DBCC CHECKIDENT ('Supplier', RESEED, 0);
DELETE FROM dbo.Manufacturer
DBCC CHECKIDENT ('Manufacturer', RESEED, 0);


DELETE FROM dbo.UnitOfMeasure
DBCC CHECKIDENT ('UnitOfMeasure', RESEED, 0);





DELETE FROM dbo.Parameter
DBCC CHECKIDENT ('Parameter', RESEED, 0);
DELETE FROM dbo.ParameterGroup
DBCC CHECKIDENT ('ParameterGroup', RESEED, 0);




DELETE FROM dbo.JobStatus
DBCC CHECKIDENT ('JobStatus', RESEED, 0);


DELETE FROM dbo.Inspection
DBCC CHECKIDENT ('Inspection', RESEED, 0);
DELETE FROM dbo.Customer
DBCC CHECKIDENT ('Customer', RESEED, 0);

DELETE FROM dbo.[User]
DBCC CHECKIDENT ('[User]', RESEED, 0);
DELETE FROM dbo.[Role]
DBCC CHECKIDENT ('[Role]', RESEED, 0);
