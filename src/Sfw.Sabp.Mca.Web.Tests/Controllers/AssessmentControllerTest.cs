using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Contracts;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using System;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class AssessmentControllerTest
    {
        private AssessmentController _controller;
        private IQueryDispatcher _queryDispatcher;
        private IAssessmentViewModelBuilder _assessmentViewModelBuilder;
        private ICommandDispatcher _commandDispatcher;
        private IDateTimeProvider _dateTimeProvider;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _assessmentViewModelBuilder = A.Fake<IAssessmentViewModelBuilder>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();

            _controller = new AssessmentController(_queryDispatcher, _assessmentViewModelBuilder, _commandDispatcher, _dateTimeProvider);
        }

        [TestMethod] 
        public void CreateGet_Called_ShouldReturnDefaultView()
        {
            var result = CreateGetViewResult();

            result.ViewName.ShouldAllBeEquivalentTo(string.Empty);
        }

        [TestMethod]
        public void CreateGet_Called_ShouldReturnValidModel()
        {
            var result = CreateGetViewResult();

            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void CreatePost_Called_ShouldReturnEmptyModel()
        {
            var result = PostValidAssessmentModel();

            result.Should().NotBeNull();
        }

        [TestMethod]
        public void CreatePost_CalledWithInvalidModel_ShouldNotCallViewModelBuilder()
        {
            _controller.ModelState.AddModelError("error", "error");
            var result = _controller.Create(new AssessmentViewModel()) as ViewResult;
            A.CallTo(() => _assessmentViewModelBuilder.BuildAssessmentViewModel(A<Assessment>._)).MustNotHaveHappened();
        }

        [TestMethod]
        public void CreatePost_CalledWithInvalidModel_ShouldNotCallCommandDispatcher()
        {
            _controller.ModelState.AddModelError("error", "error");
            var result = _controller.Create(new AssessmentViewModel()) as ViewResult;
            A.CallTo(() => _commandDispatcher.Dispatch(A<ICommand>._)).MustNotHaveHappened();
        }
        
        [TestMethod]
        public void CreatePost_CalledWithValidModelAndValidParameters_ShouldCallViewModelBuilder()
        {
            var model = PostValidAssessmentModel();
            var result = _controller.Create(model) as ViewResult;
            A.CallTo(() => _assessmentViewModelBuilder.BuildAddAssessmentCommand(A<AssessmentViewModel>._)).MustHaveHappened();
        }

        [TestMethod]
        public void CreatePost_CalledWithAllValidParameters_ShouldCallViewModelBuilder()
        {
            var model = PostValidAssessmentModel();
            var result = _controller.Create(model) as ViewResult;
            A.CallTo(() => _assessmentViewModelBuilder.BuildAddAssessmentCommand(A<AssessmentViewModel>._)).MustHaveHappened();
        }

        [TestMethod]
        public void Index_NoParameterProvided_ShouldReturnIndexView()
        {
            var result = _controller.Index((Guid?) null) as RedirectToRouteResult;
            result.ShouldBeEquivalentTo(null);
        }

        [TestMethod]
        public void Index_WithValidAssessmentId_ShouldReturnAssessmentIndexView()
        {
            var result = IndexViewGetResult(A<Guid>._);
            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void Index_WithValidAssessmentId_ShouldCallAssessmentViewModelBuilder()
        {
            var guid = Guid.NewGuid();

            var result = IndexViewGetResult(guid);
            A.CallTo(() => _assessmentViewModelBuilder.BuildAssessmentViewModel(A<Assessment>._)).MustHaveHappened();
        }

        #region private

        private ViewResult IndexViewGetResult(Guid? Id)
        {
            var result = _controller.Index(Id) as ViewResult;
            return result;
        }

        private ViewResult CreateGetViewResult()
        {
            var result = _controller.Create() as ViewResult;
            return result;
        }

        private AssessmentViewModel PostValidAssessmentModel()
        {
            var model = new AssessmentViewModel()
            {
                ClinicalSystemId = "First",
                DateAssessmentStarted = new DateTime(2015,4,4),
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionConfirmation = "Confirm about decision",
            };
            return model;
        }

        private AssessmentViewModel AssessmentModelMissingClinicalSystemId()
        {
            var model = new AssessmentViewModel()
            {
                DateAssessmentStarted = new DateTime(2015, 4, 4),
                Stage1DecisionToBeMade = "MCA Decision",
                Stage1DecisionConfirmation = "Confirm about decision",
            };
            return model;
        }
        #endregion
    }
}
