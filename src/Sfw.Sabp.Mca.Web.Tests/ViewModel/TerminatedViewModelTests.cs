using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class TerminatedViewModelTests
    {
        [TestMethod]
        public void TerminatedViewModel_ShouldContainQuestionViewModelValidatorAttribute()
        {
            typeof(TerminatedViewModel).Should().BeDecoratedWith<FluentValidation.Attributes.ValidatorAttribute>(x => x.ValidatorType == typeof(TerminatedViewModelValidator));
        }
    }
}
