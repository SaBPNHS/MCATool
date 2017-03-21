CREATE TABLE [dbo].[Disclaimer] (
    [DisclaimerId]       UNIQUEIDENTIFIER NOT NULL,
    [AssessorDomainName] NVARCHAR (50)    NOT NULL,
    [IsAgreed]           BIT              NOT NULL,
    CONSTRAINT [PK_Disclaimer] PRIMARY KEY CLUSTERED ([DisclaimerId] ASC)
);



