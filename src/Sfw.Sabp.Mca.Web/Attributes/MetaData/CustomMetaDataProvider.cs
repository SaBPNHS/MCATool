using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Web.Attributes.MetaData
{
    public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;

        public CustomModelMetadataProvider(IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
        {
            _clinicalSystemIdDescriptionProvider = clinicalSystemIdDescriptionProvider;
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, 
            Type containerType, 
            Func<object> modelAccessor, 
            Type modelType, 
            string propertyName) 
        {
            var data = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            var description = attributes.SingleOrDefault(a => a is ClinicalSystemIdDisplayAttribute);

            if (description != null) data.DisplayName = _clinicalSystemIdDescriptionProvider.GetDescription();

            return data;
        }
    }
}