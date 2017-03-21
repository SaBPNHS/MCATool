using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Sfw.Sabp.Mca.Web.Pdf
{
    public interface IPdfHelper
    {
        XFont RegularFont { get;}
        XFont BoldFont { get;}
        PdfDocument PdfDocument { get; }
        bool AddTitle(string content, bool question=true, bool centerAligned=false);
        PdfDocument CreatePdfDocument();
        PdfPage AddPage();
        bool AppendText(string content);
        bool WriteText(string content);
        bool NewPageRequired(string content, XFont font);
        void AddLine(bool single=true);
    }
}