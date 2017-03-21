using System.Collections.Generic;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Model
{
    public class Patients : IQueryResult
    {
        public IEnumerable<Patient> Items { get; set; }
    }
}
