using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class ClinicalSystemViewModelTests
    {
        [TestMethod]
        public void ClinicalSystemViewModel_ShouldContainClinicalSystemViewModelValidatorAttribute()
        {
            typeof(ClinicalSystemViewModel).Should().BeDecoratedWith<FluentValidation.Attributes.ValidatorAttribute>(x => x.ValidatorType == typeof(ClinicalSystemViewModelValidator));
        }

        [TestMethod]
        public void ClinicalSystemViewModel_GivenClinicalSystemIdIsNull_NullShouldBeReturned()
        {
            var model = new ClinicalSystemViewModel();

            model.ClinicalSystemIdText.Should().Be(null);
        }

        [TestMethod]
        public void ClinicalSystemViewModel_GivenEmptyString_NullShouldBeReturned()
        {
            var model = new ClinicalSystemViewModel() {ClinicalSystemIdText = string.Empty};

            model.ClinicalSystemIdText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ClinicalSystemViewModel_GivenValidString_ClinicalSystemIdShouldBeReturned()
        {
            var model = new ClinicalSystemViewModel() { ClinicalSystemIdText = "sometext"};

            model.ClinicalSystemIdText.Should().Be("sometext");
        }
    }
}
