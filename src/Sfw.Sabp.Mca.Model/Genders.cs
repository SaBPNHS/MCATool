using System.Collections.Generic;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Model
{
    public class Genders : IQueryResult
    {
        public IEnumerable<Gender> Items { get; set; }
    }
}

