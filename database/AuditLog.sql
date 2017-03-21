CREATE TABLE [dbo].[AuditLog] (
    [AuditLogId]    UNIQUEIDENTIFIER NOT NULL,
    [User]          NVARCHAR (50)    NOT NULL,
    [EventDateTime] DATETIME         NOT NULL,
    [Action]        NVARCHAR (100)   NOT NULL,
    [Controller]    NVARCHAR (100)   NOT NULL,
    [AuditData]     NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY NONCLUSTERED ([AuditLogId] ASC)
);



