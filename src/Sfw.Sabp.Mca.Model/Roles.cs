using System.Collections.Generic;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Model
{
    public class Roles : IQueryResult
    {
        public IEnumerable<Role> Items { get; set; }
    }
}
