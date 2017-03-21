using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.Attributes.MetaData;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class CustomMetaDataProviderTests
    {
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private CustomModelMetadataProvider _customModelMetadataProvider;
        private const string Description = "description";

        [TestInitialize]
        public void Setup()
        {
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();
            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(Description);

            _customModelMetadataProvider = new CustomModelMetadataProvider(_clinicalSystemIdDescriptionProvider);

            ModelMetadataProviders.Current = _customModelMetadataProvider;
        }

        [TestMethod]
        public void CreateMetadata_GivenModelWithNoClinicalSystemIdDisplayAttributes_DisplayNameShouldNotBeModified()
        {
            var metaData = GetMetaData<PropertiesWithNoClinicalSystemIdDisplayAttributeModel>();

            metaData.Properties.First(x => x.PropertyName == "Property1").DisplayName.Should().Be("Property1");
            metaData.Properties.First(x => x.PropertyName == "Property2").DisplayName.Should().Be("Property2");
        }

        [TestMethod]
        public void CreateMetadata_GivenModelWithClinicalSystemIdDisplayAttributes_DisplayNameShouldBeModified()
        {
            var metaData = GetMetaData<PropertiesWithClinicalSystemIdDisplayAttributeModel>();

            metaData.Properties.First(x => x.PropertyName == "Property1").DisplayName.Should().Be(Description);
            metaData.Properties.First(x => x.PropertyName == "Property2").DisplayName.Should().Be(Description);
            metaData.Properties.First(x => x.PropertyName == "Property3").DisplayName.Should().Be("Property3");
        }

        #region private

        private ModelMetadata GetMetaData<T>() where T : class 
        {
            return ModelMetadataProviders.Current.GetMetadataForType(null, typeof(T));
        }

        private abstract class PropertiesWithNoClinicalSystemIdDisplayAttributeModel
        {
            [Display(Name="Property1")]
            public string Property1 { get; set; }

            [Display(Name = "Property2")]
            public string Property2 { get; set; }
        }

        private abstract class PropertiesWithClinicalSystemIdDisplayAttributeModel
        {
            [ClinicalSystemIdDisplay]
            public string Property1 { get; set; }

            [ClinicalSystemIdDisplay]
            public string Property2 { get; set; }

            [Display(Name="Property3")]
            public string Property3 { get; set; }
        }

        #endregion
    }
}
