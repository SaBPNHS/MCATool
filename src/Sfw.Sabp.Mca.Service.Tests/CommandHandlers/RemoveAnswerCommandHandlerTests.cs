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
    public class RemoveAnswerCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private RemoveAnswerCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _handler = new RemoveAnswerCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenRemoveAnswerCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x =>  x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenRemoveAnswerCommandAndAnswerExists_AnswerShouldBeRemovedFromContext()
        {
            var questionAnswerId = Guid.NewGuid();

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<QuestionAnswer>{new QuestionAnswer() {QuestionAnswerId = questionAnswerId}};

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Execute(new RemoveAnswerCommand {QuestionAnswerId = questionAnswerId});

            fakeContext.Set<QuestionAnswer>().FirstOrDefault(x => x.QuestionAnswerId == questionAnswerId)
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void Execute_GivenRemoveAnswerCommandAndAnswerDoesNotExist_InvalidOperationExceptionExpected()
        {
            var questionAnswerId = Guid.NewGuid();

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer()};

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<QuestionAnswer>()).Returns(set);

            _handler.Invoking(x => x.Execute(new RemoveAnswerCommand { QuestionAnswerId = questionAnswerId })).ShouldThrow<InvalidOperationException>();
        }
    }
}
