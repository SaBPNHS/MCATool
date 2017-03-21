using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class QuestionAnswerByIdQuery : IQuery
    {
        public Guid QuestionAnswerId { get; set; }
    }
}
