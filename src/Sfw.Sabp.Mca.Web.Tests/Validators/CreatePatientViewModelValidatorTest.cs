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
    public class CreatePatientViewModelValidatorTest
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
        public void CreatePatientViewModelValidator_GivenClinicalSystemIdIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenFirstNameIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenLastNameIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePatientViewModelValidator_GivenNhsNumberIsNotProvided_ValidationShouldPass()
        {
            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenDateOfBirthIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
            {
                ClinicalSystemId = "PatientId",
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePatientViewModelValidator_GivenGenderIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenInvalidNHSNumberIsProvided_ValidationShouldFail()
        {
            const long nhsNumber = 4567899898;

            A.CallTo(() => _nhsValidator.Valid(nhsNumber)).Returns(false);

            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenNhsNumberIsNotProvided_ValidationShouldFail()
        {
            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenDateIsInTheFuture_ValidationShouldFail()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(false);

            var model = new CreatePatientViewModel()
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
        public void CreatePatientViewModelValidator_GivenDuplicateClinicalId_ValidationShouldFail()
        {
            const string clinicalSystemId = "PatientId";

            A.CallTo(() => _clinicalIdValidator.Unique(clinicalSystemId)).Returns(false);

            var model = new CreatePatientViewModel()
            {
                ClinicalSystemId = clinicalSystemId,
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePatientViewModelValidator_GivenNonDuplicateClinicalId_ValidationShouldPass()
        {
            const string clinicalSystemId = "PatientId";
            A.CallTo(() => _clinicalIdValidator.Unique(clinicalSystemId)).Returns(true);

            var model = new CreatePatientViewModel()
            {
                ClinicalSystemId = clinicalSystemId,
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
        public void CreatePatientViewModelValidator_GivenDuplicateClinicalSystemId_ErrorMessageShouldContainClinicalSystemIdDescription()
        {
            const string clinicalsystemid = "clinicalSystemId";
            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(clinicalsystemid);
            A.CallTo(() => _clinicalIdValidator.Unique(A<string>._)).Returns(false);
            
            var model = new CreatePatientViewModel()
            {
                ClinicalSystemId = "clinicalSystemId",
                DateOfBirthViewModel = DateOfBirthViewModel(),
                FirstName = "David",
                LastName = "Miller",
                GenderId = 1,
                NhsNumber = 4567899881
            };

            var result = ValidationResult(model);

            result.Errors.Should()
                .Contain(x =>
                        x.PropertyName == "ClinicalSystemId" &&
                        x.ErrorMessage == string.Format("A person with this {0} already exists", clinicalsystemid));
        }
        #region private

        private ValidationResult ValidationResult(CreatePatientViewModel model)
        {
            var validator = new CreatePatientViewModelValidator(_futureDateValidator, _clinicalIdValidator, _nhsValidator, _clinicalSystemIdDescriptionProvider);
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
