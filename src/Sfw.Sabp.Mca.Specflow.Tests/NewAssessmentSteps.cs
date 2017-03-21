using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Workflow;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Pdf;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;
using System;
using System.Web.Mvc;
using TechTalk.SpecFlow;

namespace Sfw.Sabp.Mca.Specflow.Tests
{
    [Binding]
    public class NewAssessmentSteps
    {
        private FluentValidation.Results.ValidationResult _validatorResult;
        private readonly IFutureDateValidator _futureDateValidator = A.Fake<IFutureDateValidator>();
        private readonly IAssessmentViewModelBuilder _assessmentBuilder = A.Fake<IAssessmentViewModelBuilder>();
        private readonly IWorkflowHandler _workflowHandler = A.Fake<IWorkflowHandler>();
        private readonly IPdfCreationProvider _pdfCreationProvider = A.Fake<IPdfCreationProvider>();
        private readonly IAssessmentHelper _assessmentHelper = A.Fake<IAssessmentHelper>();
        private readonly ITerminatedViewModelBuilder _terminatedViewModelBuilder = A.Fake<ITerminatedViewModelBuilder>();
        private readonly IRoleHelper _roleHelper = A.Fake<IRoleHelper>();
        private readonly IPatientHelper _patientHelper = A.Fake<IPatientHelper>();
        private readonly IFeedBackBuilder _feedBackBuilder = A.Fake<IFeedBackBuilder>();
        private readonly ICopyrightViewModelBuilder _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();

        private ActionResult _result;
        private AssessmentController _controller;
        private AssessmentViewModel _assessmentModel;

        [Given(@"The user has entered all the information")]
        public void GivenTheUserHasEnteredAllTheInformation()
        {
            _assessmentModel = new AssessmentViewModel
            {
                AssessmentId = new Guid(),
                PatientFirstName = "FirstName",
                Stage1DecisionToBeMade = "Test decision about mca",
                Stage1DecisionClearlyMade = true
            };
            _controller = new AssessmentController(_assessmentBuilder, _workflowHandler, _pdfCreationProvider, _assessmentHelper, _terminatedViewModelBuilder, _patientHelper, _roleHelper, _feedBackBuilder, _copyrightViewModelBuilder);
        }

        [Given(@"the user has not entered the decision to be made")]
        public void GivenTheUserHasNotEnteredTheDecisionToBeMade()
        {
            _assessmentModel = new AssessmentViewModel
            {
                AssessmentId = new Guid(),
                PatientFirstName = "FirstName",
                Stage1DecisionToBeMade = string.Empty,
                Stage1DecisionClearlyMade = true
            };
        }

        [Given(@"the user has not selected the stage (.*) decision clearly made checkbox")]
        public void GivenTheUserHasNotSelectedTheStageDecisionClearlyMadeCheckbox(int p0)
        {
            _assessmentModel = new AssessmentViewModel
            {
                AssessmentId = new Guid(),
                PatientFirstName = "FirstName",
                Stage1DecisionToBeMade = "Decisiontobemade",
                DateAssessmentStarted = DateTime.Now.AddDays(-2),
                Stage1DecisionClearlyMade = false
            };
        }

        [Given(@"the user has entered future date for assessment start date")]
        public void GivenTheUserHasEnteredFutureDateForAssessmentStartDate()
        {
            _assessmentModel = new AssessmentViewModel
            {
                AssessmentId = new Guid(),
                PatientFirstName = "FirstName",
                Stage1DecisionToBeMade = "Decisiontobemade",
                DateAssessmentStarted = DateTime.Now.AddDays(2),
                Stage1DecisionClearlyMade = true
            };
        }

        [When(@"He Clicks on Create button")]
        public void WhenHeClicksOnCreateButton()
        {
            _result = _controller.Create(_assessmentModel);
        }

        [When(@"click on Create button")]
        public void WhenClickOnCreateButton()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime>._)).Returns(true);
            var validator = new AssessmentViewModelValidator(_futureDateValidator);

            _validatorResult = validator.Validate(_assessmentModel);
        }

        [When(@"clicked on Create button")]
        public void WhenClickedOnCreateButton()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime>._)).Returns(false);
            var validator = new AssessmentViewModelValidator(_futureDateValidator);

            _validatorResult = validator.Validate(_assessmentModel);
        }

        [Then(@"He should be redirected to the Question Page")]
        public void ThenHeShouldBeRedirectedToTheQuestionPage()
        {
            var expected = "Index";
            Assert.IsNotNull(_result);
            var tresults = _result as RedirectToRouteResult;
            Assert.AreEqual(expected, tresults.RouteValues["action"]);
        }

        [Then(@"he should be shown the error message ""(.*)""")]
        public void ThenHeShouldBeShownTheErrorMessage(string p0)
        {
            Assert.IsFalse(_validatorResult.IsValid);
            var err = _validatorResult.Errors[0].ErrorMessage;
            Assert.AreEqual(err, (p0));
        }
    }
}
