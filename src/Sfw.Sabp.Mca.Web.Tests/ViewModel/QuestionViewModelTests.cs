using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class QuestionViewModelTests
    {
        private QuestionViewModel _model;

        [TestInitialize]
        public void Setup()
        {
            _model = new QuestionViewModel();
        }

        [TestMethod]
        public void QuestionViewModel_ShouldContainQuestionViewModelValidatorAttribute()
        {
            typeof(QuestionViewModel).Should().BeDecoratedWith<FluentValidation.Attributes.ValidatorAttribute>(x => x.ValidatorType == typeof(QuestionViewModelValidator));
        }

        [TestMethod]
        public void DisplayGuidance_GivenGuidanceIsWhiteSpace_FalseShouldBeReturned()
        {
            _model.Guidance = " ";
            _model.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayGuidance_GivenGuidanceIsEmpty_FalseShouldBeReturned()
        {
            _model.Guidance = "";
            _model.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayGuidance_GivenGuidanceNULL_FalseShouldBeReturned()
        {
            _model.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayGuidance_GivenGuidancePopulated_TrueShouldBeReturned()
        {
            _model.Guidance = "guidance";
            _model.DisplayGuidance.Should().BeTrue();
        }

        [TestMethod]
        public void DisplayOptions_GivenNullOptions_FalseShouldBeReturned()
        {
            _model.DisplayOptions.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayOptions_GivenZeroOptions_FalseShouldBeReturned()
        {
            var options = new List<OptionViewModel>();
            _model.Options = options;
            _model.DisplayOptions.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayOptions_GivenSingleOption_FalseShouldBeReturned()
        {
            var options = new List<OptionViewModel>() {new OptionViewModel()};
            _model.Options = options;
            _model.DisplayOptions.Should().BeFalse();
        }

        [TestMethod]
        public void DisplayOptions_GivenMultipleOptions_TrueShouldBeReturned()
        {
            var options = new List<OptionViewModel>() { new OptionViewModel(), new OptionViewModel() };
            _model.Options = options;
            _model.DisplayOptions.Should().BeTrue();
        }

        [TestMethod]
        public void QuestionViewModel_QuestionProperty_ShouldBeDecoratedWithAllowHtmlAttribute()
        {
            typeof (QuestionViewModel).GetProperty("Question").Should().BeDecoratedWith<AllowHtmlAttribute>();
        }

        [TestMethod]
        public void QuestionViewModel_GuidanceProperty_ShouldBeDecoratedWithAllowHtmlAttribute()
        {
            typeof(QuestionViewModel).GetProperty("Guidance").Should().BeDecoratedWith<AllowHtmlAttribute>();
        }
    }
}
