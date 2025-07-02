using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;

namespace Rapide.Web.PdfReportGenerator.Reports
{
    public static class SalesReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<InvoiceDTO> invoice { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }
        private static List<ExpensesDTO> expenses { get; set; }
        private static List<QuickSalesDTO> quickSales { get; set; }
        private static List<DepositDTO> depositInfo { get; set; }

        private static bool isCashier { get; set; }

        public static async Task Generate(
            List<InvoiceDTO> invoiceData, 
            IJSRuntime JSRuntime, 
            CompanyInfoDTO companyInfoDto, 
            string preparedBy,
            List<ExpensesDTO> expensesData,
            List<QuickSalesDTO> quickSalesData,
            List<DepositDTO> depositData,
            bool isCashierInfo)
        {
            invoice = invoiceData;
            companyInfo = companyInfoDto;
            expenses = expensesData;
            quickSales = quickSalesData;
            isCashier = isCashierInfo;
            depositInfo = depositData;

            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
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

                var dateCoverage = $"{((DateTime)invoice.Max(x => x.InvoiceDate)!).ToString("MMMM-dd")}-to-{((DateTime)invoice.Max(x => x.InvoiceDate)!).ToString("dd-yyyy")}";
                //var rootFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
                var rootFileName = $"SALES-{dateCoverage.ToUpper()}.pdf";
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
        }

        private static void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(1);
                column.Item().Text($"SALES REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Coverage: {((DateTime)invoice.Min(x => x.InvoiceDate)!).ToShortDateString()} ~ {((DateTime)invoice.Max(x => x.InvoiceDate)!).ToShortDateString()}").FontSize(8).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                //column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            decimal discountTotalFooter = 0;

            container.Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.ConstantColumn(90);
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();

                        if (!isCashier)
                            columns.RelativeColumn();
                        
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).PaddingTop(5).Text("DATE");
                        header.Cell().Element(CellStyle).PaddingTop(5).Text("CUSTOMER");
                        header.Cell().Element(CellStyle).Text($"PARTICULARS{Environment.NewLine}(PARTS & SERVICE)");


                        header.Cell().Element(CellStyle).PaddingLeft(30).Text($"SERVICE{Environment.NewLine}AMOUNT");
                        header.Cell().Element(CellStyle).PaddingLeft(30).Text($"DISCOUNT");
                        header.Cell().Element(CellStyle).PaddingLeft(30).Text($"SELLING{Environment.NewLine}PRICE");
                        
                        // Hide for casher
                        if (!isCashier)
                            header.Cell().Element(CellStyle).PaddingLeft(30).Text($"PURCHASE{Environment.NewLine}COST");
                        
                        header.Cell().Element(CellStyle).PaddingTop(5).PaddingLeft(33).Text("TOTAL");

                        header.Cell().Element(CellStyle).Text($"INVOICE{Environment.NewLine}REF. NO.");
                        header.Cell().Element(CellStyle).Text($"MODE{Environment.NewLine}OF PAYMENT");
                        header.Cell().Element(CellStyle).PaddingLeft(15).Text($"PAYMENT{Environment.NewLine}REF. NO.");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2).BorderBottom(1)
                                .BorderTop(1).PaddingBottom(5)
                                .PaddingTop(5).BorderColor(Colors.Black);
                        }
                    });

                    //invoice = invoice.OrderBy(x => x.InvoiceDate).ToList();

                    var paymentData = invoice
                        .SelectMany(x => x.PaymentDetailsList)
                        .ToList()
                        .DistinctBy(y => y.InvoiceId).ToList().OrderBy(x => x.Payment.PaymentDate)
                        .ToList();

                    foreach (var pd in paymentData)
                    {
                        var i = invoice.Where(x => x.Id == pd.InvoiceId).FirstOrDefault();

                        var customerName = $"{i.Customer.FirstName} {i.Customer.LastName}";
                        var vehicleName = $"{i.JobOrder.Vehicle.VehicleModel.VehicleMake.Name} {i.JobOrder.Vehicle.VehicleModel.Name} {i.JobOrder.Vehicle.YearModel}";

                        // Loop invoice here
                        table.Cell().Element(CellStyle).Text(pd.Payment.PaymentDate!.Value.ToShortDateString());
                        table.Cell().Element(CellStyle).Text($"{customerName}{Environment.NewLine}{vehicleName}");
                        table.Cell().Element(CellStyle).Text(i.IsPackage ? string.Join(", ", i.PackageList.Select(x => x.Package.Name)) : "-").Bold();
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Service Amount
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Discount
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Selling Price

                        if (!isCashier)
                            table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Purchase Cost
                        
                        table.Cell().Element(CellStyle).PaddingRight(30).Text(i.TotalAmount.ToString("N2")).AlignRight();
                        table.Cell().Element(CellStyle).Text(i.InvoiceNo);

                        var paymentTypeName = (i.PaymentDetailsList != null && i.PaymentDetailsList.Select(x => x.PaymentTypeParameter.Name).Any()) ?
                            string.Join(", ", i.PaymentDetailsList.Select(x => x.PaymentTypeParameter.Name))
                            : string.Empty;

                        table.Cell().Element(CellStyle).Text(paymentTypeName);
                            table.Cell().Element(CellStyle).Text(string.Join(", ", i.PaymentDetailsList.Select(x => x.PaymentReferenceNo)));


                        if (i.JobOrder.ProductList != null)
                        {
                            foreach (var ij in i.JobOrder.ProductList)
                            {
                                var sellingPriceTotal = ij.Price * ij.Qty;
                                var purchaseCodeTotal = ij.Product.PurchaseCost * ij.Qty;

                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text(ij.Product.Name);
                                table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
                                table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
                                table.Cell().Element(CellStyle).PaddingRight(35).Text(sellingPriceTotal.ToString("N2")).AlignRight();

                                if (!isCashier)
                                    table.Cell().Element(CellStyle).PaddingRight(35).Text(purchaseCodeTotal.ToString("N2")).AlignRight();

                                table.Cell().Element(CellStyle).PaddingRight(30).Text("-").AlignRight();
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                            }
                        }

                        if (i.JobOrder.ServiceList != null)
                        {
                            var discountAmount = i.LaborDiscount + i.AdditionalDiscount + i.ProductDiscount;
                            discountTotalFooter += discountAmount;

                            bool isDiscountDisplayed = false;

                            foreach (var isvc in i.JobOrder.ServiceList)
                            {
                                var rateTotal = isvc.Rate * isvc.Hours;
                                
                                var discountDisplay = (discountAmount > 0 && !isDiscountDisplayed) ? $"({discountAmount.ToString("N2")})" : "-";

                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text(isvc.Service.Name);
                                table.Cell().Element(CellStyle).PaddingRight(35).Text(rateTotal.ToString("N2")).AlignRight();
                                table.Cell().Element(CellStyle).PaddingRight(35).Text(discountDisplay).AlignRight();
                                table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
                                
                                if (!isCashier)
                                    table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();

                                table.Cell().Element(CellStyle).PaddingRight(30).Text("-").AlignRight();
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");

                                if (discountAmount > 0)
                                    isDiscountDisplayed = true;
                            }
                        }
                    }

                    #region Spacer
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).PaddingLeft(10).Text("");
                    table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
                    table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
                    

                    if (!isCashier)
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();

                    table.Cell().Element(CellStyle).PaddingRight(40).Text("").AlignRight();
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    #endregion

                    foreach (var qs in quickSales)
                    {
                        var customerName = $"{qs.Customer.FirstName} {qs.Customer.LastName}";
                        var qsTotalAmount = qs.TotalAmount;

                        // Loop invoice here
                        table.Cell().Element(CellStyle).Text(qs.TransactionDate!.Value.ToShortDateString());
                        table.Cell().Element(CellStyle).Text($"{customerName}");

                        table.Cell().Element(CellStyle).Text("-QUICK SALES-").Bold();
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Service Amount
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Discount
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Selling Price

                        if (!isCashier)
                            table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Purchase Cost

                        table.Cell().Element(CellStyle).PaddingRight(30).Text(qsTotalAmount.ToString("N2")).AlignRight(); // Total

                        table.Cell().Element(CellStyle).Text(qs.ReferenceNo);
                        table.Cell().Element(CellStyle).Text(qs.PaymentTypeParameter.Name);
                        table.Cell().Element(CellStyle).Text(qs.PaymentReferenceNo);

                        if (qs.ProductList != null)
                        {
                            foreach (var p in qs.ProductList)
                            {
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");

                                table.Cell().Element(CellStyle).Text(p.Product.Name);
                                table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Service Amount
                                table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight(); // Discount
                                table.Cell().Element(CellStyle).PaddingRight(35).Text(p.Price.ToString("N2")).AlignRight(); // Selling Price

                                if (!isCashier)
                                    table.Cell().Element(CellStyle).PaddingRight(35).Text(p.Product.PurchaseCost.ToString("N2")).AlignRight(); // Purchase Cost
                                
                                table.Cell().Element(CellStyle).PaddingRight(30).Text("-").AlignRight();

                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                                table.Cell().Element(CellStyle).Text("");
                            }
                        }
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(6)).PaddingVertical(4);
                    }
                });

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.ConstantColumn(90);
                        columns.ConstantColumn(90);
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.RelativeColumn();

                        if (!isCashier)
                            columns.RelativeColumn();
                        
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Footer(footer =>
                    {
                        var serviceAmountTotal = invoice.Sum(x => x.JobOrder.ServiceList.Sum(y => y.Rate * y.Hours));
                        var sellingPriceTotal = invoice.Where(x => x.JobOrder.ProductList != null).Sum(x => x.JobOrder.ProductList.Sum(y => y.Price * y.Qty));
                        var purchaseCostTotal = invoice.Where(x => x.JobOrder.ProductList != null).Sum(x => x.JobOrder.ProductList.Sum(y => y.Product.PurchaseCost * y.Qty));
                        var grandTotal = invoice.Sum(x => x.TotalAmount);

                        var totalQuickSalesBankTransfer = quickSales.Where(x => x.PaymentTypeParameter.Name == "BANK TRANSFER").Sum(x => x.TotalAmount);
                        var totalQuickSalesLocCredit = quickSales.Where(x => x.PaymentTypeParameter.Name == "LOC. CREDIT CARD").Sum(x => x.TotalAmount);
                        var totalQuickSalesLocDebit = quickSales.Where(x => x.PaymentTypeParameter.Name == "LOC. DEBIT CARD").Sum(x => x.TotalAmount);
                        var totalQuickSalesCash = quickSales.Where(x => x.PaymentTypeParameter.Name == "CASH").Sum(x => x.TotalAmount);
                        var totalQuickSalesGCash = quickSales.Where(x => x.PaymentTypeParameter.Name.ToUpper() == "GCASH").Sum(x => x.TotalAmount);
                        var totalQuickSalesAll = quickSales.Sum(x => x.TotalAmount);

                        grandTotal = grandTotal + totalQuickSalesAll + discountTotalFooter;

                        footer.Cell().Element(CellStyle).Text("");
                        footer.Cell().Element(CellStyle).Text("");
                        footer.Cell().Element(CellStyle).Text("");
                        footer.Cell().Element(CellStyle).PaddingLeft(35).Text(serviceAmountTotal.ToString("N2"));

                        if (!isCashier)
                            footer.Cell().Element(CellStyle).PaddingLeft(35).Text(discountTotalFooter.ToString("N2"));
                        
                        footer.Cell().Element(CellStyle).PaddingLeft(35).Text(sellingPriceTotal.ToString("N2"));
                        footer.Cell().Element(CellStyle).PaddingLeft(35).Text(purchaseCostTotal.ToString("N2"));
                        footer.Cell().Element(CellStyle).PaddingLeft(35).Text(grandTotal.ToString("N2"));
                        footer.Cell().Element(CellStyle).Text("");

                        var paymentDetails = invoice.SelectMany(x => x.PaymentDetailsList).Distinct().ToList();
                        var paymentTypes = paymentDetails.Select(x => x.PaymentTypeParameter.Name).Distinct().ToList();

                        var totalBalance = paymentDetails.Sum(x => x.Payment.Balance);

                        var quickSalesPaymentTypes = quickSales.Select(x => x.PaymentTypeParameter.Name).Distinct().ToList();
                        foreach (var q in quickSalesPaymentTypes)
                        {
                            if (!paymentTypes.Contains(q))
                            {
                                paymentTypes.Add(q);
                            }
                        }

                        footer.Cell().Element(CellStyle).AlignRight().Column(c =>
                        {
                            c.Item().AlignRight().Text("TOTAL AMOUNT:");

                            foreach (var pt in paymentTypes)
                            {
                                if (pt!.ToUpper() != "CASH")
                                { 
                                    c.Item().AlignRight().PaddingTop(3).Text($"{pt}").FontSize(6);
                                }
                                    
                            }
                            
                            c.Item().AlignRight().PaddingTop(3).Text("EXPENSES (ES):").FontSize(6);
                            c.Item().AlignRight().PaddingTop(3).Text("DISCOUNT:").FontSize(6);
                            
                            c.Item().AlignRight().PaddingTop(3).Text("Deposit (All Types):").FontSize(6);

                            c.Item().AlignRight().PaddingTop(10).Text("CASH ONHAND:");
                            c.Item().AlignRight().PaddingTop(3).Text("Overall Diff:").FontSize(6).FontColor(Colors.Red.Darken4);
                        });

                        footer.Cell().Element(CellStyle).PaddingRight(10).AlignRight().Column(c =>
                        {
                            // Total Amount
                            var totalAmountValue = invoice.Sum(x => x.TotalAmount);
                            totalAmountValue = totalAmountValue + quickSales.Sum(x => x.TotalAmount) + discountTotalFooter;

                            c.Item().AlignRight().Text(totalAmountValue.ToString("N2"));
                            var cashOnHand = totalAmountValue - discountTotalFooter;



                            foreach (var pt in paymentTypes)
                            {
                                if (pt!.ToUpper() != "CASH")
                                {
                                    var invoicePerPT = paymentDetails.Where(x => x.PaymentTypeParameter.Name == pt).ToList();
                                    var sumPT = invoicePerPT.Sum(x => x.AmountPaid);

                                    if (pt!.ToUpper() == "BANK TRANSFER")
                                        sumPT = sumPT + totalQuickSalesBankTransfer;
                                    if (pt!.ToUpper() == "LOC. DEBIT CARD")
                                        sumPT = sumPT + totalQuickSalesLocDebit;
                                    if (pt!.ToUpper() == "LOC. CREDIT CARD")
                                        sumPT = sumPT + totalQuickSalesLocCredit;
                                    if (pt!.ToUpper() == "GCASH")
                                        sumPT = sumPT + totalQuickSalesGCash;

                                    cashOnHand = cashOnHand - sumPT;

                                    c.Item().AlignRight().PaddingTop(3).Text($"-{sumPT.ToString("N2")}").FontSize(6);
                                }

                            }

                            // Expenses
                            var totalExpense = expenses.Sum(x => x.Amount);
                            c.Item().AlignRight().PaddingTop(3).Text($"-{totalExpense.ToString("N2")}").FontSize(6);
                            c.Item().AlignRight().PaddingTop(3).Text($"-{discountTotalFooter.ToString("N2")}").FontSize(6);

                            // Deposit?
                            c.Item().AlignRight().PaddingTop(3).Text($"{depositInfo.Sum(x => x.DepositAmount).ToString("N2")}").FontSize(6);

                            cashOnHand = cashOnHand - totalExpense;
                            
                            // Cash onhand
                            c.Item().AlignRight().PaddingTop(10).Text(cashOnHand.ToString("N2")).Bold();
                            c.Item().AlignRight().PaddingTop(3)
                                .Text(totalBalance > 0 ? $"-{totalBalance.ToString("N2").Replace("-", "")}" : $"+{totalBalance.ToString("N2").Replace("-", "")}")
                                .Bold().FontColor(Colors.Red.Darken4);
                        });

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
