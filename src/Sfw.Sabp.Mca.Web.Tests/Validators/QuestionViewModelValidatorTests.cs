using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class QuestionViewModelValidatorTests
    {
        [TestMethod]
        public void AssessmentViewModelValidator_ShouldContainValidatorAttribute()
        {
            var attributes = typeof(QuestionViewModel).GetCustomAttributes(typeof(FluentValidation.Attributes.ValidatorAttribute), true);

            var validationAttribute = (FluentValidation.Attributes.ValidatorAttribute)attributes[0];

            validationAttribute.ValidatorType.Name.Should().Be(typeof(QuestionViewModelValidator).Name);
        }

        [TestMethod]
        public void Valid_GivenNoOptionsAreAvailableAndNoOptionSelected_ValidationShouldPass()
        {
            var validator = new QuestionViewModelValidator();

            var model = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
            };

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void Valid_GivenOptionsAreAvailableAndNoOptionSelected_ValidationShouldFail()
        {
            var validator = new QuestionViewModelValidator();

            var model = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                }
            };

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenAnOptionHasBeenSelectedAndTheOptionHasAFurtherInformationQuestionThatHasNotBeenAnswered_ValidationShouldFail()
        {
            var validator = new QuestionViewModelValidator();
            var chosenGuid = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                    {
                        QuestionOptionId = chosenGuid,
                        FurtherQuestion = "question"
                    }
                },
                ChosenOption = chosenGuid,
                DisplayFurtherInformationQuestion = true,
                FurtherInformationAnswer = null
            };

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenAnOptionHasBeenSelectedAndTheOptionHasAFurtherInformationQuestionThatHasBeenAnswered_ValidationShouldPass()
        {
            var validator = new QuestionViewModelValidator();
            var chosenGuid = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                    {
                        QuestionOptionId = chosenGuid,
                        FurtherQuestion = "question"
                    }
                },
                ChosenOption = chosenGuid,
                FurtherInformationAnswer = "answer",
                DisplayFurtherInformationQuestion =false
            };

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void Valid_GivenAnOptionHasBeenSelectedAndTheOptionHasNoAFurtherInformationQuestion_ValidationShouldPass()
        {
            var validator = new QuestionViewModelValidator();
            var chosenGuid = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                    {
                        QuestionOptionId = Guid.NewGuid()
                    }
                },
                ChosenOption = chosenGuid
            };

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void Valid_GivenModelWithNoOptions_ValidationShouldPass()
        {
            var validator = new QuestionViewModelValidator();

            var model = new QuestionViewModel();

            var valid = validator.Validate(model);

            valid.IsValid.Should().BeTrue();
        }
    }
}
