using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Builders.Mappings;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class AssessmentViewModelBuilderTests
    {
        private AssessmentViewModelBuilder _builder;
        private IDateTimeProvider _dateTimeProvider;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private const string Description = "description";
        private IUserPrincipalProvider _userPrincipalProvider;
        private IUserRoleProvider _userRoleProvider;
        private const string AssessorDomainName = "name";
        private const string AssessorName = "assessorname";
        private const string Stage1InfoText = "stage1infotext";
        private const string Stage1DecisionToBeMade = "stage1decisiontobemade";
        private const string Stage1DecisionConfirmation = "stage1decisionconfirmation";
        private const string Genderdescription = "genderdescription";
        private const string ClinicalSystemId = "clinicalsystemid";
        private const string LastName = "lastname";
        private const string FirstName = "firstname";

        [TestInitialize]
        public void Setup()
        {
            Mapper.AddProfile(new AutomapperMappingProfile());

            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            A.CallTo(() => _dateTimeProvider.Now).Returns(new DateTime(2015, 1, 1));

            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();
            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(Description);

            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _userRoleProvider = A.Fake<IUserRoleProvider>();

            _builder = new AssessmentViewModelBuilder(_dateTimeProvider, _clinicalSystemIdDescriptionProvider, _userPrincipalProvider, _userRoleProvider);
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenValidModel_ViewModelShouldBeReturned()
        {
            var rolelist = new List<Role> { new Role() };
            var roles = new Roles { Items = rolelist };

            var result = _builder.BuildAssessmentViewModel(A.Dummy<Patient>(), roles);

            result.Should().BeOfType<AssessmentViewModel>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenValidModel_AssessmentViewModelPropertiesShouldBeMapped()
        {
            var patient = A.Fake<Patient>();
            var rolelist = new List<Role> { new Role() };
            var roles = new Roles { Items = rolelist };
            var result = _builder.BuildAssessmentViewModel(patient, roles);

            result.Stage1DecisionClearlyMade.ShouldBeEquivalentTo(false);
            result.Stage1InfoText.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionPromptText);
            result.Stage1DecisionConfirmation.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionConfirmationText);
            result.CurrentWorkflowQuestionId.Should().NotHaveValue();
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenNullPatient_ArgumentNullExceptionShouldBeThrown()
        {
            var rolelist = new List<Role> { new Role() };
            var roles = new Roles { Items = rolelist };
            _builder.Invoking(x => x.BuildAssessmentViewModel((Patient)null, roles)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenNullRoles_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildAssessmentViewModel(A.Fake<Patient>(), null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenValidPatient_PatientPropertiesShouldBeMapped()
        {
            var id = Guid.NewGuid();

            var patient = new Patient()
            {
                PatientId = id,
                ClinicalSystemId = ClinicalSystemId,
                NhsNumber = 1,
                FirstName = AssessorDomainName,
                LastName = LastName,
                DateOfBirth = new DateTime(2015, 1, 1),
                GenderId = 1,
                Gender = new Gender() { Description = "Female", GenderId = 1 },
            };

            var rolelist = new List<Role> { new Role{RoleId = (int)RoleIdEnum.Assessor, Description = "First role"} };
            var roles = new Roles { Items = rolelist };

            var result = _builder.BuildAssessmentViewModel(patient, roles);

            result.Patient.ClinicalSystemId.Should().Be(ClinicalSystemId);
            result.Patient.NhsNumber.Should().Be(1);
            result.Patient.FirstName.Should().Be(AssessorDomainName);
            result.Patient.LastName.Should().Be(LastName);
            result.Patient.DateOfBirthViewModel.Date.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
            result.Patient.GenderId.Should().Be(1);
            result.Patient.PatientId.ShouldBeEquivalentTo(id);
            result.Patient.SelectedGender.Should().Be("Female");
        }

        [TestMethod]
        public void BuildAssessmentQuery_GivenNullModel_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildAddAssessmentCommand(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentQuery_GivenValidAssessmentViewModel_CommandShouldBeReturned()
        {
            var result = _builder.BuildAddAssessmentCommand(new AssessmentViewModel());

            result.Should().BeOfType<AddAssessmentCommand>();
        }

        [TestMethod]
        public void BuildAddAssessmentCommand_GivenValidAssessmentViewModel_AddAssessmentCommand()
        {
            var guid = Guid.NewGuid();
            var assessmentStartDate = new DateTime(2015, 03, 31);
            var patientId = Guid.NewGuid();

            const string stage1Decision = "MCA Decision";

            var assessment = new AssessmentViewModel()
            {
                AssessmentId = guid,
                DateAssessmentStarted = assessmentStartDate,
                Stage1InfoText = ApplicationStringConstants.DecisionPromptText,
                Stage1DecisionToBeMade = stage1Decision,
                Stage1DecisionConfirmation = ApplicationStringConstants.DecisionConfirmationText,
                Stage1DecisionClearlyMade = true,
                Patient = new PatientViewModel()
                {
                    PatientId = patientId
                },
            RoleId = (int)RoleIdEnum.Assessor
            };

            var result = _builder.BuildAddAssessmentCommand(assessment);

            result.AssessmentId.Should().NotBeEmpty();
            result.DateAssessmentStarted.ShouldBeEquivalentTo(assessmentStartDate);
            result.Stage1InfoText.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionPromptText);
            result.Stage1DecisionToBeMade.ShouldBeEquivalentTo(stage1Decision);
            result.Stage1DecisionConfirmation.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionConfirmationText);
            result.Stage1DecisionClearlyMade.ShouldBeEquivalentTo(true);
            result.StatusId.Should().Be((int)AssessmentStatusEnum.InProgress);
            result.CurrentWorkflowQuestionId.ShouldBeEquivalentTo(Guid.Empty);
            result.PatientId.ShouldBeEquivalentTo(patientId);
            result.RoleId.Should().Be((int)RoleIdEnum.Assessor);
        }

        [TestMethod]
        public void BuildAddAssessmentCommand_GivenValidAssessmentViewModelWithEmptyDate_AddAssessmentCommandShouldHaveCurrentDateAsAssessmentDate()
        {
            var assessmentStartDate = DateTime.MinValue;

            const string stage1Decision = "MCA Decision";

            var assessment = new AssessmentViewModel()
            {
                DateAssessmentStarted = assessmentStartDate,
                Stage1InfoText = ApplicationStringConstants.DecisionPromptText,
                Stage1DecisionToBeMade = stage1Decision,
                Stage1DecisionConfirmation = ApplicationStringConstants.DecisionConfirmationText,
                Stage1DecisionClearlyMade = true
            };

            var result = _builder.BuildAddAssessmentCommand(assessment);

            result.AssessmentId.Should().NotBeEmpty();
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
            result.Stage1InfoText.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionPromptText);
            result.Stage1DecisionToBeMade.ShouldBeEquivalentTo(stage1Decision);
            result.Stage1DecisionConfirmation.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionConfirmationText);
            result.Stage1DecisionClearlyMade.ShouldBeEquivalentTo(true);
            result.StatusId.Should().Be((int)AssessmentStatusEnum.InProgress);
            result.CurrentWorkflowQuestionId.ShouldBeEquivalentTo(Guid.Empty);
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenNullAssessments_ArgumentNullExceptionExpected()
        {
            _builder.Invoking(x => x.BuildAssessmentListViewModel(Guid.NewGuid(), null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessments_ModelShouldBeMapped()
        {
            var assessmentsList = new List<Assessment>
            {
                new Assessment()
                {
                    DateAssessmentStarted = new DateTime(2016, 1, 1),
                    Stage1DecisionToBeMade = "decision1",
                    AssessorName = "assessor1",
                    DateAssessmentEnded = _dateTimeProvider.Now,
                    Status = new Status()
                    {
                        Description = "complete"
                    },
                    Patient = new Patient()
                    {
                         ClinicalSystemId = "1"
                    },
                    RoleId = (int)RoleIdEnum.Assessor
                }, 
                new Assessment()
                {
                    DateAssessmentStarted = new DateTime(2017, 1, 1),
                    Stage1DecisionToBeMade = "decision2",
                    AssessorName = "assessor2",
                    Status = new Status()
                    {
                        Description = "progress"
                    },
                    Patient =  new Patient()
                    {
                        ClinicalSystemId = "2"    
                    },
                    RoleId = (int)RoleIdEnum.DecisionMaker
                }
            };
            var assessment = new Assessments()
            {
                Items = assessmentsList
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessment);

            result.Items.Should().HaveCount(2);
            result.Items.ElementAt(0).Patient.ClinicalSystemId.Should().Be("1");
            result.Items.ElementAt(0).Stage1DecisionToBeMade.Should().Be("decision1");
            result.Items.ElementAt(0).DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2016, 1, 1));
            result.Items.ElementAt(0).AssessorName.Should().Be("assessor1");
            result.Items.ElementAt(0).Status.Description.Should().Be("complete");
            result.Items.ElementAt(0).RoleId.Should().Be((int)RoleIdEnum.Assessor);
            result.Items.ElementAt(1).Patient.ClinicalSystemId.Should().Be("2");
            result.Items.ElementAt(1).Stage1DecisionToBeMade.Should().Be("decision2");
            result.Items.ElementAt(1).DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2017, 1, 1));
            result.Items.ElementAt(1).AssessorName.Should().Be("assessor2");
            result.Items.ElementAt(1).Status.Description.Should().Be("progress");
            result.Items.ElementAt(1).RoleId.Should().Be((int)RoleIdEnum.DecisionMaker);
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentsWithDecisionOver50Chars_Only50CharsShouldBeDisplayed()
        {
            var assessmentsList = new List<Assessment> { 
                new Assessment(){
                    Stage1DecisionToBeMade = "12345678901234567890123456789012345678901234567890111"
                },
                 new Assessment(){
                    Stage1DecisionToBeMade = "123"
                },
            };

            var assessment = new Assessments()
            {
                Items = assessmentsList
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessment);
            result.Items.ElementAt(0).Stage1DecisionToBeMade.Should().Be("12345678901234567890123456789012345678901234567890");
            result.Items.ElementAt(1).Stage1DecisionToBeMade.Should().Be("123");
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenEmptyPatientId_ArgumentExceptionExpected()
        {
            _builder.Invoking(x => x.BuildAssessmentListViewModel(Guid.Empty, A.Dummy<Assessments>()))
                .ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessment_StageDescriptionStylePropertyShouldBeSet()
        {
            var patient = A.Fake<Patient>();
            var rolelist = new List<Role> {new Role()};
            var roles = new Roles {Items = rolelist};
            var result = _builder.BuildAssessmentViewModel(patient, roles);

            result.Stage1DecisionClearlyMade.ShouldBeEquivalentTo(false);
            result.Stage1InfoText.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionPromptText);
            result.Stage1DecisionConfirmation.ShouldBeEquivalentTo(ApplicationStringConstants.DecisionConfirmationText);
            result.CurrentWorkflowQuestionId.Should().NotHaveValue();
            result.StageDescriptionStyle.Should().Be("section1");
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenNullAssessmentModel_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildAssessmentViewModel(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessmentModel_AssessmentViewModelPropertiesShouldBeSet()
        {
            var assessmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var clinicalSystemName = "ClinicalSystemName";
            var assessment = Assessment(assessmentId, patientId, A<int>._);
            assessment.TerminatedAssessmentReason="some reason";
            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetName()).Returns(clinicalSystemName);
            var result = _builder.BuildAssessmentViewModel(assessment);

            result.AssessmentId.ShouldBeEquivalentTo(assessmentId);
            result.Patient.PatientId.ShouldBeEquivalentTo(patientId);
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 5, 5));
            result.AssessorName.ShouldAllBeEquivalentTo(AssessorName);
            result.Status.StatusId.Should().Be(1);
            result.Stage1InfoText.ShouldAllBeEquivalentTo(Stage1InfoText);
            result.Stage1DecisionToBeMade.ShouldAllBeEquivalentTo(Stage1DecisionToBeMade);
            result.Stage1DecisionConfirmation.ShouldAllBeEquivalentTo(Stage1DecisionConfirmation);
            result.StageDescription.Should().Be(ApplicationStringConstants.Stage1Text);
            result.StageDescriptionStyle.Should().Be(ApplicationStringConstants.Stage1ShortDescription);
            result.Terminated.Should().BeTrue();
            result.ClinicalSystemName.ShouldBeEquivalentTo(clinicalSystemName);
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessmentModelWithNoTerminationReason_AssessmentViewModelPropertiesShouldBeSet()
        {
            var assessmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var assessment = Assessment(assessmentId, patientId, A<int>._);
            assessment.TerminatedAssessmentReason = string.Empty;

            var result = _builder.BuildAssessmentViewModel(assessment);

            result.AssessmentId.ShouldBeEquivalentTo(assessmentId);
            result.Patient.PatientId.ShouldBeEquivalentTo(patientId);
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 5, 5));
            result.AssessorName.ShouldAllBeEquivalentTo(AssessorName);
            result.Status.StatusId.Should().Be(1);
            result.Stage1InfoText.ShouldAllBeEquivalentTo(Stage1InfoText);
            result.Stage1DecisionToBeMade.ShouldAllBeEquivalentTo(Stage1DecisionToBeMade);
            result.Stage1DecisionConfirmation.ShouldAllBeEquivalentTo(Stage1DecisionConfirmation);
            result.StageDescription.Should().Be(ApplicationStringConstants.Stage1Text);
            result.StageDescriptionStyle.Should().Be(ApplicationStringConstants.Stage1ShortDescription);
            result.Terminated.Should().BeFalse();
        }


        [TestMethod]
        public void BuildAssessmentListViewModel_GivenClinicalSystemIdDescription_ClinicalSystemIdDescriptionPropertyShouldBeSet()
        {
            var assessments = new Assessments()
            {
                Items = A.Fake<IEnumerable<Assessment>>()
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.ClinicalIdDescription.Should().Be(Description);
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsNotComplete_CanViewPdfShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.InProgress
                    }
                }
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanViewPdf.Should().BeFalse();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsNotComplete_CanReviseShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.InProgress
                    }
                }
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRevise.Should().BeFalse();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsAssignedAssessor_CanViewPdfShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(AssessorDomainName);

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanViewPdf.Should().BeTrue();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsAssignedAssessor_CanReviseShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(AssessorDomainName);

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRevise.Should().BeTrue();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsNotComplete_CanViewPDFShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.InProgress
                    }
                }
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanViewPdf.Should().BeFalse();

        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsNotComplete_CanViewReviseBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.InProgress
                    }
                }
            };

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRevise.Should().BeFalse();

        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsNotAssignedAssessorOrAdmin_CanViewPdfShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanViewPdf.Should().BeFalse();
   
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsNotAssignedAssessorOrAdmin_CanReviseShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRevise.Should().BeFalse();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsNotAssignedAssorButIsAdmin_CanViewPdfShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanViewPdf.Should().BeTrue();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsCompleteAndAssessorIsNotAssignedAssorButIsAdmin_CanReviseShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        StatusId = (int)AssessmentStatusEnum.Complete,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRevise.Should().BeTrue();
        }


        [TestMethod]
        public void BuildAssessmentListViewModel_GivenAssessmentIsComplete_CanRestartPropertyShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        AssessorDomainName = AssessorDomainName,
                        StatusId = (int)AssessmentStatusEnum.Complete
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(AssessorDomainName);

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRestart.Should().BeFalse();

        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenUserIsAdministrator_CanRestartPropertyShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRestart.Should().BeTrue();
        }


        [TestMethod]
        public void BuildAssessmentListViewModel_GivenUserIsAssessor_CanRestartPropertyShouldBeTrue()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(AssessorDomainName);

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRestart.Should().BeTrue();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenUserIsNotAssessor_CanRestartPropertyShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRestart.Should().BeFalse();
        }

        [TestMethod]
        public void BuildAssessmentListViewModel_GivenUserIsNotAdministratorAndNotAssessor_CanRestartPropertyShouldBeFalse()
        {
            var assessments = new Assessments()
            {
                Items = new List<Assessment>()
                {
                    new Assessment()
                    {
                        Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                        AssessorDomainName = AssessorDomainName
                    }
                }
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("name1");

            var result = _builder.BuildAssessmentListViewModel(Guid.NewGuid(), assessments);

            result.Items.First().CanRestart.Should().BeFalse();
        }

        [TestMethod]
        public void BuildAssessmentUpdateCommand_GivenAssessmentViewModelIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildUpdateAssessmentCommand(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentUpdateCommand_GivenValidAssessmentViewModel_PropertiesShouldBeMapped()
        {
            var assessmentId = Guid.NewGuid();
            var assessmentDate = new DateTime(2015, 1, 1);

            var model = new AssessmentViewModel()
            {
                AssessmentId = assessmentId,
                DateAssessmentStarted = assessmentDate,
                Stage1DecisionToBeMade = Stage1DecisionToBeMade
            };

            var result = _builder.BuildUpdateAssessmentCommand(model);

            result.AssessmentId.Should().Be(assessmentId);
            result.DateAssessmentStarted.Should().Be(assessmentDate);
            result.Stage1DecisionToBeMade.Should().Be(Stage1DecisionToBeMade);
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenValidAssessmentAndPatient_PatientSummaryPropertyShouldBeMapped()
        {
            var model = new Assessment()
            {
                Patient = new Patient()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    ClinicalSystemId = ClinicalSystemId,
                    Gender = new Gender()
                    {
                        Description = Genderdescription
                    }
                }
            };

            var roles = Roles(A<int>._, A<string>._, A<int>._, A<string>._);

            var result = _builder.BuildAssessmentViewModel(model, roles);

            result.PatientSummary.Should()
                .Be(string.Format("{0} {1}, {2}, {3}", FirstName, LastName, ClinicalSystemId, Genderdescription));
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessmentAndNullRoles_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildAssessmentViewModel(A.Fake<Assessment>(), null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessmentAndRoles_RolesShouldBeMapped()
        {
            var assessment = new Assessment()
            {
                Patient = Patient()
            };

            const string description1 = "description1";
            const string description2 = "description2";
            const int roleId1 = 1;
            const int roleId2 = 2;

            var roles = Roles(roleId1, description1, roleId2, description2);

            var result = _builder.BuildAssessmentViewModel(assessment, roles);

            result.Roles.ElementAt(0).Text.ToLower().Should().Be("select role");
            result.Roles.ElementAt(0).Value.Should().BeEmpty();
            result.Roles.ElementAt(2).Text.Should().Be(description2);
            result.Roles.ElementAt(2).Value.Should().Be(roleId2.ToString());
            result.Roles.ElementAt(1).Text.Should().Be(description1);
            result.Roles.ElementAt(1).Value.Should().Be(roleId1.ToString());
            result.Roles.ElementAt(2).Text.Should().Be(description2);
            result.Roles.ElementAt(2).Value.Should().Be(roleId2.ToString());
        }

        

        [TestMethod]
        public void BuildAssessmentViewModel_GivenAssessmentAndRoles_AssessmentPropertiesShouldBeMapped()
        {
            var assessmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            const int roleId = (int)RoleIdEnum.Assessor;

            var assessment = Assessment(assessmentId, patientId, roleId);

            var roles = Roles(A<int>._, A<string>._, A<int>._, A<string>._);

            var result = _builder.BuildAssessmentViewModel(assessment, roles);

            result.AssessmentId.ShouldBeEquivalentTo(assessmentId);
            result.Patient.PatientId.ShouldBeEquivalentTo(patientId);
            result.DateAssessmentStarted.ShouldBeEquivalentTo(new DateTime(2015, 5, 5));
            result.AssessorName.ShouldAllBeEquivalentTo(AssessorName);
            result.Status.StatusId.Should().Be(1);
            result.Stage1InfoText.ShouldAllBeEquivalentTo(Stage1InfoText);
            result.Stage1DecisionToBeMade.ShouldAllBeEquivalentTo(Stage1DecisionToBeMade);
            result.Stage1DecisionConfirmation.ShouldAllBeEquivalentTo(Stage1DecisionConfirmation);
            result.StageDescription.Should().Be(ApplicationStringConstants.Stage1Text);
            result.StageDescriptionStyle.Should().Be(ApplicationStringConstants.Stage1ShortDescription);
            result.RoleId.Should().Be(roleId);
        }

        #region private

        private Roles Roles(int roleId1, string description1, int roleId2, string description2)
        {
            var roles = new Roles()
            {
                Items = new List<Role>()
                {
                    new Role() {RoleId = roleId1, Description = description1},
                    new Role() {RoleId = roleId2, Description = description2}
                }
            };
            return roles;
        }

        private Patient Patient()
        {
            return new Patient()
            {
                FirstName = FirstName,
                LastName = LastName,
                ClinicalSystemId = ClinicalSystemId,
                Gender = new Gender()
                {
                    Description = "gender"
                }
            };
        }

        private Assessment Assessment(Guid assessmentId, Guid patientId, int roleId)
        {
            
            var assessment = new Assessment()
            {
                AssessmentId = assessmentId,
                DateAssessmentStarted = new DateTime(2015, 5, 5),
                AssessorName = AssessorName,
                Stage1InfoText = Stage1InfoText,
                Stage1DecisionToBeMade = Stage1DecisionToBeMade,
                Stage1DecisionConfirmation = Stage1DecisionConfirmation,
                Patient = new Patient()
                {
                    PatientId = patientId,
                    Gender = new Gender()
                    {
                        Description = Genderdescription
                    }
                },
                Status = new Status()
                {
                    StatusId = 1
                },
                RoleId = roleId
            };
            return assessment;
        }

        #endregion
    }
}
