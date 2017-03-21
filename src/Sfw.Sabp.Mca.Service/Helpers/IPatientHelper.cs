using System;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public interface IPatientHelper
    {
        Patient GetPatient(Guid id);
    }
}