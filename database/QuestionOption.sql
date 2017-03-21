CREATE TABLE [dbo].[QuestionOption] (
    [QuestionOptionId]             UNIQUEIDENTIFIER NOT NULL,
    [QuestionId]                   UNIQUEIDENTIFIER NOT NULL,
    [OptionId]                     UNIQUEIDENTIFIER NOT NULL,
    [FurtherInformationQuestionId] UNIQUEIDENTIFIER NULL,
    [Order]                        INT              NOT NULL,
    CONSTRAINT [PK_QuestionOption] PRIMARY KEY CLUSTERED ([QuestionOptionId] ASC),
    CONSTRAINT [FK_QuestionOption_FurtherQuestion] FOREIGN KEY ([FurtherInformationQuestionId]) REFERENCES [dbo].[Question] ([QuestionId]),
    CONSTRAINT [FK_QuestionOption_Option] FOREIGN KEY ([OptionId]) REFERENCES [dbo].[Option] ([OptionId]),
    CONSTRAINT [FK_QuestionOption_Question] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Question] ([QuestionId])
);

















