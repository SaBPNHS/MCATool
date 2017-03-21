using System;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public interface IWorkflowStepHelper
    {
        WorkflowStep GetWorkflowStep(Guid questionOptionId, Assessment assessment);
    }
}