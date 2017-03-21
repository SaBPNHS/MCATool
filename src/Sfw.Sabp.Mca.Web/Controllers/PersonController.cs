using System;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;
using System.Web.Mvc;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.Controllers
{
    [AgreedToDisclaimerAuthorizeAttributeNinject]
    public partial class PersonController : LayoutController
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IPatientViewModelBuilder _patientViewModelBuilder;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IUnitOfWork _unitOfWork;

        public PersonController(IQueryDispatcher queryDispatcher, 
            IPatientViewModelBuilder patientViewModelBuilder, 
            ICommandDispatcher commandDispatcher, 
            IUnitOfWork unitOfWork, 
            IFeedBackBuilder feedBackBuilder,
            ICopyrightViewModelBuilder copyrightViewModelBuilder)
            : base(feedBackBuilder, copyrightViewModelBuilder)
        {
            _queryDispatcher = queryDispatcher;
            _patientViewModelBuilder = patientViewModelBuilder;
            _commandDispatcher = commandDispatcher;
            _unitOfWork = unitOfWork;            
        }
        
        [HttpGet]        
        public virtual ActionResult Create()
        {
            var genderQuery = new GenderListQuery();

            var result = _queryDispatcher.Dispatch<GenderListQuery, Genders>(genderQuery);

            var model = _patientViewModelBuilder.BuildPatientViewModel(result);

            return View(model);
        }

        [HttpPost]        
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(CreatePatientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var command = _patientViewModelBuilder.BuildAddPatientCommand(model);

                _commandDispatcher.Dispatch(command);

                _unitOfWork.SaveChanges();

                return RedirectToAction(MVC.Assessment.Create(command.PatientId));
            }

            return View(model);
        }

        [HttpGet]        
        public virtual ActionResult Index()
        {
            return View(new PatientSearchViewModel());
        }

        [HttpPost]
        [Audit(AuditProperties = "ClinicalSystemId")]
        public virtual ActionResult Index(PatientSearchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var patientQuery = new PatientByClinicalIdQuery { ClinicalId = viewModel.ClinicalSystemId };

                var result = _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(patientQuery);

                var model = _patientViewModelBuilder.BuildPatientSearchViewModel(result);
                               
                return View(model);
            }

            return View(viewModel);
        }

        [AuthorizeAdministratorAttributeNinject]
        public virtual ActionResult Edit(Guid id)
        {
            var patientQuery = new PatientByIdQuery { PatientId = id };

            var patient = _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(patientQuery);

            var genders = _queryDispatcher.Dispatch<GenderListQuery, Genders>(new GenderListQuery());

            var model = _patientViewModelBuilder.BuildEditPatientViewModel(patient, genders);

            return View(model);
        }

        [HttpPost]
        [AuthorizeAdministratorAttributeNinject]
        [ValidateAntiForgeryToken]
        [Audit(AuditProperties = ApplicationStringConstants.EditPersonAuditValues)]
        public virtual ActionResult Edit(EditPatientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var command = _patientViewModelBuilder.BuildUpdatePatientCommand(model);

                _commandDispatcher.Dispatch(command);

                _unitOfWork.SaveChanges();

                return RedirectToAction(MVC.Person.Index());
            }

            return View(model);
        }
            
    }
}
