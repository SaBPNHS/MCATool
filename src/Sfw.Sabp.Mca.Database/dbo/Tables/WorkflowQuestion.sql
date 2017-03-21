CREATE TABLE [dbo].[WorkflowQuestion] (
    [WorkflowQuestionId] UNIQUEIDENTIFIER NOT NULL,
    [WorkflowStageId]    UNIQUEIDENTIFIER NOT NULL,
    [QuestionId]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_WorkflowQuestion] PRIMARY KEY CLUSTERED ([WorkflowQuestionId] ASC),
    CONSTRAINT [FK_Workflow_Question] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Question] ([QuestionId]),
    CONSTRAINT [FK_Workflow_WorkflowStage] FOREIGN KEY ([WorkflowStageId]) REFERENCES [dbo].[WorkflowStage] ([WorkflowStageId])
);



