using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class UpdateAssessmentStatusCommandHandler : ICommandHandler<UpdateAssessmentStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAssessmentStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(UpdateAssessmentStatusCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            
            _unitOfWork.Context.Set<Assessment>().First(x => x.AssessmentId == command.AssessmentId).StatusId = command.StatusId;
        }
    }
}
