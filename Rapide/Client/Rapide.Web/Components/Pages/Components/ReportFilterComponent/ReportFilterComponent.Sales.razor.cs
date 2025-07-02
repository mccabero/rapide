using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator.Reports;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        private async Task PrintSalesReport(CompanyInfoDTO companyData, string preparedBy)
        {
            SalesReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            SalesReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

            var jobOrders = await JobOrderService.GetAllJobOrderAsync();
            var invoice = await InvoiceService.GetAllInvoiceAsync();
            var expenses = await ExpensesService.GetAllExpensesAsync();
            var quickSales = await QuickSalesService.GetAllQuickSalesAsync();

            var invoiceList = new List<InvoiceDTO>();
            var quickSalesList = new List<QuickSalesDTO>();

            var packageListData = await PackageService.GetAllPackageAsync();

            // Convert filter from invoice to payment:
            var payments = await PaymentService.GetAllPaymentAsync();
            var filteredPayments = payments
                .Where(x => x.JobStatus.Name.Equals(Constants.JobStatus.Completed))
                .Where(x => ((DateTime)x.PaymentDate!).Date >= ((DateTime)_dateRange.Start!).Date 
                        && ((DateTime)x.PaymentDate!).Date <= ((DateTime)_dateRange.End!).Date)
                .ToList();
            
            var invoiceFromPayments = filteredPayments.Select(x => x.InvoiceList);
            var paymentDetails = await PaymentDetailsService.GetAllPaymentDetailsAsync();

            var invoiceNew = paymentDetails.Where(x => filteredPayments.Any(y => y.Id == x.PaymentId)).ToList();
            var invoiceIds = invoice.Where(x => invoiceNew.Any(y => y.InvoiceId == x.Id)).ToList();

            //var filteredInvoice = invoice.Where(x => ((DateTime)x.InvoiceDate!).Date >= ((DateTime)_dateRange.Start!).Date && ((DateTime)x.InvoiceDate!).Date <= ((DateTime)_dateRange.End!).Date).ToList();
            var filteredInvoice = invoiceIds;

            var filteredExpenses = expenses.Where(x => ((DateTime)x.ExpenseDateTime!).Date >= ((DateTime)_dateRange.Start!).Date && ((DateTime)x.ExpenseDateTime!).Date <= ((DateTime)_dateRange.End!).Date).ToList();
            var filteredQuickSales = quickSales.Where(x => ((DateTime)x.TransactionDate!).Date >= ((DateTime)_dateRange.Start!).Date && ((DateTime)x.TransactionDate!).Date <= ((DateTime)_dateRange.End!).Date).ToList();

            var invoiceToCheck = filteredInvoice.Except(invoiceIds).ToList();
            if (invoiceToCheck.Any())
            {
                IsLoading = false;
                StateHasChanged();

                var invoiceToCheckList = string.Join(", ", invoiceToCheck.Select(x => x.InvoiceNo));

                mBoxCustomMessage = $"Error generating report. Please check invoices: [{invoiceToCheckList}]";
                await mboxError.ShowAsync();

                return;
            }

            if (!filteredInvoice.Any())
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "No record found for the selected date. Please try again.";
                await mboxError.ShowAsync();

                return;
            }

            foreach (var qsp in filteredQuickSales)
            {
                var qsProducts = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(qsp.Id);
                qsp.ProductList = qsProducts;

                quickSalesList.Add(qsp);
            }

            foreach (var i in filteredInvoice)
            {
                var invoicePackageList = new List<InvoicePackageDTO>();

                var jobOrderId = i.JobOrderId;
                i.JobOrder = await JobOrderService.GetJobOrderByIdAsync(jobOrderId);
                i.JobOrder.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(jobOrderId);
                i.JobOrder.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(jobOrderId);

                // payments
                i.PaymentDetailsList = await PaymentDetailsService.GetAllPaymentDetailsByInvoiceIdAsync(i.Id);

                // loop package
                if (i.IsPackage)
                {
                    var packageListInfo = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(i.Id);

                    foreach (var p in packageListInfo)
                    {
                        var package = packageListData.Where(x => x.Id == p.PackageId);

                        invoicePackageList.Add(new InvoicePackageDTO()
                        {
                            Id = p.Id,
                            Package = (package.Any() && package != null) ? package.FirstOrDefault() : new PackageDTO(),

                        });
                    }

                    i.PackageList = invoicePackageList;
                }

                invoiceList.Add(i);
            }


            // Check for invoice without service
            var invoiceWithoutService = invoiceList.Select(x => x.JobOrder).Where(x => x.ServiceList == null).ToList();
            if (invoiceWithoutService.Any())
            {
                IsLoading = false;
                StateHasChanged();

                var listofJO = invoiceWithoutService.Distinct().Select(x => x.ReferenceNo);
                var invoiceNos = string.Join(", ", listofJO);

                mBoxCustomMessage = $"Unable to process the report. Invoice JO without service has been found!{Environment.NewLine}[{invoiceNos}]";
                await mboxError.ShowAsync();

                return;
            }

            // Check for invoice without payment
            var invoiceWithoutPayment = invoiceList.Where(x => x.PaymentDetailsList == null).ToList();
            if (invoiceWithoutPayment.Any())
            {
                IsLoading = false;
                StateHasChanged();

                var listOfInvoiceNo = invoiceWithoutPayment.Distinct().Select(x => x.InvoiceNo);
                var invoiceNos = string.Join(", ", listOfInvoiceNo);

                mBoxCustomMessage = $"Unable to process the report. Invoice without payment has been found!{Environment.NewLine}[{invoiceNos}]";
                await mboxError.ShowAsync();

                return;
            }

            await SalesReportGenerator.Generate(invoiceList, JSRuntime, companyData, preparedBy, filteredExpenses, quickSalesList, isCashier);
        }
    }
}
