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
    public class PatientByIdQueryHandlerTests
    {
        private PatientByIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new PatientByIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenPatientByIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<PatientByIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenPatientByIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var patientId = Guid.NewGuid();

            var set = new TestDbSet<Patient> { new Patient() { PatientId = Guid.NewGuid() }, new Patient() { PatientId = patientId } };

            A.CallTo(() => _fakeContext.Set<Patient>()).Returns(set);

            var query = new PatientByIdQuery() { PatientId = patientId };

            var patient = _queryHandler.Retrieve(query);

            patient.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenPatientByIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var set = new TestDbSet<Patient> { new Patient() };

            A.CallTo(() => _fakeContext.Set<Patient>()).Returns(set);

            var query = new PatientByIdQuery() { PatientId = Guid.NewGuid() };

            var assessment = _queryHandler.Retrieve(query);

            assessment.Should().BeNull();
        }
    }
}
