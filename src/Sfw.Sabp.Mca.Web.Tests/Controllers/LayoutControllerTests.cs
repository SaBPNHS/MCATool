using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class LayoutControllerTests
    {
        private LayoutController _layoutController;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();
            _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();

            _layoutController = new LayoutController(_feedBackBuilder, _copyrightViewModelBuilder);
        }

        [TestMethod]
        public void FeedBackLink_ShouldReturnFeedBackPartialView()
        {
            var result = _layoutController.FeedBack() as PartialViewResult;

            result.ViewName.Should().Be(MVC.Shared.Views._FeedBack);
        }

        [TestMethod]
        public void FeedBackLink_FeedBackBuilderShouldBeCalled()
        {
            _layoutController.FeedBack();

            A.CallTo(() => _feedBackBuilder.CreateFeedBackViewModel()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void FeedBackLink_ShouldReturnFeedBackModel()
        {
            var model = new FeedBackViewModel();

            A.CallTo(() => _feedBackBuilder.CreateFeedBackViewModel()).Returns(model);

            var result = _layoutController.FeedBack() as PartialViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void Copyright_ShouldReturnCopyrightPartialView()
        {
            var result = _layoutController.Copyright() as PartialViewResult;

            result.ViewName.Should().Be(MVC.Shared.Views._Copyright);
        }

        [TestMethod]
        public void Copyright_CopyrightBuilderShouldBeCalled()
        {
            _layoutController.Copyright();

            A.CallTo(() => _copyrightViewModelBuilder.CreateCopyrightViewModel()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Copyright_ShouldReturnCopyrightViewModel()
        {
            var model = new CopyrightViewModel();

            A.CallTo(() => _copyrightViewModelBuilder.CreateCopyrightViewModel()).Returns(model);

            var result = _layoutController.Copyright() as PartialViewResult;

            result.Model.Should().Be(model);
        }
    }
}
