CREATE TABLE [dbo].[Patient] (
    [PatientId]        UNIQUEIDENTIFIER NOT NULL,
    [ClinicalSystemId] NVARCHAR (50)    NOT NULL,
    [NhsNumber]        NUMERIC (10)     NULL,
    [FirstName]        NVARCHAR (50)    NOT NULL,
    [LastName]         NVARCHAR (50)    NOT NULL,
    [DateOfBirth]      DATE             NOT NULL,
    [GenderId]         INT              NOT NULL,
    CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([PatientId] ASC),
    CONSTRAINT [FK_Patient_Gender] FOREIGN KEY ([GenderId]) REFERENCES [dbo].[Gender] ([GenderId])
);



