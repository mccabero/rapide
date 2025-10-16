using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using System.Reflection.PortableExecutable;

namespace Rapide.Web.PdfReportGenerator.Reports
{
    public static class PettyCashVourcherReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<PettyCashDTO> pettyCash { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        private static bool IsChangan { get; set; }

        public static async Task Generate(
            List<PettyCashDTO> pettyCashData, 
            IJSRuntime JSRuntime, 
            CompanyInfoDTO companyInfoDto, 
            string preparedBy,
            bool isChangan)
        {
            pettyCash = pettyCashData;
            companyInfo = companyInfoDto;

            IsChangan = isChangan;

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Legal.Landscape());
                    page.Margin(6, Unit.Millimetre);
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
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(10);
                                        columns.RelativeColumn(10);
                                        columns.RelativeColumn(10);
                                    });

                                    table.Cell().AlignLeft().Element(CellStyle).Text(t =>
                                    {
                                        t.Span("Prepared By: ").Bold();
                                        t.Span(preparedBy);
                                    });

                                    table.Cell().AlignCenter().Element(CellStyle).Text(x =>
                                    {
                                        x.Span("Page: ").FontSize(6);
                                        x.CurrentPageNumber().FontSize(6);
                                        x.Span(" of ").FontSize(6);
                                        x.TotalPages().FontSize(6);
                                    });

                                    table.Cell().AlignRight().Element(CellStyle).Text(t =>
                                    {
                                        t.Span("Prepared Date & Time: ").Bold();
                                        t.Span(DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm tt"));
                                    });

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.FontSize(6)).PaddingVertical(5);
                                    }
                                });
                            });
                        });
                });
            });

            var dateCoverage = $"{((DateTime)pettyCashData.Min(x => x.TransactionDateTime)!).ToString("MMMM-dd")}-to-{((DateTime)pettyCashData.Max(x => x.TransactionDateTime)!).ToString("dd-yyyy")}";
            //var rootFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
            var rootFileName = $"PETTY-CASH-{dateCoverage.ToUpper()}.pdf";
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
                    column.Item().AlignCenter().Text(IsChangan ? companyInfo.Name1 : companyInfo.Name).FontSize(12).SemiBold();
                    column.Item().AlignCenter().Text(IsChangan ? companyInfo.Address1 : companyInfo.Address).FontSize(8);
                    column.Item().AlignCenter().Text(IsChangan ? companyInfo.MobileNumber1 : companyInfo.MobileNumber).FontSize(8);
                    column.Item().AlignCenter().Text(IsChangan ? companyInfo.Email1 : companyInfo.Email).FontSize(8).Underline();
                    column.Item().AlignCenter().Text(IsChangan ? companyInfo.TIN1 : companyInfo.TIN).FontSize(8);
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
                column.Item().Text($"PETTY CASH VOUCHER REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Coverage: {((DateTime)pettyCash.Min(x => x.TransactionDateTime)!).ToShortDateString()} ~ {((DateTime)pettyCash.Max(x => x.TransactionDateTime)!).ToShortDateString()}").FontSize(8).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                //column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            decimal cashInTotal = pettyCash.Sum(x => x.CashIn);
            decimal cashOutTotal = pettyCash.Sum(x => x.CashOut);
            decimal balanceTotal = pettyCash.OrderBy(x => x.Id).LastOrDefault().Balance;

            container.Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(60); // PCV No
                        columns.ConstantColumn(70); // Transaction Date
                        columns.ConstantColumn(90); // Pay To
                        columns.ConstantColumn(120); // Payment Received By
                        columns.ConstantColumn(170); // Particulars
                        
                        columns.ConstantColumn(120); // Cash In
                        columns.ConstantColumn(120); // Cash Out
                        columns.ConstantColumn(120); // Balance

                        columns.ConstantColumn(100); // Encoded By
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("PCV NO.");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("DATE");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("PAY TO");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("PAYMENT RECEIVED BY");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("PARTICULARS");
                        
                        header.Cell().Element(CellStyle).PaddingTop(1).PaddingLeft(15).Text("CASH IN");
                        header.Cell().Element(CellStyle).PaddingTop(1).PaddingLeft(10).Text("CASH OUT");
                        header.Cell().Element(CellStyle).PaddingTop(1).PaddingLeft(10).Text("BALANCE");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("ENCODED BY");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2).BorderBottom(1)
                                .BorderTop(1).PaddingBottom(5)
                                .PaddingTop(5).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var i in pettyCash)
                    {
                        // Loop petty cash data here
                        table.Cell().Element(CellStyle).Text(i.PCNo);
                        table.Cell().Element(CellStyle).Text(i.TransactionDateTime.ToShortDateString());
                        table.Cell().Element(CellStyle).Text(i.PayTo);
                        table.Cell().Element(CellStyle).Text(i.PaymentReceivedBy);
                        table.Cell().Element(CellStyle).Text(i.Particulars);
                        

                        table.Cell().Element(CellStyle).PaddingRight(70)
                                .Text(i.CashIn > 0 ? i.CashIn.ToString("N2") : "-")
                                .AlignRight();

                        table.Cell().Element(CellStyle).PaddingRight(70)
                               .Text(i.CashOut > 0 ? i.CashOut.ToString("N2") : "-")
                               .AlignRight();

                        table.Cell().Element(CellStyle).PaddingRight(70)
                               .Text(i.Balance > 0 ? i.Balance.ToString("N2") : "-")
                               .AlignRight();

                        table.Cell().Element(CellStyle).Text($"{i.PaidByUser.FirstName} {i.PaidByUser.LastName}");
                    }

                    #region Spacer
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                    #endregion

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(6)).PaddingVertical(4);
                    }
                });

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(60); // PCV No
                        columns.ConstantColumn(70); // Transaction Date
                        columns.ConstantColumn(90); // Pay To
                        columns.ConstantColumn(120); // Payment Received By
                        columns.ConstantColumn(170); // Particulars

                        columns.ConstantColumn(120); // Cash In
                        columns.ConstantColumn(120); // Cash Out
                        columns.ConstantColumn(120); // Balance

                        columns.ConstantColumn(100); // Encoded By
                    });

                    table.Footer(footer =>
                    {
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");

                        table.Cell().Element(CellStyle).PaddingRight(70).Text(cashInTotal.ToString("N2")).AlignRight(); // cash in total
                        table.Cell().Element(CellStyle).PaddingRight(70).Text(cashOutTotal.ToString("N2")).AlignRight(); // cash out total
                        table.Cell().Element(CellStyle).PaddingRight(70).Text(balanceTotal.ToString("N2")).AlignRight(); // balance total
                        table.Cell().Element(CellStyle).PaddingTop(1).Text("");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2)
                                .BorderTop(1)
                                .BorderBottom(1)
                                .PaddingTop(5)
                                .PaddingBottom(5)
                                .BorderColor(Colors.Black);
                        }

                    });
                });
            });
        }
    }
}
