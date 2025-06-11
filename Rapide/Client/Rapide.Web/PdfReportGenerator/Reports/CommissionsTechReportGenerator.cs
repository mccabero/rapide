using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using System.Reflection.PortableExecutable;

namespace Rapide.Web.PdfReportGenerator.Reports
{
    public static class CommissionsTechReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static List<InvoiceDTO> invoice { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }
        private static List<UserDTO> technicians { get; set; }

        public static async Task Generate(
            List<InvoiceDTO> invoiceData, 
            IJSRuntime JSRuntime, 
            CompanyInfoDTO companyInfoDto, 
            string preparedBy,
            List<UserDTO> techniciansDto)
        {
            invoice = invoiceData;
            companyInfo = companyInfoDto;
            technicians = techniciansDto;

            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Legal);
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
            var rootFileName = $"COMMISSIONS-TECH-{dateCoverage.ToUpper()}.pdf";
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
                column.Item().Text($"COMMISSIONS TECH REPORT").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Date Coverage: {((DateTime)invoice.Min(x => x.InvoiceDate)!).ToShortDateString()} ~ {((DateTime)invoice.Max(x => x.InvoiceDate)!).ToShortDateString()}").FontSize(8).AlignCenter();

                column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
                //column.Item().Element(ComposeTableDetails);

            });
        }

        private static void ComposeTableTop(IContainer container)
        {
            container.Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.ConstantColumn(100);

                        foreach (var t in technicians)
                        {
                            columns.RelativeColumn();
                        }
                        
                        columns.ConstantColumn(60);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("DATE");
                        header.Cell().Element(CellStyle).PaddingTop(1).Text("VEHICLE");

                        // loop technicians here from user list
                        foreach (var t in technicians)
                        {
                            header.Cell().Element(CellStyle).PaddingTop(1).Text(t.FirstName.ToUpper());
                        }
                        
                        header.Cell().Element(CellStyle).PaddingLeft(20).PaddingTop(1).Text($"TOTAL");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                                .PaddingVertical(2).BorderBottom(1)
                                .BorderTop(1).PaddingBottom(5)
                                .PaddingTop(5).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var i in invoice)
                    {
                        // Loop invoice here
                        table.Cell().Element(CellStyle).Text(i.InvoiceDate.Value.ToShortDateString());
                        table.Cell().Element(CellStyle).Text($"{i.JobOrder.Vehicle.VehicleModel.VehicleMake.Name} {i.JobOrder.Vehicle.VehicleModel.Name} {i.JobOrder.Vehicle.YearModel}");

                        // total amount / numbers of technicians
                        var technicianCount = i.JobOrder.TechnicianList.Count;
                        var invoiceAmountForCommission = i.TotalAmount / technicianCount;

                        foreach (var t in technicians)
                        {
                            var isTechnicianFound = i.JobOrder.TechnicianList.Where(x => x.TechnicianUserId == t.Id);

                            var commAmount = isTechnicianFound.Any()
                                ? invoiceAmountForCommission
                                : 0;

                            table.Cell().Element(CellStyle).PaddingRight(35).Text(commAmount > 0 ? commAmount.ToString("N2") : "-").AlignRight();
                        }
                       
                        table.Cell().Element(CellStyle).PaddingRight(10).Text(i.TotalAmount.ToString("N2")).AlignRight();

                    }

                    #region Spacer
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");

                    foreach (var t in technicians)
                    {
                        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
                    }
                    
                    table.Cell().Element(CellStyle).PaddingRight(10).Text("").AlignRight();
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
                        columns.ConstantColumn(40);
                        columns.ConstantColumn(90);

                        foreach (var t in technicians)
                        {
                            columns.RelativeColumn();
                        }

                        columns.ConstantColumn(60);
                    });

                    table.Footer(footer =>
                    {
                        var grandTotal = invoice.Sum(x => x.TotalAmount);

                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");

                        foreach (var t in technicians)
                        {
                            decimal totalAmount = 0;
                            foreach (var i in invoice)
                            {
                                var technicianCount = i.JobOrder.TechnicianList.Count;
                                var invoiceAmountForCommission = i.TotalAmount / technicianCount;

                                if (i.JobOrder.TechnicianList.Where(x => x.TechnicianUserId == t.Id).Any())
                                    totalAmount += invoiceAmountForCommission;
                            }

                            table.Cell().Element(CellStyle).PaddingRight(25).Text(totalAmount > 0 ? totalAmount.ToString("N2") : "-").AlignRight();
                        }

                        table.Cell().Element(CellStyle).PaddingRight(10).Text(grandTotal.ToString("N2")).AlignRight();

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
