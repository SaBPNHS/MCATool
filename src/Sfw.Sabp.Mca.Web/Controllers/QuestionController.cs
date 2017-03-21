using System;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Core.Enum.Exceptions;
using Sfw.Sabp.Mca.Infrastructure.Web.Helper;
using Sfw.Sabp.Mca.Infrastructure.Web.Render;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Workflow;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Controllers
{
    [AgreedToDisclaimerAuthorizeAttributeNinject]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public partial class QuestionController : LayoutController
    {
        private readonly IQuestionViewModelBuilder _questionViewModelBuilder;
        private readonly IWorkflowHandler _workflowHandler;
        private readonly IAssessmentHelper _assessmentHelper;
        private readonly IQuestionAnswerHelper _questionAnswerHelper;
        private readonly IPartialViewRenderer _partialViewRenderer;
        private readonly IUrlHelper _urlHelper;

        public QuestionController(IQuestionViewModelBuilder questionViewModelBuilder, 
            IWorkflowHandler workflowHandler, 
            IAssessmentHelper assessmentHelper, 
            IQuestionAnswerHelper questionAnswerHelper, 
            IFeedBackBuilder feedBackBuilder,
            ICopyrightViewModelBuilder copyrightViewModelBuilder, 
            IPartialViewRenderer partialViewRenderer, 
            IUrlHelper urlHelper)
            : base(feedBackBuilder, copyrightViewModelBuilder)
        {
            _questionViewModelBuilder = questionViewModelBuilder;
            _workflowHandler = workflowHandler;
            _assessmentHelper = assessmentHelper;
            _questionAnswerHelper = questionAnswerHelper;
            _partialViewRenderer = partialViewRenderer;
            _urlHelper = urlHelper;
        }

        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult Index(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(assessment);

            var viewModel = _questionViewModelBuilder.BuildQuestionViewModel(assessment, questionAnswer);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult Index(QuestionViewModel model, bool? continueButton, Guid? chosenOption, Guid assessmentId)
        {
            if (ModelState.IsValid)
            {
                if (ContinueClicked(continueButton))
                {
                    if (model.ChosenOption.HasValue)
                    {
                        var status = _workflowHandler.SetAssessmentNextStep(model.AssessmentId, model.ChosenOption.Value,
                            model.FurtherInformationAnswer);

                        var actionResult = RedirectAssessment(model, status);
                        if (Request.IsAjaxRequest())
                        {
                            return Json(new {Redirect = _urlHelper.RouteUrl(actionResult.RouteValues)});
                        }

                        return actionResult;
                    }
                }
            }

            if (OptionClicked(chosenOption))
            {
                model = _questionViewModelBuilder.UpdateQuestionViewModel(model, chosenOption.Value);
                ModelState.Remove("DisplayFurtherInformationQuestion");
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new { Html = _partialViewRenderer.RenderPartialViewToString(ControllerContext, ViewData, TempData, MVC.Question.Views._OptionsPartial, model), Valid = ModelState.IsValid });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult Back(Guid assessmentId)
        {
            _workflowHandler.SetAssessmentPreviousStep(assessmentId);

            return RedirectToAction(MVC.Question.Index(assessmentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult Reset(Guid assessmentId)
        {
            _workflowHandler.ResetAssessmentStage(assessmentId);

            return RedirectToAction(MVC.Question.Index(assessmentId));
        }

        [AssessmentComplete(ActionParameterId = "assessmentId")]
        public virtual ActionResult Edit(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(assessment);

            var model = _questionViewModelBuilder.BuildQuestionViewModel(assessment, questionAnswer);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentComplete(ActionParameterId = "assessmentId")]
        public virtual ActionResult Edit(QuestionViewModel model, Guid assessmentId)
        {
            if (ModelState.IsValid)
            {
                var result = _workflowHandler.SetAssessmentReviseNextStep(model.AssessmentId, model.QuestionAnswerId, model.FurtherInformationAnswer);

                if (result)
                {
                    return RedirectToAction(MVC.Question.Edit(model.AssessmentId));
                }

                return RedirectToAction(MVC.Assessment.Index(model.PatientId));
            }

            return View(model);
        }

        #region private

        private RedirectToRouteResult RedirectAssessment(QuestionViewModel model, AssessmentStatusEnum status)
        {
            switch (status)
            {
                case AssessmentStatusEnum.Complete:
                case AssessmentStatusEnum.ReadyToComplete:
                    return RedirectToAction(MVC.Assessment.Complete(model.AssessmentId));

                case AssessmentStatusEnum.InProgress:
                    return RedirectToAction(MVC.Question.Index(model.AssessmentId));
  
                case AssessmentStatusEnum.Break:
                    return RedirectToAction(MVC.BreakPage.Index());

                default:
                    throw new UnknownAssessmentStatusEnum();
            }
        }

        private bool OptionClicked(Guid? chosenOption)
        {
            return chosenOption.HasValue;
        }

        private bool ContinueClicked(bool? continueButton)
        {
            return continueButton.HasValue && continueButton.Value;
        }

        #endregion
    }
}