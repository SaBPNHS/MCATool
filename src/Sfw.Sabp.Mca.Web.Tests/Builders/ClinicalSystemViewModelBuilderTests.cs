using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class ClinicalSystemViewModelBuilderTests
    {
        private ClinicalSystemViewModelBuilder _clinicalSystemViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _clinicalSystemViewModelBuilder = new ClinicalSystemViewModelBuilder();
        }

        [TestMethod]
        public void BuildClinicalSystemText_CalledWithEmptyString_ShouldReturnDefaultViewModel()
        {
            var viewModel = _clinicalSystemViewModelBuilder.BuildClinicalSystemText(string.Empty);

            viewModel.Should().BeOfType<ClinicalSystemViewModel>();
        }

        [TestMethod]
        public void BuildClinicalSystemText_CalledWithNull_ShouldReturnDefaultViewModel()
        {
            var viewModel = _clinicalSystemViewModelBuilder.BuildClinicalSystemText(null);

            viewModel.Should().BeOfType<ClinicalSystemViewModel>();
        }

        [TestMethod]
        public void BuildClinicalSystemText_CalledWithWhiteSpaceCharactersOnly_ShouldReturnDefaultViewModel()
        {
            var viewModel = _clinicalSystemViewModelBuilder.BuildClinicalSystemText("  ");

            viewModel.Should().BeOfType<ClinicalSystemViewModel>();
        }

        [TestMethod]
        public void BuildClinicalSystemText_CalledWithValidString_ShouldReturnDefaultViewModel()
        {
            var description = "sometext";
            var viewModel = _clinicalSystemViewModelBuilder.BuildClinicalSystemText(description);

            viewModel.Should().BeOfType<ClinicalSystemViewModel>();
            viewModel.ClinicalSystemIdText.ShouldBeEquivalentTo(description);
        }
    }
}
