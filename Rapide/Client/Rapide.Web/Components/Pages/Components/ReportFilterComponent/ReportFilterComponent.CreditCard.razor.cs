using Rapide.DTO;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator.Reports;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        private async Task PrintCreditCardPaymentReport(CompanyInfoDTO companyData, string preparedBy)
        {
            CreditCardPaymentReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            CreditCardPaymentReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

            var jobOrders = await JobOrderService.GetAllJobOrderAsync();
            var invoice = await InvoiceService.GetAllInvoiceAsync();
            var quickSales = await QuickSalesService.GetAllQuickSalesAsync();

            var invoiceList = new List<InvoiceDTO>();
            var quickSalesList = new List<QuickSalesDTO>();
            var packageListData = await PackageService.GetAllPackageAsync();


            // Convert filter from invoice to payment:
            var payments = await PaymentService.GetAllPaymentAsync();
            var filteredPayments = payments.Where(x => ((DateTime)x.PaymentDate!).Date >= ((DateTime)_dateRange.Start!).Date && ((DateTime)x.PaymentDate!).Date <= ((DateTime)_dateRange.End!).Date).ToList();
            var invoiceFromPayments = filteredPayments.Select(x => x.InvoiceList);
            var paymentDetails = await PaymentDetailsService.GetAllPaymentDetailsAsync();

            var invoiceNew = paymentDetails.Where(x => filteredPayments.Any(y => y.Id == x.PaymentId)).ToList();
            var invoiceIds = invoice.Where(x => invoiceNew.Any(y => y.InvoiceId == x.Id)).ToList();

            var filteredInvoice = invoiceIds;
            //var filteredInvoice = invoice
            //    .Where(x => x.InvoiceDate >= _dateRange.Start && x.InvoiceDate <= _dateRange.End)
            //    .ToList();

            var filteredQuickSales = quickSales.Where(x => ((DateTime)x.TransactionDate!).Date >= ((DateTime)_dateRange.Start!).Date && ((DateTime)x.TransactionDate!).Date <= ((DateTime)_dateRange.End!).Date).ToList();

            if (!filteredInvoice.Any())
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "No record found for the selected date. Please try again.";
                await mboxError.ShowAsync();

                return;
            }

            foreach (var i in filteredInvoice)
            {
                var invoicePackageList = new List<InvoicePackageDTO>();

                var jobOrderId = i.JobOrderId;

                // payments
                i.PaymentDetailsList = await PaymentDetailsService.GetAllPaymentDetailsByInvoiceIdAsync(i.Id);

                invoiceList.Add(i);
            }

            foreach (var qsp in filteredQuickSales)
            {
                var qsProducts = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(qsp.Id);
                qsp.ProductList = qsProducts;

                quickSalesList.Add(qsp);
            }

            await CreditCardPaymentReportGenerator.Generate(invoiceList, quickSalesList, JSRuntime, companyData, preparedBy);
        }
    }
}
