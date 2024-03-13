CREATE TABLE [dbo].[T_EngineerAssignmentHistory] (
    [AssignmentHistoryId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [TicketId]            BIGINT   NULL,
    [AssignmentId]        BIGINT         NULL,
    [AssignedTo]          NVARCHAR (100)  NULL,
    [AssignedToEmail]     VARCHAR (100)  NULL,
    [AssignedToADID]      VARCHAR (50)   NULL,
    [AssignedOnUTC]       DATETIME       NULL,
    [AssignedBy]          NVARCHAR (100)  NULL,
    [AssignedByEmail]     VARCHAR (100)  NULL,
    [AssignedByADID]      VARCHAR (50)   NULL,
    [CallActionId]        INT            NULL,
    [CallStatusId]        INT            NULL,
    [ClosedBy]            VARCHAR (100)  NULL,
    [ClosedByEmail]       VARCHAR (100)  NULL,
    [ClosedByADID]        VARCHAR (50)   NULL,
    [ClosedOnUTC]         DATETIME       NULL,
    [AdminClosureRemarks] NVARCHAR (MAX) NULL,
    [CreatedOnUTC]        DATETIME       NULL,
    [CaseNumber]          VARCHAR (50)   NULL
    PRIMARY KEY CLUSTERED ([AssignmentHistoryId] ASC)
);

