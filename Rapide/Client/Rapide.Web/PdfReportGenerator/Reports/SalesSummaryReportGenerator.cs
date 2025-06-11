using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using Rapide.Entities;
using System.Reflection.PortableExecutable;

namespace Rapide.Web.PdfReportGenerator.Reports
{
    public static class SalesSummaryReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<PaymentDTO> payments { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }
        private static List<ExpensesDTO> expenses { get; set; }
        private static List<QuickSalesDTO> quickSales { get; set; }

        public static async Task Generate(
            List<PaymentDTO> paymentData, 
            IJSRuntime JSRuntime, 
            CompanyInfoDTO companyInfoDto, 
            string preparedBy,
            List<ExpensesDTO> expensesData,
            List<QuickSalesDTO> quickSalesData)
        {
            payments = paymentData;
            companyInfo = companyInfoDto;
            expenses = expensesData;
            quickSales = quickSalesData;

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

            var dateCoverage = $"{((DateTime)paymentData.Min(x => x.PaymentDate)!).ToString("MMMM-dd")}-to-{((DateTime)paymentData.Max(x => x.PaymentDate)!).ToString("dd-yyyy")}";
            //var rootFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
            var rootFileName = $"SALES-SUMMARY-REPORT-{dateCoverage.ToUpper()}.pdf";
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
        }

        private static void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(1);
                column.Item().Text($"SALES SUMMARY REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Coverage: {((DateTime)payments.Min(x => x.PaymentDate)!).ToShortDateString()} ~ {((DateTime)payments.Max(x => x.PaymentDate)!).ToShortDateString()}").FontSize(8).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                //column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            decimal totalSalesFooter = 0;
            decimal totalESFooter = 0;
            decimal totalDiscountFooter = 0;
            decimal totalQuickSalesFooter = 0;
            decimal totalRightFooter = 0;

            container.Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(); // Date
                        columns.RelativeColumn(); // Total Sales

                        var paymentDetails = payments.SelectMany(x => x.PaymentDetailsList);
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct();
                        foreach (var t in paymentTypes)
                        {
                            columns.RelativeColumn();
                        }

                        columns.RelativeColumn(); // ES
                        columns.RelativeColumn(); // DISCOUNT
                        columns.RelativeColumn(); // CLT

                        columns.RelativeColumn(); // TOTAL
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("DATE");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("SALES");

                        var paymentDetails = payments.SelectMany(x => x.PaymentDetailsList);
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct();
                        foreach (var t in paymentTypes)
                        {
                            header.Cell().Element(CellStyle).PaddingTop(1).Text(t.ToUpper());
                        }

                        header.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text($"ES");
                        header.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text($"DISCOUNT");
                        header.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text($"QS");

                        header.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text($"TOTAL");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2).BorderBottom(1)
                                .BorderTop(1).PaddingBottom(5)
                                .PaddingTop(5).BorderColor(Colors.Black);
                        }
                    });

                    var paymentPerDay = payments.GroupBy(x => x.PaymentDate.Value.Date).ToList().OrderBy(x => x.Key);

                    foreach (var i in paymentPerDay)
                    {
                        var quickSalesPerDay = quickSales.Where(x => x.TransactionDate.Value.Date == i.Key.Date).ToList();
                        var expensesPerDay = expenses.Where(x => x.ExpenseDateTime.Value.Date == i.Key.Date).ToList();
                        
                        var paymentsFromDate = payments.Where(x => x.PaymentDate.Value.Date == i.Key.Date);
                        var paymentDetailsFromDate = paymentsFromDate.SelectMany(x => x.PaymentDetailsList);

                        decimal totalPerDay = 0;
                        var invoicePerPDDay = paymentDetailsFromDate.Select(x => x.Invoice).ToList();
                        var invoiceDistinct = invoicePerPDDay.DistinctBy(x => x.Id).ToList();

                        decimal totalDiscount = invoiceDistinct.Sum(x => x.ProductDiscount)
                            + invoiceDistinct.Sum(x => x.LaborDiscount)
                            + invoiceDistinct.Sum(x => x.AdditionalDiscount);

                        totalDiscountFooter += totalDiscount;

                        var totalQuickSales = quickSalesPerDay.Sum(x => x.TotalAmount);
                        totalQuickSalesFooter += totalQuickSales;

                        table.Cell().Element(CellStyle).Text(i.Key.ToShortDateString()); // Date

                        var totalSales = i.Sum(x => x.TotalPaidAmount) + totalDiscount + totalQuickSales; // includes discount?
                        totalSalesFooter += totalSales;

                        table.Cell().Element(CellStyle).Text(totalSales.ToString("N2")); // Sales

                        var totalExpenses = expensesPerDay.Sum(x => x.Amount);
                        totalESFooter += totalExpenses;

                        


                        var paymentDetails = payments.SelectMany(x => x.PaymentDetailsList);
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct();
                        foreach (var t in paymentTypes)
                        {
                            var sumPerType = paymentDetailsFromDate.Where(x => x.PaymentTypeParameter.Name.ToUpper() == t.ToUpper()).Sum(x => x.AmountPaid);
                            totalPerDay += sumPerType;

                            var quickSalesPerType = quickSalesPerDay.Where(x => x.PaymentTypeParameter.Name.ToUpper() == t.ToUpper());

                            if (t.ToUpper() == "CASH")
                            {
                                sumPerType = (sumPerType - totalExpenses); // + quick sales
                            }

                            sumPerType += quickSalesPerType.Sum(x => x.TotalAmount);

                            table.Cell().Element(CellStyle).PaddingTop(1).Text(sumPerType.ToString("N2"));
                        }

                        totalRightFooter += (totalPerDay + totalDiscount + totalQuickSales);


                        table.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text(totalExpenses.ToString("N2"));
                        table.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text(totalDiscount.ToString("N2"));
                        table.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text(totalQuickSales.ToString("N2"));

                        table.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text((totalPerDay + totalDiscount + totalQuickSales).ToString("N2"));
                    }

                    #region Spacer
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");

                    var paymentTypesSpacer = payments.Select(x => x.PaymentDetailsList.Distinct());
                    foreach (var t in paymentTypesSpacer)
                    {
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
                    }

                    table.Cell().Element(CellStyle).PaddingRight(10).Text("").AlignRight();
                    table.Cell().Element(CellStyle).PaddingRight(10).Text("").AlignRight();
                    table.Cell().Element(CellStyle).PaddingRight(10).Text("").AlignRight();

                    table.Cell().Element(CellStyle).PaddingRight(10).Text("").AlignRight();
                    #endregion

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(6)).PaddingVertical(4);
                    }
                });

                // Footer
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(); // Date
                        columns.RelativeColumn(); // Total Sales

                        var paymentDetails = payments.SelectMany(x => x.PaymentDetailsList);
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct();
                        foreach (var t in paymentTypes)
                        {
                            columns.RelativeColumn();
                        }

                        columns.RelativeColumn(); // ES
                        columns.RelativeColumn(); // DISCOUNT
                        columns.RelativeColumn(); // CLT

                        columns.RelativeColumn(); // TOTAL
                    });

                    table.Footer(footer =>
                    {
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text(totalSalesFooter.ToString("N2"));

                        var paymentDetails = payments.SelectMany(x => x.PaymentDetailsList);
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct();
                        foreach (var t in paymentTypes)
                        {
                            var quickSalesPerType = quickSales.Where(x => x.PaymentTypeParameter.Name.ToUpper() == t.ToUpper());
                            var sumPerType = paymentDetails.Where(x => x.PaymentTypeParameter.Name.ToUpper() == t.ToUpper()).Sum(y => y.AmountPaid);

                            if (t.ToUpper() == "CASH")
                            {
                                sumPerType = (sumPerType - totalESFooter); // + quick sales
                            }

                            sumPerType += quickSalesPerType.Sum(x => x.TotalAmount);

                            table.Cell().Element(CellStyle).PaddingTop(1).Text(sumPerType.ToString("N2"));
                        }


                        table.Cell().Element(CellStyle).PaddingRight(10).Text(totalESFooter.ToString("N2")).AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(10).Text(totalDiscountFooter.ToString("N2")).AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(10).Text(totalQuickSalesFooter.ToString("N2")).AlignRight();

                        table.Cell().Element(CellStyle).PaddingRight(10).Text((totalRightFooter).ToString("N2")).AlignRight();

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
