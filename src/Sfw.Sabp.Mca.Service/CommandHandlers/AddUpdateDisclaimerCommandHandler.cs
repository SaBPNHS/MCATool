using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class AddUpdateDisclaimerCommandHandler : ICommandHandler<AddUpdateDisclaimerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPrincipalProvider _userPrincipalProvider;
        
        public AddUpdateDisclaimerCommandHandler(IUnitOfWork unitOfWork, IUserPrincipalProvider userPrincipalProvider)
        {
            _unitOfWork = unitOfWork;
            _userPrincipalProvider = userPrincipalProvider;            
        }

        public void Execute(AddUpdateDisclaimerCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var existing = _unitOfWork.Context.Set<Disclaimer>().FirstOrDefault(x => x.DisclaimerId == command.DisclaimerId);

            if (existing != null)
            {
                existing.IsAgreed = command.IsAgreed;
            }
            else
            {
                var disclaimer = new Disclaimer
                {
                    DisclaimerId = command.DisclaimerId,
                    IsAgreed=command.IsAgreed,
                    AssessorDomainName = _userPrincipalProvider.CurrentUserName                
                };

                _unitOfWork.Context.Set<Disclaimer>().Add(disclaimer);
            }
        }
    }
}
