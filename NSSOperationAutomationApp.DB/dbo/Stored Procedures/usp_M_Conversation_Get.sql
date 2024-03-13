
CREATE PROCEDURE [dbo].[usp_M_Conversation_Get]
(
	@ConversationId NVARCHAR(200)=NULL,
	@UserId UNIQUEIDENTIFIER =NULL,
	@AppName NVARCHAR(50)=NULL,
	@UserName NVARCHAR(50)=NULL,
	@UserEmail NVARCHAR(200)=NULL,
	@Filter NVARCHAR(500)=NULL
)
AS
BEGIN
	SELECT 
		V.ActivityId,
		V.BotInstalledOn,
		V.BotRemovedOn,
		V.ConversationId,
		V.RecipientId,
		V.RecipientName,
		V.ServiceUrl,
		V.UserEmail,
		V.TenantId,
		V.UserId,
		V.UserName,
		V.UserEmail,
		V.UserPrincipalName,
		V.AppName,
		V.Active

	FROM dbo.[T_Conversations] V WITH (NOLOCK)
	WHERE V.ConversationId=CASE WHEN ISNULL(@ConversationId,'')='' THEN  V.ConversationId ELSE @ConversationId END
	AND V.UserId=CASE WHEN @UserId IS NULL  THEN  V.UserId ELSE @UserId END
	AND V.AppName=CASE WHEN @AppName IS NULL  THEN  V.AppName ELSE @AppName END
	AND (ISNULL(@UserName,'')='' OR V.UserName LIKE @UserName + '%')
	AND (ISNULL(@UserEmail,'')='' OR V.UserEmail = @UserEmail)
	AND (ISNULL(@Filter,'')='' OR (V.UserEmail = @Filter OR V.UserName LIKE @Filter + '%'))
END
