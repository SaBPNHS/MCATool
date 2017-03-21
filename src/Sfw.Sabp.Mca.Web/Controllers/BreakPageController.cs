using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;

namespace Sfw.Sabp.Mca.Web.Controllers
{
    [AgreedToDisclaimerAuthorizeAttributeNinject]
    public partial class BreakPageController : LayoutController
    {
        private readonly IClinicalSystemViewModelBuilder _clinicalSystemViewModelBuilder;
        private readonly IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;

        public BreakPageController(IClinicalSystemViewModelBuilder clinicalSystemViewModelBuilder, 
            IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider, 
            IFeedBackBuilder feedBackBuilder,
            ICopyrightViewModelBuilder copyrightViewModelBuilder)
            : base(feedBackBuilder, copyrightViewModelBuilder)
        {
            _clinicalSystemViewModelBuilder = clinicalSystemViewModelBuilder;
            _clinicalSystemIdDescriptionProvider = clinicalSystemIdDescriptionProvider;
        }

        public virtual ActionResult Index()
        {
            var clinicalSystemIdDescription = _clinicalSystemIdDescriptionProvider.GetDescription();
            var viewModel = _clinicalSystemViewModelBuilder.BuildClinicalSystemText(clinicalSystemIdDescription);
            return View(viewModel);
        }
    }
}