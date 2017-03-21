using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp.Pdf;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.Pdf;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfw.Sabp.Mca.Web.Tests.Pdf
{
    [TestClass]
    public class GeneratePdfTests
    {
        private GeneratePdf _pdfGenerator;
        private IDateTimeProvider _dateTimeProvider;
        private IQuestionAnswerViewModelBuilder _questionAnswerViewModelBuilder;
        private IQueryDispatcher _queryDispatcher;
        private IClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private IPdfHelper _pdfHelper;

        private const string ClinicalSystemId = "clinicalId";

        [TestInitialize]
        public void Setup()
        {
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            _questionAnswerViewModelBuilder = A.Fake<IQuestionAnswerViewModelBuilder>();
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _pdfHelper = A.Fake<IPdfHelper>();
            _clinicalSystemIdDescriptionProvider = A.Fake<IClinicalSystemIdDescriptionProvider>();

            A.CallTo(() => _clinicalSystemIdDescriptionProvider.GetDescription()).Returns(ClinicalSystemId);

            _pdfGenerator = new GeneratePdf(_dateTimeProvider, _questionAnswerViewModelBuilder, _queryDispatcher, _pdfHelper, _clinicalSystemIdDescriptionProvider);
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithEmptyAssessment_ShouldReturnError()
        {
            PdfDocument generatedPdfDocument;
            _pdfGenerator.Invoking(x => x.CreatePdfForAssessment(null, out generatedPdfDocument)).ShouldThrow<NullReferenceException>();
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithValidAssessment_ShouldCreateFileWithCorrectConvention()
        {
            var assessment = A.Dummy<Assessment>();
            PdfDocument generatedPdfDocument;
            var expectedFileName = "MCA Assessment-" + assessment.Patient.ClinicalSystemId + "-" +
                           _dateTimeProvider.Now.ToString(@"yyyy-MM-dd") + "-" + assessment.AssessmentId + ".pdf";
            var fileName = _pdfGenerator.CreatePdfForAssessment(assessment, out generatedPdfDocument);
            Assert.AreEqual(expectedFileName, fileName, "File name not as expected");
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithValidAssessmentAndEmptyDecisionMaker_ShouldCreatePdfContainingCommonInformation()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
                DateAssessmentStarted = _dateTimeProvider.Now,
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Name, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Dob, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString())).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false,false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.RoleText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Role.Description)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText, true, false)).MustNotHaveHappened();
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithValidAssessmentAndDecisionMaker_ShouldCreatePdfContainingCommonInformation()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
                DecisionMaker = "Decision maker name",
                DateAssessmentStarted = _dateTimeProvider.Now,
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Name, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Dob, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString())).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.RoleText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Role.Description)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.DecisionMaker)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithRoleChangedFromAssessorToDecisionMaker_ShouldCreatePdfContainingCommonInformationWithoutDecisionMaker()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                Role = new Role {RoleId = (int) RoleIdEnum.Assessor, Description = "A random role"},
                DecisionMaker = "Decision maker name",
                DateAssessmentStarted = _dateTimeProvider.Now,
                RoleId = (int) RoleIdEnum.DecisionMaker,
            };
            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Name, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Dob, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString())).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.RoleText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Role.Description)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText, true, false)).MustNotHaveHappened();
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.DecisionMaker)).MustNotHaveHappened();
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithValidAssessment_ShouldCreatePdfContainingSectionInformation()
        {
            PdfDocument pdfDocumentCreated;

            var questionAnswersList = new List<QuestionAnswer>
            {
                new QuestionAnswer()
                {
                    FurtherInformation = string.Empty,
                    QuestionOption = new QuestionOption()
                    {
                        Question = new Question() {Description = "First Question"},
                        Option = new Option() {Description = "Yes"},
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        WorkflowStage = new WorkflowStage(){Description = "Stage 1"}
                    }
                },
                new QuestionAnswer()
                {
                    FurtherInformation = "Further information on second question",
                    QuestionOption = new QuestionOption()
                    {
                        Question = new Question() {Description = "Second Question"},
                        Option = new Option() {Description = "No"}
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        WorkflowStage = new WorkflowStage(){Description = "Stage 2"}
                    }
                },
                new QuestionAnswer()
                {
                    FurtherInformation = "Further information on third question",
                    QuestionOption = new QuestionOption()
                    {
                        Question = new Question(){Description = "Third Question"}, 
                        Option = new Option(){Description = "Single option"}
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        WorkflowStage = new WorkflowStage(){Description = "Stage 3"}
                    }
                },
            };
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                DateAssessmentStarted = _dateTimeProvider.Now,
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
                QuestionAnswers = questionAnswersList
            };
            var questionAnswers = new QuestionAnswers()
            {
                Items = questionAnswersList
            };
            var viewModelList = new List<QuestionAnswerViewModel>
            {
                new QuestionAnswerViewModel()
                {
                    Answer = "Yes",
                    FurtherInformation = "",
                    Question = "First Question",
                    StageDescription = "Stage 1"
                },
                new QuestionAnswerViewModel()
                {
                    Answer = "No",
                    FurtherInformation = "Further information on second question",
                    Question = "Second Question",
                    StageDescription = "Stage 2"
                },
                new QuestionAnswerViewModel()
                {
                    Answer = "",
                    FurtherInformation = "Further information on third question",
                    Question = "Third Question",
                    StageDescription = "Stage 3"
                },
            };

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByAssessmentQuery, QuestionAnswers>(A<QuestionAnswersByAssessmentQuery>._)).Returns(questionAnswers);

            var viewModels = new QuestionAnswerListViewModel() {Items = viewModelList};

            A.CallTo(() => _questionAnswerViewModelBuilder.BuildQuestionAnswerListViewModel(questionAnswers)).Returns(viewModels);
            Assert.AreEqual(_questionAnswerViewModelBuilder.BuildQuestionAnswerListViewModel(questionAnswers), viewModels);

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);

            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(0).StageDescription, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(0).Question, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(viewModels.Items.ElementAt(0).Answer)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(viewModels.Items.ElementAt(0).FurtherInformation)).MustNotHaveHappened();

            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(1).StageDescription, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(1).Question, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(viewModels.Items.ElementAt(1).Answer + ". " + viewModels.Items.ElementAt(1).FurtherInformation)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(2).StageDescription, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(viewModels.Items.ElementAt(2).Question, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(viewModels.Items.ElementAt(2).FurtherInformation)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithValidAssessment_ShouldCreatePdfWithCorrectClinicalIdDescription()
        {
            var assessment = new Assessment()
            {
                Patient = A.Fake<Patient>(),
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
            };

            PdfDocument pdfDocumentCreated;
            
            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);

            A.CallTo(() => _pdfHelper.AddTitle(string.Format("{0}:", ClinicalSystemId), true, false)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithTerminatedReason_ShouldCreatePdfWithTerminatedReason()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                DateAssessmentStarted = _dateTimeProvider.Now,
                TerminatedAssessmentReason = "Assessment terminated for some reason",
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() =>_pdfHelper.AddTitle(ApplicationStringConstants.AssessmentTerminationReasonString, true,false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.TerminatedAssessmentReason)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithoutTerminatedReason_ShouldCreatePdfWithNoTerminatedReason()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                DateAssessmentStarted = _dateTimeProvider.Now,
                TerminatedAssessmentReason = string.Empty,
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(assessment.TerminatedAssessmentReason, false, false)).MustNotHaveHappened();
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithAssessmentEndDate_ShouldCreatePdfWithAssessmentCompleteDate()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                DateAssessmentStarted = _dateTimeProvider.Now,
                DateAssessmentEnded = _dateTimeProvider.Now.AddDays(1),
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Name, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Dob, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString())).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentCompleteDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.RoleText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Role.Description)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText, true, false)).MustNotHaveHappened();
        }

        [TestMethod]
        public void CreatePdfForAssessment_CalledWithNoAssessmentEndDate_ShouldCreatePdfWithNoAssessmentCompleteDate()
        {
            PdfDocument pdfDocumentCreated;

            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                Stage1DecisionToBeMade = "Decision",
                Patient =
                    new Patient()
                    {
                        FirstName = "First name",
                        LastName = "Last name",
                        DateOfBirth = new DateTime(1956, 1, 02),
                        NhsNumber = 4010232137,
                        ClinicalSystemId = "1234567890"
                    },
                AssessorName = "assessor",
                DateAssessmentStarted = _dateTimeProvider.Now,
                Role = new Role { RoleId = (int)RoleIdEnum.Assessor, Description = "A random role" },
            };

            _pdfGenerator.CreatePdfForAssessment(assessment, out pdfDocumentCreated);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.McaSubject, false, true)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Name, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.FirstName + " " + assessment.Patient.LastName)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Dob, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.DateOfBirth.ToLongDateString())).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.NhsNumber, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Patient.NhsNumber)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentStartDateText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.ClinicianNameText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.Stage1Text, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionToBeMade, false, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.WriteText(assessment.Stage1DecisionToBeMade)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.AssessmentCompleteDateText, true, false)).MustNotHaveHappened();
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.RoleText, true, false)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AppendText(" " + assessment.Role.Description)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _pdfHelper.AddTitle(ApplicationStringConstants.DecisionMakerText, true, false)).MustNotHaveHappened();
        }
    }
}
