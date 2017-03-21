using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Helpers
{
    [TestClass]
    public class PatientHelperTests
    {
        private IQueryDispatcher _queryDispatcher;
        private PatientHelper _patientHelper;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            _patientHelper = new PatientHelper(_queryDispatcher);
        }

        [TestMethod]
        public void GetPatient_PatientShouldBeReturned()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(A<PatientByIdQuery>._)).Returns(new Patient());

            var result = _patientHelper.GetPatient(A<Guid>._);

            result.Should().BeOfType<Patient>();
        }

        [TestMethod]
        public void GetPatient_GivePatientId_QueryDispatcherShouldBeCalled()
        {
            var patientId = Guid.NewGuid();

            _patientHelper.GetPatient(patientId);

            A.CallTo(() => _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(A<PatientByIdQuery>.That.Matches(x => x.PatientId == patientId))).MustHaveHappened();
        }
    }
}
