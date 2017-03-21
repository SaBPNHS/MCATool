using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class ClinicalIdValidatorTests
    {
        private ClinicalIdValidator _validator;
        private IQueryDispatcher _queryDispatcher;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            _validator = new ClinicalIdValidator(_queryDispatcher);
        }

        [TestMethod]
        public void Valid_GivenEmptyClinicalId_FalseShouldbeReturned()
        {
            const string clinicalId = "";

            var result = _validator.Unique(clinicalId);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenWhitespaceClinicalId_FalseShouldbeReturned()
        {
            const string clinicalId = " ";

            var result = _validator.Unique(clinicalId);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenNullClinicalId_FalseShouldbeReturned()
        {
            var result = _validator.Unique(null);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenNonDuplicateClinicalId_TrueShouldBeReturned()
        {
            const string clinicalId = "clinicalId";

            A.CallTo(
                () =>
                    _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(
                        A<PatientByClinicalIdQuery>.That.Matches(x => x.ClinicalId == "clinicalId")))
                .Returns(new Patients()
                {
                    Items = new List<Patient>()
                });

            var result = _validator.Unique(clinicalId);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Valid_GivenDuplicateClinicalId_FalseShouldBeReturned()
        {
            const string clinicalId = "clinicalId";

            A.CallTo(
                () =>
                    _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(
                        A<PatientByClinicalIdQuery>.That.Matches(x => x.ClinicalId == "clinicalId")))
                .Returns(new Patients()
                {
                    Items = new List<Patient>() { new Patient()}
                });

            var result = _validator.Unique(clinicalId);

            result.Should().BeFalse();
        }
    }
}
