CREATE PROCEDURE [dbo].[usp_M_Conversation_Insert]
	@ActivityId NVARCHAR(200)=NULL,
	@ConversationId  NVARCHAR(200),
	@RecipientId  NVARCHAR(200)=NULL,
	@RecipientName  NVARCHAR(200)=NULL,
	@ServiceUrl NVARCHAR(200)=NULL,
	@UserEmail  NVARCHAR(100)=NULL,
	@TenantId  UNIQUEIDENTIFIER=NULL,
	@UserId   UNIQUEIDENTIFIER,
	@UserName NVARCHAR(100)=NULL,
	@BotInstalledOn DATETIME NULL,
	@UserPrincipalName NVARCHAR(100)=NULL,
	@AppName NVARCHAR(100)=NULL
AS
BEGIN

	
	INSERT INTO dbo.[T_Conversations]
	(
		ActivityId,ConversationId,RecipientId,RecipientName,ServiceUrl,UserEmail,TenantId,UserId,UserName,BotInstalledOn,UserPrincipalName,AppName,Active
	)
	VALUES
	(
		@ActivityId,@ConversationId,@RecipientId,@RecipientName,@ServiceUrl,@UserEmail,@TenantId,@UserId,@UserName,DATEADD(minute,330,GETUTCDATE()),@UserPrincipalName,@AppName,1
	)
	
	IF @@ERROR<>0
	BEGIN
		SELECT 
			'Something went wrong, Unable to insert Conversation data'	AS [Message],
			''						AS ErrorMessage,
			0						AS [Status],
			0						AS Id,
			''						AS ReferenceNo
		RETURN 
	END

	SELECT 
		'Conversation data saved successfully!'			AS	[Message],
		''								AS ErrorMessage,
		1								AS [Status],
		1					AS Id,
		@ConversationId					AS ReferenceNo
END
