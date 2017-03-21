using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private IDisclaimerViewModelBuilder _disclaimerViewModelBuilder;
        private ICommandDispatcher _commandDispatcher;
        private IUnitOfWork _unitOfWork;
        private IQueryDispatcher _queryDispatcher;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _disclaimerViewModelBuilder = A.Fake<IDisclaimerViewModelBuilder>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();

            _controller = new HomeController(_disclaimerViewModelBuilder, _commandDispatcher, _unitOfWork, _queryDispatcher, _userPrincipalProvider, _feedBackBuilder, _copyrightViewModelBuilder);            
        }

        [TestMethod]
        public void IndexGET_ModelShouldBeReturned()
        {
            var result = _controller.Index() as ViewResult;

            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void IndexGET_GivenCurrentUser_DisclaimerQueryShouldBeCalled()
        {            
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user@domain.com");

            _controller.Index();

            A.CallTo(() => _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(A<DisclaimerByUserQuery>.That.Matches(x => x.AssessorDomainName == _userPrincipalProvider.CurrentUserName))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenDisclaimer_DisclaimerViewModelBuilderShouldBeCalled()
        {
            var disclaimer = new Disclaimer();

            A.CallTo(() => _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(A<DisclaimerByUserQuery>._)).Returns(disclaimer);

            _controller.Index();

            A.CallTo(() => _disclaimerViewModelBuilder.BuildDisclaimerViewModel(disclaimer)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenDisclaimer_DisclaimerModelShouldBeReturnedToView()
        {
            var model = new DisclaimerViewModel();

            A.CallTo(() => _disclaimerViewModelBuilder.BuildDisclaimerViewModel(A<Disclaimer>._)).Returns(model);

            var result = _controller.Index() as ViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void TutorialGET_DefaultViewShouldBeReturned()
        {
            var result = _controller.Tutorial() as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void IndexPOST_GivenInvalidModelState_ModelShouldBeReturned()
        {
            var model = new DisclaimerViewModel();

            _controller.ModelState.AddModelError("error", "error");

            var result = _controller.Index(model) as ViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void IndexPOST_GivenInvalidModelState_DefaultViewShouldBeReturned()
        {
            _controller.ModelState.AddModelError("error", "error");

            var result = _controller.Index(A<DisclaimerViewModel>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void IndexPOST_GivenInvalidModelState_DisclaimerShouldNotBeSaved()
        {
            _controller.ModelState.AddModelError("error", "error");

            _controller.Index(A<DisclaimerViewModel>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustNotHaveHappened();
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelState_DisclaimerViewModelBuilderShouldBeCalled()
        {
            var model = new DisclaimerViewModel();

            _controller.Index(model);

            A.CallTo(() => _disclaimerViewModelBuilder.BuildAddDisclaimerCommand(model)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelState_CommandDispatcherShouldBeCalled()
        {
            var command = new AddUpdateDisclaimerCommand();

            A.CallTo(() => _disclaimerViewModelBuilder.BuildAddDisclaimerCommand(A<DisclaimerViewModel>._))
                .Returns(command);

            _controller.Index(A<DisclaimerViewModel>._);

            A.CallTo(() => _commandDispatcher.Dispatch(command)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelState_ChangesShouldBeSaved()
        {
            _controller.Index(A<DisclaimerViewModel>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void HomeController_ShouldInheritFromBaseController()
        {
            typeof (HomeController).BaseType.Name.Should().Be(typeof (LayoutController).Name);
        }
    }
}
