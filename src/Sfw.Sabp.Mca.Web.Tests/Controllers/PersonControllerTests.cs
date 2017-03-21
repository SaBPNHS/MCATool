using System;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Controllers;
using Sfw.Sabp.Mca.Web.Controllers.Base;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Web.Attributes;
namespace Sfw.Sabp.Mca.Web.Tests.Controllers
{
    [TestClass]
    public class PersonControllerTests
    {
        private IPatientViewModelBuilder _patientBuilder;
        private PersonController _personController;
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUnitOfWork _unitOfWork;
        private IFeedBackBuilder _feedBackBuilder;
        private ICopyrightViewModelBuilder _copyrightViewModelBuilder;

        [TestInitialize]
        public void Setup()
        {
            _patientBuilder = A.Fake<IPatientViewModelBuilder>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _feedBackBuilder = A.Fake<IFeedBackBuilder>();
            _copyrightViewModelBuilder = A.Fake<ICopyrightViewModelBuilder>();

            _unitOfWork = A.Fake<IUnitOfWork>();
            _personController = new PersonController(_queryDispatcher, _patientBuilder, _commandDispatcher, _unitOfWork, _feedBackBuilder, _copyrightViewModelBuilder);
        }

        [TestMethod]
        public void IndexGET_DefaultViewShouldBeReturned()
        {
            var result = _personController.Index() as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void IndexGET_PatientSearchModelShouldBeReturned()
        {
            var result = _personController.Index() as ViewResult;

            result.Model.Should().BeOfType<PatientSearchViewModel>();
            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void IndexPOST_GivenPatientSearchViewModelIsValid_PatientQueryShouldBeCalled()
        {
            var viewModel = new PatientSearchViewModel()
            {
                ClinicalSystemId = "1"
            };

            _personController.Index(viewModel);

            A.CallTo(() => _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(A<PatientByClinicalIdQuery>.That.Matches(x => x.ClinicalId == "1"))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]        
        public void IndexPOST_GivenPatientSearchViewModelIsValid_ModelBuilderShouldBeCalled()
        {
            var model = A.Fake<PatientSearchViewModel>();

            var patients = new Patients();

            A.CallTo(() => _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(A<PatientByClinicalIdQuery>._)).Returns(patients);

            _personController.Index(model);

            A.CallTo(() => _patientBuilder.BuildPatientSearchViewModel(patients)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void IndexPOST_GivenPatientSearchViewModelIsValid_BuilderModelShouldBeReturned()
        {
            var model = new PatientSearchViewModel();

            A.CallTo(() => _patientBuilder.BuildPatientSearchViewModel(A<Patients>._)).Returns(model);

            var result = _personController.Index(model) as ViewResult;

            result.Model.ShouldBeEquivalentTo(model);

        }

        [TestMethod]
        public void IndexPOST_GivenPatientSearchViewModelIsNotValid_ModelShouldBeReturned()
        {
            var model = new PatientSearchViewModel();

            _personController.ModelState.AddModelError("error", "error");

            var result = _personController.Index(model) as ViewResult;

            result.Model.ShouldBeEquivalentTo(model);

        }

        [TestMethod]
        public void CreatePOST_GivenValidModel_PatientCommandShouldBeBuilt()
        {
            var model = new CreatePatientViewModel();

            _personController.Create(model);

            A.CallTo(() => _patientBuilder.BuildAddPatientCommand(model)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePOST_GivenValidModel_ShouldBeRedirectedToAssessmentCreateAction()
        {
            var patientId = Guid.NewGuid();
            var command = new AddUpdatePatientCommand() { PatientId = patientId };

            A.CallTo(() => _patientBuilder.BuildAddPatientCommand(A<CreatePatientViewModel>._)).Returns(command);

            var result = _personController.Create(A<CreatePatientViewModel>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Assessment.ActionNames.Create);
            result.RouteValues["controller"].Should().Be(MVC.Assessment.Name);
            result.RouteValues["id"].Should().Be(command.PatientId);
        }

        [TestMethod]
        public void CreatePOST_GivenInvalidModel_ShouldReturnDefaultModel()
        {
            var viewModel = new CreatePatientViewModel();
            _personController.ModelState.AddModelError("invalid", "Last Name is required");

            var result = _personController.Create(viewModel) as ViewResult;

            result.Model.Should().Be(viewModel);
        }

        [TestMethod]
        public void Index_ShouldBeDecoratedWithAuditAttribute()
        {
            typeof(PersonController).GetMethod("Index", new[] { typeof(PatientSearchViewModel) }).Should().BeDecoratedWith<AuditAttribute>(x => x.AuditProperties == "ClinicalSystemId");
        }

        [TestMethod]
        public void EditGET_GivenPatientId_DefaultViewShouldBeReturned()
        {
            var result = _personController.Edit(A<Guid>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithValidateAntiForgeryToken()
        {
            typeof (PersonController).GetMethod("Edit", new[] {typeof (EditPatientViewModel)})
                .Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [TestMethod]
        public void EditGET_GivenPatientId_PatientDetailsShouldBeRetrieved()
        {
            var patientId = Guid.NewGuid();

            _personController.Edit(patientId);

            A.CallTo(() => _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(A<PatientByIdQuery>.That.Matches(x => x.PatientId == patientId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenPatientId_GendersListShouldBeRetrieved()
        {
            _personController.Edit(A<Guid>._);

            A.CallTo(() => _queryDispatcher.Dispatch<GenderListQuery, Genders>(A<GenderListQuery>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenPatient_PatientBuilderShouldBeCalled()
        {
            var patient = new Patient();
            var genders = new Genders();

            A.CallTo(() => _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(A<PatientByIdQuery>._)).Returns(patient);
            A.CallTo(() => _queryDispatcher.Dispatch<GenderListQuery, Genders>(A<GenderListQuery>._)).Returns(genders);

            _personController.Edit(A<Guid>._);

            A.CallTo(() => _patientBuilder.BuildEditPatientViewModel(patient, genders)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditGET_GivenPatient_EditPatientViewModelShouldBeReturned()
        {
            var viewModel = new EditPatientViewModel();

            A.CallTo(() => _patientBuilder.BuildEditPatientViewModel(A<Patient>._, A<Genders>._)).Returns(viewModel);

            var result = _personController.Edit(A<Guid>._) as ViewResult;

            result.Model.Should().BeOfType<EditPatientViewModel>();
            result.Model.Should().NotBeNull();
        }

        [TestMethod]
        public void EditPOST_GivenModelIsNotValid_DefaultViewShouldBeReturned()
        {
            _personController.ModelState.AddModelError("error", "error");

            var result = _personController.Edit(A<EditPatientViewModel>._) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void EditPOST_GivenModelIsNotValid_ModelShouldBeReturned()
        {
            var model = new EditPatientViewModel();

            _personController.ModelState.AddModelError("error", "error");

            var result = _personController.Edit(model) as ViewResult;

            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void EditPOST_GivenValidModel_PatientBuilderShouldBeCalled()
        {
            var model = new EditPatientViewModel();

            _personController.Edit(model);

            A.CallTo(() => _patientBuilder.BuildUpdatePatientCommand(model)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditPOST_GivenValidModel_CommandDispatcherShouldBeCalled()
        {
            var command = new AddUpdatePatientCommand();

            A.CallTo(() => _patientBuilder.BuildUpdatePatientCommand(A<EditPatientViewModel>._)).Returns(command);

            _personController.Edit(A<EditPatientViewModel>._);

            A.CallTo(() => _commandDispatcher.Dispatch(command)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditPOST_GivenValidModel_ChangesShouldBeSaved()
        {
            _personController.Edit(A<EditPatientViewModel>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void EditPOST_GivenValidModel_ShouldBeRedirectedToPersonSearch()
        {
            var result = _personController.Edit(A<EditPatientViewModel>._) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Person.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Person.Name);
        }

        [TestMethod]
        public void EditGET_ShouldBeDecoratedWithAuthorizeAdministratorAttribute()
        {
            typeof (PersonController).GetMethod("Edit", new[] {typeof (Guid)})
                .Should()
                .BeDecoratedWith<AuthorizeAdministratorAttributeNinject>();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithAuthorizeAdministratorAttribute()
        {
            typeof(PersonController).GetMethod("Edit", new[] { typeof(EditPatientViewModel) })
                .Should()
                .BeDecoratedWith<AuthorizeAdministratorAttributeNinject>();
        }

        [TestMethod]
        public void EditPOST_ShouldBeDecoratedWithAuditAttribute()
        {
            typeof(PersonController).GetMethod("Edit", new[] { typeof(EditPatientViewModel) }).Should().BeDecoratedWith<AuditAttribute>(x => x.AuditProperties == ApplicationStringConstants.EditPersonAuditValues);
        }

        [TestMethod]
        public void PersonController_ShouldBeDecoratedWithAgreedToDisclaimerAuthorizeAttributeNinjectAttribute()
        {
            typeof(PersonController).Should().BeDecoratedWith<AgreedToDisclaimerAuthorizeAttributeNinject>();
        }

        [TestMethod]
        public void PersonController_ShouldInheritFromBaseController()
        {
            typeof(PersonController).BaseType.Name.Should().Be(typeof(LayoutController).Name);
        }
    }
}
