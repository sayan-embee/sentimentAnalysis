CREATE PROCEDURE [dbo].[usp_T_EngineerAssignment_InsertUpdate]
(
	@TransactionType VARCHAR(10) = NULL,
	@TicketId BIGINT = NULL,
	@AssignmentId BIGINT = 0,
	@CaseNumber VARCHAR(50) = NULL,
    @AssignedTo NVARCHAR(100) = NULL,
    @AssignedToEmail VARCHAR(100) = NULL,
    @AssignedToADID VARCHAR(50) = NULL,
    @AssignedBy NVARCHAR(100) = NULL,
    @AssignedByEmail VARCHAR(100) = NULL,
    @AssignedByADID VARCHAR(50) = NULL,
    @CallActionId INT = NULL,
    @CallStatusId INT = NULL,
    @ClosedBy VARCHAR(100) = NULL,
    @ClosedByEmail VARCHAR(100) = NULL,
    @ClosedByADID VARCHAR(100) = NULL,
    @AdminClosureRemarks NVARCHAR(MAX) = NULL
)
AS
BEGIN
	
	DECLARE @AssignmentHistoryId BIGINT = 0;
	DECLARE @SuccessMsg VARCHAR(100);
	DECLARE @ErrorMsg VARCHAR(100);

	DECLARE @OutputTbl AS TABLE
	(
		[CaseNumber]		VARCHAR (50),
		[TicketId]			BIGINT,
		[CreatedOn]			VARCHAR(50),
		[AssignedBy]		NVARCHAR (100),
		[AssignedByEmail]	VARCHAR (100),
		[AssignedByADID]	VARCHAR (50),
		[AssignedTo]		NVARCHAR (100),
		[AssignedToEmail]	VARCHAR (100),
		[AssignedToADID]	VARCHAR (50),
		[AssignedOn]		VARCHAR(50),
		[ServiceAccount]    VARCHAR (100),
		[CallStatus]		VARCHAR (100),
		[AssignmentId]		BIGINT,
		[AssignmentHistoryId]	BIGINT
	);

	DECLARE @JSONString NVARCHAR(MAX) = NULL;


	IF(@TransactionType = 'ENG-I')
	BEGIN
	BEGIN TRY
	-----------------

	IF EXISTS (SELECT * FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK) WHERE A.CaseNumber = @CaseNumber AND A.CallStatusId <> 3)
	BEGIN
		SET @ErrorMsg = 'DB execution failed - CaseNumber already assigned'
		SELECT
			@ErrorMsg												AS [Message],
			''														AS ErrorMessage,
			0					                                    AS [Status],
			@AssignmentId				                            AS Id
			RETURN
	END

	BEGIN TRANSACTION
	-----------------

		INSERT INTO dbo.[T_EngineerAssignment]
		(
			CaseNumber,
			TicketId,
			AssignedTo,
			AssignedToEmail,
			AssignedToADID,
			AssignedOnUTC,
			AssignedBy,
			AssignedByEmail,
			AssignedByADID,
			CallStatusId,
			CreatedOnUTC,
			AdminClosureRemarks
		)
		VALUES
		(
			@CaseNumber,
			@TicketId,
			@AssignedTo,
			@AssignedToEmail,
			@AssignedToADID,
			GETUTCDATE(),
			@AssignedBy,
			@AssignedByEmail,
			@AssignedByADID,
			1,
			GETUTCDATE(),
			@AdminClosureRemarks
		);

		SET @AssignmentId = @@IDENTITY;

		IF(@AssignmentId > 0)
		BEGIN

			INSERT INTO dbo.[T_EngineerAssignmentHistory]
			(
				AssignmentId,
				TicketId,
				AssignedTo,
				AssignedToEmail,
				AssignedToADID,
				AssignedOnUTC,
				AssignedBy,
				AssignedByEmail,
				AssignedByADID,
				CallActionId,
				CallStatusId,
				ClosedBy,
				ClosedByEmail,
				ClosedByADID,
				AdminClosureRemarks,
				CreatedOnUTC,
				CaseNumber
			)
			SELECT
				A.AssignmentId,
				A.TicketId,
				A.AssignedTo,
				A.AssignedToEmail,
				A.AssignedToADID,
				A.AssignedOnUTC,
				A.AssignedBy,
				A.AssignedByEmail,
				A.AssignedByADID,
				A.CallActionId,
				A.CallStatusId,
				A.ClosedBy,
				A.ClosedByEmail,
				A.ClosedByADID,
				A.AdminClosureRemarks,
				GETUTCDATE(),
				A.CaseNumber
			FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
			WHERE A.AssignmentId = @AssignmentId

			SET @AssignmentHistoryId = @@IDENTITY;

			UPDATE dbo.[T_TicketDetails] SET TicketId = @TicketId WHERE CaseNumber = @CaseNumber;			

			SET @SuccessMsg = 'DB execution successful - EngineerAssignment inserted';

			INSERT INTO @OutputTbl
			SELECT
				TD.CaseNumber,
				TD.TicketId,
				--FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS CreatedOn,
				FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS CreatedOn,
				A.AssignedBy,
				A.AssignedByEmail,
				A.AssignedByADID,
				A.AssignedTo,
				A.AssignedToEmail,
				A.AssignedToADID,
				--FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS AssignedOn,
				FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS AssignedOn,
				TD.ServiceAccount,
				CS.CallStatus,
				A.AssignmentId,
				@AssignmentHistoryId AS AssignmentHistoryId
			FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
			INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = A.CaseNumber
			LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
			WHERE A.AssignmentId = @AssignmentId

			SET @JSONString = (
				SELECT 
					CaseNumber,
					TicketId,
					CreatedOn,
					AssignedBy,
					AssignedByEmail,
					AssignedByADID,
					AssignedTo,
					AssignedToEmail,
					AssignedToADID,
					AssignedOn,
					ServiceAccount,
					CallStatus,
					AssignmentId,
					AssignmentHistoryId
				FROM @OutputTbl
			FOR JSON AUTO
			);

		END		

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Insert EngineerAssignment'
    SELECT
        @ErrorMsg												AS [Message],
        ERROR_MESSAGE()		                                    AS ErrorMessage,
        0					                                    AS [Status],
        ''				                                        AS Id
        RETURN
	END CATCH
	-----------------
	END

	--------------------------------------------------------------------------------

	IF(@TransactionType = 'ENG-U')
	BEGIN
	BEGIN TRY
	-----------------

		IF NOT EXISTS (SELECT * FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK) WHERE A.AssignmentId = @AssignmentId)
		BEGIN
			SET @ErrorMsg = 'DB execution failed - AssignmentId does not exists'
			SELECT
				@ErrorMsg												AS [Message],
				''														AS ErrorMessage,
				0					                                    AS [Status],
				@AssignmentId				                            AS Id
				RETURN
		END

	BEGIN TRANSACTION
	-----------------

		UPDATE dbo.[T_EngineerAssignment]
		SET AssignedTo = ISNULL(@AssignedTo, AssignedTo),
		AssignedToEmail = ISNULL(@AssignedToEmail, AssignedToEmail),
		AssignedToADID = ISNULL(@AssignedToADID, AssignedToADID),
		AssignedOnUTC = GETUTCDATE(),
		AssignedBy = ISNULL(@AssignedBy, AssignedBy),
		AssignedByEmail = ISNULL(@AssignedByEmail, AssignedByEmail),
		AssignedByADID = ISNULL(@AssignedByADID, AssignedByADID),
		AdminClosureRemarks = ISNULL(@AdminClosureRemarks, AdminClosureRemarks)
		WHERE AssignmentId = @AssignmentId AND CaseNumber = @CaseNumber AND CallStatusId <> 3


		INSERT INTO dbo.[T_EngineerAssignmentHistory]
		(
			AssignmentId,
			TicketId,
			AssignedTo,
			AssignedToEmail,
			AssignedToADID,
			AssignedOnUTC,
			AssignedBy,
			AssignedByEmail,
			AssignedByADID,
			CallActionId,
			CallStatusId,
			ClosedBy,
			ClosedByEmail,
			ClosedByADID,
			AdminClosureRemarks,
			CreatedOnUTC,
			CaseNumber
		)
		SELECT
			A.AssignmentId,
			A.TicketId,
			A.AssignedTo,
			A.AssignedToEmail,
			A.AssignedToADID,
			A.AssignedOnUTC,
			A.AssignedBy,
			A.AssignedByEmail,
			A.AssignedByADID,
			A.CallActionId,
			A.CallStatusId,
			A.ClosedBy,
			A.ClosedByEmail,
			A.ClosedByADID,
			A.AdminClosureRemarks,
			GETUTCDATE(),
			A.CaseNumber
		FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
		WHERE A.AssignmentId = @AssignmentId;

		SET @AssignmentHistoryId = @@IDENTITY;	

		SET @SuccessMsg = 'DB execution successful - EngineerAssignment updated';

		INSERT INTO @OutputTbl
		SELECT
			TD.CaseNumber,
			TD.TicketId,
			--FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS CreatedOn,
			FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS CreatedOn,
			A.AssignedBy,
			A.AssignedByEmail,
			A.AssignedByADID,
			A.AssignedTo,
			A.AssignedToEmail,
			A.AssignedToADID,
			--FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as AssignedOn,
			FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS AssignedOn,
			TD.ServiceAccount,
			CS.CallStatus,
			A.AssignmentId,
			@AssignmentHistoryId AS AssignmentHistoryId
		FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
		INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = A.CaseNumber
		LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
		WHERE A.AssignmentId = @AssignmentId

		SET @JSONString = (
			SELECT 
				CaseNumber,
				TicketId,
				CreatedOn,
				AssignedBy,
				AssignedByEmail,
				AssignedByADID,
				AssignedTo,
				AssignedToEmail,
				AssignedToADID,
				AssignedOn,
				ServiceAccount,
				CallStatus,
				AssignmentId,
				AssignmentHistoryId
			FROM @OutputTbl
		FOR JSON AUTO
		);

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Update EngineerAssignment'
    SELECT
        @ErrorMsg												AS [Message],
        ERROR_MESSAGE()		                                    AS ErrorMessage,
        0					                                    AS [Status],
        ''				                                        AS Id
        RETURN
	END CATCH
	-----------------
	END
	
	--------------------------------------------------------------------------------

	IF(@TransactionType = 'ADMIN-U')
	BEGIN
	BEGIN TRY
	-----------------

		IF NOT EXISTS (SELECT * FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK) WHERE A.AssignmentId = @AssignmentId)
		BEGIN
			SET @ErrorMsg = 'DB execution failed - AssignmentId does not exists'
			SELECT
				@ErrorMsg												AS [Message],
				''														AS ErrorMessage,
				0					                                    AS [Status],
				@AssignmentId				                            AS Id
				RETURN
		END

		DECLARE @OutputTbl2 AS TABLE
		(
			[CaseNumber]		VARCHAR (50),
			[TicketId]			BIGINT,
			[CreatedOn]			VARCHAR(50),
			[AssignedBy]		NVARCHAR (100),
			[AssignedByEmail]	VARCHAR (100),
			[AssignedByADID]	VARCHAR (50),
			[AssignedTo]		NVARCHAR (100),
			[AssignedToEmail]	VARCHAR (100),
			[AssignedToADID]	VARCHAR (50),
			[AssignedOn]		VARCHAR(50),
			[ServiceAccount]    VARCHAR (100),
			[CallStatus]		VARCHAR (100),
			[AssignmentId]		BIGINT,
			[AssignmentHistoryId]	BIGINT,
			[CallDetailId]      BIGINT,
			[CallAction]		VARCHAR (50),
			[UpdatedOnIST]		VARCHAR(50),
			[UpdatedBy]			VARCHAR(100),
			[UpdatedByEmail]	VARCHAR(100),
			[UpdatedByADID]		VARCHAR(50),
			[CloserRemarks]		NVARCHAR(MAX),
			[AdminClosureRemarks] NVARCHAR(MAX)
		);

	BEGIN TRANSACTION
	-----------------

		UPDATE dbo.[T_EngineerAssignment]
		SET CallStatusId = ISNULL(@CallStatusId, CallStatusId),
		AdminClosureRemarks = ISNULL(@AdminClosureRemarks, AdminClosureRemarks)
		WHERE AssignmentId = @AssignmentId AND CaseNumber = @CaseNumber;

		--IF(@CallActionId = 1 AND @CallStatusId = 3)
		IF(@CallStatusId = 3)
		BEGIN

			UPDATE dbo.[T_EngineerAssignment]
			SET ClosedBy = ISNULL(@ClosedBy, ClosedBy),
			ClosedByEmail = ISNULL(@ClosedByEmail, ClosedByEmail),
			ClosedByADID = ISNULL(@ClosedByADID, ClosedByADID),
			ClosedOnUTC = GETUTCDATE()
			WHERE AssignmentId = @AssignmentId AND CaseNumber = @CaseNumber;

		END
		
		SET @SuccessMsg = 'DB execution successful - EngineerAssignment updated by Admin';

		INSERT INTO @OutputTbl2
		(
			CaseNumber,
			TicketId,
			CreatedOn,
			AssignedBy,
			AssignedByEmail,
			AssignedByADID,
			AssignedTo,
			AssignedToEmail,
			AssignedToADID,
			AssignedOn,
			ServiceAccount,
			CallStatus,
			AssignmentId,
			AssignmentHistoryId,
			CallDetailId,
			CallAction,
			UpdatedOnIST,
			UpdatedBy,
			UpdatedByEmail,
			UpdatedByADID,
			CloserRemarks,
			AdminClosureRemarks
		)
		SELECT
			TD.CaseNumber,
			TD.TicketId,
			--FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS CreatedOn,
			FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS CreatedOn,
			A.AssignedBy,
			A.AssignedByEmail,
			A.AssignedByADID,
			A.AssignedTo,
			A.AssignedToEmail,
			A.AssignedToADID,
			--FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS AssignedOn,
			FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS AssignedOn,
			TD.ServiceAccount,
			CS.CallStatus,
			A.AssignmentId,
			AH.AssignmentHistoryId,
			0,
			NULL,
			FORMAT(SWITCHOFFSET(A.ClosedOnUTC, '+05:30'),'dd-MM-yyyy HH:mm:ss') AS UpdatedOnIST,
			A.ClosedBy,
			A.ClosedByEmail,
			A.ClosedByADID,
			A.AdminClosureRemarks,
			A.AdminClosureRemarks
		FROM dbo.[T_TicketDetails] TD WITH(NOLOCK)
		INNER JOIN dbo.[T_EngineerAssignment] A WITH(NOLOCK) ON A.CaseNumber = TD.CaseNumber
		INNER JOIN dbo.[T_EngineerAssignmentHistory] AH WITH(NOLOCK) ON AH.AssignmentId = A.AssignmentId
		LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
		WHERE TD.CaseNumber = @CaseNumber
		AND A.CaseNumber = @CaseNumber
		AND A.AssignmentId = @AssignmentId

		SET @JSONString = (
			SELECT 
				CaseNumber,
				TicketId,
				CreatedOn,
				AssignedBy,
				AssignedByEmail,
				AssignedByADID,
				AssignedTo,
				AssignedToEmail,
				AssignedToADID,
				AssignedOn,
				ServiceAccount,
				CallStatus,
				AssignmentId,
				AssignmentHistoryId,
				CallDetailId,
				CallAction,
				UpdatedOnIST,
				UpdatedBy,
				UpdatedByEmail,
				UpdatedByADID,
				CloserRemarks,
				AdminClosureRemarks
			FROM @OutputTbl2
		FOR JSON AUTO
		);

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Admin Update EngineerAssignment'
    SELECT
        @ErrorMsg												AS [Message],
        ERROR_MESSAGE()		                                    AS ErrorMessage,
        0					                                    AS [Status],
        ''				                                        AS Id
        RETURN
	END CATCH
	-----------------
	END


	COMMIT TRANSACTION
	-----------------
	SELECT
		@SuccessMsg												AS [Message],
		''						                                AS ErrorMessage,
		1					                                    AS [Status],
		@AssignmentId											AS Id,
		@CaseNumber												AS ReferenceNo,
		@JSONString												AS ReferenceObject
END