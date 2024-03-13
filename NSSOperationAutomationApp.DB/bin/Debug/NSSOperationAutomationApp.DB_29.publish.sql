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
PRINT N'Altering Procedure [dbo].[usp_TicketInBulk_Insert]...';


GO
ALTER PROCEDURE [dbo].[usp_TicketInBulk_Insert]
(
	@udt_TicketDetailsList dbo.[udt_T_TicketDetails] READONLY
)
AS
BEGIN

	DECLARE @SuccessMsg VARCHAR(100);
	DECLARE @ErrorMsg VARCHAR(100);

	DECLARE @DuplicateTicketIds VARCHAR(MAX) = NULL;

	DECLARE @DuplicateTicketTbl AS TABLE
	(
		CaseNumber VARCHAR(50) NULL
	)

	DECLARE @OutputTbl AS TABLE
	(
		[CaseNumber]		VARCHAR (50)	NULL,
		[TicketId]			BIGINT			NULL,
		[CaseSubject]       NVARCHAR (500)  NULL,
		[ContactName]       VARCHAR (100)   NULL,
		[ContactEmail]      VARCHAR (100)   NULL,
		[SerialNumber]      NVARCHAR (50)   NULL,
		[ProductName]       NVARCHAR (100)  NULL,
		[ProductNumber]     VARCHAR (50)    NULL,
		[CreatedOn]			VARCHAR (50)	NULL,
		[AssignedBy]		NVARCHAR (100)	NULL,
		[AssignedByEmail]	VARCHAR (100)	NULL,
		[AssignedByADID]	VARCHAR (50)	NULL,
		[AssignedTo]		NVARCHAR (100)	NULL,
		[AssignedToEmail]	VARCHAR (100)	NULL,
		[AssignedToADID]	VARCHAR (50)	NULL,
		[AssignedOn]		VARCHAR(50)		NULL,
		[ServiceAccount]    VARCHAR (100)	NULL,
		[CallStatus]		VARCHAR (100)	NULL,
		[AssignmentId]		BIGINT			NULL,
		[AssignmentHistoryId]	BIGINT		NULL,
		[ConversationId]    NVARCHAR (200)  NULL,
		[ServiceUrl]        NVARCHAR (200)  NULL,
		[InsertStatus]		BIT				NULL,
		[InsertMsg]			NVARCHAR (200)  NULL
	);

	DECLARE @JSONString NVARCHAR(MAX) = NULL;


	--INSERT INTO @DuplicateTicketTbl SELECT udt.CaseNumber FROM @udt_TicketDetailsList udt WHERE udt.CaseNumber IN (SELECT TD.CaseNumber FROM dbo.[T_TicketDetails] TD WITH(NOLOCK));

	--IF EXISTS (SELECT * FROM @DuplicateTicketTbl)
	--BEGIN
		
	--	;WITH CTE AS
 --       (
	--		SELECT CaseNumber
	--		FROM @DuplicateTicketTbl
 --       )
 --       SELECT @DuplicateTicketIds = CONCAT(@DuplicateTicketIds,',',CaseNumber) FROM CTE

	--	SET @DuplicateTicketIds = 'Following ticket id(s) already exist in the database: ' + @DuplicateTicketIds;

	--END


	-- INSERT DUPLICATE CASE NUMBER RECORDS
	INSERT INTO @OutputTbl
	(
	  [CaseNumber],
	  [TicketId],
	  [CaseSubject],
	  [ContactName],
	  [ContactEmail],
	  [SerialNumber],
	  [ProductName],
	  [ProductNumber],
	  [CreatedOn],
	  [AssignedBy],
	  [AssignedByEmail],
	  [AssignedByADID],
	  [AssignedTo],
	  [AssignedToEmail],
	  [AssignedToADID],
	  [AssignedOn],
	  [ServiceAccount],
	  [CallStatus],
	  [AssignmentId],
	  [AssignmentHistoryId],
	  --[ConversationId],
	  --[ServiceUrl],
	  [InsertStatus],
	  [InsertMsg]
	)
	SELECT
		udt.CaseNumber,
		udt.TicketId,
		TD.CaseSubject,
		TD.ContactName,
		TD.ContactEmail,
		TD.SerialNumber,
		TD.ProductName,
		TD.ProductNumber,
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
		'Ticket Creation Failed: Duplicate ticket id - Ticket already exist in the database'		
	FROM @udt_TicketDetailsList udt
	INNER JOIN dbo.[T_TicketDetails] TD WITH(NOLOCK) ON TD.CaseNumber = udt.CaseNumber
	INNER JOIN dbo.[T_EngineerAssignment] A WITH(NOLOCK) ON A.CaseNumber = TD.CaseNumber
	INNER JOIN dbo.[T_EngineerAssignmentHistory] AH WITH(NOLOCK) ON AH.AssignmentId = A.AssignmentId
	LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId;


	IF EXISTS (SELECT * FROM @OutputTbl)
	BEGIN
		
		;WITH CTE AS
        (
			SELECT CaseNumber
			FROM @OutputTbl
        )
        SELECT @DuplicateTicketIds = CONCAT(@DuplicateTicketIds,',',CaseNumber) FROM CTE

		SET @DuplicateTicketIds = 'Following ticket id(s) already exist in the database: ' + @DuplicateTicketIds;

	END


	-- INSERT USER APP NOT INSTALLED RECORDS
	INSERT INTO @OutputTbl
	(
	  [CaseNumber],
	  [TicketId],
	  [CaseSubject],
	  [ContactName],
	  [ContactEmail],
	  [SerialNumber],
	  [ProductName],
	  [ProductNumber],
	  --[CreatedOn],
	  --[AssignedBy],
	  [AssignedByEmail],
	  --[AssignedByADID],
	  --[AssignedTo],
	  [AssignedToEmail],
	  --[AssignedToADID],
	  --[AssignedOn],
	  --[ServiceAccount],
	  --[CallStatus],
	  --[AssignmentId],
	  --[AssignmentHistoryId],
	  --[ConversationId],
	  --[ServiceUrl],
	  [InsertStatus],
	  [InsertMsg]
	)
	SELECT
		udt.CaseNumber,
		udt.TicketId,
		udt.CaseSubject,
		udt.ContactName,
		udt.ContactEmail,
		udt.SerialNumber,
		udt.ProductName,
		udt.ProductNumber,
		udt.AssignedByEmail,
		udt.AssignedToEmail,
		0,
		'Ticket Creation Failed: Engineer app not installed by roaster engineer'		
	FROM @udt_TicketDetailsList udt 
	WHERE udt.AssignedToEmail NOT IN (SELECT C.UserEmail FROM dbo.[T_Conversations] C WITH(NOLOCK) WHERE C.Active = 1)
	AND udt.CaseNumber NOT IN (SELECT CaseNumber FROM @OutputTbl);
	

	UPDATE udt
	SET udt.AssignedTo = C.UserName,
	udt.AssignedToADID = C.UserId
	FROM @udt_TicketDetailsList udt,
	[dbo].[T_Conversations] C
	WHERE C.UserEmail = udt.AssignedToEmail;


	UPDATE udt
	SET udt.AssignedBy = C.UserName,
	udt.AssignedByADID = C.UserId
	FROM @udt_TicketDetailsList udt,
	[dbo].[T_Conversations] C
	WHERE C.UserEmail = udt.AssignedByEmail;


	BEGIN TRY
	-----------------

		BEGIN TRANSACTION
		-----------------

		-- INSERT TICKETS
		INSERT INTO [dbo].[T_TicketDetails] 
		(
			[CaseNumber],
			[TicketId],
			--[FromEmail],
			--[ToEmail],
			[CaseSubject],
			--[WorkOrderNumber],
			--[ServiceAccount],
			[ContactName],
			[ContactEmail],
			--[ContactPhone],
			--[ServiceDeliveryStreetAddress],
			--[ServiceDeliveryCity],
			--[ServiceDeliveryCountry],
			--[PostalCode],
			[SerialNumber],
			[ProductName],
			[ProductNumber],
			--[OTCCode],
			--[PartNumber],
			--[PartDescription],
			--[EmailSubject],
			--[EmailDate],
			[CreatedOn],
			--[PartNumber2],
			--[PartDescription2],
			--[PartNumber3],
			--[PartDescription3]
			[TicketType]
		)
		SELECT
			udt.CaseNumber,
			udt.TicketId,
			udt.CaseSubject,
			udt.ContactName,
			udt.ContactEmail,
			udt.SerialNumber,
			udt.ProductName,
			udt.ProductNumber,
			SWITCHOFFSET((CONVERT(DATETIME, udt.CreatedOn, 103)), '-05:30'),
			udt.TicketType
		FROM @udt_TicketDetailsList udt 
		WHERE udt.CaseNumber NOT IN (SELECT CaseNumber FROM @OutputTbl)

		SET @SuccessMsg = 'DB execution successful - Ticket(s) inserted from CSV';

		-- ASSIGN ENGINEERS
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
		SELECT
			udt.CaseNumber,
			udt.TicketId,
			udt.AssignedTo,
			udt.AssignedToEmail,
			udt.AssignedToADID,
			GETUTCDATE(),
			udt.AssignedBy,
			udt.AssignedByEmail,
			udt.AssignedByADID,
			1,
			GETUTCDATE()
		FROM @udt_TicketDetailsList udt
		INNER JOIN dbo.[T_TicketDetails] TD ON TD.CaseNumber = udt.CaseNumber
		WHERE udt.CaseNumber = TD.CaseNumber
		AND udt.CaseNumber NOT IN (SELECT CaseNumber FROM @OutputTbl);

		SET @SuccessMsg = @SuccessMsg + ' & assigned to engineer(s)';

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
			INNER JOIN @udt_TicketDetailsList udt ON udt.CaseNumber = A.CaseNumber
			WHERE A.CaseNumber = udt.CaseNumber
			AND A.CaseNumber NOT IN (SELECT CaseNumber FROM @OutputTbl);

		SET @SuccessMsg = @SuccessMsg + ' & updated assignment history';


		-- INSERT SUCCESS RECORDS
		INSERT INTO @OutputTbl
		(
			[CaseNumber],
			[TicketId],
			[CaseSubject],
			[ContactName],
			[ContactEmail],
			[SerialNumber],
			[ProductName],
			[ProductNumber],
			[CreatedOn],
			[AssignedBy],
			[AssignedByEmail],
			[AssignedByADID],
			[AssignedTo],
			[AssignedToEmail],
			[AssignedToADID],
			[AssignedOn],
			[ServiceAccount],
			[CallStatus],
			[AssignmentId],
			[AssignmentHistoryId],
			--[ConversationId],
			--[ServiceUrl],
			[InsertStatus],
			[InsertMsg]
		)
		SELECT
			TD.CaseNumber,
			TD.TicketId,
			TD.CaseSubject,
			TD.ContactName,
			TD.ContactEmail,
			TD.SerialNumber,
			TD.ProductName,
			TD.ProductNumber,
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
			1,
			'Ticket Creation Successful: Ticket Created & Assigned'
		FROM dbo.[T_TicketDetails] TD WITH(NOLOCK)
		INNER JOIN dbo.[T_EngineerAssignment] A WITH(NOLOCK) ON A.CaseNumber = TD.CaseNumber
		INNER JOIN dbo.[T_EngineerAssignmentHistory] AH WITH(NOLOCK) ON AH.AssignmentId = A.AssignmentId
		LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = A.CallStatusId
		WHERE TD.CaseNumber IN (SELECT CaseNumber FROM @udt_TicketDetailsList)
		AND TD.CaseNumber NOT IN (SELECT CaseNumber FROM @OutputTbl);


		UPDATE tbl
		SET tbl.ConversationId = C.ConversationId,
		tbl.ServiceUrl = C.ServiceUrl
		FROM @OutputTbl tbl,
		[dbo].[T_Conversations] C
		WHERE (C.UserEmail = tbl.AssignedToEmail OR C.UserEmail = tbl.AssignedByEmail)
		AND tbl.InsertStatus = 1;


		SET @JSONString = (
			SELECT
				CaseNumber,
				TicketId,
				CaseSubject,
				ContactName,
				ContactEmail,
				SerialNumber,
				ProductName,
				ProductNumber,
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
				ConversationId,
				ServiceUrl,
				InsertStatus,
				InsertMsg
			FROM @OutputTbl
		FOR JSON AUTO
		);

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
		
		ROLLBACK TRANSACTION
		SET @ErrorMsg = 'DB execution failed - Insert ticket(s) from CSV'
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
		@@IDENTITY												AS Id,
		@DuplicateTicketIds										AS ReferenceNo,
		@JSONString												AS ReferenceObject

END
GO
PRINT N'Update complete.';


GO
