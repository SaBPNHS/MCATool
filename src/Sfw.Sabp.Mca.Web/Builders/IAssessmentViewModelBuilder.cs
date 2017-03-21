using System;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IAssessmentViewModelBuilder
    {
        AssessmentListViewModel BuildAssessmentListViewModel(Guid patientId, Assessments assessments);

        AssessmentViewModel BuildAssessmentViewModel(Patient patient, Roles roles);

        AssessmentViewModel BuildAssessmentViewModel(Assessment assessment);

        AssessmentViewModel BuildAssessmentViewModel(Assessment assessment, Roles roles);

        AddAssessmentCommand BuildAddAssessmentCommand(AssessmentViewModel assessment);

        UpdateAssessmentCommand BuildUpdateAssessmentCommand(AssessmentViewModel assessment);
    }
}