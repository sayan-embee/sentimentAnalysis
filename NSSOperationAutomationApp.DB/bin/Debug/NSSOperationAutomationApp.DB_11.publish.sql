﻿/*
Deployment script for NSSOperationAutomationP1

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "NSSOperationAutomationP1"
:setvar DefaultFilePrefix "NSSOperationAutomationP1"
:setvar DefaultDataPath "D:\Microsoft SQL Server\MSSQL16.MSSQLSERVER01\MSSQL\DATA\"
:setvar DefaultLogPath "D:\Microsoft SQL Server\MSSQL16.MSSQLSERVER01\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Altering Procedure [dbo].[usp_T_EngineerAssignment_InsertUpdate]...';


GO
ALTER PROCEDURE [dbo].[usp_T_EngineerAssignment_InsertUpdate]
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

	--DECLARE @OutputTbl AS TABLE
	--(
	--	[CaseNumber]		VARCHAR (50),
	--	[TicketId]			BIGINT,
	--	[CreatedOn]			DATETIME,
	--	[AssignedBy]		NVARCHAR (100),
	--	[AssignedByEmail]	VARCHAR (100),
	--	[AssignedOn]		DATETIME,
	--	[ServiceAccount]    VARCHAR (100)
	--);

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
			CreatedOnUTC
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
			GETUTCDATE()
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

			SET @JSONString = (
				SELECT
					TD.CaseNumber,
					TD.TicketId,
					FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS CreatedOn,
					A.AssignedBy,
					A.AssignedByEmail,
					A.AssignedByADID,
					A.AssignedTo,
					A.AssignedToEmail,
					A.AssignedToADID,
					FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS AssignedOn,
					TD.ServiceAccount,
					CS.CallStatus,
					A.AssignmentId,
					@AssignmentHistoryId AS AssignmentHistoryId
				FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
				INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = A.CaseNumber
				LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
			FOR JSON AUTO
			);

			SET @SuccessMsg = 'DB execution successful - EngineerAssignment inserted';

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
		AssignedByADID = ISNULL(@AssignedByADID, AssignedByADID)
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

		SET @JSONString = (
			SELECT
				TD.CaseNumber,
				TD.TicketId,
				FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS CreatedOn,
				A.AssignedBy,
				A.AssignedByEmail,
				A.AssignedByADID,
				A.AssignedTo,
				A.AssignedToEmail,
				A.AssignedToADID,
				FORMAT(SWITCHOFFSET(A.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as AssignedOn,
				TD.ServiceAccount,
				CS.CallStatus,
				A.AssignmentId,
				@AssignmentHistoryId AS AssignmentHistoryId
			FROM dbo.[T_EngineerAssignment] A WITH(NOLOCK)
			INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = A.CaseNumber
			LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
			WHERE A.AssignmentId = @AssignmentId
		FOR JSON AUTO
		);

		SET @SuccessMsg = 'DB execution successful - EngineerAssignment updated';

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
GO
PRINT N'Update complete.';


GO