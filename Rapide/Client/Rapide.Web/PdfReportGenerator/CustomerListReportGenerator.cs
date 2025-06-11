using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;

namespace Rapide.Web.PdfReportGenerator
{
    public static class CustomerListReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<CustomerDTO> data { get; set; }
        private static DateTime dateFrom { get; set; }
        private static DateTime dateTo { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(List<CustomerDTO> dto, IJSRuntime JSRuntime, DateTime _from, DateTime _to, CompanyInfoDTO companyInfoDto)
        {
            try
            {
                data = dto;
                dateFrom = _from;
                dateTo = _to;
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
                            _.FontSize(10)
                            .FontFamily(Fonts.Arial)
                        );

                        // Report Header
                        page.Header().Element(ComposeHeader);

                        // Report Body
                        //page.Content().Element(ComposeContent);

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
                var rootFileName = $"CUSTOMER_LIST_REPORT.pdf";
                var fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PDFReports", rootFileName);
                var outputFileName = $"PDFReports/{rootFileName}";

                doc.GeneratePdf(fileName);
                //doc.GeneratePdfAndShow();

                await JSRuntime.InvokeVoidAsync("open", outputFileName, "_blank");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void ComposeHeader(IContainer container)
        {
            // Report Header
            container.Row(row =>
            {
                row.ConstantItem(70).Height(59).Image(ImageFileCompany);

                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(companyInfo.Name).FontSize(12).SemiBold();
                    column.Item().Text(companyInfo.Address).FontSize(8);
                    column.Item().Text(companyInfo.MobileNumber).FontSize(8);
                    column.Item().Text(companyInfo.Email).FontSize(8).Underline();
                    column.Item().Text(companyInfo.TIN).FontSize(8);
                });

                // Change image with "PLARIDEL, BULACAN and underline
                row.ConstantItem(90).Height(60).AlignRight().Image(ImageFile);
            });

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(1);
                    column.Item().Text($"CUSTOMERS LIST REPORT").FontSize(12).Bold().Underline().AlignCenter();
                    column.Item().Text($"Date Range: {dateFrom.ToShortDateString()} ~ {dateTo.AddDays(5).ToShortDateString()}").FontSize(10).AlignCenter();

                    //column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                    //column.Item().Element(ComposeTableDetails);
                });
            });
            //container.PaddingVertical(10).Column(column =>
            //{
            //    column.Spacing(1);
            //    column.Item().Text($"CUSTOMERS LIST REPORT").FontSize(12).Bold().Underline().AlignCenter();
            //    column.Item().Text($"Date Range: {dateFrom.ToShortDateString()} ~ {dateTo.AddDays(5).ToShortDateString()}").FontSize(10).AlignCenter();

            //    column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
            //    column.Item().Element(ComposeTableDetails);

            //});
        }

        private static void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(1);
                column.Item().Text($"CUSTOMERS LIST REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Range: {dateFrom.ToShortDateString()} ~ {dateTo.AddDays(5).ToShortDateString()}").FontSize(10).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Customer Code
                    columns.RelativeColumn(4); // Full Name
                    columns.RelativeColumn(3); // Mobile Number
                    columns.RelativeColumn(6); // Address
                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Customer Code").Bold();
                });
                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Full Name").Bold();
                });
                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Mobile Number").Bold();
                });
                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Address").Bold();
                });

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(10)).BorderBottom(1).BorderTop(1).PaddingVertical(5);
                }
            });
        }

        private static void ComposeTableDetails(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Customer Code
                    columns.RelativeColumn(4); // Full Name
                    columns.RelativeColumn(3); // Mobile Number
                    columns.RelativeColumn(6); // Address
                });

                foreach (var c in data)
                {
                    table.Cell().Element(CellStyle).Text(t =>
                    {
                        t.Span(c.CustomerCode);
                    });
                    table.Cell().Element(CellStyle).Text(t =>
                    {
                        t.Span($"{c.FirstName} {c.LastName}");
                    });
                    table.Cell().Element(CellStyle).Text(t =>
                    {
                        t.Span(c.MobileNumber);
                    });
                    table.Cell().Element(CellStyle).Text(t =>
                    {
                        t.Span(c.HomeAddress);
                    });
                }
                
                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(10)).PaddingVertical(5);
                }
            });
        }
    }
}
