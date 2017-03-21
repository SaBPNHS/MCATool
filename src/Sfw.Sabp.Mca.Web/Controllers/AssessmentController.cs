using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Workflow;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.Pdf;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Controllers
{
    [AgreedToDisclaimerAuthorizeAttributeNinject]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public partial class AssessmentController : LayoutController
    {
        private readonly IAssessmentViewModelBuilder _assessmentViewModelBuilder;
        private readonly IWorkflowHandler _workflowHandler;
        private readonly IPdfCreationProvider _pdfCreationProvider;
        private readonly IAssessmentHelper _assessmentHelper;
        private readonly ITerminatedViewModelBuilder _terminatedViewModelBuilder;
        private readonly IPatientHelper _patientHelper;
        private readonly IRoleHelper _roleHelper;

        public AssessmentController(IAssessmentViewModelBuilder assessmentViewModelBuilder, 
            IWorkflowHandler workflowHandler, 
            IPdfCreationProvider pdfCreationProvider, 
            IAssessmentHelper assessmentHelper,
            ITerminatedViewModelBuilder terminatedViewModelBuilder, 
            IPatientHelper patientHelper, 
            IRoleHelper roleHelper, 
            IFeedBackBuilder feedBackBuilder,
            ICopyrightViewModelBuilder copyrightViewModelBuilder)
            : base(feedBackBuilder, copyrightViewModelBuilder)
        {
            _assessmentViewModelBuilder = assessmentViewModelBuilder;
            _workflowHandler = workflowHandler;
            _pdfCreationProvider = pdfCreationProvider;
            _assessmentHelper = assessmentHelper;
            _terminatedViewModelBuilder = terminatedViewModelBuilder;
            _patientHelper = patientHelper;
            _roleHelper = roleHelper;
        }
        
        public virtual ActionResult Index(Guid id)
        {
            var result = _assessmentHelper.GetAssessmentsByPatient(id);

            var model = _assessmentViewModelBuilder.BuildAssessmentListViewModel(id, result);

            return View(model);
        }

        [HttpGet]        
        [Audit(AuditProperties = "id,clinicalSystemId")]
        public virtual ActionResult CreatePdf(Guid id, string clinicalSystemId)
        {
            var assessment = _assessmentHelper.GetAssessment(id);

            PdfDocument pdfGeneratedDocument;
            var generatedPdf = _pdfCreationProvider.CreatePdfForAssessment(assessment, out pdfGeneratedDocument);

            using (var stream = new MemoryStream())
            {
                if (pdfGeneratedDocument != null)
                {
                    pdfGeneratedDocument.Info.Title = generatedPdf;
                    pdfGeneratedDocument.Save(stream, false);
                }
                return File(stream.ToArray(), "application/pdf", generatedPdf);
            }
        }

        [HttpGet]
        public virtual ActionResult Create(Guid id)
        {
            var patient = _patientHelper.GetPatient(id);

            var roles = _roleHelper.GetRoles();

            var viewModel = _assessmentViewModelBuilder.BuildAssessmentViewModel(patient, roles);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(AssessmentViewModel model)
        {
            if (ModelState.IsValid && !(model.RoleId == (int)RoleIdEnum.Assessor && string.IsNullOrWhiteSpace(model.DecisionMaker)))
            {
                var command = _assessmentViewModelBuilder.BuildAddAssessmentCommand(model);

                _workflowHandler.SetAssessmentWorkflow(command);

                return RedirectToAction(MVC.Question.Index(command.AssessmentId));
            }
            
            return View(model);
        }

        [HttpGet]
        [AssessmentInProgress(ActionParameterId = "id")]
        public virtual ActionResult Restart(Guid id)
        {
            var assessment = _assessmentHelper.GetAssessment(id);

            var viewModel = _assessmentViewModelBuilder.BuildAssessmentViewModel(assessment);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult RestartNo(AssessmentViewModel model, Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var viewModel = _terminatedViewModelBuilder.BuildTerminatedAssessmentViewModel(assessment);
            viewModel.TerminatedReasonRequired = true;
            return View(MVC.Assessment.Views.Terminated, viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult RestartAssessment(AssessmentViewModel model, Guid assessmentId)
        {
            model.YesClicked = true;

            return View(MVC.Assessment.Views.Restart, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult RestartStage(Guid assessmentId)
        {
            _workflowHandler.ResetAssessmentStage(assessmentId);

            return RedirectToAction(MVC.Question.Index(assessmentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult RestartStartAssessment(Guid assessmentId)
        {
            _workflowHandler.ResetAssessment(assessmentId);

            return RedirectToAction(MVC.Question.Index(assessmentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentInProgress(ActionParameterId = "assessmentId")]
        public virtual ActionResult RestartFromBreak(Guid assessmentId)
        {
            _workflowHandler.RestartBreak(assessmentId);

            return RedirectToAction(MVC.Question.Index(assessmentId));
        }

        [HttpPost]
        public virtual ActionResult AddCompletionDetails(TerminatedViewModel model)
        {
            _workflowHandler.AddCompletionDetails(model.AssessmentId, model.DateAssessmentEnded, model.TerminatedAssessmentReason);

            var assessment = _assessmentHelper.GetAssessment(model.AssessmentId);
            var viewModel = _assessmentViewModelBuilder.BuildAssessmentViewModel(assessment);
            _workflowHandler.CompleteAssessment(model.AssessmentId);
            return View(MVC.Assessment.Views.Complete, viewModel);
        }

        [HttpGet]
        public virtual ActionResult Complete(Guid id)
        {         
            var assessment = _assessmentHelper.GetAssessment(id);

            var viewModel = _terminatedViewModelBuilder.BuildTerminatedAssessmentViewModel(assessment);

            return View(MVC.Assessment.Views.Terminated, viewModel);
        }

        [HttpGet]
        [AssessmentComplete(ActionParameterId = "id")]
        public virtual ActionResult Edit(Guid id)
        {
            var assessment = _assessmentHelper.GetAssessment(id);

            var roles = _roleHelper.GetRoles();

            if (assessment==null) throw new HttpException((int)HttpStatusCode.NotFound, "assessment");

            var model = _assessmentViewModelBuilder.BuildAssessmentViewModel(assessment, roles);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AssessmentComplete(ActionParameterId = "assessmentId")]
        public virtual ActionResult Edit(AssessmentViewModel model, Guid assessmentId)
        {
            if (ModelState.IsValid)
            {
                var command = _assessmentViewModelBuilder.BuildUpdateAssessmentCommand(model);

                _workflowHandler.UpdateAssessmentWorkflowQuestion(command);

                return RedirectToAction(MVC.Question.Edit(model.AssessmentId));
            }

            return View(model);
        }
    }
}

