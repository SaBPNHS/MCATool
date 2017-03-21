using System;
using System.Data.Entity;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Service.Tests.Helpers;

namespace Sfw.Sabp.Mca.Service.Tests.Queries
{
    [TestClass]
    public class QuestionAnswerByIdQueryHandlerTests
    {
        private QuestionAnswerByIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new QuestionAnswerByIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenQuestionAnswerByIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<QuestionAnswerByIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenQuestionAnswerByIdQuery_ContextShouldBeQueriedAndQuestionAnswerReturned()
        {
            var questionAnswerId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() { QuestionAnswerId = questionAnswerId } };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswerByIdQuery() { QuestionAnswerId = questionAnswerId };

            var questionAnswer = _queryHandler.Retrieve(query);

            questionAnswer.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenQuestionAnswerByIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var questionAnswerId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswerByIdQuery() { QuestionAnswerId = questionAnswerId};

            var questionAnswer = _queryHandler.Retrieve(query);

            questionAnswer.Should().BeNull();
        }
    }
}
