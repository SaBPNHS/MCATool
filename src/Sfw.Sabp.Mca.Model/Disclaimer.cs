//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sfw.Sabp.Mca.Model
{
    using Sfw.Sabp.Mca.Core.Contracts;
    using System;
    using System.Collections.Generic;
    
    
    
    public partial class Disclaimer  : IQueryResult
    {
        public System.Guid DisclaimerId { get; set; }
        public string AssessorDomainName { get; set; }
        public bool IsAgreed { get; set; }
    }
}
