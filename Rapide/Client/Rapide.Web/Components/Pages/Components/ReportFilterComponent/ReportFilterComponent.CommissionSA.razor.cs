using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator.Reports;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        private async Task PrintCommissionsSAReport(CompanyInfoDTO companyData, string preparedBy)
        {
            CommissionsSAReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            CommissionsSAReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

            var usersData = await UserService.GetAllUserRoleAsync();
            var users = new List<UserDTO>();
            foreach (var ud in usersData)
            {
                var userRole = await UserRolesService.GetUserRolesByUserIdAsync(ud.Id);
                ud.UserRoles = userRole;

                users.Add(ud);
            }

            //var jobOrders = await JobOrderService.GetAllJobOrderAsync();
            var invoice = await InvoiceService.GetAllInvoiceAsync();

            var invoiceList = new List<InvoiceDTO>();
            var packageListData = await PackageService.GetAllPackageAsync();
            var serviceAdvisors = users.Where(x => x.UserRoles.Any(x => x.Role.Name.ToUpper().Contains("ADVISOR")) && x.IsActive == true).ToList();

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

            var filteredInvoice = invoiceIds;

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

            foreach (var i in filteredInvoice)
            {
                var invoicePackageList = new List<InvoicePackageDTO>();

                var jobOrderId = i.JobOrderId;
                i.JobOrder = await JobOrderService.GetJobOrderByIdAsync(jobOrderId);
                i.JobOrder.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(jobOrderId);
                i.JobOrder.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(jobOrderId);
                i.JobOrder.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(jobOrderId);
                
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

            await CommissionsSAReportGenerator.Generate(invoiceList, JSRuntime, companyData, preparedBy, serviceAdvisors);
        }
    }
}
