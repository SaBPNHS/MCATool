using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Tests.Helpers;
using System;
using System.Data.Entity;
using System.Linq;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class UpdateAssessmentCompleteCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateAssessmentCompleteCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateAssessmentCompleteCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_UpdateAssessmentCompleteCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentCommand_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            var assessmentDate = new DateTime(2015, 1, 1);

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> { new Assessment() { AssessmentId = assessmentId } };

            var command = new UpdateAssessmentCompleteCommand()
            {
                AssessmentId = assessmentId,
                DateAssessmentEnded = assessmentDate
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            var assessment = set.First(x => x.AssessmentId == assessmentId);
            assessment.DateAssessmentEnded.Should().Be(assessmentDate);
        }
    }
}
