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
    public class AddUpdatePatientCommandHandlerTests
    {

        private IUnitOfWork _unitOfWork;
        private AddUpdatePatientCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();


            _handler = new AddUpdatePatientCommandHandler(_unitOfWork);
        }

        [TestMethod]
        public void Execute_GivenAddPatientCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x => x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenAddUpdatePatientCommandAdd_PatientShouldBeAddedToContext()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Patient>();

            var patientId = Guid.NewGuid();

            var patientCommand = new AddUpdatePatientCommand()
            {
                PatientId = patientId,
                ClinicalSystemId = "1",
                DateOfBirth = new DateTime(1965, 01, 01),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 123
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Patient>()).Returns(set);

            _handler.Execute(patientCommand);

            set.Count(x => x.PatientId == patientId).Should().Be(1);

            var patient = set.First(x => x.PatientId == patientId);
            patient.PatientId.Should().Be(patientId);
            patient.ClinicalSystemId.Should().Be("1");
            patient.DateOfBirth.ShouldBeEquivalentTo(new DateTime(1965, 01, 01));
            patient.FirstName.Should().Be("David");
            patient.LastName.Should().Be("Miller");
            patient.GenderId.Should().Be(1);
            patient.NhsNumber.Should().Be(123);
        }

        [TestMethod]
        public void Execute_GivenUpdatePatientCommandUpdate_PatientShouldBeUpdatedInContext()
        {
            var patientId = Guid.NewGuid();

            var fakeContext = A.Fake<DbContext>();
            var set = new TestDbSet<Patient> { new Patient() { PatientId = patientId } };

            var command = new AddUpdatePatientCommand()
            {
                PatientId = patientId,
                ClinicalSystemId = "clinicalsystemid",
                NhsNumber = 123456789,
                FirstName = "firstname",
                LastName = "lastname",
                DateOfBirth = new DateTime(2015, 1, 1),
                GenderId = 1
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Patient>()).Returns(set);

            _handler.Execute(command);

            var patient = set.First(x => x.PatientId == patientId);
            patient.ClinicalSystemId.Should().Be("clinicalsystemid");
            patient.NhsNumber.Should().Be(123456789);
            patient.FirstName.Should().Be("firstname");
            patient.LastName.Should().Be("lastname");
            patient.DateOfBirth.Should().Be(new DateTime(2015, 1, 1));
            patient.GenderId.Should().Be(1);
        }
    }
}
