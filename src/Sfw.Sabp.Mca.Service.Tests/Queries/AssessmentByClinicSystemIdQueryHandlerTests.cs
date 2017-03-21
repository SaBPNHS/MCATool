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
    public class AssessmentByClinicSystemIdQueryHandlerTests
    {
        private AssessmentsByPatientIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new AssessmentsByPatientIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByPatientIdIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<AssessmentsByPatientIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByPatientIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var patientId = Guid.NewGuid();

            var set = new TestDbSet<Assessment> { new Assessment() { Patient = new Patient() { PatientId = patientId} } };

            A.CallTo(() => _fakeContext.Set<Assessment>()).Returns(set);

            var query = new AssessmentsByPatientIdQuery() { PatientId = patientId };

            var assessment = _queryHandler.Retrieve(query);

            assessment.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByPatientIdQueryAndPatientDoesNotExist_ContextShouldBeQueriedAndAssessmentItemsEmpty()
        {
            var patientId = Guid.NewGuid();

            var set = new TestDbSet<Assessment> { new Assessment() { Patient = new Patient()} };

            A.CallTo(() => _fakeContext.Set<Assessment>()).Returns(set);

            var query = new AssessmentsByPatientIdQuery() { PatientId = patientId };

            var assessment = _queryHandler.Retrieve(query);

            assessment.Items.Should().HaveCount(0);
        }

    }
}
