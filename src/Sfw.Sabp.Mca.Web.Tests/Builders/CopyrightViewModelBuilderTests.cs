using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class CopyrightViewModelBuilderTests
    {
        private CopyrightViewModelBuilder _copyrightViewModelBuilder;
        private IFileVersionInfoProvider _fileVersionInfoProvider;

        [TestInitialize]
        public void Setup()
        {
            _fileVersionInfoProvider = A.Fake<IFileVersionInfoProvider>();

            _copyrightViewModelBuilder = new CopyrightViewModelBuilder(_fileVersionInfoProvider);
        }

        [TestMethod]
        public void CreateCopyrightViewModel_ShouldReturnCopyrightViewModel()
        {
            var result = _copyrightViewModelBuilder.CreateCopyrightViewModel();

            result.Should().BeOfType<CopyrightViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateCopyrightViewModel_GivenCopyrightInfo_CopyrightPropertyShouldBeSet()
        {
            const string copyright = "copyright";
            A.CallTo(() => _fileVersionInfoProvider.GetCopyright(A<Type>._)).Returns(copyright);

            var result = _copyrightViewModelBuilder.CreateCopyrightViewModel();

            result.Copyright.Should().Be(copyright);
        }
    }
}
