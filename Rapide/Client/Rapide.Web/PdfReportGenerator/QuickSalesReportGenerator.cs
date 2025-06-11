using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;

namespace Rapide.Web.PdfReportGenerator
{
    public static class QuickSalesReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static QuickSalesDTO data { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(QuickSalesDTO dto, IJSRuntime JSRuntime, CompanyInfoDTO companyInfoDto)
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
                column.Item().Text($"SALES RECEIPT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Transaction Date: {data.CreatedDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")}").FontSize(10).AlignCenter();

                column.Item().PaddingBottom(5).Element(ComposeTableTop);

                //column.Item().PaddingBottom(5).Element(ComponseTopMessage);

                column.Item().PaddingTop(20).Text($"PARTS & MATERIALS").Bold().Underline().AlignCenter();
                column.Item().Element(ComposeTable);

                //column.Item().PaddingBottom(25).Element(ComponseBottomMessage);

                column.Item().PaddingBottom(25).PaddingTop(25).Element(ComposeSignatories);

                //column.Item().PaddingBottom(25).Element(ComposeTech);
                //column.Item().PaddingBottom(25).AlignCenter().Element(ComposeTechQA);
            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(4);
                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Date: ").Bold();
                    t.Span(data.TransactionDate == null ? string.Empty : ((DateTime)data.TransactionDate).ToString("MM/dd/yyyy"));

                    t.Span(Environment.NewLine);
                    t.Span("Customer: ").Bold();
                    t.Span($"{data.Customer.FirstName} {data.Customer.LastName}");

                    
                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Address: ").Bold();
                    t.Span(data.Customer.HomeAddress);

                    t.Span(Environment.NewLine);
                    t.Span("Mobile No.: ").Bold();
                    t.Span($"0{String.Format("{0:### ### ####}", Convert.ToInt64(data.Customer.MobileNumber))}");
                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Payment Type: ").Bold();
                    t.Span(data.PaymentTypeParameter.Name);

                    t.Span(Environment.NewLine);
                    t.Span("Sales Person: ").Bold();
                    t.Span($"{data.SalesPersonUser.FirstName} {data.SalesPersonUser.LastName}");

                    t.Span(Environment.NewLine);
                    t.Span("Reference No.: ").Bold();
                    t.Span($"{data.ReferenceNo}");
                });

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(10)).BorderBottom(1).BorderTop(1).PaddingVertical(5);
                }
            });
        }

        private static void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);
                    columns.ConstantColumn(55);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).PaddingLeft(5).Text("Qty");
                    header.Cell().Element(CellStyle).PaddingLeft(5).Text("Unit");
                    header.Cell().Element(CellStyle).PaddingLeft(5).Text("Parts & Materials Description");
                    header.Cell().Element(CellStyle).PaddingRight(5).AlignRight().Text("Unit Price");
                    header.Cell().Element(CellStyle).PaddingRight(5).AlignRight().Text("Total Amount");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                            .PaddingVertical(5).BorderBottom(1)
                            .BorderTop(1).PaddingBottom(5)
                            .PaddingTop(5).BorderColor(Colors.Black);
                    }
                });

                // loop for product
                foreach (var p in data.ProductList)
                {
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text(p.Qty);
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text("");
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text($"{p.Product.DisplayName}");
                    table.Cell().Element(CellStyle).PaddingRight(5).AlignRight().Text(p.Price.ToString("N2"));
                    table.Cell().Element(CellStyle).PaddingRight(5).AlignRight().Text((p.Price * p.Qty).ToString("N2"));
                }

                table.Footer(footer =>
                {
                    footer.Cell().Element(CellStyle).Text("");
                    footer.Cell().Element(CellStyle).Text("");
                    footer.Cell().Element(CellStyle).Text("");

                    footer.Cell().Element(CellStyle).AlignRight().Column(c =>
                    {
                        c.Item().AlignRight().Text("Sub Total");
                        c.Item().AlignRight().PaddingTop(3).Text("Total Amount").FontSize(10);
                        c.Item().AlignRight().PaddingTop(10).PaddingBottom(5).Text($"Payment");
                        c.Item().AlignRight().Text("Change");
                    });

                    footer.Cell().Element(CellStyle).AlignRight().Column(c =>
                    {
                        c.Item().AlignRight().Text(data.SubTotal.ToString("N2"));
                        c.Item().AlignRight().PaddingTop(3).Text(data.TotalAmount.ToString("N2")).FontSize(10);
                        c.Item().AlignRight().PaddingTop(10).PaddingBottom(5).Text(data.Payment.ToString("N2"));
                        c.Item().AlignRight().Text(data.Change.ToString("N2"));
                    });

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                            .PaddingVertical(5)
                            .BorderTop(1)
                            .BorderBottom(1)
                            .PaddingTop(5)
                            .PaddingBottom(5)
                            .BorderColor(Colors.Black);
                    }
                });

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(8)).Border(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
                }
            });
        }

        private static void ComponseTopMessage(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(1);
                column.Item().Text("PLEASE READ:").FontSize(6).Bold();
                column.Item().Text($"Under MAP Uniform Inspection Guidelines. We are required to document all our findings on your vehicle. " +
                    $"This is your estimate. Our Store Manager should bring you to your car, show you the needed repairs and go over the estimate with you." +
                    $"Item by item. All your questions should be answered. We want you to know all your opinions. This is your car. We just want to help you keep it in good running condition.")
                    .FontSize(6);
            });
        }

        private static void ComponseBottomMessage(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Item().Text($"By signing this Repair Estimate Form, the customer acknowledges that the Store Manager has brought him/her to his/her vehicle" +
                    $"and clearly showed and explained all the services/repair that are required and suggested services, parts & labor" +
                    $"is on three (3) months warranty after the date service.")
                    .FontSize(6);
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
                    c.Item().PaddingTop(30).Text($"{data.SalesPersonUser.FirstName} {data.SalesPersonUser.LastName}").Bold().FontSize(8);
                    c.Item().Text(data.SalesPersonUser.Role.Name).FontSize(8);
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
