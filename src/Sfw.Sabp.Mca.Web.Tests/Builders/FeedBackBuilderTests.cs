using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class FeedBackBuilderTests
    {
        private FeedBackBuilder _feedBackBuilder;
        private IConfigurableEmailLinkProvider _configurableEmailLinkProvider;
        private const string Mailto = "mailTo";

        [TestInitialize]
        public void Setup()
        {
            _configurableEmailLinkProvider = A.Fake<IConfigurableEmailLinkProvider>();

            A.CallTo(() => _configurableEmailLinkProvider.GetEmailAddress()).Returns(Mailto);

            _feedBackBuilder = new FeedBackBuilder(_configurableEmailLinkProvider);
        }

        [TestMethod]
        public void CreateFeedBackViewModel_ShouldReturnFeedBackViewModel()
        {
            var result = _feedBackBuilder.CreateFeedBackViewModel();

            result.Should().BeOfType<FeedBackViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateFeedBackViewModel_GivenFeedBackLink_MailToPropertyShouldBeSet()
        {
            var result = _feedBackBuilder.CreateFeedBackViewModel();

            result.MailTo.Should().Be(Mailto);
        }
    }
}
