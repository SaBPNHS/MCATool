using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Builders.Mappings;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;


namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class PatientViewModelBuilderTests
    {
        private PatientViewModelBuilder _builder;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private IUserRoleProvider _userRoleProvider;
        private IDateOfBirthBuilder _dateOfBirthBuilder;

        private const string Description = "description";

        [TestInitialize]
        public void Setup()
        {
            Mapper.AddProfile(new AutomapperMappingProfile());

            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();
            _userRoleProvider = A.Fake<IUserRoleProvider>();
            _dateOfBirthBuilder = A.Fake<IDateOfBirthBuilder>();

            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(Description);
            A.CallTo(() => _dateOfBirthBuilder.BuildDateOfBirthViewModel(null)).Returns(new DateOfBirthViewModel());

            _builder = new PatientViewModelBuilder(_clinicalSystemIdDescriptionProvider, _userRoleProvider, _dateOfBirthBuilder);
        }

        [TestMethod]
        public void BuildPatientViewModel_GivenNullModel_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildPatientViewModel(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildPatientViewModel_GivenGenders_CreatePatientViewModelShouldBeReturned()
        {
            var genders = new Genders()
            {
                Items = new List<Gender>()
            };

            var result = _builder.BuildPatientViewModel(genders);

            result.Should().BeOfType<CreatePatientViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildPatientViewModel_GivenGenders_GendersShouldBeMapped()
        {
            var genders = new Genders()
               {
                   Items = new List<Gender>()
                   {
                       new Gender() { Description = "description1", GenderId = 1},
                       new Gender() { Description = "description2", GenderId = 2}
                   }
               };

            var result = _builder.BuildPatientViewModel(genders);

            result.Genders.First().Value.Should().BeEmpty();
            result.Genders.First().Text.Should().Be("Select Gender");
            result.Genders.Count(x => x.Value == "1").Should().Be(1);
            result.Genders.First(x => x.Value == "1").Text.Should().Be("description1");
            result.Genders.Count(x => x.Value == "2").Should().Be(1);
            result.Genders.First(x => x.Value == "2").Text.Should().Be("description2");
        }

        [TestMethod]
        public void BuildPatientViewModel_GivenGenders_DateOfBirthShouldNotBeNull()
        {
            var genders = new Genders()
            {
                Items = new List<Gender> { new Gender() }
            };

            var dateOfBirth = new DateOfBirthViewModel();

            A.CallTo(() => _dateOfBirthBuilder.BuildDateOfBirthViewModel(A<DateTime?>._)).Returns(dateOfBirth);

            var result = _builder.BuildPatientViewModel(genders);

            A.CallTo(() => _dateOfBirthBuilder.BuildDateOfBirthViewModel(null)).MustHaveHappened(Repeated.Exactly.Once);
            result.DateOfBirthViewModel.Should().NotBeNull();
        }


        [TestMethod]
        public void BuildAddPatientCommand_GivenValidPatientViewModel_AddPatientCommand()
        {
            var guid = Guid.NewGuid();
            const string firstName = "David";
            const string lastName = "Miller";
            const int genderId = 1;
            const string clinicalSystemId = "PatientId";

            var patient = new PatientViewModel()
            {
                PatientId = guid,
                ClinicalSystemId = clinicalSystemId,
                NhsNumber = null,
                FirstName = firstName,
                LastName = lastName,
                GenderId = genderId,
                DateOfBirthViewModel = new DateOfBirthViewModel()
                {
                    Day = 1,
                    Month = 1,
                    Year = 2015
                }
            };

            var result = _builder.BuildAddPatientCommand(patient);

            result.PatientId.Should().NotBeEmpty();
            result.ClinicalSystemId.ShouldBeEquivalentTo(clinicalSystemId);
            result.FirstName.ShouldBeEquivalentTo(firstName);
            result.LastName.ShouldBeEquivalentTo(lastName);
            result.GenderId.ShouldBeEquivalentTo(genderId);
            result.NhsNumber.ShouldBeEquivalentTo(null);
        }

        [TestMethod]
        public void BuildAddPatientCommand_GivenPatientDateOfBirth_DateOfBirthPropertyShouldBeSet()
        {
            var patient = new PatientViewModel()
            {
                DateOfBirthViewModel = new DateOfBirthViewModel()
                {
                    Day = 1,
                    Month = 1,
                    Year = 2015
                }
            };

            var result = _builder.BuildAddPatientCommand(patient);

            result.DateOfBirth.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
        }

        [TestMethod]
        public void BuildAddPatientCommand_GivenPatientDateOfBirthIsNull_ArgumentNullExceptionExpected()
        {
            var patient = new PatientViewModel()
            {
                DateOfBirthViewModel = new DateOfBirthViewModel()
            };

            _builder.Invoking(x => x.BuildAddPatientCommand(patient)).ShouldThrow<ArgumentNullException>();            
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenNullModel_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildPatientSearchViewModel(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenValidPatientsModel_ViewModelShouldBeReturned()
        {
            var result = _builder.BuildPatientSearchViewModel(new Patients()
            {
                Items = new List<Patient>()
            });

            result.Should().BeOfType<PatientSearchViewModel>();
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenValidPatientsModel_PatientsShouldBeMapped()
        {
            var id = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var patients = new Patients()
            {
                Items = new List<Patient>()
                {
                    new Patient()
                    {
                        PatientId = id,
                        ClinicalSystemId = "1",
                        DateOfBirth = new DateTime(2016, 1, 1),
                        FirstName = "first",
                        GenderId = 1,
                        LastName = "last",
                        NhsNumber = 999
                    },
                    new Patient()
                    {
                        PatientId = id2,
                        ClinicalSystemId = "2",
                        DateOfBirth = new DateTime(2017, 1, 1),
                        FirstName = "first2",
                        GenderId = 1,
                        LastName = "last2",
                        NhsNumber = 9991
                    }
                }
            };

            var result = _builder.BuildPatientSearchViewModel(patients);

            result.Items.First(x => x.PatientId == id).ClinicalSystemId.Should().Be("1");
            result.Items.First(x => x.PatientId == id).DateOfBirthViewModel.Date.ShouldBeEquivalentTo(new DateTime(2016, 1, 1));
            result.Items.First(x => x.PatientId == id).FirstName.Should().Be("first");
            result.Items.First(x => x.PatientId == id).GenderId.Should().Be(1);
            result.Items.First(x => x.PatientId == id).LastName.Should().Be("last");
            result.Items.First(x => x.PatientId == id).NhsNumber.Should().Be(999);
            result.Items.First(x => x.PatientId == id2).ClinicalSystemId.Should().Be("2");
            result.Items.First(x => x.PatientId == id2).DateOfBirthViewModel.Date.ShouldBeEquivalentTo(new DateTime(2017, 1, 1));
            result.Items.First(x => x.PatientId == id2).FirstName.Should().Be("first2");
            result.Items.First(x => x.PatientId == id2).GenderId.Should().Be(1);
            result.Items.First(x => x.PatientId == id2).LastName.Should().Be("last2");
            result.Items.First(x => x.PatientId == id2).NhsNumber.Should().Be(9991);
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenClinicalSystemIdDescription_ClinicalSystemIdDescriptionPropertyShouldBeSet()
        {
            var patients = new Patients()
            {
                Items = new List<Patient>()
            };

            var result = _builder.BuildPatientSearchViewModel(patients);

            result.ClinicalIdDescription.Should().Be(Description);
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenUserIsNotAnAdministrator_CanEditShouldBeFalse()
        {
            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);

            var patients = new Patients()
            {
                Items = new List<Patient>()
            };

            var result = _builder.BuildPatientSearchViewModel(patients);

            result.CanEdit.Should().BeFalse();
        }

        [TestMethod]
        public void BuildPatientSearchViewModel_GivenUserIsAnAdministrator_CanEditShouldBeTrue()
        {
            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);

            var patients = new Patients()
            {
                Items = new List<Patient>()
            };

            var result = _builder.BuildPatientSearchViewModel(patients);

            result.CanEdit.Should().BeTrue();
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenPatientIsNull_ArgumentNullExeptionExpected()
        {
            _builder.Invoking(x => x.BuildEditPatientViewModel(null, new Genders()))
                .ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenGendersListIsEmpty_ArgumentExeptionExpected()
        {
            _builder.Invoking(x => x.BuildEditPatientViewModel(new Patient(), new Genders()))
                .ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenValidValues_EditPatientViewModelShouldBeReturned()
        {
            var genders = new Genders()
            {
                Items = new List<Gender> { new Gender() }
            };

            var result = _builder.BuildEditPatientViewModel(A.Dummy<Patient>(), genders);

            result.Should().BeOfType<EditPatientViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenValidGenders_GendersShouldBeMapped()
        {
            var genders = new Genders()
            {
                Items = new List<Gender>()
                {
                    new Gender() { Description = "description1", GenderId = 1},
                    new Gender() { Description = "description2", GenderId = 2}
                }
            };

            var result = _builder.BuildEditPatientViewModel(new Patient(), genders);

            result.Genders.First().Value.Should().BeEmpty();
            result.Genders.First().Text.Should().Be("Select Gender");
            result.Genders.Count(x => x.Value == "1").Should().Be(1);
            result.Genders.First(x => x.Value == "1").Text.Should().Be("description1");
            result.Genders.Count(x => x.Value == "2").Should().Be(1);
            result.Genders.First(x => x.Value == "2").Text.Should().Be("description2");
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenValidPatient_PatientPropertiesShouldBeMapped()
        {
            var genders = new Genders()
            {
                Items = new List<Gender> { new Gender() }
            };

            var patientId = Guid.NewGuid();
            var patient = new Patient()
            {
                PatientId = patientId,
                ClinicalSystemId = "clinicalsystemId",
                FirstName = "firstname",
                LastName = "lastname",
                GenderId = 1,
                NhsNumber = 123456
            };

            var result = _builder.BuildEditPatientViewModel(patient, genders);

            result.PatientId.Should().Be(patientId);
            result.ClinicalSystemId.Should().Be("clinicalsystemId");
            result.FirstName.Should().Be("firstname");
            result.LastName.Should().Be("lastname");
            result.GenderId.Should().Be(1);
            result.NhsNumber.Should().Be(123456);
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenPatientDateOfBirth_DateOfBirthBuilderShouldBeCalled()
        {
            var genders = new Genders()
            {
                Items = new List<Gender> { new Gender() }
            };

            var dateOfBirth = new DateOfBirthViewModel();

            A.CallTo(() => _dateOfBirthBuilder.BuildDateOfBirthViewModel(A<DateTime>._)).Returns(dateOfBirth);

            var result = _builder.BuildEditPatientViewModel(new Patient(), genders);

            A.CallTo(() => _dateOfBirthBuilder.BuildDateOfBirthViewModel(A<DateTime>._)).MustHaveHappened(Repeated.Exactly.Once);
            result.DateOfBirthViewModel.Should().Be(dateOfBirth);
        }

        [TestMethod]
        public void BuildUpdatePatientCommand_GivenEditPatientViewModelIsNull_ArgumentNullExceptionExpected()
        {
            _builder.Invoking(x => x.BuildUpdatePatientCommand(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildUpdatePatientCommand_GivenEditPatientViewModel_UpdatePatientCommandShouldBeReturned()
        {
            var result = _builder.BuildUpdatePatientCommand(A.Dummy<EditPatientViewModel>());

            result.Should().BeOfType<AddUpdatePatientCommand>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildUpdatePatientCommand_GivenValidEditPatientViewModel_CommandPropertiesShouldBeMapped()
        {
            var patientId = Guid.NewGuid();

            var viewModel = new EditPatientViewModel()
            {
                PatientId = patientId,
                ClinicalSystemId = "clinicalsystemid",
                NhsNumber = 123456789,
                FirstName = "firstname",
                LastName = "lastname",
                DateOfBirthViewModel = new DateOfBirthViewModel()
                {
                    Year = 2015,
                    Month = 1,
                    Day = 1
                },
                GenderId = 1
            };

            var result = _builder.BuildUpdatePatientCommand(viewModel);

            result.PatientId.Should().Be(patientId);
            result.ClinicalSystemId.Should().Be("clinicalsystemid");
            result.NhsNumber.Should().Be(123456789);
            result.FirstName.Should().Be("firstname");
            result.LastName.Should().Be("lastname");
            result.DateOfBirth.Should().Be(new DateTime(2015, 1, 1));
            result.GenderId.Should().Be(1);
        }

        [TestMethod]
        public void BuildEditPatientViewModel_GivenValidPatient_OldPatientPropertiesShouldBeMapped()
        {
            var dob = new DateTime(2015, 1, 1);

            var genders = new Genders()
            {
                Items = new List<Gender> { new Gender() }
            };

            var patient = new Patient()
            {
                ClinicalSystemId = "clinicalsystemId",
                NhsNumber = 123456,
                FirstName = "firstname",
                LastName = "lastname",
                DateOfBirth = dob,
                GenderId = 1
            };

            var result = _builder.BuildEditPatientViewModel(patient, genders);

            result.CurrentClinicalSystemId.Should().Be("clinicalsystemId");
            result.CurrentNhsNumber.Should().Be(123456);
            result.CurrentFirstName.Should().Be("firstname");
            result.CurrentLastName.Should().Be("lastname");
            result.CurrentDateOfBirth.Should().Be(dob.ToShortDateString());
            result.CurrentGenderId.Should().Be(1);
        }
    }
}
