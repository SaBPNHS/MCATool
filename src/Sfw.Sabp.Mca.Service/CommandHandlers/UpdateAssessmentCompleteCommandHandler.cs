using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using System;
using System.Linq;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class UpdateAssessmentCompleteCommandHandler : ICommandHandler<UpdateAssessmentCompleteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAssessmentCompleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(UpdateAssessmentCompleteCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var assessment = _unitOfWork.Context.Set<Assessment>().First(x => x.AssessmentId == command.AssessmentId);

            assessment.DateAssessmentEnded = command.DateAssessmentEnded;
            assessment.TerminatedAssessmentReason = command.TerminatedAssessmentReason;
        }
    }
}
