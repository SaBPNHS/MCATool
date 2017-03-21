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
    
    
    
    public partial class Status  : IQueryResult
    {
        public Status()
        {
            this.Assessments = new HashSet<Assessment>();
            this.WorkflowSteps = new HashSet<WorkflowStep>();
        }
    
        public int StatusId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Assessment> Assessments { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; }
    }
}