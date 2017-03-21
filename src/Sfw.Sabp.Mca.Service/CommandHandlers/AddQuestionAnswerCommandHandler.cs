using System;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class AddQuestionAnswerCommandHandler : ICommandHandler<AddQuestionAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AddQuestionAnswerCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Execute(AddQuestionAnswerCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            
            var questionAnswer = new QuestionAnswer
            {
                QuestionAnswerId = Guid.NewGuid(),
                AssessmentId = command.AssessmentId,
                WorkflowQuestionId = command.WorkflowQuestionId,
                QuestionOptionId = command.QuestionOptionId,
                FurtherInformation = !string.IsNullOrWhiteSpace(command.FurtherInformation) ? command.FurtherInformation : null,
                Created = _dateTimeProvider.Now
            };

            _unitOfWork.Context.Set<QuestionAnswer>().Add(questionAnswer);
        }
    }
}
