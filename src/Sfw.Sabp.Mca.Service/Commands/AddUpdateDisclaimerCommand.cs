using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class AddUpdateDisclaimerCommand : ICommand
    {
        public Guid DisclaimerId { get; set; }
        public bool IsAgreed { get; set; }
    }
}
