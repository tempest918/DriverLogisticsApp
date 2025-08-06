using DriverLogisticsApp.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

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
            var summaryTable = new Table(2, true);
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Total Revenue:").SetFont(boldFont))));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{viewModel.TotalRevenue:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Total Expenses:").SetFont(boldFont))));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{viewModel.TotalExpenses:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text("Net Pay:").SetFont(boldFont))));
            summaryTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(new Text($"{viewModel.NetPay:C}").SetFont(boldFont))).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            document.Add(summaryTable.SetMarginTop(20));

            // earnings table
            document.Add(new Paragraph(new Text("Earnings").SetFontSize(16).SetFont(boldFont)).SetMarginTop(20));
            var earningsTable = new Table(2, true);
            earningsTable.AddHeaderCell("Load Number");
            earningsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Rate")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            foreach (var load in viewModel.CompletedLoads)
            {
                earningsTable.AddCell(load.LoadNumber);
                earningsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{load.FreightRate:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            }
            document.Add(earningsTable);

            // deductions table
            document.Add(new Paragraph(new Text("Deductions").SetFontSize(16).SetFont(boldFont)).SetMarginTop(20));
            var deductionsTable = new Table(2, true);
            deductionsTable.AddHeaderCell("Category");
            deductionsTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Amount")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            foreach (var group in viewModel.GroupedExpenses)
            {
                deductionsTable.AddCell(new iText.Layout.Element.Cell(1, 2).Add(new Paragraph(new Text(group.LoadNumber).SetFont(boldFont))));
                foreach (var expense in group)
                {
                    deductionsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(expense.Category)).SetPaddingLeft(15));
                    deductionsTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{expense.Amount:C}")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                }
            }
            document.Add(deductionsTable);

            document.Close();
            return filePath;
        }
    }
}
