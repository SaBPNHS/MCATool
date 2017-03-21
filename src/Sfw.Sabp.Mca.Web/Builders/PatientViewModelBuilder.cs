using System.Linq;
using AutoMapper;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class PatientViewModelBuilder : IPatientViewModelBuilder
    {
        private readonly IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private readonly IUserRoleProvider _userRoleProvider;
        private readonly IDateOfBirthBuilder _dateOfBirthBuilder;

        public PatientViewModelBuilder(IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider, IUserRoleProvider userRoleProvider, IDateOfBirthBuilder dateOfBirthBuilder)
        {
            _clinicalSystemIdDescriptionProvider = clinicalSystemIdDescriptionProvider;
            _userRoleProvider = userRoleProvider;
            _dateOfBirthBuilder = dateOfBirthBuilder;
        }

        public CreatePatientViewModel BuildPatientViewModel(Genders gender)
        {
            if (gender == null) throw new ArgumentNullException("gender");
           
            var model = new CreatePatientViewModel
            {
                Genders = GenderItems(gender),
                DateOfBirthViewModel = _dateOfBirthBuilder.BuildDateOfBirthViewModel(null)
            };
            return model;
        }

        public AddUpdatePatientCommand BuildAddPatientCommand(PatientViewModel patient)
        {
            if (patient == null) throw new ArgumentNullException("patient");
            if (!patient.DateOfBirthViewModel.Date.HasValue) throw new ArgumentNullException("patient.DateOfBirthViewModel.Date");

            var command = Mapper.Map<PatientViewModel, AddUpdatePatientCommand>(patient);
            command.PatientId = Guid.NewGuid();
            command.DateOfBirth = patient.DateOfBirthViewModel.Date.Value;    
            return command;
        }

        public PatientSearchViewModel BuildPatientSearchViewModel(Patients patients)
        {
            if (patients == null) throw new ArgumentNullException("patients");

            var viewModel = new PatientSearchViewModel
            {                
                Items = patients.Items.Select(Mapper.Map<Patient, PatientViewModel>),
                ClinicalIdDescription = _clinicalSystemIdDescriptionProvider.GetDescription(),
                CanEdit = _userRoleProvider.CurrentUserInAdministratorRole()
            };

            return viewModel;
        }

        public EditPatientViewModel BuildEditPatientViewModel(Patient patient, Genders genders)
        {
            if (patient == null) throw new ArgumentNullException("patient");
            if (genders == null || !genders.Items.Any()) throw new ArgumentException("genders");

            var viewModel = Mapper.DynamicMap<Patient, EditPatientViewModel>(patient);
            viewModel.DateOfBirthViewModel = _dateOfBirthBuilder.BuildDateOfBirthViewModel(patient.DateOfBirth);
            viewModel.Genders = GenderItems(genders);
            viewModel.CurrentClinicalSystemId = patient.ClinicalSystemId;
            viewModel.CurrentNhsNumber = patient.NhsNumber;
            viewModel.CurrentFirstName = patient.FirstName;
            viewModel.CurrentLastName = patient.LastName;
            viewModel.CurrentDateOfBirth = patient.DateOfBirth.ToShortDateString();
            viewModel.CurrentGenderId = patient.GenderId;

            return viewModel;
        }

        public AddUpdatePatientCommand BuildUpdatePatientCommand(EditPatientViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException();

            return Mapper.Map<EditPatientViewModel, AddUpdatePatientCommand>(viewModel);
        }

        #region private 

        private IEnumerable<SelectListItem> BuildEmptySelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = "Select Gender", Value = ""}
            };
        }

        private IEnumerable<SelectListItem> GenderItems(Genders gender)
        {
            return BuildEmptySelectList().Union(gender.Items.Select(option => new SelectListItem { Text = option.Description, Value = option.GenderId.ToString() }));
        }

        #endregion

        
    }
}