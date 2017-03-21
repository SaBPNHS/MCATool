using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class TerminatedViewModelValidatorTests
    {
        [TestMethod]
        public void TerminatedViewModelValidator_WhenCalled_ShouldContainValidatorAttribute()
        {
            object[] attributes = typeof(TerminatedViewModel).GetCustomAttributes(typeof(FluentValidation.Attributes.ValidatorAttribute), true);

            var validationAttribute = (FluentValidation.Attributes.ValidatorAttribute)attributes[0];

            validationAttribute.ValidatorType.Name.Should().Be(typeof(TerminatedViewModelValidator).Name);
        }

        [TestMethod]
        public void TerminatedViewModelValidator_TerminationReasonNotProvided_ShouldReturnError()
        {
            var model = new TerminatedViewModel();

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void TerminatedViewModelValidator_TerminationReasonProvided_ShouldReturnError()
        {
            var model = new TerminatedViewModel(){TerminatedAssessmentReason = "some reason"};

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void TerminatedViewModelValidator_GivenTerminatedReasonHasMoreThan150Characters_ValidationShouldFail()
        {
            var model = new TerminatedViewModel()
            {
                TerminatedAssessmentReason = new string('a', 151)
            };


            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        #region private
        private ValidationResult ValidationResult(TerminatedViewModel model)
        {
            var validator = new TerminatedViewModelValidator();
            var result = validator.Validate(model);
            return result;
        }
        #endregion
    }
}
