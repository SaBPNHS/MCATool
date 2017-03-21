using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.Collections.Generic;
using Roles = Sfw.Sabp.Mca.Model.Roles;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class AssessmentViewModelBuilder : IAssessmentViewModelBuilder
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private readonly IUserPrincipalProvider _userPrincipalProvider;
        private readonly IUserRoleProvider _userRoleProvider;

        public AssessmentViewModelBuilder(IDateTimeProvider dateTimeProvider, IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider, IUserPrincipalProvider userPrincipalProvider, IUserRoleProvider userRoleProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _clinicalSystemIdDescriptionProvider = clinicalSystemIdDescriptionProvider;
            _userPrincipalProvider = userPrincipalProvider;
            _userRoleProvider = userRoleProvider;
        }

        public AssessmentViewModel BuildAssessmentViewModel(Patient patient, Roles roles)
        {
            if (patient == null) throw new ArgumentNullException("patient");
            if (roles == null) throw new ArgumentNullException("roles");

            var viewModel = new AssessmentViewModel
            {
                StageDescription = ApplicationStringConstants.Stage1Text,
                StageDescriptionStyle = ApplicationStringConstants.Stage1ShortDescription,
                Stage1DecisionConfirmation = ApplicationStringConstants.DecisionConfirmationText,
                Stage1InfoText = ApplicationStringConstants.DecisionPromptText,
                DateAssessmentStarted = _dateTimeProvider.Now.Date,
                Patient = Mapper.Map<Patient, PatientViewModel>(patient),
                Roles = RoleItems(roles),
                PatientSummary = PatientSummary(patient)
            };
            
            viewModel.Patient.SelectedGender = patient.Gender.Description;
            return viewModel;
        }

        public AssessmentViewModel BuildAssessmentViewModel(Assessment assessment)
        {
            if (assessment == null) throw new ArgumentNullException("assessment");

            var viewModel = Mapper.DynamicMap<Assessment, AssessmentViewModel>(assessment);
            viewModel.StageDescription = ApplicationStringConstants.Stage1Text;
            viewModel.StageDescriptionStyle = ApplicationStringConstants.Stage1ShortDescription;
            viewModel.Patient = Mapper.DynamicMap<Patient, PatientViewModel>(assessment.Patient);
            viewModel.Status = Mapper.DynamicMap<Status, StatusViewModel>(assessment.Status);
            viewModel.PatientSummary = PatientSummary(assessment.Patient);
            viewModel.Terminated = !string.IsNullOrWhiteSpace(assessment.TerminatedAssessmentReason);
            viewModel.ClinicalSystemName = _clinicalSystemIdDescriptionProvider.GetName();
            return viewModel;
        }

        public AssessmentViewModel BuildAssessmentViewModel(Assessment assessment, Roles roles)
        {
            if (assessment == null) throw new ArgumentNullException("assessment");
            if (roles == null) throw new ArgumentNullException("roles");

            var viewModel = BuildAssessmentViewModel(assessment);
            viewModel.Roles = RoleItems(roles);

            return viewModel;
        }

        public AddAssessmentCommand BuildAddAssessmentCommand(AssessmentViewModel assessment)
        {
            if (assessment == null) throw new ArgumentNullException("assessment");

            var command = Mapper.DynamicMap<AssessmentViewModel, AddAssessmentCommand>(assessment);

            command.AssessmentId = Guid.NewGuid();
            command.Stage1DecisionConfirmation = ApplicationStringConstants.DecisionConfirmationText;
            command.Stage1InfoText = ApplicationStringConstants.DecisionPromptText;
            command.StatusId = (int)AssessmentStatusEnum.InProgress;
            command.PatientId = assessment.Patient.PatientId;
            command.RoleId = assessment.RoleId;
            command.DecisionMaker = assessment.DecisionMaker;
            if (command.DateAssessmentStarted == DateTime.MinValue)
            {
                command.DateAssessmentStarted = _dateTimeProvider.Now;
            }
            if (command.DateAssessmentEnded == DateTime.MinValue)
            {
                command.DateAssessmentEnded = _dateTimeProvider.Now;
            }
            return command;
        }

        public AssessmentListViewModel BuildAssessmentListViewModel(Guid patientId, Assessments assessments)
        {
            if (assessments == null) throw new ArgumentNullException("assessments");
            if (patientId == Guid.Empty) throw new ArgumentException("patientId");

            var assessmentViewModels = new List<AssessmentViewModel>();

            foreach (var assessment in assessments.Items)
            {
                var assessmentViewModel = Mapper.DynamicMap<Assessment, AssessmentViewModel>(assessment);
                var statusViewModel = Mapper.DynamicMap<Status, StatusViewModel>(assessment.Status);

                assessmentViewModel.Status = statusViewModel;

                assessmentViewModel.Stage1DecisionToBeMade = assessmentViewModel.Stage1DecisionToBeMade.Length > 50
                    ? assessmentViewModel.Stage1DecisionToBeMade.Substring(0, 50)
                    : assessmentViewModel.Stage1DecisionToBeMade;

                SetCanViewPdfProperty(assessment, assessmentViewModel);

                SetCanRestartProperty(assessment, assessmentViewModel);

                SetCanReviseProperty(assessment, assessmentViewModel);

                assessmentViewModels.Add(assessmentViewModel);
            }

            var viewModel = new AssessmentListViewModel
            {
                Items = assessmentViewModels,
                PatientId = patientId,
                ClinicalIdDescription = _clinicalSystemIdDescriptionProvider.GetDescription()
            };

            return viewModel;
        }

        public UpdateAssessmentCommand BuildUpdateAssessmentCommand(AssessmentViewModel assessmentViewModel)
        {
            if (assessmentViewModel == null) throw new ArgumentNullException();

            return Mapper.DynamicMap<AssessmentViewModel, UpdateAssessmentCommand>(assessmentViewModel);
        }

        #region private

        private string PatientSummary(Patient patient)
        {
            return string.Format("{0} {1}, {2}, {3}", patient.FirstName, patient.LastName, patient.ClinicalSystemId, patient.Gender.Description);
        }

        private void SetCanRestartProperty(Assessment assessment, AssessmentViewModel assessmentViewModel)
        {
            if (assessment.StatusId != (int)AssessmentStatusEnum.Complete
                && AssignedAssessorOrAdmin(assessment))
            {
                assessmentViewModel.CanRestart = true;
            }
        }

        private void SetCanReviseProperty(Assessment assessment, AssessmentViewModel assessmentViewModel)
        {
            if (assessment.StatusId == (int)AssessmentStatusEnum.Complete
                && AssignedAssessorOrAdmin(assessment))
            {
                assessmentViewModel.CanRevise = true;
            }
        }

        private bool AssignedAssessorOrAdmin(Assessment assessment)
        {
            return (assessment.AssessorDomainName == _userPrincipalProvider.CurrentUserName ||
                    _userRoleProvider.CurrentUserInAdministratorRole());
        }

        private void SetCanViewPdfProperty(Assessment assessment, AssessmentViewModel assessmentViewModel)
        {
            if (assessment.StatusId == (int) AssessmentStatusEnum.Complete
                && AssignedAssessorOrAdmin(assessment))
            {
                assessmentViewModel.CanViewPdf = true;
            }
        }

        private IEnumerable<SelectListItem> RoleItems(Roles role)
        {
            return BuildEmptySelectList().Union(role.Items.Select(option => new SelectListItem { Text = option.Description, Value = option.RoleId.ToString() }));
        }

        private IEnumerable<SelectListItem> BuildEmptySelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = "Select Role", Value = ""}
            };
        }
        #endregion

    }
}