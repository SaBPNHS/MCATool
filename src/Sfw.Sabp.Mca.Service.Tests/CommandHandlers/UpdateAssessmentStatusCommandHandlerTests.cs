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
    public class UpdateAssessmentStatusCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateAssessmentStatusCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateAssessmentStatusCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentStatusCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentStatusCommand_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            const int statusId = (int)AssessmentStatusEnum.Complete;

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> {new Assessment() {AssessmentId = assessmentId}};

            var command = new UpdateAssessmentStatusCommand()
            {
                AssessmentId = assessmentId,
                StatusId = statusId
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            set.First(x => x.AssessmentId == assessmentId).StatusId.Should().Be(statusId);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentStatusCommandBreak_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            const int statusId = (int)AssessmentStatusEnum.Break;

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> {new Assessment() {AssessmentId = assessmentId}};

            var command = new UpdateAssessmentStatusCommand()
            {
                AssessmentId = assessmentId,
                StatusId = statusId
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            set.First(x => x.AssessmentId == assessmentId).StatusId.Should().Be(statusId);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentStatusCommandReadyToComplete_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            const int statusId = (int)AssessmentStatusEnum.ReadyToComplete;

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> { new Assessment() { AssessmentId = assessmentId } };

            var command = new UpdateAssessmentStatusCommand()
            {
                AssessmentId = assessmentId,
                StatusId = statusId
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            set.First(x => x.AssessmentId == assessmentId).StatusId.Should().Be(statusId);
        }
    }

}
