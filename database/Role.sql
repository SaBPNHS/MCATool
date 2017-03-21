CREATE TABLE [dbo].[Role] (
    [RoleId]      INT NOT NULL,
    [Description] NVARCHAR (50)    NOT NULL,
	CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

