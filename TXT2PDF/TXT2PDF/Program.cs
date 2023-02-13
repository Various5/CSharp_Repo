using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TextFilesToPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Convert text files to PDF");
            Console.WriteLine("2. Exit");
            Console.WriteLine("\nEnter your choice: ");
            Console.ResetColor();

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
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
                    break;
                case 2:
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Exiting...");
                    break;
            }
        }
    }
}
