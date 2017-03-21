using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class CreatePatientViewModelTests
    {
        [TestMethod]
        public void QuestionViewModel_ShouldContainQuestionViewModelValidatorAttribute()
        {
            typeof(CreatePatientViewModel).Should().BeDecoratedWith<FluentValidation.Attributes.ValidatorAttribute>(x => x.ValidatorType == typeof(CreatePatientViewModelValidator));
        }
    }
}
