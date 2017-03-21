using System;
using System.Data.Entity;
using System.Linq;
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
    public class AssessmentQuestionsAnswersByAssessmentIdQueryHandlerTests
    {
        private QuestionAnswersByAssessmentIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new QuestionAnswersByAssessmentIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_CalledWithEmptyQuery_ShouldThrowArgumentNullException()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<QuestionAnswersByAssessmentQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_CalledWithValidQuery_QuestionAnswersShouldBeReturned()
        {
            var patientId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();
            var questionAnswerId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer>
            {
                new QuestionAnswer() {QuestionAnswerId = questionAnswerId, Assessment = new Assessment() {AssessmentId = assessmentId, Patient = new Patient() { PatientId = patientId}}}
            };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswersByAssessmentQuery() { Assessment  = new Assessment()};

            var questionAnswer = _queryHandler.Retrieve(query);

            questionAnswer.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_CalledWithValidQuery_QuestionAnswersShouldBeReturnedInCreatedOrder()
        {
            var questionAnswer1Id = Guid.NewGuid();
            var questionAnswer2Id = Guid.NewGuid();
            var questionAnswer3Id = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer>
            {
                new QuestionAnswer() {QuestionAnswerId = questionAnswer1Id, Created = new DateTime(2015, 1, 1)},
                new QuestionAnswer() {QuestionAnswerId = questionAnswer3Id, Created = new DateTime(2017, 1, 1)},
                new QuestionAnswer() {QuestionAnswerId = questionAnswer2Id, Created = new DateTime(2014, 1, 1)}
            };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswersByAssessmentQuery() { Assessment = new Assessment() };

            var questionAnswers = _queryHandler.Retrieve(query);

            questionAnswers.Items.ElementAt(0).QuestionAnswerId.Should().Be(questionAnswer2Id);
            questionAnswers.Items.ElementAt(1).QuestionAnswerId.Should().Be(questionAnswer1Id);
            questionAnswers.Items.ElementAt(2).QuestionAnswerId.Should().Be(questionAnswer3Id);
        }
    }
}
