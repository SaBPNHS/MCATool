using System;
using FluentAssertions;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using TechTalk.SpecFlow;

namespace Sfw.Sabp.Mca.Specflow.Tests
{
    [Binding]
    public class StageSteps : BaseStageStepDefinitions
    {
        [Given(@"I have answered (.*) with (.*)")]
        public void GivenIHaveAnsweredWith(Guid p0, Guid p1)
        {
            var queryHandler = ScenarioContext.Current.Get<WorkflowStepByVersionAndQuestionOptionQueryHandler>();

            var query = new WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery()
            {
                CurrentWorkflowQuestionId = p0,
                QuestionOptionId = p1,
                WorkflowVersionId = Guid.Parse("69C13E49-E05A-4185-B9B2-CCED15D99694")
            };

            ScenarioContext.Current.Set(queryHandler.Retrieve(query));
        }

        [Then(@"the next question should be (.*)")]
        public void ThenTheNextQuestionShouldBe(Guid p0)
        {
            var step = ScenarioContext.Current.Get<WorkflowStep>();

            if (p0.Equals(Guid.Empty))
                step.NextWorkflowQuestionId.Should().NotHaveValue();
            else
                step.NextWorkflowQuestionId.ShouldBeEquivalentTo(p0);
        }

        [Then(@"the next question outcome should be (.*)")]
        public void ThenTheNextQuestionOutcomeShouldBe(AssessmentStatusEnum p0)
        {
            var step = ScenarioContext.Current.Get<WorkflowStep>();

            step.OutcomeStatusId.Should().Be((int) p0);
        }
    }
}
