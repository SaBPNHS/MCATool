CREATE TABLE [dbo].[WorkflowVersion] (
    [WorkflowVersionId]         UNIQUEIDENTIFIER NOT NULL,
    [Description]               NVARCHAR (100)   NOT NULL,
    [InitialWorkflowQuestionId] UNIQUEIDENTIFIER NOT NULL,
    [ExpiredDate]               DATETIME         NULL,
    CONSTRAINT [PK_WorkflowVersion] PRIMARY KEY CLUSTERED ([WorkflowVersionId] ASC),
    CONSTRAINT [FK_WorkflowVersion_WorkflowQuestion] FOREIGN KEY ([InitialWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId])
);





