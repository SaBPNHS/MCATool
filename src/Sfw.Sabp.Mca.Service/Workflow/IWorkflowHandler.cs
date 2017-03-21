using System;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.Workflow
{
    public interface IWorkflowHandler
    {
        void SetAssessmentWorkflow(AddAssessmentCommand command);
        void UpdateAssessmentWorkflowQuestion(UpdateAssessmentCommand command);
        AssessmentStatusEnum SetAssessmentNextStep(Guid assessmentId, Guid questionOptionId, string furtherInfo);
        void CompleteAssessment(Guid assessmentId);
        void SetAssessmentPreviousStep(Guid assessmentId);
        void ResetAssessmentStage(Guid assessmentId);
        void ResetAssessment(Guid assessmentId);
        void RestartBreak(Guid assessmentId);
        void AddCompletionDetails(Guid assessmentId, DateTime endDate, string terminationReason);
        bool SetAssessmentReviseNextStep(Guid assessmentId, Guid questionAnswerId, string furtherInformation);
    }
}
