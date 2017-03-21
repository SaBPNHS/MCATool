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
    
    
    
    public partial class WorkflowStep  : IQueryResult
    {
        public System.Guid WorkflowStepId { get; set; }
        public System.Guid WorkflowVersionId { get; set; }
        public System.Guid QuestionOptionId { get; set; }
        public Nullable<System.Guid> NextWorkflowQuestionId { get; set; }
        public System.Guid CurrentWorkflowQuestionId { get; set; }
        public int OutcomeStatusId { get; set; }
    
        public virtual WorkflowQuestion WorkflowQuestion { get; set; }
        public virtual QuestionOption QuestionOption { get; set; }
        public virtual WorkflowQuestion WorkflowQuestion1 { get; set; }
        public virtual WorkflowVersion WorkflowVersion { get; set; }
        public virtual Status Status { get; set; }
    }
}