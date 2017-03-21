using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class ClinicalSystemViewModelValidatorTests
    {
        [TestMethod]
        public void ClinicalSystemViewModelValidator_WhenCalled_ShouldContainValidatorAttribute()
        {
            object[] attributes = typeof(ClinicalSystemViewModel).GetCustomAttributes(typeof(FluentValidation.Attributes.ValidatorAttribute), true);

            var validationAttribute = (FluentValidation.Attributes.ValidatorAttribute)attributes[0];

            validationAttribute.ValidatorType.Name.Should().Be(typeof(ClinicalSystemViewModelValidator).Name);
        }

        [TestMethod]
        public void ClinicalSystemViewModelValidator_ClinicalSystemIdNotProvided_ShouldReturnError()
        {
            var model = new ClinicalSystemViewModel();

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void ClinicalSystemViewModelValidator_ClinicalSystemIdProvided_ShouldReturnError()
        {
            var model = new ClinicalSystemViewModel() {ClinicalSystemIdText = "some reason" };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        #region private
        private ValidationResult ValidationResult(ClinicalSystemViewModel model)
        {
            var validator = new ClinicalSystemViewModelValidator();
            var result = validator.Validate(model);
            return result;
        }
        #endregion
    }
}
