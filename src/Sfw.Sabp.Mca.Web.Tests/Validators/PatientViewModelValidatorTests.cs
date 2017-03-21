using System;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class PatientViewModelValidatorTests
    {
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private IFutureDateValidator _futureDateValidator;
        private INhsValidator _nhsValidator;
        private const string Clinicalsystemid = "clinicalSystemId";

        [TestInitialize]
        public void Setup()
        {
            _futureDateValidator = A.Fake<IFutureDateValidator>();
            _nhsValidator = A.Fake<INhsValidator>();
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();

            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(Clinicalsystemid);
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(true);
            A.CallTo(() => _nhsValidator.Valid(A<decimal?>._)).Returns(true);
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenClinicalSystemIdIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenFirstNameIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                LastName = "Miller",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenLastNameIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = new DateOfBirthViewModel(),
                FirstName = "David",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenNhsNumberIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenDateOfBirthIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                DateOfBirthViewModel = new DateOfBirthViewModel()
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenGenderIsNotProvided_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller"
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenInvalidNhsNumberIsProvided_ValidationShouldFail()
        {
            const long nhsNumber = 4567899898;

            A.CallTo(() => _nhsValidator.Valid(nhsNumber)).Returns(false);

            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = new DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = nhsNumber
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenValidNhsNumberProvided_ValidationShouldPass()
        {
            A.CallTo(() => _nhsValidator.Valid(9434765870)).Returns(true);

            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenDateIsInTheFuture_ValidationShouldFail()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(false);

            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = new DateOfBirthViewModel()
                {
                    Year = 2050,
                    Month = 1,
                    Day = 1
                },
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenFirstNameHasMoreThen50Charactors_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = new string('a', 51),
                LastName = "lastname",
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenLastNameHasMoreThen50Charactors_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "firstname",
                LastName = new string('a', 51),
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenClinicalSystemIdHasMoreThan50Characters_ValidationShouldFail()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = new string('a', 51),
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "firstname",
                LastName = "last name",
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenClinicalSystemIdHasNotBeEntered_ErrorMessageShouldBeClinicalSystemIdDescription()
        {
            var model = new PatientViewModel()
            {
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "firstname",
                LastName = "last name",
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.Errors.Should().Contain(x => x.PropertyName == "ClinicalSystemId" && x.ErrorMessage == string.Format("{0} is mandatory", Clinicalsystemid));
        }

        [TestMethod]
        public void PatientViewModelValidator_GivenClinicalSystemLengthIsInvalid_ErrorMessageShouldBeClinicalSystemIdDescription()
        {
            var model = new PatientViewModel()
            {
                ClinicalSystemId = new string('a', 51),
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "firstname",
                LastName = "last name",
                GenderId = 1,
                NhsNumber = 9434765870
            };

            var result = ValidationResult(model);

            result.Errors.Should().Contain(x => x.PropertyName == "ClinicalSystemId" && x.ErrorMessage == string.Format("{0} must be less than 50 characters", Clinicalsystemid));
        }

        #region private

        private ValidationResult ValidationResult(PatientViewModel model)
        {
            var validator = new PatientViewModelValidator<PatientViewModel>(_futureDateValidator, _nhsValidator, _clinicalSystemIdDescriptionProvider);
            var result = validator.Validate(model);
            return result;
        }

        private DateOfBirthViewModel DateOfBirthViewModel()
        {
            return new DateOfBirthViewModel() { Day = 1, Month = 1, Year = 2015 };
        }

        #endregion
    }
}
