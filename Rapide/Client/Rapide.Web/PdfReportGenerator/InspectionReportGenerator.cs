using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rapide.DTO;
using Rapide.Web.Models;

namespace Rapide.Web.PdfReportGenerator
{
    public static class InspectionReportGenerator
    {
        public static byte[] ImageFile { get; set; }
        public static byte[] ImageFileCompany { get; set; }
        private static InspectionDTO data { get; set; }
        private static List<InspectionGroupModel> inspectionGroupData { get; set; }
        private static CompanyInfoDTO companyInfo { get; set; }

        public static async Task Generate(InspectionDTO dto, IJSRuntime JSRuntime, List<InspectionGroupModel> inspectionGroup, CompanyInfoDTO companyInfoDto)
        {
            data = dto;
            inspectionGroupData = inspectionGroup;
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
                column.Item().Text($"BASIC INSPECTION FORM").FontSize(12).Bold().Underline().AlignCenter();
                column.Item().Text($"Transaction Date: {data.CreatedDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")}").FontSize(10).AlignCenter();

                column.Item().PaddingBottom(5).Element(ComposeTableTop);
                column.Item().Element(ComponseTopMessage);

                column.Item().Element(ComposeTable);
                column.Item().Element(ComposeTableBottom);
                column.Item().PaddingBottom(15).PaddingTop(10).Element(ComponseBottomMessage);
                //column.Item().PaddingBottom(25).PaddingTop(25).Element(ComposeSignatories);
                column.Item().PaddingBottom(15).Element(ComposeTech);

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
                    t.Span(data.Odometer.ToString());

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

        private static void ComponseTopMessage(IContainer container)
        {
            container.Background(Colors.Grey.Lighten1).Padding(2).Column(column =>
            {
                column.Spacing(1);
                column.Item().Text($"PITSTOP: [G] Up to standards at this time | [A] May require Attention | [R] Requires immediate attention")
                    .AlignCenter().FontSize(10).Bold();
            });
        }

        private static void ComposeTable(IContainer container)
        {
            container.MultiColumn(multiColumn =>
            {
                multiColumn.Columns(2);
                multiColumn.Spacing(25);

                multiColumn
                .Content()
                .Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Height(400).Column(c =>
                    {
                        // loop here...
                        var group = inspectionGroupData.OrderBy(x => x.Sequence).ToList();
                        foreach (var d in group)
                        {
                            c.Item().PaddingBottom(3).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(7);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text(d.Group.ToUpper()).AlignCenter();
                                    header.Cell().Background(Colors.Green.Medium).Element(CellStyle).Text("[G]").AlignCenter();
                                    header.Cell().Background(Colors.Amber.Medium).Element(CellStyle).Text("[A]").AlignCenter();
                                    header.Cell().Background(Colors.Red.Medium).Element(CellStyle).Text("[R]").AlignCenter();
                                    header.Cell().Element(CellStyle).Text("CONDITION").AlignCenter();
                                });

                                // loop here
                                foreach (var i in d.DetailsModelList)
                                {
                                    table.Cell().Element(CellStyleContent).Text(i.Name);
                                    table.Cell().Element(CellStyleContent).PaddingLeft(3).Text(i.IsGreen ? "\u2714" : "");
                                    table.Cell().Element(CellStyleContent).PaddingLeft(3).Text(i.IsAmber ? "\u2714" : "");
                                    table.Cell().Element(CellStyleContent).PaddingLeft(3).Text(i.IsRed ? "\u2714" : "");
                                    table.Cell().Element(CellStyleContent).Text(i.Remarks);
                                }
                            });
                        }
                    });

                    static IContainer CellStyleContent(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(6))
                            .Border(1)
                            .PaddingTop(2)
                            .PaddingLeft(3)
                            .PaddingBottom(3);
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(6))
                            .Border(1)
                            .PaddingTop(2)
                            .PaddingBottom(3);
                    }
                });
            });
        }

        private static void ComposeTableBottom(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("TECHNICIAN's COMMENTS / REMARKS / OTHER FINDINGS")
                        .AlignCenter();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.FontSize(8))
                            .Border(1)
                            .PaddingBottom(5)
                            .PaddingTop(5)
                            .BorderColor(Colors.Black);
                    }
                });

                table.Cell().Element(CellStyleContent).Text($"Remarks:{Environment.NewLine} {data.Remarks} {Environment.NewLine}" +
                    $"{Environment.NewLine} Customer's Concern:{Environment.NewLine} {data.VehicleFindings}");

                static IContainer CellStyleContent(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(6))
                        .Border(1)
                        .PaddingTop(2)
                        .PaddingLeft(3)
                        .PaddingBottom(3);
                }
            });
        }

        private static void ComponseBottomMessage(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Item().Text($"* Indicate measurements").FontSize(6);
                column.Item().Text($"1. I hereby acknowledged that the findings and recommendations have been discussed with me" +
                    $"and will not hold the establishment liable for recommendations or repairs/replacement of findings not allowed" +
                    $"by me and those uncontrolled hidden defects.").FontSize(6);
                column.Item().Text($"2. The above articles/vehicles are received in good condition & inspection have been made to my satisfaction.").FontSize(6);
                column.Item().Text($"3. It is the customer's responsibility to disclose all concerns of the vehicle prior to availing our services.").FontSize(6);
            });
        }

        private static void ComposeSignatories(IContainer container)
        {
            container.Row(r =>
            {
                r.Spacing(150);
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
                    c.Item().Text("Customer:").Bold().Underline().FontSize(8);
                    c.Item().PaddingTop(30).Text($"{data.Customer.FirstName} {data.Customer.LastName}").Bold().FontSize(8);
                    c.Item().Text("Customer").FontSize(8);
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
