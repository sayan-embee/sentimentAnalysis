CREATE PROCEDURE [dbo].[usp_AssignReassignTicket_CardResponse_Insert]
(
	@udt_CardList dbo.[udt_T_CardNotificationResponse] READONLY
)
AS
BEGIN

	DECLARE @SuccessMsg VARCHAR(100);
	DECLARE @ErrorMsg VARCHAR(100);

	BEGIN TRY
	-----------------

		BEGIN TRANSACTION
		-----------------

		INSERT INTO dbo.[T_CardNotificationResponse]
		(
			ReplyToId,
			ActivityId,
			ConversationId,
			ServiceUrl,
			UserName,
			UserADID,
			[Type],
			CreatedOnUTC,
			IsActive,
			CaseNumber,
			AssignmentId,
			AssignmentHistoryId,
			CallDetailId,
			[Status]
		)
		SELECT
			udt.ReplyToId,
			udt.ActivityId,
			udt.ConversationId,
			udt.ServiceUrl,
			udt.UserName,
			udt.UserADID,
			udt.[Type],
			GETUTCDATE(),
			1,
			udt.CaseNumber,
			udt.AssignmentId,
			udt.AssignmentHistoryId,
			udt.CallDetailId,
			udt.[Status]
		FROM @udt_CardList udt

		SET @SuccessMsg = 'DB execution successful - Card response inserted';

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
		
		ROLLBACK TRANSACTION
		SET @ErrorMsg = 'DB execution failed - Insert card response'
		SELECT
		@ErrorMsg												AS [Message],
		ERROR_MESSAGE()		                                    AS ErrorMessage,
		0					                                    AS [Status],
		''				                                        AS Id
		RETURN

	END CATCH
	-----------------

	COMMIT TRANSACTION
	-----------------
	SELECT
		@SuccessMsg												AS [Message],
		''						                                AS ErrorMessage,
		1					                                    AS [Status],
		@@IDENTITY												AS Id

END