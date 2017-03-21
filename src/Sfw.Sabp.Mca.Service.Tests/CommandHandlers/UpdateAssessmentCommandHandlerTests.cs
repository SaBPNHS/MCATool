using System;
using System.Data.Entity;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Tests.Helpers;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class UpdateAssessmentCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateAssessmentCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateAssessmentCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentCommand_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            var assessmentDate = new DateTime(2015, 1, 1);
            const string decision = "decision";
            var roleid = (int)RoleIdEnum.DecisionMaker;

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> { new Assessment() { AssessmentId = assessmentId, RoleId = (int)RoleIdEnum.Assessor, DecisionMaker = "decisionmakername" } };

            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = assessmentId,
                DateAssessmentStarted = assessmentDate,
                Stage1DecisionToBeMade = decision,
                RoleId = roleid
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            var assessment = set.First(x => x.AssessmentId == assessmentId);
            assessment.DateAssessmentStarted.Should().Be(assessmentDate);
            assessment.Stage1DecisionToBeMade.Should().Be(decision);
            assessment.RoleId.Should().Be(roleid);
            assessment.DecisionMaker.Should().BeNull();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentCommandRoleChanged_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            var assessmentDate = new DateTime(2015, 1, 1);
            const string decision = "decision";
            var roleid = (int)RoleIdEnum.Assessor;
            var decisionMaker = "Decision Maker Name";

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> { new Assessment() { AssessmentId = assessmentId, RoleId = (int)RoleIdEnum.DecisionMaker } };

            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = assessmentId,
                DateAssessmentStarted = assessmentDate,
                Stage1DecisionToBeMade = decision,
                RoleId = roleid,
                DecisionMaker = decisionMaker
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            var assessment = set.First(x => x.AssessmentId == assessmentId);
            assessment.DateAssessmentStarted.Should().Be(assessmentDate);
            assessment.Stage1DecisionToBeMade.Should().Be(decision);
            assessment.RoleId.Should().Be(roleid);
            assessment.DecisionMaker.Should().Be(decisionMaker);
        }
    }

}
