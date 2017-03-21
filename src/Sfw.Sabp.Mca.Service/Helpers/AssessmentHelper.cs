using System;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public class AssessmentHelper : IAssessmentHelper
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public AssessmentHelper(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        public Assessment GetAssessment(Guid id)
        {
            var assessment =
                _queryDispatcher.Dispatch<AssessmentByIdQuery, Assessment>(new AssessmentByIdQuery
                {
                    AssessmentId = id
                });
            return assessment;
        }

        public Assessments GetAssessmentsByPatient(Guid id)
        {
            var assessmentQuery = new AssessmentsByPatientIdQuery { PatientId = id };

           return _queryDispatcher.Dispatch<AssessmentsByPatientIdQuery, Assessments>(assessmentQuery);
        }

        public void UpdateAssessmentQuestions(Guid assessmentId, Guid nextWorkflowQuestionId, Guid? previousQuestionId, Guid? resetQuestionId)
        {
            _commandDispatcher.Dispatch(new UpdateAssessmentQuestionsCommand
            {
                AssessmentId = assessmentId,
                NextQuestionId = nextWorkflowQuestionId,
                PreviousQuestionId = previousQuestionId,
                ResetWorkflowQuestionId = resetQuestionId
            });
        }

        public void UpdateAssessmentStatus(Guid assessmentId, AssessmentStatusEnum status)
        {
            _commandDispatcher.Dispatch(new UpdateAssessmentStatusCommand
            {
                AssessmentId = assessmentId,
                StatusId = (int)status
            });
        }

        public void UpdateAssessmentReadonly(Guid assessmentId, bool readOnly)
        {
            _commandDispatcher.Dispatch(new UpdateAssessmentReadOnlyCommand { AssessmentId = assessmentId, ReadOnly = readOnly });
        }
    }
}
