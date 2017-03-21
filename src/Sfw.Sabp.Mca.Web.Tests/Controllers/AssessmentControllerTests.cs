using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Workflow;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.Pdf;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class AssessmentControllerTests
    {
        private AssessmentController _assessmentController;
        private IAssessmentViewModelBuilder _assessmentBuilder;
        private IWorkflowHandler _workflowHandler;
        private IPdfCreationProvider _pdfCreationProvider;
        private IQuestionAnswerViewModelBuilder _questionAnswerViewModelBuilder;
        private IAssessmentHelper _assessmentHelper;
        private ITerminatedViewModelBuilder _terminatedViewModelBuilder;
        private IRoleHelper _roleHelper;
        private IPatientHelper _patientHelper;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _assessmentBuilder = A.Fake<IAssessmentViewModelBuilder>();
            _workflowHandler = A.Fake<IWorkflowHandler>();
            _pdfCreationProvider = A.Fake<IPdfCreationProvider>();
            _questionAnswerViewModelBuilder = A.Fake<IQuestionAnswerViewModelBuilder>();            
            _assessmentHelper = A.Fake<IAssessmentHelper>();
            _terminatedViewModelBuilder = A.Fake<ITerminatedViewModelBuilder>();
            _roleHelper = A.Fake<IRoleHelper>();
            _patientHelper = A.Fake<IPatientHelper>();
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();
            _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();

            _assessmentController = new AssessmentController(_assessmentBuilder, _workflowHandler, _pdfCreationProvider,
                _assessmentHelper, _terminatedViewModelBuilder, _patientHelper, _roleHelper, _feedBackBuilder, _copyrightViewModelBuilder);
        }

        [TestMethod]
        public void IndexGET_DefaultViewShouldBeReturned()
        {
            var result = _assessmentController.Index(Guid.NewGuid()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void IndexGET_GivenPatientId_AssessmentsByClinicalIdShouldBeCalled()
        {
            var patientId = Guid.NewGuid();

            _assessmentController.Index(patientId);

            A.CallTo(() => _assessmentHelper.GetAssessmentsByPatient(patientId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenPatientId_AssessmentsBuilderShouldBeCalled()
        {
            var patientId = Guid.NewGuid();

            var assessments = new Assessments();

            A.CallTo(() => _assessmentHelper.GetAssessmentsByPatient(A<Guid>._)).Returns(assessments);

            _assessmentController.Index(patientId);

            A.CallTo(() => _assessmentBuilder.BuildAssessmentListViewModel(patientId, assessments)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenPatientId_AssessmentListViewModelShouldBeReturned()
        {
            var model = new AssessmentListViewModel();

            A.CallTo(() => _assessmentBuilder.BuildAssessmentListViewModel(A.Dummy<Guid>(),A<Assessments>._)).Returns(model);

            var result = _assessmentController.Index(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeOfType<AssessmentListViewModel>();
            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void CreatePdfGet_GivenAssessmentId_AssessmentQueryShouldBeCalled()
        {
            var id = Guid.NewGuid();

            _assessmentController.CreatePdf(id, A<string>._);

            A.CallTo(() => _assessmentHelper.GetAssessment(id)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfGet_GivenAssessmentId_PdfCreationProviderShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();

            var assessment = new Assessment();

            PdfDocument pdfDocumentGenerated;

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);

            var model = new QuestionAnswerListViewModel();

            A.CallTo(() => _questionAnswerViewModelBuilder.BuildQuestionAnswerListViewModel(A<QuestionAnswers>._)).Returns(model);

            _assessmentController.CreatePdf(assessmentId, A<string>._);

            A.CallTo(() => _pdfCreationProvider.CreatePdfForAssessment(assessment, out pdfDocumentGenerated)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfGet_GivenAssessmentId_ValidPdfShouldBeCreated()
        {
            var assessmentId = Guid.NewGuid();

            var assessment = new Assessment() { AssessmentId = assessmentId };

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);

            var pdfDocumentGenerated = _assessmentController.CreatePdf(assessmentId, A<string>._);
            
            Assert.IsInstanceOfType(pdfDocumentGenerated, typeof(FileContentResult));
            var file = (FileContentResult) pdfDocumentGenerated;
            Assert.AreEqual("application/pdf", file.ContentType);
        }

        [TestMethod]
        public void CreatePOST_GivenValidModel_AssessmentCommandShouldBeBuilt()
        {
            var model = new AssessmentViewModel();

            _assessmentController.Create(model);

            A.CallTo(() => _assessmentBuilder.BuildAddAssessmentCommand(model)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePOST_GivenInvalidModel_ShouldReturnDefaultView()
        {
            _assessmentController.ModelState.AddModelError("error", "error");
            var model = new AssessmentViewModel();

            var result = _assessmentController.Create(model) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void CreatePOST_GivenValidModel_WorkflowHandlerShouldBeCalled()
        {
            var model = new AssessmentViewModel();
            var assessmentCommand = new AddAssessmentCommand();

            A.CallTo(() => _assessmentBuilder.BuildAddAssessmentCommand(model)).Returns(assessmentCommand);

            _assessmentController.Create(model);

            A.CallTo(() => _workflowHandler.SetAssessmentWorkflow(assessmentCommand)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePOST_GivenValidModel_ShouldBeRedirectedToIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var viewModel = new AssessmentViewModel();

            A.CallTo(() => _assessmentBuilder.BuildAddAssessmentCommand(viewModel)).Returns(new AddAssessmentCommand() {AssessmentId = assessmentId});

            var result = _assessmentController.Create(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(assessmentId);
        }

        [TestMethod]
        public void CreateGET_DefaultViewShouldBeReturned()
        {
            var result = _assessmentController.Create(Guid.NewGuid()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateGET_GivenPatientId_PatientShouldBeRetrieved()
        {
            var id = Guid.NewGuid();

            _assessmentController.Create(id);

            A.CallTo(() => _patientHelper.GetPatient(id)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreateGET_GivenPatientId_AssessmentBuilderShouldBeCalled()
        {
            var assessment = new Assessment();
            var patient = new Patient();
            var id = Guid.NewGuid();
            var roles = new Roles();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _patientHelper.GetPatient(A<Guid>._)).Returns(patient);
            A.CallTo(() => _roleHelper.GetRoles()).Returns(roles);

            _assessmentController.Create(id);

            A.CallTo(() => _assessmentBuilder.BuildAssessmentViewModel(patient, roles)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartGET_GivenAssessmentId_RestartDefaultViewShouldBeReturned()
        {
            var result = _assessmentController.Restart(A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void RestartGET_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.Restart(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartGET_GivenAssessmentId_AssessmentBuilderShouldBeCalled()
        {
            var assessment = new Assessment();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentController.Restart(A<Guid>._);

            A.CallTo(() => _assessmentBuilder.BuildAssessmentViewModel(assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartGET_GivenAssessmentId_AssessmentViewModelShouldBeReturnedToView()
        {
            var viewModel = new AssessmentViewModel();

            A.CallTo(() => _assessmentBuilder.BuildAssessmentViewModel(A<Assessment>._)).Returns(viewModel);

            var result = _assessmentController.Restart(A<Guid>._) as ViewResult;

            result.Model.Should().Be(viewModel);
        }

        [TestMethod]
        public void RestartNoPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartNo", new[] { typeof(AssessmentViewModel), typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void RestartNoPOST_GivenAssessmentId_ShouldBeRedirectedToTerminatedView()
        {
            var assessmentId = Guid.NewGuid();

            var model = new AssessmentViewModel() {AssessmentId = assessmentId};

            var result = _assessmentController.RestartNo(model, assessmentId) as ViewResult;
            result.ViewName.Should().Be(MVC.Assessment.Views.Terminated);
        }

        [TestMethod]
        public void AddCompletionDetailsPost_GivenAssessmentId_AssessmentMustBeMarkedComplete()
        {
            var assessmentId = Guid.NewGuid();

            var model = new TerminatedViewModel { AssessmentId = assessmentId };

            _assessmentController.AddCompletionDetails(model);

            A.CallTo(() => _workflowHandler.CompleteAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AddCompletionDetailsPost_GivenAssessmentViewModel_ShouldRedirectToCompleteView()
        {
            var assessmentId = Guid.NewGuid();

            var model = new TerminatedViewModel { AssessmentId = assessmentId };

            var result = _assessmentController.AddCompletionDetails(model) as ViewResult;
            result.ViewName.Should().Be(MVC.Assessment.Views.Complete);
        }

        [TestMethod]
        public void RestartYesGET_GivenAssessmentViewModel_AssessmentRestartViewShouldBeReturned()
        {
            var result = _assessmentController.RestartAssessment(A.Dummy<AssessmentViewModel>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be(MVC.Assessment.Views.Restart);
        }

        [TestMethod]
        public void RestartYesGET_GivenAssessmentViewModel_AssessmentViewModelShouldBeReturned()
        {
            var model = new AssessmentViewModel();

            var result = _assessmentController.RestartAssessment(model, A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void RestartYesGET_GivenAssessmentViewModel_YesButtonClickedPropertyShouldBeTrue()
        {
            var result = _assessmentController.RestartAssessment(A.Dummy<AssessmentViewModel>(), A.Dummy<Guid>()) as ViewResult;

            var assessmentViewModel = result.Model as AssessmentViewModel;
            assessmentViewModel.YesClicked.Should().BeTrue();
        }

        [TestMethod]
        public void RestartYesPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartAssessment", new[] { typeof(AssessmentViewModel), typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void RestartStagePOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartStage", new[] { typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void RestartStagePOST_GivenAssessmentId_AssessmentStageShouldBeReset()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.RestartStage(assessmentId);

            A.CallTo(() => _workflowHandler.ResetAssessmentStage(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartStagePOST_GivenAssessmentId_ShouldBeRedirectedToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var result = _assessmentController.RestartStage(assessmentId) as RedirectToRouteResult;

            AssertQuestionRouteValues(result, assessmentId);
        }

       

        [TestMethod]
        public void RestartStartAssessmentPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartStartAssessment", new[] { typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void RestartStartAssessmentPOST_GivenAssessmentId_AssessmentShouldBeReset()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.RestartStartAssessment(assessmentId);

            A.CallTo(() => _workflowHandler.ResetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartStartAssessmentPOST_GivenAssessmentId_ShouldBeRedirectedToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var result = _assessmentController.RestartStartAssessment(assessmentId) as RedirectToRouteResult;

            AssertQuestionRouteValues(result, assessmentId);
        }

        [TestMethod]
        public void RestartFromBreakPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartFromBreak", new[] { typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void CreatePdf_ShouldBeDecoratedWithAuditAttribute()
        {
            typeof(AssessmentController).GetMethod("CreatePdf", new[] { typeof(Guid), typeof(string) }).Should().BeDecoratedWith<AuditAttribute>(x => x.AuditProperties == "id,clinicalSystemId");
        }

        [TestMethod]
        public void CompleteGET_GivenAssessmentId_TerminatedViewShouldBeReturned()
        {
            var assessmentId = Guid.NewGuid();
            var result = _assessmentController.Complete(assessmentId) as ViewResult;

            result.ViewName.Should().Be(MVC.Assessment.Views.Terminated);
        }

        [TestMethod]
        public void CompletePOST_GivenAssessmentId_CompleteDefaultViewShouldBeReturned()
        {
            var result = _assessmentController.AddCompletionDetails(A.Dummy<TerminatedViewModel>()) as ViewResult;

            result.ViewName.Should().Be(MVC.Assessment.Views.Complete);
        }

        [TestMethod]
        public void CompleteGET_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.Complete(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CompleteGET_GivenAssessmentId_TerminatedViewBuilderShouldBeCalled()
        {
            var assessment = new Assessment();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentController.Complete(A<Guid>._);

            A.CallTo(() => _terminatedViewModelBuilder.BuildTerminatedAssessmentViewModel(assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CompleteGET_GivenAssessmentId_AssessmentViewModelShouldBeReturnedToView()
        {
            var viewModel = new TerminatedViewModel();

            A.CallTo(() => _terminatedViewModelBuilder.BuildTerminatedAssessmentViewModel(A<Assessment>._)).Returns(viewModel);

            var result = _assessmentController.Complete(A<Guid>._) as ViewResult;

            result.Model.Should().Be(viewModel);
        }

        [TestMethod]
        public void RestartFromBreak_GivenAssessmentId_WorkflowResetBreakShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.RestartFromBreak(assessmentId);

            A.CallTo(() => _workflowHandler.RestartBreak(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);

        }

        [TestMethod]
        public void RestartStartAssessment_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof (AssessmentController).GetMethod("RestartStartAssessment", new[] {typeof (Guid)})
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void RestartFromBreak_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartFromBreak", new[] { typeof(Guid) })
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void RestartStage_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartStage", new[] { typeof(Guid) })
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void RestartNo_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartNo", new[] { typeof(AssessmentViewModel), typeof(Guid) })
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void RestartAssessment_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(AssessmentController).GetMethod("RestartAssessment", new[] { typeof(AssessmentViewModel), typeof(Guid) })
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void Restart_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(AssessmentController).GetMethod("Restart", new[] { typeof(Guid) })
                .Should()
                .BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "id");
        }

        [TestMethod]
        public void AssessmentController_ShouldBeDecoradedWithOutputCacheAttribute()
        {
            typeof(AssessmentController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.NoStore);
            typeof(AssessmentController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.Duration == 0);
            typeof(AssessmentController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.VaryByParam == "None");
        }

        [TestMethod]
        public void AssessmentController_ShouldBeDecoratedWithAgreedToDisclaimerAuthorizeAttributeNinjectAttribute()
        {
            typeof(AssessmentController).Should().BeDecoratedWith<AgreedToDisclaimerAuthorizeAttributeNinject>();
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_DefaultViewShouldBeReturned()
        {
            var result = _assessmentController.Edit(A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_AssessmentViewModelShouldBeReturned()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(new Assessment());

            var result = _assessmentController.Edit(A<Guid>._) as ViewResult;

            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentController.Edit(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_RolesShouldBeRetrieved()
        {
            _assessmentController.Edit(A<Guid>._);

            A.CallTo(() => _roleHelper.GetRoles()).MustHaveHappened(Repeated.Exactly.Once);
        }


        [TestMethod]
        public void EditGET_GivenAssessmentId_AssessmentBuilderShouldBeCalled()
        {
            var assessment = new Assessment();
            var roles = new Roles();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _roleHelper.GetRoles()).Returns(roles);

            _assessmentController.Edit(A<Guid>._);

            A.CallTo(() => _assessmentBuilder.BuildAssessmentViewModel(assessment, roles)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenAssessmentIsNull_HttpExceptionExpected()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(null);

            _assessmentController.Invoking(x => x.Edit(A<Guid>._))
                .ShouldThrow<HttpException>()
                .And.GetHttpCode()
                .Should()
                .Be((int) HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void EditPOST_GivenModelStateIsInvalid_DefaultViewShouldBeReturned()
        {
            _assessmentController.ModelState.AddModelError("error", "error");
            var result = _assessmentController.Edit(A<AssessmentViewModel>._, A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditPOST_GivenModelStateIsInvalid_ModelShouldBeReturned()
        {
            var model = new AssessmentViewModel();

            _assessmentController.ModelState.AddModelError("error", "error");
            var result = _assessmentController.Edit(model, A<Guid>._) as ViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void EditPOST_GivenAssessmentViewModelAndModelStateIsValid_AssessmentModelBuilderShouldBeCalled()
        {
            var model = new AssessmentViewModel();

            _assessmentController.Edit(model, A<Guid>._);

            A.CallTo(() => _assessmentBuilder.BuildUpdateAssessmentCommand(model)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditPOST_GivenAssessmentViewModelAndModelStateIsValid_WorkflowHandlerShouldNotBeCalled()
        {
            var command = new UpdateAssessmentCommand();

            A.CallTo(() => _assessmentBuilder.BuildUpdateAssessmentCommand(A<AssessmentViewModel>._)).Returns(command);

            _assessmentController.ModelState.AddModelError("error", "error");
            _assessmentController.Edit(A<AssessmentViewModel>._, A<Guid>._);

            A.CallTo(() => _workflowHandler.UpdateAssessmentWorkflowQuestion(command)).MustNotHaveHappened();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(AssessmentController).GetMethod("Edit", new[] { typeof(AssessmentViewModel), typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithAssessmentCompleteAttribute()
        {
            typeof(AssessmentController).GetMethod("Edit", new[] { typeof(AssessmentViewModel), typeof(Guid) }).Should().BeDecoratedWith<AssessmentCompleteAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void EditGET_ShouldBeDecoratedWithAssessmentCompleteAttribute()
        {
            typeof(AssessmentController).GetMethod("Edit", new[] { typeof(Guid) }).Should().BeDecoratedWith<AssessmentCompleteAttribute>(x => x.ActionParameterId == "id");
        }

        [TestMethod]
        public void EditPOST_GivenAssessmentViewModelAndModelStateIsValid_ShouldBeRedirectedToQuestionIndexAction()
        {
            var model = new AssessmentViewModel()
            {
                AssessmentId = Guid.NewGuid()
            };

            var result = _assessmentController.Edit(model, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Edit);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(model.AssessmentId);
        }

        [TestMethod]
        public void CreatePOST_CalledWithAdvisorRoleIdSelectedWithoutDecisionMaker_ShouldDisplayErrorMessage()
        {
            var model = new AssessmentViewModel()
            {
                AssessmentId = Guid.NewGuid()
            };
            model.RoleId = (int)RoleIdEnum.Assessor;
            model.DecisionMaker = string.Empty;
            var result = _assessmentController.Create(model.AssessmentId) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void CreatePOST_CalledWithDecisionMakerRoleSelected_ShouldRedirectToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var viewModel = new AssessmentViewModel();

            A.CallTo(() => _assessmentBuilder.BuildAddAssessmentCommand(viewModel)).Returns(new AddAssessmentCommand() {AssessmentId = assessmentId, RoleId = 2});

            var result = _assessmentController.Create(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(assessmentId);
        }
       
        [TestMethod]
        public void CreatePOST_CalledWithAdvisorRoleSelectedAndDecisionMakerNonEmpty_ShouldRedirectToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var viewModel = new AssessmentViewModel();

            A.CallTo(() => _assessmentBuilder.BuildAddAssessmentCommand(viewModel)).Returns(new AddAssessmentCommand() { AssessmentId = assessmentId, 
                RoleId = (int)RoleIdEnum.Assessor, DecisionMaker = "Name of decision maker"});

            var result = _assessmentController.Create(viewModel) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(assessmentId);
        }

        [TestMethod]
        public void AssessmentController_ShouldInheritFromBaseController()
        {
            typeof(AssessmentController).BaseType.Name.Should().Be(typeof(LayoutController).Name);
        }

        #region private

        private void AssertQuestionRouteValues(RedirectToRouteResult result, Guid assessmentId)
        {
            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(assessmentId);
        }

        #endregion
    }
}
