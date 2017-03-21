using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery : IQuery
    {
        public Guid WorkflowVersionId { get; set; }
        public Guid QuestionOptionId { get; set; }
        public Guid CurrentWorkflowQuestionId { get; set; }
    }
}
