using System;
using System.Data.Entity;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Tests.Helpers;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class AddUpdateDisclaimerCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private IUserPrincipalProvider _userPrincipalProvider;        
        private AddUpdateDisclaimerCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();            

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user");
            _handler = new AddUpdateDisclaimerCommandHandler(_unitOfWork, _userPrincipalProvider);
        }

        [TestMethod]
        public void Execute_GivenAddUpdateDisclaimerCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenAddUpdateDisclaimerCommandAdd_DisclaimerShouldBeAddedToContext()
        {
            var disclaimerId = Guid.NewGuid();

            var command = new AddUpdateDisclaimerCommand()
            {
                DisclaimerId = disclaimerId,
                IsAgreed = true
            };

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Disclaimer>();

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user");
            A.CallTo(() => fakeContext.Set<Disclaimer>()).Returns(set);
            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);

            _handler.Execute(command);

            var disclaimer = set.First(x => x.DisclaimerId == disclaimerId);
            disclaimer.AssessorDomainName.Should().Be("user");
            disclaimer.IsAgreed.Should().BeTrue();
        }

        [TestMethod]
        public void Execute_GivenAddUpdateDisclaimerCommandUpdate_DisclaimerShouldBeUpdatedInContext()
        {
            var disclaimerId = Guid.NewGuid();

            var command = new AddUpdateDisclaimerCommand()
            {
                DisclaimerId = disclaimerId,
                IsAgreed = true
            };

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Disclaimer>() { new Disclaimer(){ DisclaimerId = disclaimerId} };

            A.CallTo(() => fakeContext.Set<Disclaimer>()).Returns(set);
            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);

            _handler.Execute(command);

            set.Count(x => x.DisclaimerId == disclaimerId).Should().Be(1);

            var disclaimer = set.First(x => x.DisclaimerId == disclaimerId);
            disclaimer.IsAgreed.Should().BeTrue();
        }
    }
}
