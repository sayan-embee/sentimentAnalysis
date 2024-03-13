﻿CREATE PROCEDURE [dbo].[usp_T_CallDetails_InsertUpdate]
(
	@TransactionType VARCHAR(10) = NULL,

	@CallDetailId BIGINT = 0,
	@AssignmentId BIGINT = NULL,
    @UpdatedBy VARCHAR(100) = NULL,
    @UpdatedByEmail VARCHAR(100) = NULL,
    @UpdatedByADID VARCHAR(50) = NULL,
    @CallActionId INT = NULL,
	@CallStatusId INT = NULL,
    @CustomerName VARCHAR(200) = NULL,
    @CaseNumber VARCHAR(50) = NULL,
    @UnitSlNo VARCHAR(50) = NULL,
    @PassId VARCHAR(50) = NULL,
    @TaskStartDateTimeIST DATETIME = NULL,
    @TaskEndDateTimeIST DATETIME = NULL,
    @CloserRemarks NVARCHAR(MAX) = NULL,
    @PartConsumptionTypeId INT = NULL,
    @SONo VARCHAR(50) = NULL,
    @RequiredPart NVARCHAR(MAX) = NULL,
    @RequiredSparePartNo VARCHAR(50) = NULL,
    @RequiredPartName VARCHAR(100) = NULL,
    @FaultyPartCTNo VARCHAR(50) = NULL,
    @FailureId VARCHAR(50) = NULL,
    @IssueDescription NVARCHAR(MAX) = NULL,
    @TroubleshootingStep NVARCHAR(MAX) = NULL,
    @FirstPartConsumptionTypeId INT = NULL,
    @FirstPartSONo VARCHAR(50) = NULL,
    @FirstRequiredPartName VARCHAR(100) = NULL,
    @ReceivedPartConsumptionTypeId INT = NULL,
    @ReceivedPartName VARCHAR(100) = NULL,	

	@udt_CallDocuments dbo.[udt_T_CallDocuments] READONLY
)
AS
BEGIN
	
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

	DECLARE @JSONString NVARCHAR(MAX) = NULL;

	IF EXISTS (SELECT * FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK) WHERE A.AssignmentId = @AssignmentId AND A.CaseNumber = @CaseNumber AND A.CallStatusId = 3)
	BEGIN
		SET @ErrorMsg = 'DB execution failed - CaseNumber with this AssignmentId is already closed';
		SELECT
			@ErrorMsg												AS [Message],
			''														AS ErrorMessage,
			0					                                    AS [Status],
			@AssignmentId				                            AS Id
			RETURN
	END


	IF(@TransactionType = 'CALL-I')
	BEGIN
	BEGIN TRY
	-----------------	

	BEGIN TRANSACTION
	-----------------

		INSERT INTO dbo.[T_CallDetails]
		(
			AssignmentId,
			UpdatedBy,
			UpdatedByEmail,
			UpdatedByADID,
			UpdatedOnUTC,
			CallActionId,
			CustomerName,
			CaseNumber,
			UnitSlNo,
			PassId,
			TaskStartDateTimeIST,
			TaskEndDateTimeIST,
			CloserRemarks,
			PartConsumptionTypeId,
			SONo,
			RequiredPart,
			RequiredSparePartNo,
			RequiredPartName,
			FaultyPartCTNo,
			FailureId,
			IssueDescription,
			TroubleshootingStep,
			FirstPartConsumptionTypeId,
			FirstPartSONo,
			FirstRequiredPartName,
			ReceivedPartConsumptionTypeId,
			ReceivedPartName
		)
		VALUES
		(
			@AssignmentId,
			@UpdatedBy,
			@UpdatedByEmail,
			@UpdatedByADID,
			GETUTCDATE(),
			@CallActionId,
			@CustomerName,
			@CaseNumber,
			@UnitSlNo,
			@PassId,
			@TaskStartDateTimeIST,
			@TaskEndDateTimeIST,
			@CloserRemarks,
			@PartConsumptionTypeId,
			@SONo,
			@RequiredPart,
			@RequiredSparePartNo,
			@RequiredPartName,
			@FaultyPartCTNo,
			@FailureId,
			@IssueDescription,
			@TroubleshootingStep,
			@FirstPartConsumptionTypeId,
			@FirstPartSONo,
			@FirstRequiredPartName,
			@ReceivedPartConsumptionTypeId,
			@ReceivedPartName
		);

		SET @CallDetailId = @@IDENTITY;

		IF(@CallDetailId > 0)
		BEGIN

			UPDATE dbo.[T_EngineerAssignment]
			SET CallActionId = ISNULL(@CallActionId, CallActionId),
			CallStatusId = ISNULL(@CallStatusId, CallStatusId)
			WHERE AssignmentId = @AssignmentId AND CaseNumber = @CaseNumber;

			IF(@CallActionId = 1 AND @CallStatusId = 3)
			BEGIN

				UPDATE dbo.[T_EngineerAssignment]
				SET ClosedBy = ISNULL(@UpdatedBy, ClosedBy),
				ClosedByEmail = ISNULL(@UpdatedByEmail, ClosedByEmail),
				ClosedByADID = ISNULL(@UpdatedByADID, ClosedByADID),
				ClosedOnUTC = GETUTCDATE()
				WHERE AssignmentId = @AssignmentId AND CaseNumber = @CaseNumber;


				SET @SuccessMsg = 'DB execution successful - CallDetails inserted';

			END

			IF EXISTS(SELECT * FROM @udt_CallDocuments CD)
			BEGIN

				INSERT INTO dbo.[T_CallDocuments]
				(
					CallDetailId,
					DocumentTypeId,
					DocumentName,
					MimeType,
					DocumentUrlPath,
					IsActive,
					InternalName,
					CreatedOnUCT
				)
				SELECT
					@CallDetailId,
					CD.DocumentTypeId,
					CD.DocumentName,
					CD.MimeType,
					CD.DocumentUrlPath,
					1,
					CD.InternalName,
					GETUTCDATE()
				FROM @udt_CallDocuments CD

				SET @SuccessMsg = 'DB execution successful - CallDetails & Documents inserted';

			END			

			INSERT INTO @OutputTbl
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
				CD.CallDetailId,
				CA.CallAction,
				FORMAT(SWITCHOFFSET(CD.UpdatedOnUTC, '+05:30'),'dd-MM-yyyy HH:mm:ss'),
				CD.UpdatedBy,
				CD.UpdatedByEmail,
				CD.UpdatedByADID,
				CD.CloserRemarks,
				A.AdminClosureRemarks
			FROM dbo.[T_CallDetails] CD WITH(NOLOCK)
			INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = CD.CaseNumber
			INNER JOIN dbo.[T_EngineerAssignment] A WITH(NOLOCK) ON A.CaseNumber = TD.CaseNumber
			INNER JOIN dbo.[T_EngineerAssignmentHistory] AH WITH(NOLOCK) ON AH.AssignmentId = A.AssignmentId
			LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
			LEFT JOIN dbo.[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = CD.CallActionId
			WHERE CD.CallDetailId = @CallDetailId
			AND CD.CaseNumber = @CaseNumber
			AND CD.AssignmentId = @AssignmentId

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
				FROM @OutputTbl
			FOR JSON AUTO
			);

		END
		

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Insert CallDetails';
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

	IF(@TransactionType = 'CALL-U')
	BEGIN
	BEGIN TRY
	-----------------

	BEGIN TRANSACTION
	-----------------
		
		SET @SuccessMsg = 'DB execution successful - CallDetails updated';

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Update CallDetails'
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


	COMMIT TRANSACTION
	-----------------
	SELECT
		@SuccessMsg												AS [Message],
		''						                                AS ErrorMessage,
		1					                                    AS [Status],
		@CallDetailId											AS Id,
		@AssignmentId											AS ReferenceNo,
		@JSONString												AS ReferenceObject
	RETURN
END