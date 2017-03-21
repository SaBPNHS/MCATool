using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Helpers
{
    [TestClass]
    public class WorkflowStepHelperTests
    {
        private WorkflowStepHelper _workflowStepHelper;
        private IQueryDispatcher _queryDispatcher;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            _workflowStepHelper = new WorkflowStepHelper(_queryDispatcher);
        }

        [TestMethod]
        public void GetWorkflowStep_GivenValidInputs_QueryDispatcherShouldBeCalled()
        {
            var questionOptionId = Guid.NewGuid();
            var assessment = new Assessment()
            {
                CurrentWorkflowQuestionId = Guid.NewGuid(),
                WorkflowVersionId = Guid.NewGuid()
            };

            _workflowStepHelper.GetWorkflowStep(questionOptionId, assessment);

            A.CallTo(() => _queryDispatcher.Dispatch<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>(A<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery>.That.Matches(x => x.CurrentWorkflowQuestionId == assessment.CurrentWorkflowQuestionId
                && x.QuestionOptionId == questionOptionId 
                && x.WorkflowVersionId == assessment.WorkflowVersionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
