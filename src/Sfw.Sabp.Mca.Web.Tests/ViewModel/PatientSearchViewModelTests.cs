using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class PatientSearchViewModelTests
    {
        [TestMethod]
        public void HasOrganisations_GivenNullOrganisations_ShouldReturnFalse()
        {
            var model = new PatientSearchViewModel();

            model.HasResult().Should().BeFalse();
        }

        [TestMethod]
        public void HasOrganisations_GivenZeroOrganisations_ShouldReturnFalse()
        {
            var model = new PatientSearchViewModel()
            {
                Items = new List<PatientViewModel>().AsEnumerable()
            };

            model.HasResult().Should().BeFalse();
        }

        [TestMethod]
        public void CanCreate_GivenNullOrganisations_ShouldReturnFalse()
        {
            var model = new PatientSearchViewModel();

            model.CanCreate().Should().BeFalse();
        }

        [TestMethod]
        public void CanCreate_GivenZeroOrganisations_ShouldReturnTrue()
        {
            var model = new PatientSearchViewModel()
            {
                Items = new List<PatientViewModel>().AsEnumerable()
            };

            model.CanCreate().Should().BeTrue();
        }

        [TestMethod]
        public void CanCreate_GivenMoreThanZeroOrganisations_ShouldReturnTrue()
        {
            var model = new PatientSearchViewModel()
            {
                Items = new List<PatientViewModel>() { new PatientViewModel() }
            };

            model.CanCreate().Should().BeTrue();
        }

        [TestMethod]
        public void DisplayEmptySearchResultsMessage_GivenNullOrganisations_ShouldReturnFalse()
        {
            var model = new PatientSearchViewModel()
            {
                Items = null
            };

            model.DisplayEmptySearchResultsMessage().Should().BeFalse();
        }

        [TestMethod]
        public void DisplayEmptySearchResultsMessagee_GivenZeroOrganisations_ShouldReturnTrue()
        {
            var model = new PatientSearchViewModel()
            {
                Items = new List<PatientViewModel>()
            };

            model.DisplayEmptySearchResultsMessage().Should().BeTrue();
        }

        [TestMethod]
        public void DisplayEmptySearchResultsMessagee_GivenMoreThanZeroOrganisations_ShouldReturnFalse()
        {
            var model = new PatientSearchViewModel()
            {
                Items = new List<PatientViewModel>() { new PatientViewModel() }
            };

            model.DisplayEmptySearchResultsMessage().Should().BeFalse();
        }

        [TestMethod]
        public void ClinicalSystemId_MustBeDecoratedWithClinicalSystemIdDisplayAttribute()
        {
            typeof (PatientSearchViewModel).Properties().FirstOrDefault(x => x.Name == "ClinicalSystemId")
                .Should()
                .BeDecoratedWith<ClinicalSystemIdDisplayAttribute>();
        }

    }
}
