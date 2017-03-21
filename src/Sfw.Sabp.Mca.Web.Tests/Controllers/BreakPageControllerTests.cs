using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Controllers.Base;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class BreakPageControllerTests
    {
        private BreakPageController _controller;
        private IClinicalSystemViewModelBuilder _clinicalSystemViewModelBuilder;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _clinicalSystemViewModelBuilder = A.Fake<IClinicalSystemViewModelBuilder>();
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();
            _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();

            _controller = new BreakPageController(_clinicalSystemViewModelBuilder, _clinicalSystemIdDescriptionProvider, _feedBackBuilder, _copyrightViewModelBuilder);
        }

        [TestMethod]
        public void IndexGET_WhenCalled_DefaultViewShouldBeReturned()
        {
            var result = _controller.Index() as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void BreakPageController_ShouldBeDecoratedWithAgreedToDisclaimerAuthorizeAttributeNinjectAttribute()
        {
            typeof(BreakPageController).Should().BeDecoratedWith<AgreedToDisclaimerAuthorizeAttributeNinject>();
        }

        [TestMethod]
        public void BreakPageController_ShouldInheritFromBaseController()
        {
            typeof(BreakPageController).BaseType.Name.Should().Be(typeof(LayoutController).Name);
        }
    }
}
