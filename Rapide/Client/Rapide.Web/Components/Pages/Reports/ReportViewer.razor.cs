using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Rapide.Web.Components.Pages.Reports
{
    public partial class ReportViewer
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        #endregion

        #region Private Properties
        #endregion

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Legal);
                    page.Header().Text("Hello World");
                });
            });

            await doc.ShowInCompanionAsync();

            await base.OnInitializedAsync();
        }
    }
}
