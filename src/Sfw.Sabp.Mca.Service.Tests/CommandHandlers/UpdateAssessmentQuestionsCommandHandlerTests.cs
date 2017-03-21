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
    public class UpdateAssessmentQuestionsCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateAssessmentQuestionsCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateAssessmentQuestionsCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentQuestionsCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateAssessmentQuestionsCommand_AssessmentShouldBeUpdatedInContext()
        {
            var assessmentId = Guid.NewGuid();
            var nextQuestion = Guid.NewGuid();
            var previousQuestionId = Guid.NewGuid();
            var resetQuestionId = Guid.NewGuid();

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Assessment> {new Assessment() {AssessmentId = assessmentId}};

            var command = new UpdateAssessmentQuestionsCommand()
            {
                AssessmentId = assessmentId,
                NextQuestionId = nextQuestion,
                PreviousQuestionId = previousQuestionId,
                ResetWorkflowQuestionId = resetQuestionId
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);

            _handler.Execute(command);

            var assessment = set.First(x => x.AssessmentId == assessmentId);
            assessment.CurrentWorkflowQuestionId.Should().Be(nextQuestion);
            assessment.PreviousWorkflowQuestionId.Should().Be(previousQuestionId);
            assessment.ResetWorkflowQuestionId.Should().Be(resetQuestionId);
        }
    }
}
