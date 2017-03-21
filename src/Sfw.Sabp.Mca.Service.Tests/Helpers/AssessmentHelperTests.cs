using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Helpers
{
    [TestClass]
    public class AssessmentHelperTests
    {
        private IQueryDispatcher _queryDispatcher;
        private ICommandDispatcher _commandDispatcher;
        private AssessmentHelper _assessmentHelper;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();

            _assessmentHelper = new AssessmentHelper(_queryDispatcher, _commandDispatcher);
        }

        [TestMethod]
        public void GetAssessment_GivenAssessmentId_QueryDispatcherShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentHelper.GetAssessment(assessmentId);

            A.CallTo(() =>
                    _queryDispatcher.Dispatch<AssessmentByIdQuery, Assessment>(
                        A<AssessmentByIdQuery>.That.Matches(x => x.AssessmentId == assessmentId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void UpdateAssessmentQuestions_GivenInputs_CommandDispatcherShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            var nextQuestionId = Guid.NewGuid();
            var previousQuestionId = Guid.NewGuid();
            var resetQuestionId = Guid.NewGuid();
            
            _assessmentHelper.UpdateAssessmentQuestions(assessmentId, nextQuestionId, previousQuestionId, resetQuestionId);

            A.CallTo(() => _commandDispatcher.Dispatch(A<UpdateAssessmentQuestionsCommand>.That.Matches(x => x.AssessmentId == assessmentId &&
                x.NextQuestionId == nextQuestionId &&
                x.PreviousQuestionId == previousQuestionId &&
                x.ResetWorkflowQuestionId == resetQuestionId))).MustHaveHappened();
        }

        [TestMethod]
        public void UpdateAssessmentStatus_GivenInputs_CommandDispatcherShouldBeCalled()
        {
            const AssessmentStatusEnum status = AssessmentStatusEnum.Complete;
            var assessmentId = Guid.NewGuid();

            _assessmentHelper.UpdateAssessmentStatus(assessmentId, status);

            A.CallTo(() => _commandDispatcher.Dispatch(A<UpdateAssessmentStatusCommand>.That.Matches(x => x.AssessmentId == assessmentId &&
                x.StatusId == (int)status))).MustHaveHappened();
        }

        [TestMethod]
        public void UpdateAssessmentReadonly_GivenInputs_CommandDispatcherShouldBeCalled()
        {
            var assessmentId = Guid.NewGuid();
            const bool readOnly = true;

            _assessmentHelper.UpdateAssessmentReadonly(assessmentId, readOnly);

            A.CallTo(() => _commandDispatcher.Dispatch(A<UpdateAssessmentReadOnlyCommand>.That.Matches(x => x.AssessmentId == assessmentId && x.ReadOnly == readOnly))).MustHaveHappened();
        }

        [TestMethod]
        public void GetAssessmentsByPatentId_AssessmentsShouldBeReturned()
        {
            A.CallTo(
                () =>
                    _queryDispatcher.Dispatch<AssessmentsByPatientIdQuery, Assessments>(A<AssessmentsByPatientIdQuery>._))
                .Returns(new Assessments());

            var result = _assessmentHelper.GetAssessmentsByPatient(A<Guid>._);

            result.Should().BeOfType<Assessments>();
        }

        [TestMethod]
        public void GetAssessmentsbyPatientId_GivenPatientId_QueryDispatcherShouldBeCalled()
        {
            var patientId = Guid.NewGuid();

            _assessmentHelper.GetAssessmentsByPatient(patientId);

            A.CallTo(() => _queryDispatcher.Dispatch<AssessmentsByPatientIdQuery, Assessments>(A<AssessmentsByPatientIdQuery>.That.Matches(x => x.PatientId == patientId))).MustHaveHappened();
        }
    }
}
