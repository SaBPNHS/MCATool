using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Service.Workflow;
using System;
using System.Collections.Generic;

namespace Sfw.Sabp.Mca.Service.Tests.Workflow
{
    [TestClass]
    public class WorkflowHandlerTests
    {
        private WorkFlowHandler _workFlowHandler;
        private IQueryDispatcher _queryDispatcher;
        private ICommandDispatcher _commandDispatcher;
        private IUnitOfWork _unitOfWork;
        private IAssessmentHelper _assessmentHelper;
        private IQuestionAnswerHelper _questionAnswerHelper;
        private IWorkflowStepHelper _workflowStepHelper;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _assessmentHelper = A.Fake<IAssessmentHelper>();
            _questionAnswerHelper = A.Fake<IQuestionAnswerHelper>();
            _workflowStepHelper = A.Fake<IWorkflowStepHelper>();

            _workFlowHandler = new WorkFlowHandler(_unitOfWork, _commandDispatcher, _queryDispatcher, _assessmentHelper, _questionAnswerHelper, _workflowStepHelper);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenQuestionOptionId_WorkflowStepShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var questionOptionId = Guid.NewGuid();
            var workflowVersionId = Guid.NewGuid();
            var currentWorkflowQuestionId = Guid.NewGuid();

            var assessment = new Assessment() { AssessmentId = assessmentId, WorkflowVersionId = workflowVersionId, CurrentWorkflowQuestionId = currentWorkflowQuestionId };
            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId))
                .Returns(assessment);

            _workFlowHandler.SetAssessmentNextStep(assessmentId, questionOptionId, A<string>._);

            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(questionOptionId, assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenQuestionOptionIdAndWorkflowStepIsNotFound_WorkflowStepNotFoundExceptionExpected()
        {
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(null);

            _workFlowHandler.Invoking(x => x.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._)).ShouldThrow<WorkflowStepNotFoundException>();
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenNewWorkflowStep_UpdateAssessmentNextQuestionCommandHandlerShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var questionOptionId = Guid.NewGuid();
            var workflowVersionId = Guid.NewGuid();
            var nextQuestionId = Guid.NewGuid();
            var currentWorkflowQuestionId = Guid.NewGuid();

            var assessment = new Assessment() { AssessmentId = assessmentId, WorkflowVersionId = workflowVersionId, CurrentWorkflowQuestionId = currentWorkflowQuestionId };
            var workflowStep = new WorkflowStep() { WorkflowVersionId = workflowVersionId, QuestionOptionId = questionOptionId, NextWorkflowQuestionId = nextQuestionId };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            _workFlowHandler.SetAssessmentNextStep(assessmentId, questionOptionId, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, nextQuestionId, currentWorkflowQuestionId, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenUpdates_UnitOfWorkSaveChangesShouldBeCalled()
        {
            _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentWorkflow_GivenAssessment_CurrentWorkflowVersionShouldBeRetrieved()
        {
            _workFlowHandler.SetAssessmentWorkflow(A.Dummy<AddAssessmentCommand>());

            A.CallTo(() => _queryDispatcher.Dispatch<CurrentWorkflowQuery, WorkflowVersion>(A<CurrentWorkflowQuery>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentWorkflow_GivenAssessment_AddAssessmentCommandHandlerShouldBeCalled()
        {
            var workflowVersionId = Guid.NewGuid();
            var initialQuestionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var assessmentCommand = new AddAssessmentCommand() {AssessmentId = assessmentId};
            var currentWorklow = new WorkflowVersion() {WorkflowVersionId = workflowVersionId, InitialWorkflowQuestionId = initialQuestionId};

            A.CallTo(() => _queryDispatcher.Dispatch<CurrentWorkflowQuery, WorkflowVersion>(A<CurrentWorkflowQuery>._))
                .Returns(currentWorklow);

            _workFlowHandler.SetAssessmentWorkflow(assessmentCommand);

            A.CallTo(() => _commandDispatcher.Dispatch(A<AddAssessmentCommand>.That.Matches(x => x.AssessmentId == assessmentId && x.CurrentWorkflowQuestionId == initialQuestionId && x.WorkflowVersionId == workflowVersionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentWorkflow_GivenAssessment_ChangesShouldBeSaved()
        {
            _workFlowHandler.SetAssessmentWorkflow(A.Dummy<AddAssessmentCommand>());

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessment_QuestionAnswerCommandHandlerShouldBeCalled()
        {
            var optionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();
            const string furtherInfo = "info";

            var assessment = new Assessment() { AssessmentId = assessmentId, CurrentWorkflowQuestionId = currentQuestionId };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _workFlowHandler.SetAssessmentNextStep(A<Guid>._, optionId, furtherInfo);

            A.CallTo(() => _questionAnswerHelper.AddQuestionAnswer(optionId, furtherInfo, assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CompleteAssessment_GivenAssessment_UpdateAssessmentStatusCommandHandlerShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            const AssessmentStatusEnum statusId = AssessmentStatusEnum.Complete;

            _workFlowHandler.CompleteAssessment(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentStatus(assessmentId, statusId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void CompleteAssessment_GivenAssessment_ChangesShouldBeSaved()
        {
            _workFlowHandler.CompleteAssessment(A.Dummy<Guid>());

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessment_AssessmentStatusShouldBeUpdated()
        {
            var assessmentId = Guid.NewGuid();
            var questionOptionId = Guid.NewGuid();

            var assessment = new Assessment() {AssessmentId = assessmentId, WorkflowVersionId = Guid.NewGuid()};
            var workflowStep = new WorkflowStep(){ OutcomeStatusId = 1};

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

             A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._ )).Returns(workflowStep);

            _workFlowHandler.SetAssessmentNextStep(assessmentId, questionOptionId, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentStatus(assessmentId, AssessmentStatusEnum.InProgress)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessment_AssessmentStatusShouldBeReturned()
        {
            var assessmentId = Guid.NewGuid();
            var questionOptionId = Guid.NewGuid();

            var workflowStep = new WorkflowStep() { OutcomeStatusId = 1 };

            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            var result = _workFlowHandler.SetAssessmentNextStep(assessmentId, questionOptionId, A<string>._);

            result.ShouldBeEquivalentTo(AssessmentStatusEnum.InProgress);
        }

        [TestMethod]
        public void SetAssessmentPreviousStep_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _workFlowHandler.SetAssessmentPreviousStep(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentPreviousStep_GivenAssessmentAndPreviousQuestionIsNotNull_AssessmentCurrentQuestionShouldBeSetToPreviousQuestion()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();
            var previousQuestionId = Guid.NewGuid();

            var assessment = new Assessment()
            {
                AssessmentId = assessmentId,
                CurrentWorkflowQuestionId = currentQuestionId,
                PreviousWorkflowQuestionId = previousQuestionId
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);

            _workFlowHandler.SetAssessmentPreviousStep(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, previousQuestionId, previousQuestionId, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentPreviousStep_GivenAssessmentAndPreviousQuestionIsNull_AssessmentCurrentQuestionShouldNotBeSet()
        {
            var assessmentId = Guid.NewGuid();
            var previousQuestionId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();

            var assessment = new Assessment()
            {
                AssessmentId = assessmentId,
                CurrentWorkflowQuestionId = currentQuestionId,
                PreviousWorkflowQuestionId = null
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);

            _workFlowHandler.SetAssessmentPreviousStep(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, previousQuestionId, null, null)).MustNotHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentAndQuestionHasNotBeenAnsweredBefore_PreviousAnswerShouldNotBeRemoved()
        {
            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(null);

            _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>._)).MustNotHaveHappened();
        }


        [TestMethod]
        public void SetAssessmentPreviousStep_GivenAssessment_ChangesShouldBeSaved()
        {
            _workFlowHandler.SetAssessmentPreviousStep(A.Dummy<Guid>());

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(QuestionAnswers(Guid.NewGuid(), Guid.NewGuid()));

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
                .Returns(new WorkflowStage() { InitialWorkflowQuestionId = Guid.NewGuid()});

            _workFlowHandler.ResetAssessmentStage(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_AssessmentCurrentStageShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var workflowStageId = Guid.NewGuid();

            var assessment = new Assessment
            {
                AssessmentId = assessmentId,
                WorkflowQuestion = new WorkflowQuestion
                {
                    WorkflowStage = new WorkflowStage
                    {
                        WorkflowStageId = workflowStageId,
                        InitialWorkflowQuestionId = Guid.NewGuid()
                    }
                }
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(assessment);

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
                .Returns(new WorkflowStage {InitialWorkflowQuestionId = Guid.NewGuid()});

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(QuestionAnswers(Guid.NewGuid(), Guid.NewGuid()));

            _workFlowHandler.ResetAssessmentStage(assessmentId);

            A.CallTo(() => 
                _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>.That.Matches(x => x.WorkflowStageId == workflowStageId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_ChangesShouldBeSaved()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
                .Returns(new WorkflowStage() { InitialWorkflowQuestionId = Guid.NewGuid() });

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(QuestionAnswers(Guid.NewGuid(), Guid.NewGuid()));

            _workFlowHandler.ResetAssessmentStage(A.Dummy<Guid>());

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_AssessmentStatusQuestionsShouldBeSet()
        {
            var assessmentId = Guid.NewGuid();

            var stageInitialQuestionId = Guid.NewGuid();

            var workflowStage = new WorkflowStage
            {
                InitialWorkflowQuestionId = stageInitialQuestionId
            };

            A.CallTo(() =>
                _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
                .Returns(workflowStage);

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(QuestionAnswers(Guid.NewGuid(), Guid.NewGuid()));

            _workFlowHandler.ResetAssessmentStage(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, stageInitialQuestionId, null, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessmentWithInvalidInitialStageQuestion_WorkflowStageInitialQuestionNotFoundExceptionExpected()
        {
            _workFlowHandler.Invoking(x => x.ResetAssessmentStage(A<Guid>._)).ShouldThrow<WorkflowStageInitialQuestionNotFoundException>();
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_AssessmentStageQuestionAnswersShouldBeRetrieved()
        {
            var workflowStageId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).Returns(new Assessment() {AssessmentId = assessmentId, WorkflowQuestion = new WorkflowQuestion() {WorkflowStage = new WorkflowStage()}});

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
                .Returns(new WorkflowStage() {WorkflowStageId = workflowStageId, InitialWorkflowQuestionId = Guid.NewGuid()});

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(QuestionAnswers(Guid.NewGuid(), Guid.NewGuid(), assessmentId));

            _workFlowHandler.ResetAssessmentStage(assessmentId);

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(A<QuestionAnswersByWorkflowStageIdQuery>
                .That.Matches(x => x.WorkflowStageId == workflowStageId && x.AssessmentId == assessmentId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessmentStage_GivenAssessment_AssessmentStageQuestionAnswersShouldBeRemoved()
        {
            var questionOneId = Guid.NewGuid();
            var questionTwoId = Guid.NewGuid();

            var questionAnswers = QuestionAnswers(questionOneId, questionTwoId);

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStageByIdQuery, WorkflowStage>(A<WorkflowStageByIdQuery>._))
             .Returns(new WorkflowStage() { InitialWorkflowQuestionId = Guid.NewGuid() });

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>(
                        A<QuestionAnswersByWorkflowStageIdQuery>._)).Returns(questionAnswers);

            _workFlowHandler.ResetAssessmentStage(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionOneId))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionTwoId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessment_AssessmentQuestionAnswerShouldBeSavedBeforeAssessmentQuestionsAreUpdated()
        {
            using (var scope = Fake.CreateScope())
            {
                A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(new WorkflowStep(){ NextWorkflowQuestionId = Guid.NewGuid()});

                _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() =>  _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>._)).MustHaveHappened();
                    A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(A<Guid>._, A<Guid>._, A<Guid?>._, A<Guid?>._)).MustHaveHappened();
                }
            }
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentAndQuestionHasPreviouslyBeenAnswered_AssessmentQuestionAnswerShouldBeRemovedBeforeAssessmentQuestionsAreUpdated()
        {
            using (var scope = Fake.CreateScope())
            {
                A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(new WorkflowStep() { NextWorkflowQuestionId = Guid.NewGuid() });

                A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(new QuestionAnswer());

                _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>._)).MustHaveHappened();
                    A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(A<Guid>._, A<Guid>._, A<Guid?>._, A<Guid?>._)).MustHaveHappened();
                }
            }
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentAndQuestionHasPreviouslyBeenAnswered_AssessmentQuestionAnswerShouldBeRemovedBeforeAssessmentAnswerIsSaved()
        {
            using (var scope = Fake.CreateScope())
            {
                A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>(A<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery>._)).Returns(new WorkflowStep() { NextWorkflowQuestionId = Guid.NewGuid() });

                A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(new QuestionAnswer());

                _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() =>  _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>._)).MustHaveHappened();
                    A.CallTo(() => _questionAnswerHelper.AddQuestionAnswer(A<Guid>._, A<string>._, A<Assessment>._)).MustHaveHappened();
                }
            }
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _workFlowHandler.ResetAssessment(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessment_AssessmentQuestionsShouldBeSet()
        {
            var initialWorkflowQuestionId = Guid.NewGuid();
            var currentWorkflowQuestionId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var assessment = new Assessment()
            {
                AssessmentId = assessmentId,
                CurrentWorkflowQuestionId = currentWorkflowQuestionId,
                WorkflowVersion = new WorkflowVersion()
                {
                    InitialWorkflowQuestionId = initialWorkflowQuestionId
                }
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _workFlowHandler.ResetAssessment(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, initialWorkflowQuestionId, null, currentWorkflowQuestionId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessment_ChangesShouldBeSaved()
        {
            _workFlowHandler.ResetAssessment(A<Guid>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessment_AssessmentShouldBeSetToReadonly()
        {
            var assessmentId = Guid.NewGuid();

            _workFlowHandler.ResetAssessment(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentReadonly(assessmentId, true)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentIsReadonly_QuestionAnswerShouldNotBeSaved()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(new Assessment() {ReadOnly = true});

            _workFlowHandler.SetAssessmentNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _commandDispatcher.Dispatch(A<AddQuestionAnswerCommand>._)).MustNotHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentIsReadonlyAndAssessmentHasBeenProgressedToWhereItWasStopped_ReadonlyShouldBeRemoved()
        {
            var assessmentId = Guid.NewGuid();
            var nextRestWorkflowQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment() { ReadOnly = true, ResetWorkflowQuestionId = nextRestWorkflowQuestionId });

            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(new WorkflowStep()
                        {
                            NextWorkflowQuestionId = nextRestWorkflowQuestionId
                        });

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentReadonly(assessmentId, false)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentIsReadonlyAndAssessmentHasBeenProgressedToWhereItWasStopped_ResetQuestionIdShouldBeNull()
        {
            var assessmentId = Guid.NewGuid();
            var nextWorkflowQuestionId = Guid.NewGuid();
            var currentWorkflowQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment() { ReadOnly = true, ResetWorkflowQuestionId = nextWorkflowQuestionId, CurrentWorkflowQuestionId = currentWorkflowQuestionId });

            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(new WorkflowStep()
                        {
                            NextWorkflowQuestionId = nextWorkflowQuestionId
                        });

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, nextWorkflowQuestionId, currentWorkflowQuestionId, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentIsReadonlyAndAssessmentHasNotBeenProgressedToWhereItWasStopped_ResetQuestionIdShouldNotBeReset()
        {
            var assessmentId = Guid.NewGuid();
            var nextWorkflowQuestionId = Guid.NewGuid();
            var currentWorkflowQuestionId = Guid.NewGuid();
            var resetQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment() { ReadOnly = true, ResetWorkflowQuestionId = resetQuestionId, CurrentWorkflowQuestionId = currentWorkflowQuestionId });

            A.CallTo(() =>
                    _queryDispatcher.Dispatch<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>(
                        A<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery>._)).Returns(new WorkflowStep()
                        {
                            NextWorkflowQuestionId = nextWorkflowQuestionId
                        });

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, nextWorkflowQuestionId, currentWorkflowQuestionId, null)).MustNotHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentIsReadonlyAndAssessmentHasNotBeenProgressedToWhereItWasStopped_ReadonlyShouldNotBeRemoved()
        {
            var assessmentId = Guid.NewGuid();
            var nextRestWorkflowQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment() { ReadOnly = true, ResetWorkflowQuestionId = Guid.NewGuid() });

            A.CallTo(() =>
                    _queryDispatcher.Dispatch<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>(
                        A<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery>._)).Returns(new WorkflowStep()
                        {
                            NextWorkflowQuestionId = nextRestWorkflowQuestionId
                        });

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentReadonly(assessmentId, false)).MustNotHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessment_QuestionAnswerByAssessmentAndQuestionIdQueryShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var workflowQuestionId = Guid.NewGuid();

            var assessment = new Assessment()
            {
                AssessmentId = assessmentId,
                CurrentWorkflowQuestionId = workflowQuestionId
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>.That.Matches(x => x.AssessmentId == assessmentId &&
                    x.CurrentWorkflowQuestionId == workflowQuestionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentNextStep_GivenAssessmentAndQuestionHasBeenAnsweredBefore_PreviousAnswerShouldBeRemoved()
        {
            var assessmentId = Guid.NewGuid();
            var questionAnswerId = Guid.NewGuid();

            var questionAnswer = new QuestionAnswer
            {
                QuestionAnswerId = questionAnswerId
            };

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(questionAnswer);

            _workFlowHandler.SetAssessmentNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionAnswerId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessmentHasBreakStatus_ThenLastAnsweredQuestion_ShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    AssessmentId = assessmentId,
                    CurrentWorkflowQuestionId = currentQuestionId,
                    WorkflowVersion = A.Fake<WorkflowVersion>(),
                    StatusId = (int)AssessmentStatusEnum.Break
                });

            _workFlowHandler.ResetAssessment(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>.That.Matches(x => x.AssessmentId == assessmentId && x.CurrentWorkflowQuestionId == currentQuestionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessmentDoesNotHaveBreakStatus_ThenLastAnsweredQuestion_ShouldNotBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    AssessmentId = assessmentId,
                    CurrentWorkflowQuestionId = currentQuestionId,
                    WorkflowVersion = A.Fake<WorkflowVersion>(),
                    StatusId = (int)AssessmentStatusEnum.InProgress
                });

            _workFlowHandler.ResetAssessment(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.AssessmentId == assessmentId && x.WorkflowQuestionId == currentQuestionId))).MustNotHaveHappened();

        }

        [TestMethod]
        public void ResetAssessment_GivenAssessmentHasBreakStatus_ThenLastAnsweredQuestionShouldBeRemoved()
        {
            var questionAnswerId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
               .Returns(new Assessment()
               {
                   WorkflowVersion = A.Fake<WorkflowVersion>(),
                   StatusId = (int)AssessmentStatusEnum.Break
               });

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(new QuestionAnswer()
                {
                    QuestionAnswerId = questionAnswerId
                });

            _workFlowHandler.ResetAssessment(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionAnswerId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void ResetAssessment_GivenAssessmentIsNotBreakStatus_ThenLastAnsweredQuestion_ShouldNotBeRemoved()
        {
            var questionAnswerId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
               .Returns(new Assessment()
               {
                   WorkflowVersion = A.Fake<WorkflowVersion>(),
                   StatusId = (int)AssessmentStatusEnum.InProgress
               });

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._))
                .Returns(new QuestionAnswer()
                {
                    QuestionAnswerId = questionAnswerId
                });

            _workFlowHandler.ResetAssessment(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionAnswerId))).MustNotHaveHappened();
        }

        [TestMethod]
        public void RestartBreak_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    StatusId = (int)AssessmentStatusEnum.Break,
                    PreviousWorkflowQuestionId = Guid.NewGuid()
                });

            _workFlowHandler.RestartBreak(assessmentId);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartBreak_GivenAssessmentDoesNotHaveBreakStatus_WorkflowInvalidStatusExceptionExpected()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    StatusId = (int) AssessmentStatusEnum.InProgress
                });

            _workFlowHandler.Invoking(x => x.RestartBreak(A<Guid>._)).ShouldThrow<WorkflowInvalidStatusException>();
        }

        [TestMethod]
        public void RestartBreak_GivenAssessment_AssessmentQuestionsShouldBeSet()
        {
            var currentQuestionId = Guid.NewGuid();
            var previousQuestionid = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    StatusId = (int)AssessmentStatusEnum.Break,
                    CurrentWorkflowQuestionId = currentQuestionId,
                    PreviousWorkflowQuestionId = previousQuestionid,
                    AssessmentId = assessmentId
                });

            _workFlowHandler.RestartBreak(assessmentId);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessmentId, previousQuestionid, previousQuestionid, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartBreak_GivenAssessmentWithInvalidPreviousQuestion_InvalidAssessmentQuestionExceptionExpected()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
                .Returns(new Assessment()
                {
                    StatusId = (int)AssessmentStatusEnum.Break,
                    PreviousWorkflowQuestionId = null
                });

            _workFlowHandler.Invoking(x => x.RestartBreak(A<Guid>._)).ShouldThrow<InvalidAssessmentQuestionException>();
        }

        [TestMethod]
        public void RestartBreak_GivenAssessment_ChangesShouldBeSaved()
        {
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
               .Returns(new Assessment()
               {
                   StatusId = (int)AssessmentStatusEnum.Break,
                   CurrentWorkflowQuestionId = Guid.NewGuid(),
                   PreviousWorkflowQuestionId = Guid.NewGuid()
               });

            _workFlowHandler.RestartBreak(A<Guid>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartBreak_GivenAssessment_LastAnsweredQuestionShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
              .Returns(new Assessment()
              {
                  StatusId = (int)AssessmentStatusEnum.Break,
                  CurrentWorkflowQuestionId = currentQuestionId,
                  PreviousWorkflowQuestionId = Guid.NewGuid(),
                  AssessmentId = assessmentId
              });

            _workFlowHandler.RestartBreak(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>.That.Matches(x => x.AssessmentId == assessmentId && x.CurrentWorkflowQuestionId  == currentQuestionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void RestartBreak_GivenAssessment_LastAnsweredQuestionShouldBeRemoved()
        {
            var assessmentId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();
            var questionAnswerId = Guid.NewGuid();

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._))
              .Returns(new Assessment()
              {
                  StatusId = (int)AssessmentStatusEnum.Break,
                  CurrentWorkflowQuestionId = currentQuestionId,
                  PreviousWorkflowQuestionId = Guid.NewGuid(),
                  AssessmentId = assessmentId
              });

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Assessment>._)).Returns(new QuestionAnswer()
              {
                  QuestionAnswerId = questionAnswerId
              });

            _workFlowHandler.RestartBreak(A<Guid>._);

            A.CallTo(() => _questionAnswerHelper.RemoveQuestionAnswer(A<QuestionAnswer>.That.Matches(x => x.QuestionAnswerId == questionAnswerId))).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [TestMethod]
        public void UpdateAssessmentWorkflowQuestion_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = Guid.NewGuid()
            };

            _workFlowHandler.UpdateAssessmentWorkflowQuestion(command);

            A.CallTo(() => _assessmentHelper.GetAssessment(command.AssessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void UpdateAssessmentWorkflowQuestion_GivenAssessment_WorkflowVersionForAssessmentShouldBeRetrieved()
        {
            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = Guid.NewGuid()
            };

            var assessment = new Assessment()
            {
                WorkflowVersionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _workFlowHandler.UpdateAssessmentWorkflowQuestion(command);

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowVersionByIdQuery, WorkflowVersion>(A<WorkflowVersionByIdQuery>.That.Matches(x => x.WorkflowVersionId == assessment.WorkflowVersionId))).MustHaveHappened(Repeated.Exactly.Once);
        }


        [TestMethod]
        public void UpdateAssessmentWorkflowQuestion_GivenAssessment_AssessmentWorkflowQuestionsShouldBeUpdated()
        {
            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = Guid.NewGuid()
            };

            var workflowVersion = new WorkflowVersion()
            {
                InitialWorkflowQuestionId = Guid.NewGuid()
            };

            A.CallTo(() =>_queryDispatcher.Dispatch<WorkflowVersionByIdQuery, WorkflowVersion>(A<WorkflowVersionByIdQuery>._))
                .Returns(workflowVersion);

            _workFlowHandler.UpdateAssessmentWorkflowQuestion(command);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(command.AssessmentId, workflowVersion.InitialWorkflowQuestionId, null, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void UpdateAssessmentWorkflowQuestion_GivenAssessment_AssessmentShouldBeUpdated()
        {
            var command = new UpdateAssessmentCommand()
            {
                AssessmentId = Guid.NewGuid()
            };

            _workFlowHandler.UpdateAssessmentWorkflowQuestion(command);

            A.CallTo(() => _commandDispatcher.Dispatch(command)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void UpdateAssessmentWorkflowQuestion_GivenAssessment_ChangesShouldBeSaved()
        {
            var command = new UpdateAssessmentCommand();

            _workFlowHandler.UpdateAssessmentWorkflowQuestion(command);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessment_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _workFlowHandler.SetAssessmentReviseNextStep(assessmentId, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessment_NextWorkflowStepShouldBeRetrieved()
        {
            var questionAnswer = new QuestionAnswer()
            {
                QuestionOptionId = Guid.NewGuid()
            };
            
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid(),
                WorkflowVersionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(A<Guid>._)).Returns(questionAnswer);

            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(questionAnswer.QuestionOptionId, assessment)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentAndWorkflowStepIsNull_WorkflowStepNotFoundExceptionExpected()
        {
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(null);

            _workFlowHandler.Invoking(x => x.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._)).ShouldThrow<WorkflowStepNotFoundException>();
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentAndQuestionAnswer_QuestionAnswerShouldBeUpdated()
        {
            var questionAnswerId = Guid.NewGuid();
            const string furtherInfo = "further";

            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, questionAnswerId, furtherInfo);

            A.CallTo(() => _questionAnswerHelper.UpdateQuestionAnswer(questionAnswerId, furtherInfo)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAnsweredQuestion_PreviousAnswerShouldBeRetrieved()
        {
            var questionAnswerId = Guid.NewGuid();

            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, questionAnswerId, A<string>._);

            A.CallTo(() => _questionAnswerHelper.GetQuestionAnswer(questionAnswerId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentAndAnswer_ChangesShouldBeSaved()
        {
            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentHasNextStep_WorkflowQuestionsShouldBeUpdated()
        {            
            var workflowStep = new WorkflowStep()
            {
                NextWorkflowQuestionId = Guid.NewGuid()
            };
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(assessment.AssessmentId, workflowStep.NextWorkflowQuestionId.Value, assessment.CurrentWorkflowQuestionId, null)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentDoesNotHaveNextStep_WorkflowQuestionsShouldNotBeUpdated()
        {
            var workflowStep = new WorkflowStep()
            {
                NextWorkflowQuestionId = null
            };
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            A.CallTo(() => _assessmentHelper.UpdateAssessmentQuestions(A<Guid>._, A<Guid>._, A<Guid?>._, A<Guid?>._)).MustNotHaveHappened();
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentHasNextStep_TrueShouldBeReturned()
        {
            var workflowStep = new WorkflowStep()
            {
                NextWorkflowQuestionId = Guid.NewGuid()
            };
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            var result = _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void SetAssessmentReviseNextStep_GivenAssessmentDoesNotHaveNextStep_FalseShouldBeReturned()
        {
            var workflowStep = new WorkflowStep()
            {
                NextWorkflowQuestionId = null
            };
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);
            A.CallTo(() => _workflowStepHelper.GetWorkflowStep(A<Guid>._, A<Assessment>._)).Returns(workflowStep);

            var result = _workFlowHandler.SetAssessmentReviseNextStep(A<Guid>._, A<Guid>._, A<string>._);

            result.Should().BeFalse();
        }

        #region private

        private QuestionAnswers QuestionAnswers(Guid questionOneId, Guid questionTwoId)
        {
            var questionAnswersList = new List<QuestionAnswer>()
            {
                new QuestionAnswer() {QuestionAnswerId = questionOneId},
                new QuestionAnswer() {QuestionAnswerId = questionTwoId},
            };

            var questionAnswers = new QuestionAnswers()
            {
                Items = questionAnswersList
            };
            return questionAnswers;
        }

        private QuestionAnswers QuestionAnswers(Guid questionOneId, Guid questionTwoId, Guid assessmentId)
        {
            var questionAnswersList = new List<QuestionAnswer>()
            {
                new QuestionAnswer() {QuestionAnswerId = questionOneId, AssessmentId = assessmentId},
                new QuestionAnswer() {QuestionAnswerId = questionTwoId, AssessmentId = assessmentId},
            };

            var questionAnswers = new QuestionAnswers()
            {
                Items = questionAnswersList
            };
            return questionAnswers;
        }

        #endregion
    }
}
