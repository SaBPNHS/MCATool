using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Pdf
{
    public class GeneratePdf : IPdfCreationProvider
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IQuestionAnswerViewModelBuilder _questionAnswerViewModelBuilder;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IPdfHelper _pdfHelper;
        private readonly IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;

        private string _currentStage = string.Empty;

        public GeneratePdf(IDateTimeProvider dateTimeProvider,
            IQuestionAnswerViewModelBuilder questionAnswerViewModelBuilder, IQueryDispatcher queryDispatcher,
            IPdfHelper pdfHelper, IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _questionAnswerViewModelBuilder = questionAnswerViewModelBuilder;
            _queryDispatcher = queryDispatcher;
            _pdfHelper = pdfHelper;
            _clinicalSystemIdDescriptionProvider = clinicalSystemIdDescriptionProvider;
        }

        public string CreatePdfForAssessment(Assessment assessment, out PdfDocument pdfDocumentGenerated)
        {
            _pdfHelper.CreatePdfDocument();
            _pdfHelper.AddPage();

            AddCommonInformation(assessment);
            var questionAnswers = GetQuestionAnswers(assessment);
            AddSectionInformation(questionAnswers);
            AddEndAssessmentInformation(assessment);
            var pdfFileName = GetFileNameToCreate(assessment);
            pdfDocumentGenerated = _pdfHelper.PdfDocument;

            return pdfFileName;
        }

        private QuestionAnswerListViewModel GetQuestionAnswers(Assessment assessment)
        {
            var query2 = new QuestionAnswersByAssessmentQuery {Assessment = assessment};

            var questionAnswers =
                _queryDispatcher.Dispatch<QuestionAnswersByAssessmentQuery, QuestionAnswers>(query2);
            var viewModel = _questionAnswerViewModelBuilder.BuildQuestionAnswerListViewModel(questionAnswers);
            return viewModel;
        }

        private void AddEndAssessmentInformation(Assessment assessment)
        {
            if (assessment.DateAssessmentEnded != null)
            {
                _pdfHelper.AddLine();
                var date1 = (DateTime)assessment.DateAssessmentEnded;
                var datePart = date1.Date;
                _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentCompleteDateText);
                _pdfHelper.AppendText(" " + datePart.ToString("D"));
            }

            if (!string.IsNullOrWhiteSpace(assessment.TerminatedAssessmentReason))
            {
                _pdfHelper.AddLine();
                _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentTerminationReasonString);
                _pdfHelper.AppendText(" " + assessment.TerminatedAssessmentReason);
            }
        }

        private void AddSectionInformation(QuestionAnswerListViewModel questionAnswers)
        {
            if (questionAnswers.Items != null)
                foreach (var questionAnswer in questionAnswers.Items)
                {
                    if ( _currentStage != questionAnswer.StageDescription)
                    {
                        _currentStage = questionAnswer.StageDescription;
                        _pdfHelper.AddLine(false);
                        _pdfHelper.AddTitle(_currentStage, false);
                        _pdfHelper.AddLine();
                    }

                    _pdfHelper.AddLine(false);
                    _pdfHelper.AddTitle(questionAnswer.Question);
                    _pdfHelper.AddLine();

                    if (string.IsNullOrEmpty(questionAnswer.Answer))
                    {
                        _pdfHelper.WriteText(questionAnswer.FurtherInformation);
                    }
                    else
                    {
                        var questionAnswerText = questionAnswer.Answer;

                        if (!string.IsNullOrWhiteSpace(questionAnswer.FurtherInformation))
                            questionAnswerText += ". " + questionAnswer.FurtherInformation;

                        _pdfHelper.WriteText(questionAnswerText);
                    }
                }
        }

        private void AddCommonInformation(Assessment assessment)
        {
            var datePart = new DateTime();
            if (assessment.DateAssessmentStarted != null)
            {
                var date1 = (DateTime) assessment.DateAssessmentStarted;
                datePart = date1.Date;
            }
            _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.Name);
            _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.Dob);
            _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString());
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber);
            _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText);
            _pdfHelper.AppendText(" " + datePart.ToString("D"));
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(string.Format("{0}:", _clinicalSystemIdDescriptionProvider.GetDescription()));
            _pdfHelper.AppendText(" " + assessment.Patient.ClinicalSystemId);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText);
            _pdfHelper.AppendText(" " + assessment.AssessorName);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.RoleText);
            _pdfHelper.AppendText(" " + assessment.Role.Description);
            _pdfHelper.AddLine();
            if ((assessment.RoleId != (int)RoleIdEnum.DecisionMaker) && !string.IsNullOrWhiteSpace(assessment.DecisionMaker))
            {
                _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText);
                _pdfHelper.AppendText(" " + assessment.DecisionMaker);
                _pdfHelper.AddLine();
            }
            _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false);
            _pdfHelper.AddLine();
            _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false);
            _pdfHelper.AddLine();
            _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade);
            _pdfHelper.AddLine();

        }

        private string GetFileNameToCreate(Assessment assessment)
        {
            var fileName = "MCA Assessment-" + assessment.Patient.ClinicalSystemId + "-" +
                           _dateTimeProvider.Now.ToString(@"yyyy-MM-dd") + "-" + assessment.AssessmentId + ".pdf";
            return fileName;
        }
    }
}