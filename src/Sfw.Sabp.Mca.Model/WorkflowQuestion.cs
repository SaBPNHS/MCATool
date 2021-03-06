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
    
    
    
    public partial class WorkflowQuestion  : IQueryResult
    {
        public WorkflowQuestion()
        {
            this.WorkflowSteps = new HashSet<WorkflowStep>();
            this.WorkflowVersions = new HashSet<WorkflowVersion>();
            this.WorkflowSteps1 = new HashSet<WorkflowStep>();
            this.Assessments = new HashSet<Assessment>();
            this.Assessments1 = new HashSet<Assessment>();
            this.Assessments11 = new HashSet<Assessment>();
            this.WorkflowStages = new HashSet<WorkflowStage>();
            this.QuestionAnswers = new HashSet<QuestionAnswer>();
        }
    
        public System.Guid WorkflowQuestionId { get; set; }
        public System.Guid WorkflowStageId { get; set; }
        public System.Guid QuestionId { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; }
        public virtual ICollection<WorkflowVersion> WorkflowVersions { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowSteps1 { get; set; }
        public virtual ICollection<Assessment> Assessments { get; set; }
        public virtual ICollection<Assessment> Assessments1 { get; set; }
        public virtual ICollection<Assessment> Assessments11 { get; set; }
        public virtual WorkflowStage WorkflowStage { get; set; }
        public virtual ICollection<WorkflowStage> WorkflowStages { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
    }
}
