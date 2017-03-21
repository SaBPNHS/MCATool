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
    public class UpdateQuestionAnswerCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private UpdateQuestionAnswerCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new UpdateQuestionAnswerCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenUpdateQuestionAnswerCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenUpdateQuestionAnswerCommand_QuestionAnswerShouldBeUpdatedInContext()
        {
            var questionAnswerId = Guid.NewGuid();
            const string furtherInformation = "further";

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() { QuestionAnswerId = questionAnswerId } };

            var command = new UpdateQuestionAnswerCommand()
            {
                QuestionAnswerId = questionAnswerId,
                FurtherInformation = furtherInformation
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Execute(command);

            var questionAnswer = set.First(x => x.QuestionAnswerId == questionAnswerId);
            questionAnswer.FurtherInformation.Should().Be(furtherInformation);
        }
    }

}
