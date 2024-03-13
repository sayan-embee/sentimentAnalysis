CREATE TYPE [dbo].[udt_T_CardNotificationResponse] AS TABLE (
	--[NotificationId] BIGINT NOT NULL,
	ReplyToId NVARCHAR(100) NULL,
	ActivityId NVARCHAR(100) NULL,
	ConversationId NVARCHAR(500) NULL,
	ServiceUrl NVARCHAR(100) NULL,
	UserName NVARCHAR(100) NULL,
	UserADID NVARCHAR(100) NULL,
	[Type] NVARCHAR(50) NULL,
	--[CreatedOnUTC] DATETIME NULL,
    --[IsActive] BIT NULL,
	[CaseNumber] VARCHAR (50) NULL,
	[AssignmentId] BIGINT NULL,
	[AssignmentHistoryId] BIGINT NULL,
	[CallDetailId] BIGINT NULL,
	[Status] BIT NULL
);

