using System.Collections.Generic;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Model
{
    public class Assessments : IQueryResult
    {
        public IEnumerable<Assessment> Items { get; set; }
    }
}
