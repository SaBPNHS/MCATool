using System;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public interface IAssessmentHelper
    {
        Assessment GetAssessment(Guid id);

        Assessments GetAssessmentsByPatient(Guid id);

        void UpdateAssessmentQuestions(Guid assessmentId, Guid nextWorkflowQuestionId, Guid? previousQuestionId, Guid? resetQuestionId);

        void UpdateAssessmentStatus(Guid assessmentId, AssessmentStatusEnum status);

        void UpdateAssessmentReadonly(Guid assessmentId, bool readOnly);
    }
}