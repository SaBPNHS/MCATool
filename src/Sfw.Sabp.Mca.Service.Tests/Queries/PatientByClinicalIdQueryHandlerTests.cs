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
    public class PatientByClinicalIdQueryHandlerTests
    {
        private PatientByClinicalIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new PatientByClinicalIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenPatientByClinicalIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<PatientByClinicalIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenPatientByClinicalIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            const string systemId = "systemId";

            var set = new TestDbSet<Patient> { new Patient() { ClinicalSystemId = systemId }, new Patient() { ClinicalSystemId = "systemid1" } };

            A.CallTo(() => _fakeContext.Set<Patient>()).Returns(set);

            var query = new PatientByClinicalIdQuery() { ClinicalId = systemId };

            var patient = _queryHandler.Retrieve(query);

            patient.Items.Where(x => x.ClinicalSystemId == systemId).Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivePatientByClinicalIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var set = new TestDbSet<Patient> { new Patient() };

            A.CallTo(() => _fakeContext.Set<Patient>()).Returns(set);

            var query = new PatientByClinicalIdQuery() { ClinicalId = "id" };

            var patient = _queryHandler.Retrieve(query);

            patient.Items.Should().HaveCount(0);
        }
    }
}
