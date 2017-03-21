CREATE TABLE [dbo].[WorkflowStep] (
    [WorkflowStepId]            UNIQUEIDENTIFIER NOT NULL,
    [WorkflowVersionId]         UNIQUEIDENTIFIER NOT NULL,
    [CurrentWorkflowQuestionId] UNIQUEIDENTIFIER NOT NULL,
    [QuestionOptionId]          UNIQUEIDENTIFIER NOT NULL,
    [NextWorkflowQuestionId]    UNIQUEIDENTIFIER NULL,
    [OutcomeStatusId]           INT              CONSTRAINT [DF_WorkflowStep_OutcomeStatusId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_WorkflowSteps] PRIMARY KEY CLUSTERED ([WorkflowStepId] ASC),
    CONSTRAINT [FK_WorkflowStep_WorkflowQuestion] FOREIGN KEY ([OutcomeStatusId]) REFERENCES [dbo].[Status] ([StatusId]),
    CONSTRAINT [FK_WorkflowStep_WorkflowQuestion1] FOREIGN KEY ([CurrentWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId]),
    CONSTRAINT [FK_WorkflowStep_WorkflowVersion] FOREIGN KEY ([WorkflowVersionId]) REFERENCES [dbo].[WorkflowVersion] ([WorkflowVersionId]),
    CONSTRAINT [FK_WorkflowSteps_QuestionOption] FOREIGN KEY ([QuestionOptionId]) REFERENCES [dbo].[QuestionOption] ([QuestionOptionId])
);













