CREATE TABLE [dbo].[Question] (
    [QuestionId]  UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR (200)   NOT NULL,
    [Guidance] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([QuestionId] ASC)
);

