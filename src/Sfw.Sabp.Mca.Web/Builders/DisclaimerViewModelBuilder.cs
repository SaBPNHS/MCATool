using AutoMapper;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class DisclaimerViewModelBuilder : IDisclaimerViewModelBuilder
    {
        public AddUpdateDisclaimerCommand BuildAddDisclaimerCommand(DisclaimerViewModel disclaimer)
        {
            if (disclaimer == null) throw new ArgumentNullException("disclaimer");

            var command = Mapper.DynamicMap<DisclaimerViewModel, AddUpdateDisclaimerCommand>(disclaimer);
            if (command.DisclaimerId == Guid.Empty) command.DisclaimerId = Guid.NewGuid();

            return command;
        }

        public DisclaimerViewModel BuildDisclaimerViewModel(Disclaimer disclaimer)
        {
            return new DisclaimerViewModel()
            {
                TermsAndConditionsEnabled = disclaimer == null || !disclaimer.IsAgreed,
                IsAgreed = disclaimer!= null && disclaimer.IsAgreed,
                DisclaimerId = disclaimer != null ? disclaimer.DisclaimerId : Guid.Empty
            };
        }
    }
}