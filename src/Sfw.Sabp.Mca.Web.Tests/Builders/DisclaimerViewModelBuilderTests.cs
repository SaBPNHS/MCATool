using FakeItEasy;
using FluentAssertions;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class DisclaimerViewModelBuilderTests
    {
        private DisclaimerViewModelBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            A.Fake<IUserPrincipalProvider>();
            _builder = new DisclaimerViewModelBuilder();
        }

        [TestMethod]
        public void BuildAddDisclaimerCommand_GivenValidDisclaimerViewModel_AddDisclaimerCommand()
        {
            var disclaimerId = Guid.NewGuid();
            const bool isAgreed = true;

            var disclaimer = new DisclaimerViewModel()
            {
                DisclaimerId = disclaimerId,
                IsAgreed = isAgreed
            };

            var result = _builder.BuildAddDisclaimerCommand(disclaimer);
            result.DisclaimerId.Should().NotBeEmpty();
            result.IsAgreed.ShouldBeEquivalentTo(true);
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenValidDisclaimer_DisclaimerViewModelShouldBeReturned()
        {
            var result = _builder.BuildDisclaimerViewModel(A.Dummy<Disclaimer>());

            result.Should().BeOfType<DisclaimerViewModel>();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimerIsNull_TermsAndConditionsPropertyShouldBeTrue()
        {
            var result = _builder.BuildDisclaimerViewModel(null);

            result.TermsAndConditionsEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimerHasBeenAgreed_TermsAndConditionsPropertyShouldBeFalse()
        {
            var disclaimer = new Disclaimer()
            {
                IsAgreed = true
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.TermsAndConditionsEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimerIsNotNullAndHasNotBeenAgreed_TermsAndConditionsPropertyShouldBeTrue()
        {
            var disclaimer = new Disclaimer()
            {
                IsAgreed = false
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.TermsAndConditionsEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimerIsAgreed_IsAgreedPropertyShouldBeMapped()
        {
            var disclaimer = new Disclaimer()
            {
                IsAgreed = true
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.IsAgreed.Should().BeTrue();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimerIsNotAgreed_IsAgreedPropertyShouldBeMapped()
        {
            var disclaimer = new Disclaimer()
            {
                IsAgreed = false
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.IsAgreed.Should().BeFalse();
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimer_DisclaimerIdShouldBeMapped()
        {
            var disclaimerId = Guid.NewGuid();

            var disclaimer = new Disclaimer()
            {
                DisclaimerId = disclaimerId
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.DisclaimerId.Should().Be(disclaimerId);
        }

        [TestMethod]
        public void BuildDisclaimerViewModel_GivenDisclaimer_DisclaimerIdShouldBeSet()
        {
            var disclaimerId = Guid.NewGuid();

            var disclaimer = new Disclaimer()
            {
                DisclaimerId = disclaimerId
            };

            var result = _builder.BuildDisclaimerViewModel(disclaimer);

            result.DisclaimerId.Should().Be(disclaimerId);
        }

        [TestMethod]
        public void BuildAddDisclaimerCommand_GivenDecisionIdIsEmpty_DecisionIdShouldBeSet()
        {
            var disclaimer = new DisclaimerViewModel()
            {
                DisclaimerId = Guid.Empty
            };

            var result = _builder.BuildAddDisclaimerCommand(disclaimer);

            result.DisclaimerId.Should().NotBeEmpty();
        }

        [TestMethod]
        public void BuildAddDisclaimerCommand_GivenDecisionIsNotEmpty_DecisionIdShouldBeMapped()
        {
            var disclaimerId = Guid.NewGuid();

            var disclaimer = new DisclaimerViewModel()
            {
                DisclaimerId = disclaimerId
            };

            var result = _builder.BuildAddDisclaimerCommand(disclaimer);

            result.DisclaimerId.Should().Be(disclaimerId);
        }
    }
}
