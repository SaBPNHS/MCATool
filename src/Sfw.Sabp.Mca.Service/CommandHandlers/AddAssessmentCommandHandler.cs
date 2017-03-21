using System;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class AddAssessmentCommandHandler : ICommandHandler<AddAssessmentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPrincipalProvider _userPrincipalProvider;
        private readonly IActiveDirectoryPrincipal _activeDirectoryPrincipal;

        public AddAssessmentCommandHandler(IUnitOfWork unitOfWork, IActiveDirectoryPrincipal activeDirectoryPrincipal, IUserPrincipalProvider userPrincipalProvider)
        {
            _unitOfWork = unitOfWork;
            _userPrincipalProvider = userPrincipalProvider;
            _activeDirectoryPrincipal = activeDirectoryPrincipal;
        }

        public void Execute(AddAssessmentCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var assessment = new Assessment
            {
                AssessmentId = command.AssessmentId,
                DateAssessmentStarted = (command.DateAssessmentStarted == DateTime.MinValue) ? DateTime.Now : command.DateAssessmentStarted,
                Stage1DecisionToBeMade = command.Stage1DecisionToBeMade,
                Stage1DecisionClearlyMade = command.Stage1DecisionClearlyMade,
                Stage1DecisionConfirmation = command.Stage1DecisionConfirmation,
                Stage1InfoText = command.Stage1InfoText,
                WorkflowVersionId = command.WorkflowVersionId,
                CurrentWorkflowQuestionId = command.CurrentWorkflowQuestionId,
                StatusId = command.StatusId,
                AssessorDomainName = _userPrincipalProvider.CurrentUserName,
                AssessorName = _activeDirectoryPrincipal.DisplayNameForCurrentUser(),
                PatientId = command.PatientId,
                RoleId = command.RoleId,
                DecisionMaker = command.DecisionMaker
            };

            _unitOfWork.Context.Set<Assessment>().Add(assessment);
        }
    }
}
