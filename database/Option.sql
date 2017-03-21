CREATE TABLE [dbo].[Option] (
    [OptionId]    UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR (200)   NOT NULL,
    CONSTRAINT [PK_Option] PRIMARY KEY CLUSTERED ([OptionId] ASC)
);

