using System;
using System.IO;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TextFilesToPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the location of the text files: ");
            string textFilesLocation = Console.ReadLine();

            Console.WriteLine("Enter the name of the output PDF file: ");
            string pdfFileName = Console.ReadLine();

            // Get all text files in the folder
            string[] textFiles = Directory.GetFiles(textFilesLocation, "*.txt");

            // Create a PDF document
            Document pdfDocument = new Document();
            PdfWriter.GetInstance(pdfDocument, new FileStream(Path.Combine(textFilesLocation, pdfFileName), FileMode.Create));
            pdfDocument.Open();

            // Add the contents of each text file to the PDF document
            foreach (string textFile in textFiles)
            {
                string fileName = Path.GetFileName(textFile);
                pdfDocument.Add(new Paragraph(fileName + ":\n"));

                string fileContents = File.ReadAllText(textFile);
                pdfDocument.Add(new Paragraph(fileContents + "\n"));

                pdfDocument.Add(new Paragraph("This is the end of file\n"));
            }

            // Close the PDF document
            pdfDocument.Close();

            Console.WriteLine("The text files have been combined into a single PDF file.");
        }
    }
}
