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
    
    
    
    public partial class WorkflowStage  : IQueryResult
    {
        public WorkflowStage()
        {
            this.WorkflowQuestions = new HashSet<WorkflowQuestion>();
        }
    
        public System.Guid WorkflowStageId { get; set; }
        public System.Guid WorkflowVersionId { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Nullable<System.Guid> InitialWorkflowQuestionId { get; set; }
        public bool DisplayStage1DecisionMade { get; set; }
    
        public virtual ICollection<WorkflowQuestion> WorkflowQuestions { get; set; }
        public virtual WorkflowQuestion WorkflowQuestion { get; set; }
        public virtual WorkflowVersion WorkflowVersion { get; set; }
    }
}