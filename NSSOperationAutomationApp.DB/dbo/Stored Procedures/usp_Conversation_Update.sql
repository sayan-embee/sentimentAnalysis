CREATE PROCEDURE [dbo].[usp_Conversation_Update]
	@ActivityId NVARCHAR(200)=NULL,
	@ConversationId  NVARCHAR(200),
	@ServiceUrl NVARCHAR(200)=NULL,
	@UserEmail  NVARCHAR(100)=NULL,
	@UserName NVARCHAR(100)=NULL,
	@BotInstalledOn DATETIME NULL,
	@UserPrincipalName NVARCHAR(100)=NULL,
	@AppName NVARCHAR(100)=NULL,
	@Active BIT=1,
	@UserId   UNIQUEIDENTIFIER
AS
BEGIN

	UPDATE dbo.[T_Conversations]
	SET ActivityId=@ActivityId,
		ServiceUrl=@ServiceUrl,
		UserEmail=@UserEmail,
		UserName=@UserName,
		BotInstalledOn=DATEADD(minute,330,GETUTCDATE()),
		UserPrincipalName=@UserPrincipalName,
		AppName=@AppName,
		Active=@Active
	WHERE UserId=@UserId AND AppName=@AppName
	
	IF @@ERROR<>0
	BEGIN
		SELECT 
			'Something went wrong, Unable to update conversation'	AS [Message],
			''						AS ErrorMessage,
			0						AS [Status],
			0						AS Id,
			''						AS ReferenceNo
		RETURN 
	END

	SELECT 
		'Conversation data updated successfully!'			AS	[Message],
		''								AS ErrorMessage,
		1								AS [Status],
		1								AS Id,
		@ConversationId					AS ReferenceNo
END
