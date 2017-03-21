using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class UpdateQuestionAnswerCommandHandler : ICommandHandler<UpdateQuestionAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(UpdateQuestionAnswerCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var questionAnswer = _unitOfWork.Context.Set<QuestionAnswer>().First(x => x.QuestionAnswerId == command.QuestionAnswerId);

            questionAnswer.FurtherInformation = command.FurtherInformation;
        }
    }
}
