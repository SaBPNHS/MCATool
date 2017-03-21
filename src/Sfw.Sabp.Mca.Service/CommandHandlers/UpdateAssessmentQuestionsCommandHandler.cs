using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class UpdateAssessmentQuestionsCommandHandler : ICommandHandler<UpdateAssessmentQuestionsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAssessmentQuestionsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(UpdateAssessmentQuestionsCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var assessment = _unitOfWork.Context.Set<Assessment>().First(x => x.AssessmentId == command.AssessmentId);
            assessment.CurrentWorkflowQuestionId = command.NextQuestionId;
            assessment.PreviousWorkflowQuestionId = command.PreviousQuestionId;
            assessment.ResetWorkflowQuestionId = command.ResetWorkflowQuestionId;
        }
    }
}
