CREATE TABLE [dbo].[QuestionAnswer] (
    [QuestionAnswerId]   UNIQUEIDENTIFIER NOT NULL,
    [AssessmentId]       UNIQUEIDENTIFIER NOT NULL,
    [WorkflowQuestionId] UNIQUEIDENTIFIER NOT NULL,
    [QuestionOptionId]   UNIQUEIDENTIFIER NOT NULL,
    [FurtherInformation] NVARCHAR (MAX)   NULL,
    [Created]            DATETIME         CONSTRAINT [DF_QuestionAnswer_LastUpdated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_QuestionAnswer] PRIMARY KEY CLUSTERED ([QuestionAnswerId] ASC),
    CONSTRAINT [FK_QuestionAnswer_Assessment] FOREIGN KEY ([AssessmentId]) REFERENCES [dbo].[Assessment] ([AssessmentId]),
    CONSTRAINT [FK_QuestionAnswer_QuestionOption] FOREIGN KEY ([QuestionOptionId]) REFERENCES [dbo].[QuestionOption] ([QuestionOptionId]),
    CONSTRAINT [FK_QuestionAnswer_WorkflowQuestion] FOREIGN KEY ([WorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId])
);













