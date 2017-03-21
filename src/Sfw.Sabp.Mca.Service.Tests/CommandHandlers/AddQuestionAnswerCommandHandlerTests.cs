using System;
using System.Data.Entity;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class AddQuestionAnswerCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private IDateTimeProvider _dateTimeProvider;

        private AddQuestionAnswerCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();

            _handler = new AddQuestionAnswerCommandHandler(_unitOfWork, _dateTimeProvider);
        }

        [TestMethod]
        public void Execute_GivenAddQuestionAnswerCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x =>  x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenAddQuestionAnswerCommand_AssessmentShouldBeAddedToContext()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<QuestionAnswer>>();
            var workflowQuestionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();
            var dateTime = new DateTime(2015, 1, 1);

            var questionAnswerCommand = AddQuestionAnswerCommand(assessmentId, workflowQuestionId, optionId, "info");

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);
            
            A.CallTo(() => _dateTimeProvider.Now).Returns(dateTime);

            _handler.Execute(questionAnswerCommand);

            A.CallTo(() => fakeContext.Set<QuestionAnswer>().Add(A<QuestionAnswer>.That.Matches(
                x => x.AssessmentId == assessmentId
                && x.WorkflowQuestionId == workflowQuestionId
                && x.QuestionOptionId == optionId
                && x.FurtherInformation == "info"
                && x.Created == dateTime))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Execute_GivenAddQuestionAnswerCommandAndFurtherInformationIsEmpty_FurtherInformationShouldBeNull()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<QuestionAnswer>>();
            var workflowQuestionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var questionAnswerCommand = AddQuestionAnswerCommand(assessmentId, workflowQuestionId, optionId, "");

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Execute(questionAnswerCommand);

            AssertNullFurtherInfomation(fakeContext);
        }

        [TestMethod]
        public void Execute_GivenAddQuestionAnswerCommandAndFurtherInformationIsWhitespace_FurtherInformationShouldBeNull()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<QuestionAnswer>>();
            var workflowQuestionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var questionAnswerCommand = AddQuestionAnswerCommand(assessmentId, workflowQuestionId, optionId, " ");

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Execute(questionAnswerCommand);

            AssertNullFurtherInfomation(fakeContext);
        }

        [TestMethod]
        public void Execute_GivenAddQuestionAnswerCommandAndFurtherInformationNull_FurtherInformationShouldBeNull()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<QuestionAnswer>>();
            var workflowQuestionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var questionAnswerCommand = AddQuestionAnswerCommand(assessmentId, workflowQuestionId, optionId, null);

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Execute(questionAnswerCommand);

            AssertNullFurtherInfomation(fakeContext);
        }

        #region private
        private AddQuestionAnswerCommand AddQuestionAnswerCommand(Guid assessmentId, Guid workflowQuestionId,
            Guid optionId, string furtherInformation)
        {
            var questionAnswerCommand = new AddQuestionAnswerCommand()
            {
                AssessmentId = assessmentId,
                WorkflowQuestionId = workflowQuestionId,
                QuestionOptionId = optionId,
                FurtherInformation = furtherInformation
            };
            return questionAnswerCommand;
        }

        private void AssertNullFurtherInfomation(DbContext fakeContext)
        {
            A.CallTo(() => fakeContext.Set<QuestionAnswer>().Add(A<QuestionAnswer>.That.Matches(
                x => x.FurtherInformation == null))).MustHaveHappened(Repeated.Exactly.Once);
        }

        #endregion
    }
}
