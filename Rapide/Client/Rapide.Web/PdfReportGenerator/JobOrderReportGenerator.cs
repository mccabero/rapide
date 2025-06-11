using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;

namespace Rapide.Web.PdfReportGenerator
{
    public static class JobOrderReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static JobOrderDTO data { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(JobOrderDTO dto, IJSRuntime JSRuntime, CompanyInfoDTO companyInfoDto)
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
                column.Item().Text($"JOB ORDER FORM").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Transaction Date: {data.CreatedDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")}").FontSize(10).AlignCenter();

                column.Item().PaddingBottom(5).Element(ComposeTableTop);

                //column.Item().PaddingBottom(5).Element(ComponseTopMessage);

                column.Item().PaddingTop(10).Text($"MAIN CONCERNS").Bold().Underline().AlignCenter();
                column.Item().Element(ComposeTable);

                column.Item().PaddingBottom(25).Element(ComponseBottomMessage);

                //column.Item().PaddingBottom(25).Element(ComposeSignatories);

                column.Item().PaddingBottom(25).Element(ComposeTech);
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
                    t.Span("Customer: ").Bold();
                    t.Span($"{data.Customer.FirstName} {data.Customer.LastName}");

                    t.Span(Environment.NewLine);
                    t.Span("Company: ").Bold();
                    t.Span(data.Customer.CompanyName);

                    t.Span(Environment.NewLine);
                    t.Span("Address: ").Bold();
                    t.Span(data.Customer.HomeAddress);

                    t.Span(Environment.NewLine);
                    t.Span("Mobile No.: ").Bold(); 
                    t.Span($"0{String.Format("{0:### ### ####}", Convert.ToInt64(data.Customer.MobileNumber))}");

                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Plate No: ").Bold();
                    t.Span(data.Vehicle.PlateNo);

                    t.Span(Environment.NewLine);
                    t.Span("Car Details: ").Bold();
                    t.Span($"{data.Vehicle.VehicleModel.VehicleMake.Name} {data.Vehicle.VehicleModel.Name} {data.Vehicle.YearModel}");

                    t.Span(Environment.NewLine);
                    t.Span("Transmission (M/A): ").Bold();
                    t.Span(data.Vehicle.TransmissionParameter.Name);

                    t.Span(Environment.NewLine);
                    t.Span("VIN: ").Bold();
                    t.Span(data.Vehicle.VIN);

                });

                table.Cell().Element(CellStyle).Text(t =>
                {
                    t.Span("Odometer: ").Bold();
                    t.Span(data.Odometer.ToString("#,#"));

                    t.Span(Environment.NewLine);
                    t.Span("Next PMS: ").Bold();
                    t.Span($"{(data.NextOdometerReminder).ToString("#,#")} day/s");

                    t.Span(Environment.NewLine);
                    t.Span("Inspected By: ").Bold();
                    t.Span($"{data.AdvisorUser.FirstName} {data.AdvisorUser.LastName}");

                    t.Span(Environment.NewLine);
                    t.Span("Reference No.: ").Bold();
                    t.Span(data.ReferenceNo);
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
                    header.Cell().Element(CellStyle).PaddingLeft(5).Text("[S]ervice, [P]arts & Materials Description");
                    header.Cell().Element(CellStyle).AlignRight().Text("");
                    header.Cell().Element(CellStyle).AlignRight().Text("");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                            .PaddingVertical(2).BorderBottom(1)
                            .BorderTop(1).PaddingBottom(5)
                            .PaddingTop(5).BorderColor(Colors.Black);
                    }
                });

                // check if isPackage
                if (data.IsPackage)
                {
                    foreach (var pl in data.PackageList)
                    {
                        // add package info here
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).PaddingLeft(5).Text($"[PACKAGE] {pl.Package.Name}").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text("").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text("").Bold();

                        var packageProductList = data.ProductList == null ? new() : data.ProductList.Where(x => x.PackageId == pl.Package.Id).ToList();
                        var packageServiceList = data.ServiceList == null ? new() : data.ServiceList.Where(x => x.PackageId == pl.Package.Id).ToList();

                        #region Loop Package
                        // loop for product 
                        foreach (var p in packageProductList)
                        {
                            var notInPackageTag = p.IsPackage ? string.Empty : "~ (Additional)";

                            table.Cell().Element(CellStyle).PaddingLeft(5).Text(p.Qty);
                            table.Cell().Element(CellStyle).PaddingLeft(5).Text(p.Product.UnitOfMeasure.Name);
                            table.Cell().Element(CellStyle).PaddingLeft(5).PaddingLeft(25).Text($"[P] {p.Product.Name} {notInPackageTag}");
                            table.Cell().Element(CellStyle).AlignRight().Text("");
                            table.Cell().Element(CellStyle).AlignRight().Text("");
                        }

                        // loop for services
                        foreach (var s in packageServiceList)
                        {
                            var notInPackageTag = s.IsPackage ? string.Empty : "~ (Additional)";

                            table.Cell().Element(CellStyle).Text("");
                            table.Cell().Element(CellStyle).Text("");
                            table.Cell().Element(CellStyle).PaddingLeft(5).PaddingLeft(25).Text($"[S] {s.Service.Name} {notInPackageTag}");
                            table.Cell().Element(CellStyle).AlignRight().Text("");
                            table.Cell().Element(CellStyle).AlignRight().Text("");
                        }
                        #endregion
                    }

                }

                
                var regularProductList = data.ProductList == null ? new() : data.ProductList.Where(x => x.IsPackage == false).ToList();
                var regularServicetList = data.ServiceList == null ? new() : data.ServiceList.Where(x => x.IsPackage == false).ToList();

                #region Loop Regular Items
                // loop for product 
                foreach (var p in regularProductList)
                {
                    var notInPackageTag = p.IsPackage ? string.Empty : "~ (Additional)";

                    table.Cell().Element(CellStyle).PaddingLeft(5).Text(p.Qty);
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text(p.Product.UnitOfMeasure.Name);
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text($"[P] {p.Product.Name} {notInPackageTag}");
                    table.Cell().Element(CellStyle).AlignRight().Text("");
                    table.Cell().Element(CellStyle).AlignRight().Text("");
                }

                // loop for services
                foreach (var s in regularServicetList)
                {
                    var notInPackageTag = s.IsPackage ? string.Empty : "~ (Additional)";

                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).Text("");
                    table.Cell().Element(CellStyle).PaddingLeft(5).Text($"[S] {s.Service.Name} {notInPackageTag}");
                    table.Cell().Element(CellStyle).AlignRight().Text("");
                    table.Cell().Element(CellStyle).AlignRight().Text("");
                }
                #endregion

                table.Footer(footer =>
                {
                    footer.Cell().Element(CellStyle).Text("");
                    footer.Cell().Element(CellStyle).Text("");
                    footer.Cell().Element(CellStyle).Text("");

                    footer.Cell().Element(CellStyle).AlignRight().Column(c =>
                    {
                    });

                    footer.Cell().Element(CellStyle).AlignRight().Column(c =>
                    {
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
                    return container.DefaultTextStyle(x => x.FontSize(6)).Border(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(4);
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
                r.Spacing(100);
                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Prepared by:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.EstimatorUser.FirstName} {data.EstimatorUser.LastName}").Bold().FontSize(8);
                    c.Item().Text(data.EstimatorUser.Role.Name).FontSize(8);
                });

                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Approved By:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.ApproverUser.FirstName} {data.ApproverUser.LastName}").Bold().FontSize(8);
                    c.Item().Text(data.ApproverUser.Role.Name).FontSize(8);
                });

                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Conforme by:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.Customer.FirstName} {data.Customer.LastName}").Bold().FontSize(8);
                    c.Item().Text("Customer").FontSize(8);
                });
            });
        }

        private static void ComposeTechQA(IContainer container)
        {
            container.Row(r =>
            {
                r.Spacing(150);
                
                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Approved By:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.ApproverUser.FirstName} {data.ApproverUser.LastName}").Bold().FontSize(8);
                    c.Item().Text(data.ApproverUser.Role.Name).FontSize(8);
                });
            });
        }

        private static void ComposeTech(IContainer container)
        {
            container.Row(r =>
            {
                r.Spacing(150);
                r.AutoItem().Column(c =>
                {
                    c.Item().Text("Technician(s):").Bold().Underline().FontSize(8);

                    c.Item().Row(r =>
                    {
                        foreach (var t in data.TechnicianList)
                        {
                            r.AutoItem().PaddingRight(30).PaddingTop(30).AlignLeft().Column(cc =>
                            {
                                cc.Item().Text($"{t.TechnicianUser.FirstName} {t.TechnicianUser.LastName}").Bold().FontSize(8);
                                cc.Item().Text(t.TechnicianUser.Role.Name).FontSize(8);
                            });
                        }
                    });

                });
            });
        }
    }
}
