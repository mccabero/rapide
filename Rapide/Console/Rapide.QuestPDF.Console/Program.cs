using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                            t.Span("Marxis Cabero");
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

doc.ShowInCompanion();


void ComposeHeader(IContainer container)
{
    var image = QuestPDF.Infrastructure.Image.FromFile("images/default-logo.png");
    var imageCompany = QuestPDF.Infrastructure.Image.FromFile("images/company-logo.png");

    // Report Header
    container.Row(row =>
    {
        row.ConstantItem(70).Height(59).Image(imageCompany);

        row.RelativeItem().Column(column =>
        {
            column.Item().Text($"GOLDER WRENCH CAR CARE OPC").FontSize(12).SemiBold();
            column.Item().Text("9002 PUROK 1, PARULAN, PLARIDEL BULACAN").FontSize(8);
            column.Item().Text("0920-904-3869").FontSize(8);
            column.Item().Text("rapideplaridel@gmail.com").FontSize(8).Underline();
            column.Item().Text("650-772-372-00000").FontSize(8);
        });

        // Change image with "PLARIDEL, BULACAN and underline
        row.ConstantItem(90).Height(60).AlignRight().Image(image);
    });
}

void ComposeContent(IContainer container)
{
    container.ShowEntire().PaddingVertical(10).Column(column =>
    {
        column.Spacing(1);
        column.Item().Text($"COMMISSIONS REPORT").FontSize(10).Bold().Underline().AlignCenter();
        column.Item().Text($"Date Coverage: {DateTime.Now.ToShortDateString()} ~ {DateTime.Now.AddDays(2).ToShortDateString()}").FontSize(6).AlignCenter();

        column.Item().PaddingBottom(5).PaddingTop(10).Element(ComposeTableTop);
        //column.Item().Element(ComposeTableDetails);

    });
}

void ComposeTableTop(IContainer container)
{
    container.Table(table =>
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(40);
            columns.ConstantColumn(90);
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
            header.Cell().Element(CellStyle).PaddingTop(1).Text("DATE");
            header.Cell().Element(CellStyle).PaddingTop(1).Text("VEHICLE");

            // loop technicians here from user list
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"IVAN");
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"CRIS");
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"ANTHONY");
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"DAREIL");
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"MARLON");
            header.Cell().Element(CellStyle).PaddingTop(1).Text($"ELMAR");

            header.Cell().Element(CellStyle).PaddingTop(1).Text($"TOTAL");

            static IContainer CellStyle(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold().FontSize(8))
                    .PaddingVertical(2).BorderBottom(1)
                    .BorderTop(1).PaddingBottom(5)
                    .PaddingTop(5).BorderColor(Colors.Black);
            }
        });

        // Loop invoice here
        table.Cell().Element(CellStyle).Text("03/01/2025");
        table.Cell().Element(CellStyle).Text("TOYOTA VIOS 2016");
        table.Cell().Element(CellStyle).PaddingRight(45).Text("-").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("1,100.00").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("-").AlignRight();

        table.Cell().Element(CellStyle).PaddingRight(35).Text("1,100.00").AlignRight();


        #region Spacer
        table.Cell().Element(CellStyle).Text("");
        table.Cell().Element(CellStyle).Text("");
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        table.Cell().Element(CellStyle).PaddingRight(35).Text("").AlignRight();
        #endregion

        static IContainer CellStyle(IContainer container)
        {
            return container.DefaultTextStyle(x => x.FontSize(6)).PaddingVertical(4);
        }

        table.Footer(footer =>
        {
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");

            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");
            footer.Cell().Element(CellStyle).Text("");

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
}

