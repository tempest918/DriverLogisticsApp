using DriverLogisticsApp.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace DriverLogisticsApp.Services
{
    public class PdfService
    {
        /// <summary>
        /// creates a basic pdf report for the settlement report view model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public string CreateSettlementReportPdf(SettlementReportViewModel viewModel, string fileName)
        {
            // setup the PDF document
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            var writer = new PdfWriter(filePath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // document title
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var titleText = new Text("Driver Settlement Report").SetFontSize(20).SetFont(boldFont);
            document.Add(new Paragraph(titleText).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

            // date range values
            document.Add(new Paragraph($"For Period: {viewModel.StartDate:d} - {viewModel.EndDate:d}")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

            // summary table
            var summaryTable = new Table(2, true).SetMarginTop(20);
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Total Revenue:").SetFont(boldFont))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{viewModel.TotalRevenue:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Total Expenses:").SetFont(boldFont))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{viewModel.TotalExpenses:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Net Pay:").SetFont(boldFont))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text($"{viewModel.NetPay:C}").SetFont(boldFont))).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            document.Add(summaryTable);

            // earnings table
            document.Add(new Paragraph(new Text("Earnings").SetFontSize(16).SetFont(boldFont)).SetMarginTop(20));
            var earningsTable = new Table(2, true);
            earningsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Load Number")).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
            earningsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Rate")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
            foreach (var load in viewModel.CompletedLoads)
            {
                earningsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{load.LoadNumber} ({load.DeliveryDate:d})")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                earningsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{load.FreightRate:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            }
            document.Add(earningsTable);

            // deductions table
            document.Add(new Paragraph(new Text("Deductions").SetFontSize(16).SetFont(boldFont)).SetMarginTop(20));
            var deductionsTable = new Table(2, true);
            deductionsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Category")).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
            deductionsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Amount")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
            foreach (var group in viewModel.GroupedExpenses)
            {
                var groupCell = new iText.Layout.Element.Cell(1, 2).Add(new Paragraph(new Text(group.LoadNumber).SetFont(boldFont)));
                groupCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderTop(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f));
                deductionsTable.AddCell(groupCell);

                foreach (var expense in group)
                {
                    deductionsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{expense.Category} ({expense.Date:d})")).SetPaddingLeft(15).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    deductionsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{expense.Amount:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }
            }
            document.Add(deductionsTable);

            document.Close();
            return filePath;
        }
    }
}
