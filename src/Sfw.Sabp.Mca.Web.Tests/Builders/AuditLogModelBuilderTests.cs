using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class AuditLogModelBuilderTests
    {
        private AuditLogModelBuilder _builder;
        [TestInitialize]
        public void Setup()
        {
            _builder = new AuditLogModelBuilder();
        }

        [TestMethod]
        public void BuildAddAuditLogCommand_GivenValidAuditLogViewModel_AddAuditLogCommand()
        {
            var auditLogId = Guid.NewGuid();
            const string action = "Index";
            const string auditData = "1";
            var eventDateTime = new DateTime(2015, 05, 05);
            const string controller = "Person";
            const string user = "Miller";

            var auditLog = new AuditLogModel()
            {
                AuditLogId = auditLogId,
                Action = action,
                AuditData = auditData,
                EventDateTime = eventDateTime,
                Controller = controller,
                User = user        
            };

            var result = _builder.BuildAddAuditLogCommand(auditLog);

            result.AuditLogId.Should().NotBeEmpty();
            result.Action.ShouldBeEquivalentTo(action);
            result.AuditData.ShouldAllBeEquivalentTo(auditData);
            result.Controller.ShouldBeEquivalentTo(controller);
            result.EventDateTime.ShouldBeEquivalentTo(eventDateTime);
            result.User.ShouldBeEquivalentTo(user);            
        }
    }
}
