using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class RemoveAnswerCommand : ICommand
    {
        public Guid QuestionAnswerId { get; set; }
    }
}
