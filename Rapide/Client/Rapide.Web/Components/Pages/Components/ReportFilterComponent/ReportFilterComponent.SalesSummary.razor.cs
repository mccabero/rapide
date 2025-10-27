using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Pages.SystemConfiguration;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator.Reports;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        private async Task PrintSalesSummaryReport(CompanyInfoDTO companyData, string preparedBy, string clientType)
        {
            var jobStatusCompleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();

            bool isClientTypeAll = clientType.Equals(Constants.ClientType.All);
            bool isChangan = isClientTypeAll
                ? false
                : clientType.Equals(Constants.ClientType.Changan);

            SalesSummaryReportGenerator.ImageFile = isChangan
                ? FileHelper.GetChanganLogo()
                : FileHelper.GetRapideLogo();

            SalesSummaryReportGenerator.ImageFileCompany = isChangan
                ? FileHelper.GetChanganCompanyLogo()
                : FileHelper.GetCompanyLogo();

            var invoices = await InvoiceService.GetAllInvoiceAsync();
            var payments = await PaymentService.GetAllPaymentAsync();
            var paymentDetails = await PaymentDetailsService.GetAllPaymentDetailsAsync();
            var expenses = await ExpensesService.GetAllExpensesAsync();
            var quickSales = await QuickSalesService.GetAllQuickSalesAsync();
            // parameters;

            var invoiceList = new List<InvoiceDTO>();
            var quickSalesList = new List<QuickSalesDTO>();
            var paymentDetailsList = new List<PaymentDetailsDTO>();

            // Filtered by date range
            var filteredExpenses = expenses
                .Where(x => x.JobStatusId == jobStatusCompleted!.Id)
                .Where(x => ((DateTime)x.ExpenseDateTime!).Date >= ((DateTime)_dateRange.Start!).Date
                    && ((DateTime)x.ExpenseDateTime!).Date <= ((DateTime)_dateRange.End!).Date)
                .ToList();

            // Filter payment if changan
            if (!isClientTypeAll)
            {
                filteredExpenses = filteredExpenses.Where(x => x.IsChangan == isChangan).ToList();
            }

            var filteredQuickSales = quickSales
                .Where(x => x.JobStatusId == jobStatusCompleted!.Id)
                .Where(x => ((DateTime)x.TransactionDate!).Date >= ((DateTime)_dateRange.Start!).Date
                    && ((DateTime)x.TransactionDate!).Date <= ((DateTime)_dateRange.End!).Date)
                .ToList();

            // Filter payment if changan
            if (!isClientTypeAll)
            {
                filteredQuickSales = filteredQuickSales.Where(x => x.IsChangan == isChangan).ToList();
            }

            var filteredPayments = payments
                .Where(x => x.JobStatusId == jobStatusCompleted!.Id)
                .Where(x => ((DateTime)x.PaymentDate!).Date >= ((DateTime)_dateRange.Start!).Date 
                    && ((DateTime)x.PaymentDate!).Date <= ((DateTime)_dateRange.End!).Date)
                .ToList();

            // Filter payment if changan
            if (!isClientTypeAll)
            {
                filteredPayments = filteredPayments.Where(x => x.IsChangan == isChangan).ToList();
            }

            var filteredPaymentDetails = paymentDetails.Where(x => filteredPayments.Any(y => y.Id == x.PaymentId)).ToList();
            //filteredPaymentDetails.Where(x => x.PaymentId == payment.Id).ToList();

            foreach (var pd in filteredPaymentDetails)
            {
                var invoicePerPd = invoices.Where(x => x.Id == pd.InvoiceId);
                pd.Invoice = invoicePerPd.FirstOrDefault() ?? null;

                paymentDetailsList.Add(pd);
            }

            var paymentsWithDetails = new List<PaymentDTO>();
            foreach (var payment in filteredPayments)
            {
                var paymentToAdd = payment;
                paymentToAdd.PaymentDetailsList = paymentDetailsList.Where(x => x.PaymentId == payment.Id).ToList();

                paymentsWithDetails.Add(paymentToAdd);
            }

            if (!filteredPayments.Any())
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "No record found for the selected date. Please try again.";
                await mboxError.ShowAsync();

                return;
            }

            await SalesSummaryReportGenerator.Generate(
                paymentsWithDetails, 
                JSRuntime, 
                companyData, 
                preparedBy, 
                filteredExpenses, 
                filteredQuickSales,
                isChangan);
        }
    }
}
