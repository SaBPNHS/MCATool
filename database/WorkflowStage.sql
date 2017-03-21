CREATE TABLE [dbo].[WorkflowStage] (
    [WorkflowStageId]           UNIQUEIDENTIFIER NOT NULL,
    [WorkflowVersionId]         UNIQUEIDENTIFIER NOT NULL,
    [Description]               NVARCHAR (100)   NOT NULL,
    [ShortDescription]          NVARCHAR (20)    NOT NULL,
    [InitialWorkflowQuestionId] UNIQUEIDENTIFIER NULL,
    [DisplayStage1DecisionMade] BIT              CONSTRAINT [DF_WorkflowStage_DisplayStage1DecisionMade] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_WorkflowStage] PRIMARY KEY CLUSTERED ([WorkflowStageId] ASC),
    CONSTRAINT [FK_WorkflowStage_WorkflowQuestion] FOREIGN KEY ([InitialWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId]),
    CONSTRAINT [FK_WorkflowStage_WorkflowVersion] FOREIGN KEY ([WorkflowVersionId]) REFERENCES [dbo].[WorkflowVersion] ([WorkflowVersionId])
);















