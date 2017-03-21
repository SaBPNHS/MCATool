using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Web.Pdf
{
    public interface IPdfCreationProvider
    {
        string CreatePdfForAssessment(Assessment assessment, out PdfDocument pdfDocumentGenerated);
    }
}
