CREATE TABLE [dbo].[M_CallStatus] (
    [CallStatusId] INT          IDENTITY (1, 1) NOT NULL,
    [CallStatus]   VARCHAR (50) NULL,
    CONSTRAINT [PK_M_CallStatus] PRIMARY KEY CLUSTERED ([CallStatusId] ASC)
);

