using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Web.Helper;
using Sfw.Sabp.Mca.Infrastructure.Web.Render;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Workflow;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class QuestionControllerTests
    {
        private QuestionController _questionController;
        private IQuestionViewModelBuilder _questionViewModelBuilder;
        private IWorkflowHandler _workflowHandler;
        private IAssessmentHelper _assessmentHelper;
        private IQuestionAnswerHelper _questionAnswerHelper;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;
        private IPartialViewRenderer _partialViewRenderer;
        private IUrlHelper _urlHelper;

        [TestInitialize]
        public void Setup()
        {
            _questionViewModelBuilder = A.Fake<IQuestionViewModelBuilder>();
            _workflowHandler = A.Fake<IWorkflowHandler>();
            _assessmentHelper = A.Fake<IAssessmentHelper>();
            _questionAnswerHelper = A.Fake<IQuestionAnswerHelper>();
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();
            _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();
            _partialViewRenderer = A.Fake<IPartialViewRenderer>();
            _urlHelper = A.Fake<IUrlHelper>();

            _questionController = new QuestionController(_questionViewModelBuilder, _workflowHandler, _assessmentHelper, _questionAnswerHelper, _feedBackBuilder, _copyrightViewModelBuilder, _partialViewRenderer, _urlHelper);

            SetupAjaxRequest(false);
        }

        [TestMethod]
        public void IndexGET_ActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = _questionController.Index(A<Guid>._) as ViewResult;

            result.ViewName.Should().Be(string.Empty);
        }

        [TestMethod]
        public void IndexGET_GivenAssessmentId_AssessmentQueryShouldBeCalled()
        {
            var id = Guid.NewGuid();

            _questionController.Index(id);

            A.CallTo(() => _assessmentHelper.GetAssessment(id)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenAssessmentId_QuestionBuilderShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var questionId = Guid.NewGuid();

            var assessment = new Assessment() {CurrentWorkflowQuestionId = questionId};
            var questionAnswer = new QuestionAnswer();

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);
            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(assessment)).Returns(questionAnswer);

            _questionController.Index(assessmentId);

            A.CallTo(() => _questionViewModelBuilder.BuildQuestionViewModel(assessment, questionAnswer)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexGET_GivenAssessmentId_QuestionViewModelShouldBeReturned()
        {
            var questionViewModel = new QuestionViewModel();
            var assessmentId = Guid.NewGuid();
            var assessment = new Assessment();
            var questionAnswer = new QuestionAnswer();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(questionAnswer);

            A.CallTo(() => _questionViewModelBuilder.BuildQuestionViewModel(A<Assessment>._, A<QuestionAnswer>._)).Returns(questionViewModel);

            var result = _questionController.Index(assessmentId) as ViewResult;

            result.Model.Should().BeOfType<QuestionViewModel>();
            result.Model.ShouldBeEquivalentTo(questionViewModel);
        }

        [TestMethod]
        public void IndexGET_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(QuestionController).GetMethod("Index", new[] { typeof(Guid) }).Should().BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void IndexPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(QuestionController).GetMethod("Index", new[] { typeof(QuestionViewModel), typeof(bool), typeof(Guid?), typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void IndexPOST_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(QuestionController).GetMethod("Index", new[] { typeof(QuestionViewModel), typeof(bool), typeof(Guid?), typeof(Guid) }).Should().BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void IndexPOST_GivenInvalidModel_DefaultIndexViewShouldBeReturnedWithModel()
        {
            var model = new QuestionViewModel();

            _questionController.ModelState.AddModelError("error", "error");
            var result = _questionController.Index(model, A<bool>._, A<Guid?>._, A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndContinueClicked_WorkflowHandlerShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            const string furtherInfo = "info";

            var model = new QuestionViewModel()
            {
                AssessmentId = assessmentId,
                ChosenOption = optionId,
                FurtherInformationAnswer = furtherInfo
            };

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
             .Returns(AssessmentStatusEnum.InProgress);

            _questionController.Index(model, true, A<Guid>._, A<Guid>._);

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(assessmentId, optionId, furtherInfo)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndContinueClickedWhereOptionsAreAvailable_ShouldBeRedirectedToIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                AssessmentId = assessmentId,
                ChosenOption = Guid.NewGuid()
            };

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
              .Returns(AssessmentStatusEnum.InProgress);

            var result = _questionController.Index(model, true, A<Guid>._, A<Guid>._) as RedirectToRouteResult;

            AssertIndexAction(result, assessmentId);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndOptionClicked_UpdateQuestionViewModelShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var optionId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                AssessmentId = assessmentId
            };

            _questionController.Index(model, false, optionId, A<Guid>._);

            A.CallTo(() => _questionViewModelBuilder.UpdateQuestionViewModel(model, optionId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndOptionClicked_DefaultIndexViewAndModelShouldBeReturned()
        {
            var assessmentId = Guid.NewGuid();
            var optionId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                AssessmentId = assessmentId
            };

            A.CallTo(() => _questionViewModelBuilder.UpdateQuestionViewModel(model, optionId)).Returns(model);

            var result = _questionController.Index(model, false, optionId, A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndOptionClicked_ModelStateShouldNotContainDisplayFurtherInformationQuestionproperty()
        {
            var optionId = Guid.NewGuid();

            _questionController.Index(A<QuestionViewModel>._, false, optionId, A<Guid>._);

            _questionController.ModelState.Keys.Should().NotContain("DisplayFurtherInformationQuestion");
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndAssessmentIsComplete_ShouldBeRedirectedToAssessmentComplete()
        {
            var assessmentId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                ChosenOption = Guid.NewGuid(),
                AssessmentId = assessmentId
            };

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(AssessmentStatusEnum.Complete);

            var result = _questionController.Index(model, true, A<Guid>._, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Assessment.ActionNames.Complete);
            result.RouteValues["controller"].Should().Be(MVC.Assessment.Name);
            result.RouteValues["id"].Should().Be(assessmentId);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndAssessmentIsReadyToComplete_ShouldBeRedirectedToAddCompletionDetails()
        {
            var assessmentId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                ChosenOption = Guid.NewGuid(),
                AssessmentId = assessmentId
            };

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(AssessmentStatusEnum.ReadyToComplete);

            var result = _questionController.Index(model, true, A<Guid>._, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Assessment.ActionNames.Complete);
            result.RouteValues["controller"].Should().Be(MVC.Assessment.Name);
        }

        [TestMethod]
        public void BackPOST_GivenAssessentId_ShouldRedirectToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var result = _questionController.Back(assessmentId) as RedirectToRouteResult;

            AssertIndexAction(result, assessmentId);
        }

        [TestMethod]
        public void BackPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(QuestionController).GetMethod("Back", new[] { typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void BackPOST_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(QuestionController).GetMethod("Back", new[] { typeof(Guid) }).Should().BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void ResetPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(QuestionController).GetMethod("Reset", new[] { typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void ResetPOST_ShouldBeDecoratedWithAssessmentInProgressAttribute()
        {
            typeof(QuestionController).GetMethod("Reset", new[] { typeof(Guid) }).Should().BeDecoratedWith<AssessmentInProgressAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void BackPOST_GivenAssessmentId_WorkflowSetAssessmentPreviousStepShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();

            _questionController.Back(assessmentId);

            A.CallTo(() => _workflowHandler.SetAssessmentPreviousStep(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetPOST_GivenAssessmentId_WorkflowResetAssessmentStageShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();

            _questionController.Reset(assessmentId);

            A.CallTo(() => _workflowHandler.ResetAssessmentStage(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetPOST_GivenAssessmentId_ShouldBeRedirectedToQuestionIndexAction()
        {
            var assessmentId = Guid.NewGuid();

            var result = _questionController.Reset(assessmentId) as RedirectToRouteResult;

            AssertIndexAction(result, assessmentId);
        }

        [TestMethod]
        public void IndexGET_GivenAssessmentId_CurrentQuestionAnswerShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();

            var assessment = new Assessment() {AssessmentId = assessmentId, CurrentWorkflowQuestionId = currentQuestionId};

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(assessment);

            _questionController.Index(assessmentId);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenValidModelAndAssessmentIsBreak_ShouldBeRedirectedToBreakPage()
        {
            var patientId = Guid.NewGuid();

            var model = new QuestionViewModel()
            {
                ChosenOption = Guid.NewGuid(),
                PatientId = patientId
            };

            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(AssessmentStatusEnum.Break);

            var result = _questionController.Index(model, true, A<Guid>._, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.BreakPage.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.BreakPage.Name);
        }

        [TestMethod]
        public void QuestionController_ShouldBeDecoradedWithOutputCacheAttribute()
        {
            typeof(QuestionController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.NoStore);
            typeof(QuestionController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.Duration == 0);
            typeof(QuestionController).Should().BeDecoratedWith<OutputCacheAttribute>(x => x.VaryByParam == "None");
        }

        [TestMethod]
        public void QuestionController_ShouldBeDecoratedWithAgreedToDisclaimerAuthorizeAttributeNinjectAttribute()
        {
            typeof(QuestionController).Should().BeDecoratedWith<AgreedToDisclaimerAuthorizeAttributeNinject>();
        }

        [TestMethod]
        public void EditGET_GivenAssessmentIdDefaultViewShouldBeReturned()
        {
            var result = _questionController.Edit(A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _questionController.Edit(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_QuestionAnswerShouldBeRetrieved()
        {
            var assessment = new Assessment();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _questionController.Edit(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_QuestionViewModelShouldBeCalled()
        {
            var assessment = new Assessment();
            var questionAnswer = new QuestionAnswer();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(questionAnswer);

            _questionController.Edit(A<Guid>._);

            A.CallTo(() => _questionViewModelBuilder.BuildQuestionViewModel(assessment, questionAnswer)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenAssessmentId_QuestionViewModelShouldBeReturned()
        {
            A.CallTo(() => _questionViewModelBuilder.BuildQuestionViewModel(A<Assessment>._, A<QuestionAnswer>._))
                .Returns(new QuestionViewModel());

            var result = _questionController.Edit(A<Guid>._) as ViewResult;

            result.Model.Should().BeOfType<QuestionViewModel>();
            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            typeof(QuestionController).GetMethod("Edit", new[] { typeof(QuestionViewModel), typeof(Guid) }).Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void EditPOST_GivenModelStateIsNotValid_QuestionViewModelShouldBeReturned()
        {
            var questionViewModel = new QuestionViewModel();

            _questionController.ModelState.AddModelError("error", "error");
            var result = _questionController.Edit(questionViewModel, A<Guid>._) as ViewResult;

            result.Model.Should().Be(questionViewModel);
        }

        [TestMethod]
        public void EditPOST_GivenModelStateIsNotValid_DefaultViewShouldBeReturned()
        {
            _questionController.ModelState.AddModelError("error", "error");
            var result = _questionController.Edit(A<QuestionViewModel>._, A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditPOST_GivenQuestionViewModelAndModelStateIsValid_WorkflowHandlerShouldBeCalled()
        {
            var questionViewModel = new QuestionViewModel()
            {
                AssessmentId = Guid.NewGuid(),
                QuestionAnswerId = Guid.NewGuid(),
                FurtherInformationAnswer = "answer"
            };

            _questionController.Edit(questionViewModel, A<Guid>._);

            A.CallTo(() => _workflowHandler.SetAssessmentReviseNextStep(questionViewModel.AssessmentId, questionViewModel.QuestionAnswerId, questionViewModel.FurtherInformationAnswer)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditPOST_GivenSetAssessmentReviseNextStepReturnsTrue_ShouldBeRedirectedToQuestionEditAction()
        {
            var model = new QuestionViewModel()
            {
                AssessmentId = Guid.NewGuid()
            };

            A.CallTo(() => _workflowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(true);

            var result = _questionController.Edit(model, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Edit);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(model.AssessmentId);
        }

        [TestMethod]
        public void EditPOST_GivenSetAssessmentReviseNextStepReturnsFalse_ShouldBeRedirectedToPersonIndex()
        {
            var model = new QuestionViewModel()
            {
                PatientId = Guid.NewGuid()
            };

            A.CallTo(() => _workflowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(false);

            var result = _questionController.Edit(model, A<Guid>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Assessment.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Assessment.Name);
            result.RouteValues["id"].Should().Be(model.PatientId);
        }

        [TestMethod]
        public void EditPOST_GivenModelStateIsNotValid_WorkflowHandlerShouldNotBeCalled()
        {
            _questionController.ModelState.AddModelError("error", "error");

            _questionController.Edit(A<QuestionViewModel>._, A<Guid>._);

            A.CallTo(() => _workflowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._)).MustNotHaveHappened();
        }

        [TestMethod]
        public void EditGET_ShouldBeDecoratedWithAssessmentCompleteAttribute()
        {
            typeof(QuestionController).GetMethod("Edit", new[] { typeof(Guid) }).Should().BeDecoratedWith<AssessmentCompleteAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithAssessmentCompleteAttribute()
        {
            typeof(QuestionController).GetMethod("Edit", new[] { typeof(QuestionViewModel), typeof(Guid) }).Should().BeDecoratedWith<AssessmentCompleteAttribute>(x => x.ActionParameterId == "assessmentId");
        }

        [TestMethod]
        public void QuestionController_ShouldInheritFromBaseController()
        {
            typeof(QuestionController).BaseType.Name.Should().Be(typeof(LayoutController).Name);
        }

        [TestMethod]
        public void IndexPOST_GivenModelStateIsNotValidAndIsAjaxRequest_ResultShouldHaveValidPropertySetAsFalse()
        {
            SetupAjaxRequest(true);

            _questionController.ModelState.AddModelError("error", "error");
            var result =
                _questionController.Index(A<QuestionViewModel>._, A<bool?>._, A<Guid?>._, A<Guid>._) as JsonResult;

            AssertJsonProperty(result.Data, "Valid", false);
        }

        [TestMethod]
        public void IndexPOST_GivenModelStateIsValidAndIsAjaxRequestAndContinueButtonWasNotClicked_ResultShouldHaveValidPropertySetAsTrue()
        {
            SetupAjaxRequest(true);

            var result =
                _questionController.Index(A<QuestionViewModel>._, false, A<Guid?>._, A<Guid>._) as JsonResult;

            AssertJsonProperty(result.Data, "Valid", true);
        }


        [TestMethod]
        public void IndexPOST_GivenModelStateisNotValidAndAjaxRequest_ResultShouldHaveHtmlPropertySet()
        {
            SetupAjaxRequest(true);

            const string partialview = "partialView";
            A.CallTo(() => _partialViewRenderer.RenderPartialViewToString(A<ControllerContext>._, A<ViewDataDictionary>._,
                        A<TempDataDictionary>._, A<string>._, A<object>._)).Returns(partialview);

            var model = new QuestionViewModel()
            {
                AssessmentId = Guid.NewGuid(),
                ChosenOption = Guid.NewGuid()
            };

            
            _questionController.ModelState.AddModelError("error", "error");
            var result = _questionController.Index(model, false, A<Guid?>._, A<Guid>._) as JsonResult;

            AssertJsonProperty(result.Data, "Html", partialview);
        }

        [TestMethod]
        public void IndexPOST_GivenModalStateIsValidAndIsAjaxRequestAndContinueButtonWasClicked_ResultShouldHaveRedirectPropertSet()
        {
            SetupAjaxRequest(true);

            const string myroute = "myRoute";
            A.CallTo(() => _urlHelper.RouteUrl(A<RouteValueDictionary>._)).Returns(myroute);
            A.CallTo(() => _workflowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._))
                .Returns(AssessmentStatusEnum.Complete);

            var model = new QuestionViewModel()
            {
                AssessmentId = Guid.NewGuid(),
                ChosenOption = Guid.NewGuid()
            };

            var result =
               _questionController.Index(model, true, A<Guid?>._, A<Guid>._) as JsonResult;

            AssertJsonProperty(result.Data, "Redirect", myroute);
        }


        #region private

        private void AssertJsonProperty(object data, string property, object expected)
        {
            var validProp = data.GetType().GetProperty(property);

            var validValue = validProp.GetValue(data, null);

            validValue.Should().Be(expected);
        }

        private void AssertIndexAction(RedirectToRouteResult result, Guid assessmentId)
        {
            result.RouteValues["action"].Should().Be(MVC.Question.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Question.Name);
            result.RouteValues["assessmentId"].Should().Be(assessmentId);
        }

        private void SetupAjaxRequest(bool ajax)
        {
            var httpContext = A.Fake<HttpContextBase>();
            var httpRequest = A.Fake<HttpRequestBase>();

            var headers = new NameValueCollection()
            {
                {"X-Requested-With", ajax ? "XMLHttpRequest" : ""}
            };

            A.CallTo(() => httpRequest.Headers).Returns(headers);
            A.CallTo(() => httpContext.Request).Returns(httpRequest);

            _questionController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        #endregion
    }
}


