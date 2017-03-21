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
    public class EditPatientViewModelValidatorTests
    {
        private IFutureDateValidator _futureDateValidator;
        private IClinicalIdValidator _clinicalIdValidator;
        private INhsValidator _nhsValidator;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;

        [TestInitialize]
        public void Setup()
        {
            _futureDateValidator = A.Fake<IFutureDateValidator>();
            _clinicalIdValidator = A.Fake<IClinicalIdValidator>();
            _nhsValidator = A.Fake<INhsValidator>();
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();

            A.CallTo(() => _clinicalIdValidator.Unique(A<string>._)).Returns(true);
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(true);
            A.CallTo(() => _nhsValidator.Valid(A<decimal?>._)).Returns(true);
            A.CallTo(() => _nhsValidator.Unique(A<decimal?>._)).Returns(true);
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenClinicalSystemIdIsNotProvided_ValidationShouldFail()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenFirstNameIsNotProvided_ValidationShouldFail()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenLastNameIsNotProvided_ValidationShouldFail()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenNhsNumberIsNotProvided_ValidationShouldPass()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenDateOfBirthIsNotProvided_ValidationShouldFail()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenGenderIsNotProvided_ValidationShouldFail()
        {
            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenInvalidNhsNumberIsProvided_ValidationShouldFail()
        {
            const long nhsNumber = 4567899898;

            A.CallTo(() => _nhsValidator.Valid(nhsNumber)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = nhsNumber
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenNhsNumberIsProvided_ValidationShouldPass()
        {
            A.CallTo(() => _nhsValidator.Valid(A<decimal?>._)).Returns(true);
            A.CallTo(() => _nhsValidator.Unique(A<decimal?>._)).Returns(true);

            var model = new EditPatientViewModel()
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
        public void EditPatientViewModelValidator_GivenDateOfBirthDateIsInTheFuture_ValidationShouldFail()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881,
                DateOfBirthViewModel = new DateOfBirthViewModel()
                {
                    Year = 2050, 
                    Month = 1,
                    Day = 1
                }
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenClinicalIdHasNotBeenChanged_ValidationShouldPass()
        {
            const string clinicalSystemId = "PatientId";

            A.CallTo(() => _clinicalIdValidator.Unique(clinicalSystemId)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = clinicalSystemId,
                CurrentClinicalSystemId = clinicalSystemId,
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenClinicalIdHasBeenChangedAndIsDuplicate_ValidationShouldFail()
        {
            const string clinicalSystemId = "PatientId";
            const string oldClinicalSystemId = "OldPatientId";

            A.CallTo(() => _clinicalIdValidator.Unique(clinicalSystemId)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = clinicalSystemId,
                CurrentClinicalSystemId = oldClinicalSystemId,
                DateOfBirthViewModel = new DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenClinicalIdHasBeenChangedAndIsNotDuplicate_ValidationShouldPass()
        {
            const string clinicalSystemId = "PatientId";
            const string oldClinicalSystemId = "OldPatientId";

            A.CallTo(() => _clinicalIdValidator.Unique(clinicalSystemId)).Returns(true);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = clinicalSystemId,
                CurrentClinicalSystemId = oldClinicalSystemId,
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenNhsNumberHasNotBeenChanged_ValidationShouldPass()
        {
            const decimal nhsNumber = 4567899881;
            const decimal oldNhsNumber = 4567899881;

            A.CallTo(() => _nhsValidator.Unique(nhsNumber)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "clinicalsystemid",
                CurrentClinicalSystemId = "clinicalsystemid",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = nhsNumber,
                CurrentNhsNumber = oldNhsNumber
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenNhsNumberHasBeenChangedAndIsDuplicate_ValidationShouldFail()
        {
            const decimal nhsNumber = 4567899881;
            const decimal oldNhsNumber = 9434765919;

            A.CallTo(() => _nhsValidator.Unique(nhsNumber)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "clinicalsystemid",
                CurrentClinicalSystemId = "clinicalsystemid",
                DateOfBirthViewModel = new DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = nhsNumber,
                CurrentNhsNumber = oldNhsNumber
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenNhsNumberHasBeenChangedAndIsNotDuplicate_ValidationShouldPass()
        {
            const decimal nhsNumber = 4567899881;
            const decimal oldNhsNumber = 9434765919;

            A.CallTo(() => _nhsValidator.Unique(nhsNumber)).Returns(true);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "clinicalsystemid",
                CurrentClinicalSystemId = "clinicalsystemid",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = nhsNumber,
                CurrentNhsNumber = oldNhsNumber
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void EditPatientViewModelValidator_GivenDuplicateClinicalSystemId_ErrorMessageShouldContainClinicalSystemIdDescription()
        {
            const string clinicalsystemid = "clinicalSystemId";
            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(clinicalsystemid);
            A.CallTo(() => _clinicalIdValidator.Unique(A<string>._)).Returns(false);

            var model = new EditPatientViewModel()
            {
                ClinicalSystemId = "clinicalsystemid1",
                CurrentClinicalSystemId = "clinicalsystemid",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881,
                CurrentNhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.Errors.Should()
                .Contain(x =>
                        x.PropertyName == "ClinicalSystemId" &&
                        x.ErrorMessage == string.Format("A person with this {0} already exists", clinicalsystemid));
        }

        #region private

        private ValidationResult ValidationResult(EditPatientViewModel model)
        {
            var validator = new EditPatientViewModelValidator(_futureDateValidator, _clinicalIdValidator, _nhsValidator, _clinicalSystemIdDescriptionProvider);
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
