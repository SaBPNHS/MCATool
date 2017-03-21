using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class CopyrightViewModelBuilder : ICopyrightViewModelBuilder
    {
        private readonly IFileVersionInfoProvider _fileVersionInfoProvider;

        public CopyrightViewModelBuilder(IFileVersionInfoProvider fileVersionInfoProvider)
        {
            _fileVersionInfoProvider = fileVersionInfoProvider;
        }

        public CopyrightViewModel CreateCopyrightViewModel()
        {
            return new CopyrightViewModel()
            {
                Copyright = _fileVersionInfoProvider.GetCopyright(typeof(HomeController))
            };
        }
    }
}