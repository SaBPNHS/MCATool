using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class FeedBackBuilder : IFeedBackBuilder
    {
        private readonly IConfigurableEmailLinkProvider _configurableEmailLinkProvider;

        public FeedBackBuilder(IConfigurableEmailLinkProvider configurableEmailLinkProvider)
        {
            _configurableEmailLinkProvider = configurableEmailLinkProvider;
        }

        public FeedBackViewModel CreateFeedBackViewModel()
        {
            return new FeedBackViewModel()
            {
                MailTo = _configurableEmailLinkProvider.GetEmailAddress()
            };
        }
    }
}