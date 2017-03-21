using System.Collections.Generic;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Model
{
    public class QuestionAnswers : IQueryResult
    {
        public IEnumerable<QuestionAnswer> Items { get; set; }
    }
}