using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class PatientViewModelTests
    {
        [TestMethod]
        public void ClinicalSystemId_MustBeDecoratedWithClinicalSystemIdDisplayAttribute()
        {
            typeof(PatientViewModel).Properties().FirstOrDefault(x => x.Name == "ClinicalSystemId")
                .Should()
                .BeDecoratedWith<ClinicalSystemIdDisplayAttribute>();
        }
    }
}
