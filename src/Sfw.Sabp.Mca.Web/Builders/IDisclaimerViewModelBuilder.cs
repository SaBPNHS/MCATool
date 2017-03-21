using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IDisclaimerViewModelBuilder
    {
        AddUpdateDisclaimerCommand BuildAddDisclaimerCommand(DisclaimerViewModel disclaimer);
        DisclaimerViewModel BuildDisclaimerViewModel(Disclaimer disclaimer);
    }
}