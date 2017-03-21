using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IPatientViewModelBuilder
    {
        CreatePatientViewModel BuildPatientViewModel(Genders gender);
        AddUpdatePatientCommand BuildAddPatientCommand(PatientViewModel patient);
        PatientSearchViewModel BuildPatientSearchViewModel(Patients patients);
        EditPatientViewModel BuildEditPatientViewModel(Patient patient, Genders genders);
        AddUpdatePatientCommand BuildUpdatePatientCommand(EditPatientViewModel viewModel);
    }
}