using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator.Reports;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        private async Task PrintPettyCashVoucheReportr(CompanyInfoDTO companyData, string preparedBy, string clientType)
        {
            var start = _dateRange.Start.Value;
            var endExclusive = _dateRange.End.Value.AddDays(1);

            bool isClientTypeAll = clientType.Equals(Constants.ClientType.All);
            bool isChangan = isClientTypeAll
                ? false
                : clientType.Equals(Constants.ClientType.Changan);

            PettyCashVourcherReportGenerator.ImageFile = isChangan
                ? FileHelper.GetChanganLogo()
                : FileHelper.GetRapideLogo();

            PettyCashVourcherReportGenerator.ImageFileCompany = isChangan
                ? FileHelper.GetChanganCompanyLogo()
                : FileHelper.GetCompanyLogo();

            // get all data
            var pettyCashList = await PettyCashService.GetAllPettyCashAsync();

            var filteredPettyCash = pettyCashList.Where(x => x.TransactionDateTime >= start && x.TransactionDateTime < endExclusive).ToList();

            // Filter payment if changan
            if (!isClientTypeAll)
            {
                filteredPettyCash = filteredPettyCash.Where(x => x.IsChangan == isChangan).ToList();
            }

            if (!filteredPettyCash.Any())
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "No record found for the selected date. Please try again.";
                await mboxError.ShowAsync();

                return;
            }

            await PettyCashVourcherReportGenerator.Generate(
                filteredPettyCash,
                JSRuntime,
                companyData,
                preparedBy,
                isChangan);
        }
    }
}
