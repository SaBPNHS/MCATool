using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Controllers
{
    public partial class HomeController : LayoutController
    {
        private readonly IDisclaimerViewModelBuilder _disclaimerViewModelBuilder;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IUserPrincipalProvider _userPrincipalProvider;

        public HomeController(IDisclaimerViewModelBuilder disclaimerViewModelBuilder, 
            ICommandDispatcher commandDispatcher, 
            IUnitOfWork unitOfWork, 
            IQueryDispatcher queryDispatcher, 
            IUserPrincipalProvider userPrincipalProvider, 
            IFeedBackBuilder feedBackBuilder,
            ICopyrightViewModelBuilder copyrightViewModelBuilder)
            : base(feedBackBuilder, copyrightViewModelBuilder)
        {
            _disclaimerViewModelBuilder = disclaimerViewModelBuilder;
            _commandDispatcher = commandDispatcher;
            _unitOfWork = unitOfWork;
            _queryDispatcher = queryDispatcher;
            _userPrincipalProvider = userPrincipalProvider;
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            var disclaimer = _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(new DisclaimerByUserQuery()
               {
                   AssessorDomainName = _userPrincipalProvider.CurrentUserName
               });

            var model = _disclaimerViewModelBuilder.BuildDisclaimerViewModel(disclaimer);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Index(DisclaimerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var command = _disclaimerViewModelBuilder.BuildAddDisclaimerCommand(model);

                _commandDispatcher.Dispatch(command);

                _unitOfWork.SaveChanges();

                return RedirectToAction(MVC.Home.Tutorial());
            }

            return View(model);
        }

        public virtual ActionResult Tutorial()
        {
            return View();
        }
    }
}