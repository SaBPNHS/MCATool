using System;
using System.Data.Entity;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Infrastructure.Providers;


namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class AddAuditLogCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private AddAuditLogCommandHandler _handler;
        private IUserPrincipalProvider _userPrincipalProvider;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _handler = new AddAuditLogCommandHandler(_unitOfWork);
            
        }

        [TestMethod]
        public void Execute_GivenAddAuditLogCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenAddPatientCommand_PatientShouldBeAddedToContext()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<AuditLog>>();
            var auditLogId = Guid.NewGuid();

            var auditLogCommand = new AddAuditLogCommand()
            {
                AuditLogId = auditLogId,
                Action= "Index",
                AuditData="1",
                EventDateTime = new DateTime(2015, 05, 05),
                Controller="Person",
                User= "Miller"                
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<AuditLog>()).Returns(set);

            _handler.Execute(auditLogCommand);

            A.CallTo(() => fakeContext.Set<AuditLog>().Add(A<AuditLog>.That.Matches(
                x => x.AuditLogId == auditLogId
                && x.EventDateTime.Equals(new DateTime(2015,05,05))
                && x.Action== "Index"
                && x.User == "Miller"
                && x.Controller == "Person"
                && x.AuditData == "1"))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
