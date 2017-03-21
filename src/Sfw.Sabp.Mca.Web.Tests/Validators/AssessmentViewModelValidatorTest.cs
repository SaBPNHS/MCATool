using System;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class AssessmentViewModelValidatorTest
    {
        private IFutureDateValidator _futureDateValidator;

        [TestInitialize]
        public void Setup()
        {
            _futureDateValidator = A.Fake<IFutureDateValidator>();
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime>._)).Returns(true);
        }

        [TestMethod]
        public void AssessmentViewModelValidator_WhenCalled_ShouldContainValidatorAttribute()
        {
            object[] attributes = typeof(AssessmentViewModel).GetCustomAttributes(typeof(FluentValidation.Attributes.ValidatorAttribute), true);

            var validationAttribute = (FluentValidation.Attributes.ValidatorAttribute)attributes[0];

            validationAttribute.ValidatorType.Name.Should().Be(typeof(AssessmentViewModelValidator).Name);
        }

        [TestMethod]
        public void AssessmentViewModelValidator_ClinicalSystemIdNotProvided_ShouldReturnError()
        {
            var model = new AssessmentViewModel();

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_Stage1DecisionNotProvided_ShouldReturnError()
        {
            var model = new AssessmentViewModel()
            {
                DateAssessmentStarted = new DateTime(2015, 4, 4),
                Stage1DecisionConfirmation = "Confirm about decision",
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_DateAssessmentStartedNotProvided_ShouldSucceed()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.DecisionMaker
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_Stage1DecisionConfirmationNotProvided_ShouldReturnError()
        {
            var model = new AssessmentViewModel()
            {
                DateAssessmentStarted = new DateTime(2015, 4, 4),
                Stage1DecisionToBeMade = "decision"
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_Stage1DecisionConfirmationFalse_ShouldReturnError()
        {
            var model = new AssessmentViewModel()
            {
                DateAssessmentStarted = new DateTime(2015, 4, 4),
                Stage1DecisionToBeMade = "decision",
                Stage1DecisionClearlyMade = false
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_AssessmentDateIsInTheFuture_ValidationShouldFail()
        {
            _futureDateValidator = A.Fake<IFutureDateValidator>();
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime>._)).Returns(false);

            var model = new AssessmentViewModel()
            {
                DateAssessmentStarted = DateTime.Now.AddDays(1),
                Stage1DecisionToBeMade = "decision",
                Stage1DecisionClearlyMade = true
            };

            var validator = new AssessmentViewModelValidator(_futureDateValidator);
            var result = validator.Validate(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_AdvisorRoleSelectedEmptyDecisionMaker_ValidationShouldFail()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.Assessor,
                DecisionMaker = string.Empty
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_NoRoleSelected_ValidationShouldFail()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Equals(ApplicationStringConstants.RoleIdMandatory);
        }

        [TestMethod]
        public void AssessmentViewModelValidator_AdvisorRoleSelectedWhitespaceDecisionMaker_ValidationShouldFail()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.Assessor,
                DecisionMaker = "   "
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_AdvisorRoleSelectedValidDecisionMaker_ValidationShouldPass()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.Assessor,
                DecisionMaker = "some text"
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_DecisionMakerRoleSelected_ValidationShouldPass()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.DecisionMaker
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();

        }

        [TestMethod]
        public void AssessmentViewModelValidator_GivenDecisionMakerNameHasMoreThan50Characters_ValidationShouldFail()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.Assessor,
                DecisionMaker = new string('a', 51)
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void AssessmentViewModelValidator_GivenStage1DecisionToBeMadeHasMoreThan1000Characters_ValidationShouldFail()
        {
            var model = new AssessmentViewModel()
            {
                Stage1DecisionToBeMade = new string('a', 1001),
                Stage1DecisionClearlyMade = true,
                RoleId = (int)RoleIdEnum.DecisionMaker
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        #region private
        private ValidationResult ValidationResult(AssessmentViewModel model)
        {
            var validator = new AssessmentViewModelValidator(_futureDateValidator);
            var result = validator.Validate(model);
            return result;
        }
        #endregion
    }
}
