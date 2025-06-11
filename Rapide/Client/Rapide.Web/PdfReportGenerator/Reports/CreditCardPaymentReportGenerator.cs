using Microsoft.JSInterop;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;

namespace Rapide.Web.PdfReportGenerator.Reports
{
    public static class CreditCardPaymentReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<InvoiceDTO> invoice { get; set; }
        private static List<QuickSalesDTO> quickSales { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(
            List<InvoiceDTO> invoiceData,
            List<QuickSalesDTO> quickSalesData,
            IJSRuntime JSRuntime, 
            CompanyInfoDTO companyInfoDto, 
            string preparedBy)
        {
            invoice = invoiceData;
            quickSales = quickSalesData;
            companyInfo = companyInfoDto;

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

            var dateCoverage = $"{((DateTime)invoice.Min(x => x.InvoiceDate)!).ToString("MMMM-dd")}-to-{((DateTime)invoice.Max(x => x.InvoiceDate)!).ToString("dd-yyyy")}";
            //var rootFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
            var rootFileName = $"CC-PAYMENT-{dateCoverage.ToUpper()}.pdf";
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
                column.Item().Text($"CREDIT CARD PAYMENT REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Coverage: {((DateTime)invoice.Min(x => x.InvoiceDate)!).ToShortDateString()} ~ {((DateTime)invoice.Max(x => x.InvoiceDate)!).ToShortDateString()}").FontSize(8).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                //column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            decimal netAmountFooter = 0;
            decimal totalDeductionFooter = 0;
            decimal whtFooter = 0;
            decimal vat12Footer = 0;
            decimal locVatFooter = 0;
            decimal intVatFooter = 0;
            decimal intTotalAmountFooter = 0;
            decimal locTotalAmountFooter = 0;

            decimal locCreditTotalFooter = 0;
            decimal locDebitTotalFooter = 0;
            decimal intCreditTotalFooter = 0;
            decimal intDebitTotalFooter = 0;

            container.Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40); // Date
                        columns.ConstantColumn(60); // Invoice Number
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).PaddingTop(5).Text("DATE");
                        header.Cell().Element(CellStyle).PaddingTop(5).Text($"INVOICE{Environment.NewLine}NUMBER");

                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"LOC.{Environment.NewLine}CREDIT{Environment.NewLine}TOTAL");
                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"LOC.{Environment.NewLine}DEBIT{Environment.NewLine}TOTAL");
                        header.Cell().Element(CellStyle).PaddingRight(25).AlignCenter().Text($"LOC.{Environment.NewLine}TOTAL{Environment.NewLine}AMOUNT");

                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"INT.{Environment.NewLine}CREDIT{Environment.NewLine}TOTAL");
                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"INT.{Environment.NewLine}DEBIT{Environment.NewLine}TOTAL");
                        header.Cell().Element(CellStyle).PaddingRight(25).AlignCenter().Text($"INT.{Environment.NewLine}TOTAL{Environment.NewLine}AMOUNT");

                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"LOC.{Environment.NewLine}MDR");
                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"INT.{Environment.NewLine}MDR");

                        header.Cell().Element(CellStyle).PaddingRight(20).AlignCenter().Text($"LOC.{Environment.NewLine}VAT{Environment.NewLine}SALES");
                        header.Cell().Element(CellStyle).PaddingRight(20).AlignCenter().Text($"INT.{Environment.NewLine}VAT{Environment.NewLine}SALES");
                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"VAT{Environment.NewLine}(12%)");
                        header.Cell().Element(CellStyle).PaddingRight(30).AlignCenter().Text($"WHT{Environment.NewLine}OF 0.5%");
                        header.Cell().Element(CellStyle).PaddingRight(10).AlignCenter().Text($"TOTAL{Environment.NewLine}DEDUCTION");
                        header.Cell().Element(CellStyle).PaddingRight(15).AlignCenter().Text($"NET{Environment.NewLine}AMOUNT");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2).BorderBottom(1)
                                .BorderTop(1).PaddingBottom(5)
                                .PaddingTop(5).BorderColor(Colors.Black);
                        }
                    });

                    var paymentDetails = invoice.SelectMany(x => x.PaymentDetailsList).Distinct().ToList();
                    var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct().ToList();

                    var paymentList = paymentDetails.Where(x => x.PaymentTypeParameter.Name.Contains("CREDIT") || x.PaymentTypeParameter.Name.Contains("DEBIT")).ToList();

                    var invoiceToLoop = invoice.Where(x => x.PaymentDetailsList.Intersect(paymentList.AsEnumerable()).Any());

                    var twoPointFive = 2.5 / 100;
                    var threePointFive = 3.5 / 100;
                    var twelve = 12.0 / 100;
                    var pointFive = 0.5 / 100;
                    var oneHundredTwelve = 112.0 / 100;

                    invoiceToLoop = invoiceToLoop.OrderBy(x => x.InvoiceDate).ToList();

                    foreach (var i in invoiceToLoop)
                    {
                        // Loop invoice here
                        table.Cell().Element(CellStyle).Text(i.InvoiceDate!.Value.ToShortDateString());
                        table.Cell().Element(CellStyle).Text(i.InvoiceNo);

                        // LOCAL COMPUTATION
                        var locCreditAmount = i.PaymentDetailsList.Where(x => x.PaymentTypeParameter.Name == "LOC. CREDIT CARD").Sum(x => x.AmountPaid);
                        var locDebitAmount = i.PaymentDetailsList.Where(x => x.PaymentTypeParameter.Name == "LOC. DEBIT CARD").Sum(x => x.AmountPaid);
                        
                        var locTotalAmount = locCreditAmount + locDebitAmount;
                        locTotalAmountFooter = locTotalAmountFooter + locTotalAmount;

                        locCreditTotalFooter += locCreditAmount;
                        locDebitTotalFooter += locDebitAmount;

                        var locVAT = locTotalAmount * (decimal)twoPointFive;
                        locVatFooter = locVatFooter + locVAT;

                        // INTERNATIONAL COMPUTATION
                        var intCreditAmount = i.PaymentDetailsList.Where(x => x.PaymentTypeParameter.Name == "INT. CREDIT CARD").Sum(x => x.AmountPaid);
                        var intDebitAmount = i.PaymentDetailsList.Where(x => x.PaymentTypeParameter.Name == "INT. DEBIT CARD").Sum(x => x.AmountPaid);
                        
                        var intTotalAmount = intCreditAmount + intDebitAmount;
                        intTotalAmountFooter = intTotalAmountFooter + intTotalAmount;

                        intCreditTotalFooter += intCreditAmount;
                        intDebitTotalFooter += intDebitAmount;

                        var intVAT = intTotalAmount * (decimal)threePointFive;
                        intVatFooter = intVatFooter + intVAT;

                        // OTHERS COMPUTATION
                        var vat12 = (locVAT + intVAT) * (decimal)twelve;
                        vat12Footer = vat12Footer + vat12;

                        var wht = (((locTotalAmount + intTotalAmount) / (decimal)oneHundredTwelve) - (locVAT + intVAT + vat12)) * (decimal)pointFive;
                        whtFooter = whtFooter + wht;

                        var totalDeduction = locVAT + intVAT + vat12 + wht;
                        totalDeductionFooter = totalDeductionFooter + totalDeduction;

                        var netAmount = (locTotalAmount + intTotalAmount) - totalDeduction;
                        netAmountFooter = netAmountFooter + netAmount;

                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locCreditAmount <= 0 ? "-" : locCreditAmount.ToString("N2")).AlignRight(); // LOC CREDIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locDebitAmount <= 0 ? "-" : locDebitAmount.ToString("N2")).AlignRight(); // LOC DEBIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locTotalAmount <= 0 ? "-" : locTotalAmount.ToString("N2")).AlignRight(); // TOTAL LOC
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intCreditAmount <= 0 ? "-" : intCreditAmount.ToString("N2")).AlignRight(); // INT CREDIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intDebitAmount <= 0 ? "-" : intDebitAmount.ToString("N2")).AlignRight(); // INT DEBIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intTotalAmount <= 0 ? "-" : intTotalAmount.ToString("N2")).AlignRight(); // TOTAL INT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("2.5%").AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("3.5%").AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(locVAT <= 0 ? "-" : locVAT.ToString("N2")).AlignRight(); // LOC VAT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intVAT <= 0 ? "-" : intVAT.ToString("N2")).AlignRight(); // INT VAT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(vat12 <= 0 ? "-" : vat12.ToString("N2")).AlignRight(); // VAT 12
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(wht <= 0 ? "-" : wht.ToString("N2")).AlignRight(); // WHT
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(totalDeduction <= 0 ? "-" : totalDeduction.ToString("N2")).AlignRight(); // TOTAL DEDUCTION
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(netAmount <= 0 ? "-" : netAmount.ToString("N2")).AlignRight(); // NET AMOUNT
                    }

                    #region QUICK SALES Spacer
                    table.Cell().Element(CellStyle).Text("- QUICK").Bold();
                    table.Cell().Element(CellStyle).Text("SALES -").Bold();
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    #endregion

                    var quickSalesDetails = quickSales.Where(x => x.PaymentTypeParameter.Name.Contains("CREDIT") || x.PaymentTypeParameter.Name.Contains("DEBIT")).ToList();

                    foreach (var qs in quickSalesDetails)
                    {
                        table.Cell().Element(CellStyle).Text(qs.TransactionDate!.Value.ToShortDateString());
                        table.Cell().Element(CellStyle).Text(qs.ReferenceNo);

                        // LOCAL COMPUTATION
                        var locCreditAmount = qs.PaymentTypeParameter.Name == "LOC. CREDIT CARD" ? qs.TotalAmount : 0;
                        var locDebitAmount = qs.PaymentTypeParameter.Name == "LOC. DEBIT CARD" ? qs.TotalAmount : 0;

                        locCreditTotalFooter += locCreditAmount;
                        locDebitTotalFooter += locDebitAmount;

                        var locTotalAmount = locCreditAmount + locDebitAmount;
                        locTotalAmountFooter = locTotalAmountFooter + locTotalAmount;

                        // LOCAL COMPUTATION
                        var intCreditAmount = qs.PaymentTypeParameter.Name == "INT. CREDIT CARD" ? qs.TotalAmount : 0;
                        var intDebitAmount = qs.PaymentTypeParameter.Name == "INT. DEBIT CARD" ? qs.TotalAmount : 0;

                        var locVAT = locTotalAmount * (decimal)twoPointFive;
                        locVatFooter = locVatFooter + locVAT;

                        var intTotalAmount = intCreditAmount + intDebitAmount;
                        intTotalAmountFooter = intTotalAmountFooter + intTotalAmount;

                        var intVAT = intTotalAmount * (decimal)threePointFive;
                        intVatFooter = intVatFooter + intVAT;

                        // OTHERS COMPUTATION
                        var vat12 = (locVAT + intVAT) * (decimal)twelve;
                        vat12Footer = vat12Footer + vat12;

                        var wht = (((locTotalAmount + intTotalAmount) / (decimal)oneHundredTwelve) - (locVAT + intVAT + vat12)) * (decimal)pointFive;
                        whtFooter = whtFooter + wht;

                        var totalDeduction = locVAT + intVAT + vat12 + wht;
                        totalDeductionFooter = totalDeductionFooter + totalDeduction;

                        var netAmount = (locTotalAmount + intTotalAmount) - totalDeduction;
                        netAmountFooter = netAmountFooter + netAmount;

                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locCreditAmount <= 0 ? "-" : locCreditAmount.ToString("N2")).AlignRight(); // LOC CREDIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locDebitAmount <= 0 ? "-" : locDebitAmount.ToString("N2")).AlignRight(); // LOC DEBIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(locTotalAmount <= 0 ? "-" : locTotalAmount.ToString("N2")).AlignRight(); // TOTAL LOC
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intCreditAmount <= 0 ? "-" : intCreditAmount.ToString("N2")).AlignRight(); // INT CREDIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intDebitAmount <= 0 ? "-" : intDebitAmount.ToString("N2")).AlignRight(); // INT DEBIT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intTotalAmount <= 0 ? "-" : intTotalAmount.ToString("N2")).AlignRight(); // TOTAL INT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("2.5%").AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("3.5%").AlignRight();
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(locVAT <= 0 ? "-" : locVAT.ToString("N2")).AlignRight(); // LOC VAT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(intVAT <= 0 ? "-" : intVAT.ToString("N2")).AlignRight(); // INT VAT
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(vat12 <= 0 ? "-" : vat12.ToString("N2")).AlignRight(); // VAT 12
                        table.Cell().Element(CellStyle).PaddingRight(35).Text(wht <= 0 ? "-" : wht.ToString("N2")).AlignRight(); // WHT
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(totalDeduction <= 0 ? "-" : totalDeduction.ToString("N2")).AlignRight(); // TOTAL DEDUCTION
                        table.Cell().Element(CellStyle).PaddingRight(25).Text(netAmount <= 0 ? "-" : netAmount.ToString("N2")).AlignRight(); // NET AMOUNT
                    }

                    #region Spacer
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
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
                        columns.ConstantColumn(40); // Date
                        columns.ConstantColumn(60); // Invoice Number
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Footer(footer =>
                    {
                        footer.Cell().Element(CellStyle).Text(""); // Invoice Date
                        footer.Cell().Element(CellStyle).Text(""); // Invoice Number
                        footer.Cell().Element(CellStyle).Text(locCreditTotalFooter.ToString("N2")); // Local Credit
                        footer.Cell().Element(CellStyle).Text(locDebitTotalFooter.ToString("N2")); // Local Debit
                        footer.Cell().Element(CellStyle).Text(locTotalAmountFooter.ToString("N2")); // Local Total Amount
                        footer.Cell().Element(CellStyle).PaddingLeft(17).Text(intCreditTotalFooter.ToString("N2")); // International Credit
                        footer.Cell().Element(CellStyle).PaddingLeft(17).Text(intDebitTotalFooter.ToString("N2")); // Internation Debit
                        footer.Cell().Element(CellStyle).PaddingLeft(17).Text(intTotalAmountFooter.ToString("N2")); // Internation Total Amount
                        footer.Cell().Element(CellStyle).Text(""); // Local MDR
                        footer.Cell().Element(CellStyle).Text(""); // International MDF
                        footer.Cell().Element(CellStyle).PaddingLeft(15).Text(locVatFooter.ToString("N2")); // Local VAT
                        footer.Cell().Element(CellStyle).PaddingLeft(20).Text(intVatFooter.ToString("N2")); // Internation VAT
                        footer.Cell().Element(CellStyle).PaddingLeft(15).Text(vat12Footer.ToString("N2")); // VAT 12
                        footer.Cell().Element(CellStyle).PaddingLeft(15).Text(whtFooter.ToString("N2")); // WHT
                        footer.Cell().Element(CellStyle).PaddingLeft(10).PaddingLeft(7).Text(totalDeductionFooter.ToString("N2")); // Total Deduction
                        footer.Cell().Element(CellStyle).PaddingLeft(10).PaddingLeft(2).Text(netAmountFooter.ToString("N2")); // Net Amount
                        
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.FontSize(6))
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
