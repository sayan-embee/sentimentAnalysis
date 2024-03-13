CREATE PROCEDURE [dbo].[usp_Conversation_Remove]
	@ConversationId  NVARCHAR(200),
	@UserId   UNIQUEIDENTIFIER,
	@BotInstalledOn DATETIME NULL,
	@AppName NVARCHAR(100)=NULL
AS
BEGIN

	UPDATE dbo.[T_Conversations]
	SET BotRemovedOn=DATEADD(minute,330,GETUTCDATE()),		
		Active=0
	WHERE ConversationId=@ConversationId AND AppName=@AppName
	AND UserId=@UserId
	
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
