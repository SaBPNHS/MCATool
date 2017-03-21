using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class QuestionAnswersByWorkflowStageIdQuery : IQuery
    {
        public Guid WorkflowStageId { get; set; }

        public Guid AssessmentId { get; set; }
    }

}
