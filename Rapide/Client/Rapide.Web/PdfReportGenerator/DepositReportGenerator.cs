using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.PdfReportGenerator
{
    public static class DepositReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static DepositDTO data { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(DepositDTO dto, IJSRuntime JSRuntime, CompanyInfoDTO companyInfoDto)
        {
            data = dto;
            companyInfo = companyInfoDto;

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Legal);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(_ =>
                        _.FontSize(12)
                        .FontFamily(Fonts.Arial)
                    );

                    // Report Header
                    page.Header().Element(ComposeHeader);

                    // Report Body
                    page.Content().Element(ComposeContent);

                    // Report Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page: ").FontSize(8);
                            x.CurrentPageNumber().FontSize(8);
                            x.Span(" of ").FontSize(8);
                            x.TotalPages().FontSize(8);
                        });
                });
            });

            //var rootFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
            var rootFileName = $"{data.ReferenceNo}.pdf";
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PDFReports", rootFileName);
            var outputFileName = $"PDFReports/{rootFileName}";

            doc.GeneratePdf(fileName);
            //doc.GeneratePdfAndShow();

            await JSRuntime.InvokeVoidAsync("open", outputFileName, "_blank");
        }

        private static void ComposeHeader(IContainer container)
        {
            // Report Header
            container.Row(row =>
            {
                row.ConstantItem(90).PaddingLeft(20).Height(59).Image(ImageFileCompany);

                row.RelativeItem().AlignCenter().Column(column =>
                {
                    column.Item().AlignCenter().Text(companyInfo.Name).FontSize(12).SemiBold();
                    column.Item().AlignCenter().Text(companyInfo.Address).FontSize(8);
                    column.Item().AlignCenter().Text(companyInfo.MobileNumber).FontSize(8);
                    column.Item().AlignCenter().Text(companyInfo.Email).FontSize(8).Underline();
                    column.Item().AlignCenter().Text(companyInfo.TIN).FontSize(8);
                });

                // Change image with "PLARIDEL, BULACAN and underline
                row.ConstantItem(90).Height(60).AlignRight().Image(ImageFile);
            });
        }

        private static void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(1);
                column.Item().PaddingTop(30).Text($"ACKNOWLEDGEMENT RECEIPT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Transaction Date: {data.CreatedDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")}").FontSize(10).AlignCenter();

                column.Item().PaddingBottom(10).PaddingTop(20).Element(ComposeBody);

                column.Item().PaddingBottom(25).PaddingTop(25).Element(ComposeSignatories);
            });
        }

        private static void ComposeBody(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(3);
                column.Item().Text(text => {
                    text.Span("Date: ").Bold();
                    text.Span(DateTime.Now.ToLongDateString());
                });

                column.Item().Text(text => {
                    text.Span("Received From: ").Bold();
                    text.Span($"{data.Customer.FirstName} {data.Customer.LastName}");
                });

                column.Item().Text(text => {
                    text.Span("Mobile Number: ").Bold();
                    text.Span(data.Customer.MobileNumber);
                });

                column.Item().Text(text => {
                    text.Span("Address: ").Bold();
                    text.Span(data.Customer.HomeAddress);
                });

                column.Item().PaddingTop(30).Text(text =>
                {
                    text.Span("Dear ");
                    text.Span($"{data.Customer.FirstName} {data.Customer.LastName}");
                });

                column.Item().PaddingTop(30).Text(text =>
                {
                    text.Span("This is to acknowledge that your deposit has been received. in the amount of ");
                    text.Span($"{CurrencyHelper.NumberToWords.ConvertAmount(double.Parse(data.DepositAmount.ToString()))} ({data.DepositAmount})").Bold().Underline();
                    text.Span(" on ");
                    text.Span(((DateTime)data.TransactionDateTime).ToLongDateString()).Bold();
                    text.Span(" from you as a deposit payment for Job Order #: ");
                    text.Span(data.JobOrder.ReferenceNo).Bold().Underline();
                });

                column.Item().PaddingTop(30).PaddingBottom(20).Text("This amount constitutes the full payment for your current job order.");
            });
        }

        private static void ComposeSignatories(IContainer container)
        {
            container.Row(r =>
            {
                r.Spacing(150);

                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Prepared By:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.PreparedBy.FirstName} {data.PreparedBy.LastName}").Bold().FontSize(8);
                    c.Item().Text(data.PreparedBy.Role.Name).FontSize(8);
                });

                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Conforme by:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.Customer.FirstName} {data.Customer.LastName}").Bold().FontSize(8);
                    c.Item().Text("Customer").FontSize(8);
                });
            });
        }
    }
}
