﻿CREATE TABLE [dbo].[T_SentimentDetails]
(
	[AutoId] BIGINT NOT NULL PRIMARY KEY, 
    [SummaryText] NVARCHAR(MAX) NULL, 
    [Sentiment] NVARCHAR(50) NULL, 
    [Reason] NVARCHAR(50) NULL, 
    [TranscribeText] NVARCHAR(MAX) NULL, 
    [FileRefId] NVARCHAR(50) NULL, 
    [FileName] NVARCHAR(250) NULL, 
    [FileInternalName] NVARCHAR(300) NULL, 
    [FileUrl] NVARCHAR(500) NULL, 
    [ContentType] NVARCHAR(50) NULL, 
    [IsActive] BIT NULL, 
    [CreatedOnIST] DATETIME NULL,
    CONSTRAINT [PK_T_SentimentDetails] PRIMARY KEY CLUSTERED ([AutoId] ASC)
)