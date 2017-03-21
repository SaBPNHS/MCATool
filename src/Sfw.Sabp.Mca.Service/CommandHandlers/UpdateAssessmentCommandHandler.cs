using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class UpdateAssessmentCommandHandler : ICommandHandler<UpdateAssessmentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAssessmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(UpdateAssessmentCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var assessment = _unitOfWork.Context.Set<Assessment>().First(x => x.AssessmentId == command.AssessmentId);

            assessment.DateAssessmentStarted = command.DateAssessmentStarted;
            assessment.Stage1DecisionToBeMade = command.Stage1DecisionToBeMade;
            assessment.RoleId = command.RoleId;
            assessment.DecisionMaker = command.DecisionMaker;
        }
    }
}
