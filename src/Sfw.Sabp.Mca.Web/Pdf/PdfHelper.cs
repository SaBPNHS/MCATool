using System;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace Sfw.Sabp.Mca.Web.Pdf
{
    public class PdfHelper : IPdfHelper
    {
        private int _startYPoint;
        private int _startXPoint;
        private int _currentYPoint = PdfConstants.FirstPageHeaderHeight;
        private int _yIncrement;
        private int _appendXPoint;
        private readonly double _rightMargin = XUnit.FromCentimeter(18).Point;
        private readonly double _leftMargin = XUnit.FromCentimeter(30).Point;

        public PdfHelper()
        {
            RegularFont = new XFont(PdfConstants.FontName, PdfConstants.RegularFontSize, XFontStyle.Regular);
            BoldFont = new XFont(PdfConstants.FontName, PdfConstants.RegularFontSize, XFontStyle.Bold);
            LargeBoldFont = new XFont(PdfConstants.FontName, PdfConstants.LargeFontSize, XFontStyle.Bold);
            _startXPoint = _appendXPoint = PdfConstants.HeaderXValue;
            _startYPoint = PdfConstants.FirstPageHeaderHeight;
        }

        public XFont RegularFont { get; private set; }
        public XFont LargeBoldFont { get; set; }
        public XFont BoldFont { get; private set; }
        public PdfDocument PdfDocument { get; private set; }
        public XGraphics Graph { get; private set; }
        private PdfPage PdfPage { get; set; }

        public bool AddTitle(string content, bool question=true, bool centerAligned = false)
        {
            if (string.IsNullOrWhiteSpace(content)) return false;

            DrawString(StripHtml(content), question?BoldFont:LargeBoldFont, false, centerAligned);
            return true;
        }

        public PdfDocument CreatePdfDocument()
        {
            PdfDocument = new PdfDocument();
            PdfDocument.Info.Title = "Mental Capacity Act Assessment";
            return PdfDocument;
        }

        public PdfPage AddPage()
        {
            if (PdfDocument == null) throw new ArgumentNullException("pdfDocument");
            var page = PdfDocument.AddPage();
            page.Size = PageSize.A4;
            page.Height = PdfConstants.PageHeight;
            Graph = XGraphics.FromPdfPage(page);
            _startYPoint = _currentYPoint == PdfConstants.FirstPageHeaderHeight ? PdfConstants.FirstPageHeaderHeight : PdfConstants.HeaderYValue;
            _startXPoint = PdfConstants.HeaderXValue;
            PdfPage = page;
            return page;
        }

        public bool AppendText(string content)
        {
            if (string.IsNullOrEmpty(content)) return false;
            DrawString(content, RegularFont, true);

            return true;
        }

        public bool WriteText(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return false;
            DrawString(content, RegularFont);
            return true;
        }

        public bool NewPageRequired(string content, XFont font)
        {
            if (string.IsNullOrEmpty(content)) return false;

            var textHeight = Graph.MeasureString(content, font);

            var numberOfLines = (int)(textHeight.Width % _rightMargin > 0 ? (textHeight.Width / _rightMargin + 1) : (textHeight.Width / _rightMargin));

            var totalTextHeight = Math.Ceiling(numberOfLines*textHeight.Height);

            if (numberOfLines > 0)
            {
                _yIncrement = (int) textHeight.Height + (font.Height * numberOfLines);
            }
            else
            {
                _yIncrement = (int)textHeight.Height + font.Height;
            }

            return (_currentYPoint >= PdfPage.Height || _currentYPoint + totalTextHeight >= PdfPage.Height);
        }

        public void AddLine(bool single=true)
        {
            _currentYPoint = single?_startYPoint+PdfConstants.LinePadding :(_startYPoint + PdfConstants.LineHeight);
        }

        private void DrawString(string text, XFont font, bool append = false, bool centerAligned = false)
        {
            if (NewPageRequired(text, font))
            {
                AddPage();
                _currentYPoint = _startYPoint;
            }

            var tf = new XTextFormatter(Graph);

            if (centerAligned)
            {
                tf.Alignment=XParagraphAlignment.Center;
            }

            tf.DrawString(text, font, XBrushes.Black, new XRect(append? _appendXPoint+_startXPoint : _startXPoint, _currentYPoint, _rightMargin, _leftMargin));

            _appendXPoint = (int)Graph.MeasureString(text, font).Width;
            _startYPoint += append ? 0 : _yIncrement;
        }

        private string StripHtml(string content)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(content);

            return doc.DocumentNode.InnerText;
        }
    }
}