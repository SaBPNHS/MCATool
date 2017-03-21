using FakeItEasy;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class PatientSearchViewModelValidatorTests
    {
        private PatientSearchViewModelValidator _patientSearchViewModelValidator;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private const string Clinicalsystemid = "clinicalSystemId";

        [TestInitialize]
        public void Setup()
        {
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();

            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(Clinicalsystemid);

            _patientSearchViewModelValidator = new PatientSearchViewModelValidator(_clinicalSystemIdDescriptionProvider);
        }

        [TestMethod]
        public void PatientSearchViewModelValidator_GivenClinicalSystemIdHasNotBeenProvided_ValidationShouldFail()
        {
            var model = new PatientSearchViewModel();

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void PatientSearchViewModelValidator_GivenClinicalSystemIdHasNotBeProvided_ErrorShouldContainClinicalSystemIdDescription()
        {
            var model = new PatientSearchViewModel();

            var result = ValidationResult(model);

            result.Errors.Should()
                .Contain(x => x.PropertyName == "ClinicalSystemId" && x.ErrorMessage == string.Format("{0} is mandatory", Clinicalsystemid));
        }

        #region private

        private ValidationResult ValidationResult(PatientSearchViewModel model)
        {
            return _patientSearchViewModelValidator.Validate(model);
        }

        #endregion
    }
}
