CREATE TABLE [dbo].[T_EngineerAssignment] (
    [AssignmentId]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [TicketId]            BIGINT   NULL,
    [CaseNumber]          VARCHAR (50)   NULL,
    [AssignedTo]          NVARCHAR(100)  NULL,
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
    [ClosedOnUTC]         DATETIME       NULL,
    [AdminClosureRemarks] NVARCHAR (MAX) NULL,
    [ClosedByADID]        VARCHAR (50)   NULL,
    [CreatedOnUTC]        DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([AssignmentId] ASC)
);

