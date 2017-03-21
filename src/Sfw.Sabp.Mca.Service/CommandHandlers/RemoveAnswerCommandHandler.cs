using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class RemoveAnswerCommandHandler :ICommandHandler<RemoveAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(RemoveAnswerCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var questionAnswer = _unitOfWork.Context.Set<QuestionAnswer>().First(x => x.QuestionAnswerId == command.QuestionAnswerId);

            _unitOfWork.Context.Set<QuestionAnswer>().Remove(questionAnswer);
        }
    }
}
