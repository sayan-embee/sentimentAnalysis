CREATE TABLE [dbo].[T_CardNotificationResponse]
(
	[NotificationId] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	ReplyToId NVARCHAR(100) NULL,
	ActivityId NVARCHAR(100) NULL,
	ConversationId NVARCHAR(500) NULL,
	ServiceUrl NVARCHAR(100) NULL,
	UserName NVARCHAR(100) NULL,
	UserADID NVARCHAR(100) NULL,
	[Type] NVARCHAR(50) NULL,
	[CreatedOnUTC] DATETIME NULL,
    [IsActive] BIT NULL,
	[CaseNumber] VARCHAR (50) NULL,
	[AssignmentId] BIGINT NULL,
	[AssignmentHistoryId] BIGINT NULL,
	[CallDetailId] BIGINT NULL,
	[Status] BIT NULL
)
