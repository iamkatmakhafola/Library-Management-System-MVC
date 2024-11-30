using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using u21503193_HW03.Models;
using System.Web.Optimization;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;
using System.Drawing.Drawing2D;

namespace u21503193_HW03.Controllers
{
    public class ReportController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        public ActionResult Index()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/Documents/"));
            List<ReportModel> files = new List<ReportModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new ReportModel { FileName = Path.GetFileName(filePath) });
            }
            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Server.MapPath("~/Documents/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }

        public ActionResult DeleteFile(string fileName)
        {
            string path = Server.MapPath("~/Documents/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);


            System.IO.File.Delete(path);

            return RedirectToAction("Index");
        }

        public FileResult DisplayFile(string fileName)
        {
            string path = Server.MapPath("~/Documents/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public JsonResult GetChartData()
        {
            List<object> data = new List<object>();

            // Perform the query to get the top 10 books and their borrow counts
            var popularBooks = db.borrows
                .GroupBy(b => b.bookId)
                .Select(group => new
                {
                    BookId = group.Key,
                    BorrowCount = group.Count()
                })
                .OrderByDescending(result => result.BorrowCount)
                .Take(10)
                .ToList();

            // Extract book titles and borrow counts from the query result
            List<string> bookTitles = popularBooks
                .Select(result => db.books.FirstOrDefault(b => b.bookId == result.BookId)?.name)
                .ToList();

            List<int> timesBorrowed = popularBooks
                .Select(result => result.BorrowCount)
                .ToList();

            data.Add(bookTitles);
            data.Add(timesBorrowed);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult GenerateReport(string filename, string barChartImage, string pieChartImage)
        {
            // Now, you can use the passed chart images to generate your PDF
            var pdfBytes = GeneratePdfReport(barChartImage, pieChartImage);

            // Define the path to your archive directory
            string archivePath = Server.MapPath("~/Documents/");

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fullFileName = $"{filename}_{timestamp}.pdf";

            // Specify the full path to save the PDF in the archive directory
            string fullFilePath = Path.Combine(archivePath, fullFileName);

            // Save the PDF file to the archive directory
            System.IO.File.WriteAllBytes(fullFilePath, pdfBytes);

            return File(pdfBytes, "application/pdf", fullFileName);
        }

        // Sample PDF generation method using PdfSharp
        private byte[] GeneratePdfReport(string barChartImage, string pieChartImage)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Top Borrowed Books Report";

            // Create a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Arial", 18, XFontStyle.Bold);

            // Define your report content
            string reportContent = "Report on Top Borrowed Books\n\n" +
                "Introduction\n" +
                "This report presents an analysis of the most borrowed books within a library. " +
                "The analysis includes the use of both bar and pie charts to provide a comprehensive " +
                "understanding of borrowing patterns and preferences among library patrons. By visually " +
                "representing the data, this report aims to shed light on the popularity of specific book titles " +
                "and assist library management in making data-driven decisions.\n\n" +
                "Data Source\n" +
                "The data used for this analysis is sourced from the library's records, which contain information on book borrowings. " +
                "The dataset includes book IDs and the corresponding number of times each book has been borrowed.\n\n" +
                "Overview\n" +
                "This report includes two types of charts: a bar chart and a pie chart.\n\n" +
                "Analysis and Insights\n" +
                "The insights gained from these charts can assist in optimizing the library's collection and understanding student preferences.\n\n" +
                "Conclusion\n" +
                "In conclusion, the analysis of top borrowed books using both bar and pie charts provides valuable insights into " +
                "the library's borrowing patterns. It aids in identifying which books are in high demand and how each book contributes to " +
                "the overall borrowing statistics. This data-driven approach enhances the library's ability to meet the needs of its patrons " +
                "and make informed decisions regarding its collection management.";

            // Define the XPoint for drawing text
            XPoint point = new XPoint(40, 40);

            // Draw the report content on the PDF
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(40, 40, page.Width - 80, page.Height - 80);
            tf.DrawString(reportContent, titleFont, XBrushes.Black, rect, XStringFormats.TopLeft);

            // Add the bar chart image
            if (!string.IsNullOrEmpty(barChartImage))
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(barChartImage)))
                {
                    XImage image = XImage.FromStream(ms);
                    gfx.DrawImage(image, 40, 400);
                }
            }

            // Add the pie chart image
            if (!string.IsNullOrEmpty(pieChartImage))
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(pieChartImage)))
                {
                    XImage image = XImage.FromStream(ms);
                    gfx.DrawImage(image, 40, 600);
                }
            }

            // Save the PDF content to a byte array
            using (MemoryStream pdfStream = new MemoryStream())
            {
                document.Save(pdfStream, false);
                return pdfStream.ToArray();
            }
        }
    }
}