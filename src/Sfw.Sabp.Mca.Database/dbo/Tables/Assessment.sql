CREATE TABLE [dbo].[Assessment] (
    [AssessmentId]               UNIQUEIDENTIFIER NOT NULL,
    [PatientId]                  UNIQUEIDENTIFIER NOT NULL,
    [DateAssessmentStarted]      SMALLDATETIME    NULL,
    [Stage1InfoText]             NVARCHAR (150)   CONSTRAINT [DF__Assessmen__Stage__47A6A41B] DEFAULT ('The decision must be about a specific treatment or choice and not a vague question e.g. where would you like to live?') NOT NULL,
    [Stage1DecisionToBeMade]     NVARCHAR (1000)  NOT NULL,
    [Stage1DecisionConfirmation] NVARCHAR (100)   CONSTRAINT [DF__Assessmen__Stage__489AC854] DEFAULT ('Is the decision to be made specific and clearly defined?') NOT NULL,
    [Stage1DecisionClearlyMade]  BIT              CONSTRAINT [DF__Assessmen__Stage__498EEC8D] DEFAULT ((0)) NOT NULL,
    [WorkflowVersionId]          UNIQUEIDENTIFIER NOT NULL,
    [AssessorName]               NVARCHAR (50)    NOT NULL,
    [StatusId]                   INT              NOT NULL,
    [AssessorDomainName]         NVARCHAR (50)    NOT NULL,
    [ReadOnly]                   BIT              CONSTRAINT [DF_Assessment_ReadOnly] DEFAULT ((0)) NOT NULL,
    [CurrentWorkflowQuestionId]  UNIQUEIDENTIFIER NOT NULL,
    [PreviousWorkflowQuestionId] UNIQUEIDENTIFIER NULL,
    [ResetWorkflowQuestionId]    UNIQUEIDENTIFIER NULL,
    [DateAssessmentEnded]        SMALLDATETIME    NULL,
    [TerminatedAssessmentReason] NVARCHAR (150)   NULL,
    [RoleId]                     INT              NOT NULL,
    [DecisionMaker]              NVARCHAR (50)    NULL,
    CONSTRAINT [PK_Assessment] PRIMARY KEY CLUSTERED ([AssessmentId] ASC),
    CONSTRAINT [FK_Assessment_Patient] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patient] ([PatientId]),
    CONSTRAINT [FK_Assessment_PreviousWorkflowQuestion] FOREIGN KEY ([PreviousWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId]),
    CONSTRAINT [FK_Assessment_ResetWorkflowQuestion] FOREIGN KEY ([ResetWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId]),
    CONSTRAINT [FK_Assessment_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId]),
    CONSTRAINT [FK_Assessment_Status] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([StatusId]),
    CONSTRAINT [FK_Assessment_WorkflowQuestion] FOREIGN KEY ([CurrentWorkflowQuestionId]) REFERENCES [dbo].[WorkflowQuestion] ([WorkflowQuestionId]),
    CONSTRAINT [FK_Assessment_WorkflowVersion] FOREIGN KEY ([WorkflowVersionId]) REFERENCES [dbo].[WorkflowVersion] ([WorkflowVersionId])
);



























