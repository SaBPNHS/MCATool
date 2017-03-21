using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Web.Pdf;

namespace Sfw.Sabp.Mca.Web.Tests.Pdf
{
    [TestClass]
    public class PdfHelperTests
    {
        private PdfHelper _pdfHelper;

        [TestInitialize]
        public void Setup()
        {
            _pdfHelper = new PdfHelper();
        }

        [TestMethod]
        public void CreatePdfDocument_Called_ShouldReturnValidPdfDocumentType()
        {
            var createdDocument = _pdfHelper.CreatePdfDocument();
            Assert.IsInstanceOfType(createdDocument, typeof(PdfDocument), "Document not created of PdfDocument type");
        }

        [TestMethod]
        public void CreatePdfDocument_Called_ShouldPopulateProperty()
        {
            var createdDocument = _pdfHelper.CreatePdfDocument();
            Assert.AreEqual(_pdfHelper.PdfDocument, createdDocument, "PdfDocument property is not populated");
        }

        [TestMethod]
        public void AddPage_CalledWithNullInput_ShouldThrowException()
        {
            _pdfHelper.Invoking(x => x.AddPage()).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddPage_CalledWithValidPdfDocument_ShouldReturnValidPage()
        {
            _pdfHelper.CreatePdfDocument();
            var page = _pdfHelper.AddPage();
            Assert.IsInstanceOfType(page, typeof(PdfPage), "Page not created of PdfPageType");
        }

        [TestMethod]
        public void AddPage_CalledWithValidPdfDocument_ShouldPopulateGraphicsProperty()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            Assert.IsInstanceOfType(_pdfHelper.Graph, typeof(XGraphics), "Page not created of PdfPageType");
            Assert.IsNotNull(_pdfHelper.Graph);
        }

        [TestMethod]
        public void AddTitle_CalledWithEmptyString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.AddTitle(string.Empty));
        }

        [TestMethod]
        public void AddTitle_CalledWithNullString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.AddTitle(null));
        }

        [TestMethod]
        public void AddTitle_CalledWithWhiteSpaceCharactersOnly_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.AddTitle(" "));
        }

        [TestMethod]
        public void AddTitle_CalledWithContents_ShouldReturnTrue()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            Assert.IsTrue(_pdfHelper.AddTitle("Title"));
        }

        [TestMethod]
        public void AppendText_CalledWithEmptyString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.AppendText(string.Empty));
        }

        [TestMethod]
        public void AppendText_CalledWithNullString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.AppendText(null));
        }

        [TestMethod]
        public void AppendText_CalledWithWhiteSpaceCharactersOnly_ShouldReturnTrue()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            Assert.IsTrue(_pdfHelper.AppendText(" "));
        }

        [TestMethod]
        public void AppendText_CalledWithContents_ShouldReturnTrue()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            Assert.IsTrue(_pdfHelper.AppendText("some content"));
        }

        [TestMethod]
        public void WriteText_CalledWithEmptyString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.WriteText(string.Empty));
        }

        [TestMethod]
        public void WriteText_CalledWithNullString_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.WriteText(null));
        }

        [TestMethod]
        public void WriteText_CalledWithWhiteSpaceCharactersOnly_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.WriteText(" "));
        }

        [TestMethod]
        public void WriteText_CalledWithContents_ShouldReturnTrue()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            Assert.IsTrue(_pdfHelper.WriteText("some content"));
        }

        [TestMethod]
        public void NewPageRequired_CalledWithNullInput_ShouldReturnFalse()
        {
            Assert.IsFalse( _pdfHelper.NewPageRequired(null, _pdfHelper.RegularFont));
        }

        [TestMethod]
        public void NewPageRequired_CalledWithEmptyContent_ShouldReturnFalse()
        {
            Assert.IsFalse(_pdfHelper.NewPageRequired(string.Empty, _pdfHelper.BoldFont));
        }
        
        [TestMethod]
        public void NewPageRequired_CalledWithLessContentToFitInAPage_ShouldReturnFalse()
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();
            var content = "little content";
            var result = _pdfHelper.NewPageRequired(content, _pdfHelper.RegularFont);
            Assert.IsFalse(result);
        }
    }
}
