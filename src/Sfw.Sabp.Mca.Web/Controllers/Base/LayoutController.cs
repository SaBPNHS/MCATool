using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Sfw.Sabp.Mca.Web.Builders;

namespace Sfw.Sabp.Mca.Web.Controllers.Base
{
    public partial class LayoutController : Controller
    {
        private readonly IFeedBackBuilder _feedBackBuilder;
        private readonly ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        public LayoutController(){}

        public LayoutController(IFeedBackBuilder feedBackBuilder, ICopyrightViewModelBuilder copyrightViewModelBuilder)
        {
            _feedBackBuilder = feedBackBuilder;
            _copyrightViewModelBuilder = copyrightViewModelBuilder;
        }

        [ChildActionOnly]
        public virtual ActionResult FeedBack()
        {
            var model = _feedBackBuilder.CreateFeedBackViewModel();

            return PartialView(MVC.Shared.Views._FeedBack, model);
        }

        [ChildActionOnly]
        public virtual ActionResult Copyright()
        {
            var model = _copyrightViewModelBuilder.CreateCopyrightViewModel();

            return PartialView(MVC.Shared.Views._Copyright, model);
        }
    }
}