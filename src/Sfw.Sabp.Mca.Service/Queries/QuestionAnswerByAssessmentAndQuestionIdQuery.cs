using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class QuestionAnswerByAssessmentAndQuestionIdQuery : IQuery
    {
        public Guid AssessmentId { get; set; }
        public Guid WorkflowQuestionId { get; set; }
    }
}
