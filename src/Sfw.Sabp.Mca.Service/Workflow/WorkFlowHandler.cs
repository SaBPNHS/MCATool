using System;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Workflow
{
    public class WorkFlowHandler : IWorkflowHandler
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentHelper _assessmentHelper;
        private readonly IQuestionAnswerHelper _questionAnswerHelper;
        private readonly IWorkflowStepHelper _workflowStepHelper;

        public WorkFlowHandler(IUnitOfWork unitOfWork, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IAssessmentHelper assessmentHelper, IQuestionAnswerHelper questionAnswerHelper, IWorkflowStepHelper workflowStepHelper)
        {
            _unitOfWork = unitOfWork;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _assessmentHelper = assessmentHelper;
            _questionAnswerHelper = questionAnswerHelper;
            _workflowStepHelper = workflowStepHelper;
        }

        public void SetAssessmentWorkflow(AddAssessmentCommand command)
        {
            var currentWorkflowVersion = _queryDispatcher.Dispatch<CurrentWorkflowQuery, WorkflowVersion>(new CurrentWorkflowQuery());

            command.WorkflowVersionId = currentWorkflowVersion.WorkflowVersionId;
            command.CurrentWorkflowQuestionId = currentWorkflowVersion.InitialWorkflowQuestionId;

            _commandDispatcher.Dispatch(command);

            SaveChanges();
        }

        public void UpdateAssessmentWorkflowQuestion(UpdateAssessmentCommand command)
        {
            var assessment = _assessmentHelper.GetAssessment(command.AssessmentId);

            var workflowVersion = _queryDispatcher.Dispatch<WorkflowVersionByIdQuery, WorkflowVersion>(new WorkflowVersionByIdQuery()
            {
                WorkflowVersionId = assessment.WorkflowVersionId
            });

            _assessmentHelper.UpdateAssessmentQuestions(command.AssessmentId, workflowVersion.InitialWorkflowQuestionId, null, null);

            _commandDispatcher.Dispatch(command);

            _unitOfWork.SaveChanges();
        }

        public AssessmentStatusEnum SetAssessmentNextStep(Guid assessmentId, Guid questionOptionId, string furtherInformation)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var workflowStep = _workflowStepHelper.GetWorkflowStep(questionOptionId, assessment);

            if (workflowStep == null) throw new WorkflowStepNotFoundException();

            if (!assessment.ReadOnly)
            {
                var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(assessment);

                if (HasAnsweredPreviously(questionAnswer))
                    _questionAnswerHelper.RemoveQuestionAnswer(questionAnswer);

                _questionAnswerHelper.AddQuestionAnswer(questionOptionId, furtherInformation, assessment);
            }

            if (HasNextStep(workflowStep))
            {
                var resetQuestionId = assessment.ResetWorkflowQuestionId;

                if (assessment.ReadOnly && HasProgressesedToRestartQuestion(assessment, workflowStep))
                {
                    _assessmentHelper.UpdateAssessmentReadonly(assessmentId, false);
                    resetQuestionId = null;
                }

                _assessmentHelper.UpdateAssessmentQuestions(assessmentId, workflowStep.NextWorkflowQuestionId.Value, assessment.CurrentWorkflowQuestionId, resetQuestionId);    
            }

            _assessmentHelper.UpdateAssessmentStatus(assessmentId, (AssessmentStatusEnum)workflowStep.OutcomeStatusId);

            SaveChanges();

            return (AssessmentStatusEnum)workflowStep.OutcomeStatusId;
        }

        public bool SetAssessmentReviseNextStep(Guid assessmentId, Guid questionAnswerId, string furtherInformation)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(questionAnswerId);

            _questionAnswerHelper.UpdateQuestionAnswer(questionAnswerId, furtherInformation);

            var workflowStep = _workflowStepHelper.GetWorkflowStep(questionAnswer.QuestionOptionId, assessment);

            if (workflowStep == null) throw new WorkflowStepNotFoundException();

            if (HasNextStep(workflowStep))
            {
                _assessmentHelper.UpdateAssessmentQuestions(assessment.AssessmentId, workflowStep.NextWorkflowQuestionId.Value, assessment.CurrentWorkflowQuestionId, null);
            }

            SaveChanges();

            if (HasNextStep(workflowStep))
            {
                return true;
            }

            return false;
        }

        public void CompleteAssessment(Guid assessmentId)
        {
            _assessmentHelper.UpdateAssessmentStatus(assessmentId, AssessmentStatusEnum.Complete);

            SaveChanges();
        }

        public void AddCompletionDetails(Guid assessmentId, DateTime dateAssessmentEnded, string assessmentReason)
        {
            _commandDispatcher.Dispatch(new UpdateAssessmentCompleteCommand { AssessmentId = assessmentId, DateAssessmentEnded = dateAssessmentEnded, TerminatedAssessmentReason = assessmentReason });
            SaveChanges();
        }

        public void SetAssessmentPreviousStep(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            if (assessment.PreviousWorkflowQuestionId != null)
            {
                _assessmentHelper.UpdateAssessmentQuestions(assessmentId, assessment.PreviousWorkflowQuestionId.Value,
                    assessment.PreviousWorkflowQuestionId.Value, null);
            }

            SaveChanges();
        }

        public void ResetAssessmentStage(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            var workflowStage = GetWorkflowStageQuestionAnswers(assessment);

            if (HasInitialWorkflowQuestion(workflowStage))
                _assessmentHelper.UpdateAssessmentQuestions(assessmentId, workflowStage.InitialWorkflowQuestionId.Value, null, null);
            else
            {
                throw new WorkflowStageInitialQuestionNotFoundException();
            }

            var questionAnswers = GetQuestionAnswersForStage(workflowStage, assessment);

            RemoveQuestionAnswers(questionAnswers);

            SaveChanges();
        }

        public void ResetAssessment(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            if (assessment.StatusId == (int) AssessmentStatusEnum.Break)
            {
                var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(assessment);

                _questionAnswerHelper.RemoveQuestionAnswer(questionAnswer);
            }

            _assessmentHelper.UpdateAssessmentQuestions(assessmentId, assessment.WorkflowVersion.InitialWorkflowQuestionId, null, assessment.CurrentWorkflowQuestionId);

            _assessmentHelper.UpdateAssessmentReadonly(assessmentId, true);

            SaveChanges();
        }

        public void RestartBreak(Guid assessmentId)
        {
            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            if (assessment.StatusId != (int) AssessmentStatusEnum.Break)
                throw new WorkflowInvalidStatusException();

            if (!assessment.PreviousWorkflowQuestionId.HasValue)
                throw new InvalidAssessmentQuestionException();

            var questionAnswer = _questionAnswerHelper.GetQuestionAnswer(assessment);

            _questionAnswerHelper.RemoveQuestionAnswer(questionAnswer);

            _assessmentHelper.UpdateAssessmentQuestions(assessmentId, assessment.PreviousWorkflowQuestionId.Value, assessment.PreviousWorkflowQuestionId, null);

            SaveChanges();
        }

        #region private

        private bool HasProgressesedToRestartQuestion(Assessment assessment, WorkflowStep workflowStep)
        {
            if (workflowStep.NextWorkflowQuestionId.HasValue)
                return (assessment.ResetWorkflowQuestionId == workflowStep.NextWorkflowQuestionId.Value);

            return false;
        }

        private void RemoveQuestionAnswers(QuestionAnswers questionAnswers)
        {
            foreach (var answer in questionAnswers.Items)
            {
                _questionAnswerHelper.RemoveQuestionAnswer(new QuestionAnswer() { QuestionAnswerId = answer.QuestionAnswerId });
            }
        }

        private QuestionAnswers GetQuestionAnswersForStage(WorkflowStage workflowStage, Assessment assessment)
        {
            var questionAnswers = _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                new QuestionAnswersByWorkflowStageIdQuery { WorkflowStageId = workflowStage.WorkflowStageId, AssessmentId = assessment.AssessmentId });
            return questionAnswers;
        }

        private bool HasInitialWorkflowQuestion(WorkflowStage workflowStage)
        {
            return workflowStage.InitialWorkflowQuestionId.HasValue;
        }

        private WorkflowStage GetWorkflowStageQuestionAnswers(Assessment assessment)
        {
            return _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(new WorkflowStageByIdQuery
            {
                WorkflowStageId = assessment.WorkflowQuestion.WorkflowStage.WorkflowStageId
            });
        }

        private bool HasAnsweredPreviously(QuestionAnswer questionAnswer)
        {
            return questionAnswer != null;
        }

        private bool HasNextStep(WorkflowStep workflowStep)
        {
            return workflowStep.NextWorkflowQuestionId != null;
        }

        private void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }

        #endregion
    }
}
