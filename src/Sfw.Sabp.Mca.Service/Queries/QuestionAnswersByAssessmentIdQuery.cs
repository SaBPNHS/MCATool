using Sfw.Sabp.Mca.Core.Contracts;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class QuestionAnswersByAssessmentQuery : IQuery
    {
        public Assessment Assessment { get; set; }
    }
}
