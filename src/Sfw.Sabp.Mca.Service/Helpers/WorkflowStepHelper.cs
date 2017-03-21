using System;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public class WorkflowStepHelper : IWorkflowStepHelper
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public WorkflowStepHelper(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public WorkflowStep GetWorkflowStep(Guid questionOptionId, Assessment assessment)
        {
            var workflowStep =
                _queryDispatcher.Dispatch<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>(new WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery
                {
                    QuestionOptionId = questionOptionId,
                    WorkflowVersionId = assessment.WorkflowVersionId,
                    CurrentWorkflowQuestionId = assessment.CurrentWorkflowQuestionId
                });
            return workflowStep;
        }
    }
}
