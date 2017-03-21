using System;
using System.Data.Entity;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Tests.Helpers;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class UpdateAssessmentReadOnlyCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateAssessmentReadOnlyCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateAssessmentReadOnlyCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentReadOnlyCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentReadOnlyCommand_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            const bool readOnly = true;

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> {new Assessment() {AssessmentId = assessmentId}};

            var command = new UpdateAssessmentReadOnlyCommand()
            {
                AssessmentId = assessmentId,
                ReadOnly = readOnly
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            set.First(x => x.AssessmentId == assessmentId).ReadOnly.Should().Be(readOnly);
        }
    }
}
