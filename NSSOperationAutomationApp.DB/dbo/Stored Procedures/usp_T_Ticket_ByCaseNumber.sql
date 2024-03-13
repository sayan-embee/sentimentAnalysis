CREATE PROCEDURE [dbo].[usp_T_Ticket_ByCaseNumber]
(
	@CaseNumber VARCHAR(50) = NULL,
	@TicketId BIGINT = NULL,
	@SerialNumber NVARCHAR(50) = NULL,
	@PartNumber NVARCHAR(100) = NULL,
    @FromDate VARCHAR(10) = NULL,
    @ToDate VARCHAR(10) = NULL,
	@AssignedTo NVARCHAR(100) = NULL,
	@AssignedToEmail VARCHAR(50) = NULL,
	@AssignedBy NVARCHAR(100) = NULL,
	@AssignedByEmail VARCHAR(50) = NULL,
	@IsAssigned BIT = NULL,
	@CallStatusId INT = NULL,
	@CallActionId INT = NULL
)
AS
BEGIN

	SELECT
        TD.CaseNumber,
		TD.TicketId,
		TD.TicketType,
        FromEmail,
        ToEmail,
        CaseSubject,
        WorkOrderNumber,
        ServiceAccount,
        ContactName,
        ContactEmail,
        ContactPhone,
        ServiceDeliveryStreetAddress,
        ServiceDeliveryCity,
        ServiceDeliveryCountry,
        PostalCode,
        SerialNumber,
        ProductName,
        ProductNumber,
        OTCCode,
        PartNumber,
        PartDescription,
        EmailSubject,
        FORMAT(SWITCHOFFSET(EmailDate, '+05:30'),'yyyy-MM-dd HH:mm:ss') as EmailDate,
        FORMAT(SWITCHOFFSET(CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOn,
        ISNULL(PartNumber2,'') AS PartNumber2,
        ISNULL(PartDescription2,'')  AS PartDescription2,
        ISNULL(PartNumber3,'') AS PartNumber3,
        ISNULL(PartDescription3,'') AS PartDescription3,
		EA.AssignmentId,
		CASE
			WHEN EA.AssignmentId IS NOT NULL THEN EA.AssignedTo +' (' + EA.AssignedToEmail + ')'
			ELSE 'Not Assigned'
		END AS AssignedTo,
		CASE
			WHEN EA.AssignmentId IS NOT NULL THEN EA.AssignedBy +' (' + EA.AssignedByEmail + ')'
			ELSE 'Not Assigned'
		END AS AssignedBy,
		CASE
			WHEN EA.AssignmentId IS NOT NULL THEN CS.CallStatus
			ELSE '-'
		END AS CallStatus,
		CASE
			WHEN EA.AssignmentId IS NOT NULL AND CA.CallActionId IS NOT NULL THEN CA.CallAction
			ELSE '-'
		END AS CallAction
    FROM [dbo].[T_TicketDetails] TD WITH(NOLOCK)

	LEFT JOIN [dbo].[T_EngineerAssignment] EA ON EA.CaseNumber = TD.CaseNumber

	LEFT JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = EA.CallStatusId

	LEFT JOIN [dbo].[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = EA.CallActionId	

    WHERE
        (ISNULL(@CaseNumber,'')='' OR TD.CaseNumber LIKE @CaseNumber + '%')
		AND (@TicketId IS NULL OR TD.TicketId = @TicketId)
        AND (ISNULL(@SerialNumber,'') = '' OR SerialNumber LIKE @SerialNumber + '%')
        AND (ISNULL(@PartNumber,'')= '' OR PartNumber LIKE @PartNumber + '%')
        AND (ISNULL(@FromDate,'')= '' OR CONVERT(DATE, SWITCHOFFSET(EmailDate, '+05:30')) >= CONVERT(DATE, @FromDate,103))
        AND (ISNULL(@ToDate ,'')= '' OR  CONVERT(DATE, SWITCHOFFSET(EmailDate, '+05:30')) <= CONVERT(DATE, @ToDate,103))
		AND
		(
			(@IsAssigned IS NULL)
			OR
			(@IsAssigned = 0 AND EA.AssignmentId IS NULL)
			OR
			(@IsAssigned = 1 AND EA.AssignmentId IS NOT NULL)
		)
		AND (ISNULL(@AssignedTo,'')='' OR EA.AssignedTo LIKE @AssignedTo + '%')
		AND (ISNULL(@AssignedToEmail,'') = '' OR EA.AssignedToEmail = @AssignedToEmail)
		AND (ISNULL(@AssignedBy,'')='' OR EA.AssignedBy LIKE @AssignedBy + '%')
		AND (ISNULL(@AssignedByEmail,'') = '' OR EA.AssignedByEmail = @AssignedByEmail)
		AND (@CallStatusId IS NULL OR EA.CallStatusId = @CallStatusId)
		AND (@CallActionId IS NULL OR EA.CallActionId = @CallActionId)
	ORDER BY EmailDate DESC;


	IF(ISNULL(@CaseNumber,'') != '')
	BEGIN

		SELECT 
			[AssignmentId]
			,[CaseNumber]
			--,[TicketId]
			,[AssignedTo]
			,[AssignedToEmail]
			,[AssignedToADID]
			--,[AssignedOnUTC]
			,FORMAT(SWITCHOFFSET(AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as AssignedOnUTC
			,[AssignedBy]
			,[AssignedByEmail]
			,[AssignedByADID]
			,EA.[CallActionId]
			,CASE 
				WHEN EA.[CallActionId] IS NOT NULL THEN CA.CallAction
				ELSE 'Action Pending'
			 END AS 'CallAction'
			,EA.[CallStatusId]
			,CS.CallStatus
			,[ClosedBy]
			,[ClosedByEmail]
			--,[ClosedOnUTC]
			,FORMAT(SWITCHOFFSET(ClosedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as ClosedOnUTC
			,[AdminClosureRemarks]
			,[ClosedByADID]
			--,[CreatedOnUTC]
			,FORMAT(SWITCHOFFSET(CreatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOnUTC
		FROM [dbo].[T_EngineerAssignment] EA WITH(NOLOCK)

		INNER JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = EA.CallStatusId

		LEFT JOIN [dbo].[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = EA.CallActionId		

		WHERE EA.CaseNumber = @CaseNumber;

		SELECT
			[AssignmentHistoryId]
			,EAH.[AssignmentId]
			,EAH.[AssignedTo]
			,EAH.[AssignedToEmail]
			,EAH.[AssignedToADID]
			--,[AssignedOnUTC]
			,FORMAT(SWITCHOFFSET(EAH.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as AssignedOnUTC
			,EAH.[AssignedBy]
			,EAH.[AssignedByEmail]
			,EAH.[AssignedByADID]
			,EAH.[CallActionId]
			,CASE 
				WHEN EAH.[CallActionId] IS NOT NULL THEN CA.CallAction
				ELSE 'Action Pending'
			END AS 'CallAction'
			,EAH.[CallStatusId]
			,CS.CallStatus
			,EAH.[ClosedBy]
			,EAH.[ClosedByEmail]
			,EAH.[ClosedByADID]
			--,[ClosedOnUTC]
			,FORMAT(SWITCHOFFSET(EAH.ClosedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as ClosedOnUTC
			,EAH.[AdminClosureRemarks]
			--,[CreatedOnUTC]
			,FORMAT(SWITCHOFFSET(EAH.CreatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOnUTC
		FROM [dbo].[T_EngineerAssignmentHistory] EAH WITH(NOLOCK)

		INNER JOIN [dbo].[T_EngineerAssignment] EA WITH(NOLOCK)
		ON EAH.AssignmentId = EA.AssignmentId
		AND EA.CaseNumber = @CaseNumber

		INNER JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = EAH.CallStatusId

		LEFT JOIN [dbo].[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = EAH.CallActionId		

		ORDER BY EAH.AssignmentHistoryId DESC;


		SELECT 
			[CallDetailId]
			,[AssignmentId]
			,[UpdatedBy]
			,[UpdatedByEmail]
			,[UpdatedByADID]
			--,[UpdatedOnUTC]
			,FORMAT(SWITCHOFFSET(UpdatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as UpdatedOnUTC
			,CD.[CallActionId]
			,CA.CallAction
			,[CustomerName]
			,[CaseNumber]
			,[UnitSlNo]
			,[PassId]
			,[TaskStartDateTimeIST]
			,[TaskEndDateTimeIST]
			,[CloserRemarks]
			,CD.[PartConsumptionTypeId]
			,PCT.PartConsumptionType
			,[SONo]
			,[RequiredPart]
			,[RequiredSparePartNo]
			,[RequiredPartName]
			,[FaultyPartCTNo]
			,[FailureId]
			,[IssueDescription]
			,[TroubleshootingStep]
			,[FirstPartConsumptionTypeId]
			--,PCT.PartConsumptionType AS 'FirstPartConsumptionType'
			,(SELECT PCT.PartConsumptionType FROM dbo.[M_PartConsumptionType] PCT WITH(NOLOCK) WHERE PCT.PartConsumptionTypeId = FirstPartConsumptionTypeId) AS 'FirstPartConsumptionType'
			,[FirstPartSONo]
			,[FirstRequiredPartName]
			,[ReceivedPartConsumptionTypeId]
			--,PCT.PartConsumptionType AS 'ReceivedPartConsumptionType'
			,(SELECT PCT.PartConsumptionType FROM dbo.[M_PartConsumptionType] PCT WITH(NOLOCK) WHERE PCT.PartConsumptionTypeId = ReceivedPartConsumptionTypeId) AS 'ReceivedPartConsumptionType'
			,[ReceivedPartName]
		FROM [dbo].[T_CallDetails] CD WITH(NOLOCK)

		INNER JOIN [dbo].[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = CD.CallActionId

		LEFT JOIN [dbo].[M_PartConsumptionType] PCT WITH(NOLOCK) ON (PCT.PartConsumptionTypeId = CD.PartConsumptionTypeId)

		WHERE CD.CaseNumber = @CaseNumber
		ORDER BY CallDetailId DESC;


		SELECT 
			[DocumentId]
			,CDOC.[CallDetailId]
			,CDOC.[DocumentTypeId]
			,DT.DocumentType
			,[DocumentName]
			,[MimeType]
			,[DocumentUrlPath]
			,[IsActive]
			,[InternalName]
			--,[CreatedOnUCT]
			,FORMAT(SWITCHOFFSET(CreatedOnUCT, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOnUCT
			--,CDOC.[UpdatedOnUTC]
			,FORMAT(SWITCHOFFSET(CDOC.[UpdatedOnUTC], '+05:30'),'yyyy-MM-dd HH:mm:ss') as UpdatedOnUTC
		FROM [dbo].[T_CallDocuments] CDOC WITH(NOLOCK)

		INNER JOIN [dbo].[M_DocumentType] DT WITH(NOLOCK) ON DT.DocumentTypeId = CDOC.DocumentId

		INNER JOIN [dbo].[T_CallDetails] CD WITH(NOLOCK) 
		ON CD.CallDetailId = CDOC.CallDetailId 
		AND CD.CaseNumber = @CaseNumber
		ORDER BY DocumentId DESC;


	END

END
