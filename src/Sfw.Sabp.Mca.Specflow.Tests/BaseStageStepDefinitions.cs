using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using TechTalk.SpecFlow;

namespace Sfw.Sabp.Mca.Specflow.Tests
{
    public class BaseStageStepDefinitions
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            var mcaContext = new DataAccess.Ef.Mca();
            var unitOfWork = new UnitOfWork(mcaContext);

            var query = new WorkflowStepByVersionAndQuestionOptionQueryHandler(unitOfWork);

            ScenarioContext.Current.Set(query);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ScenarioContext.Current.Clear();
        }
    }
}
