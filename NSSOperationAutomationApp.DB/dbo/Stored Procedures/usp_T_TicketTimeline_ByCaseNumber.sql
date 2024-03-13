CREATE PROCEDURE [dbo].[usp_T_TicketTimeline_ByCaseNumber]
(
	@CaseNumber VARCHAR(50) = NULL
)
AS
BEGIN

;WITH TicketTimeline AS (
    SELECT
        TD.CaseNumber,
        TD.CreatedOn AS EventTimeUTC,
        FORMAT(SWITCHOFFSET(TD.CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS EventTimeIST,
        'Ticket Created' AS EventType,
        NULL AS AssignmentId,
        NULL AS CallDetailId
    FROM dbo.[T_TicketDetails] TD WITH(NOLOCK) WHERE TD.CaseNumber = @CaseNumber

    UNION

	SELECT
		EAH.CaseNumber,
		EAH.AssignedOnUTC AS EventTimeUTC,
		FORMAT(SWITCHOFFSET(EAH.AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS EventTimeIST,
		'Engineer Assigned' AS EventType,
		EAH.AssignmentHistoryId AS AssignmentId,
		NULL AS CallDetailId
	FROM dbo.[T_EngineerAssignmentHistory] EAH WITH(NOLOCK) WHERE EAH.CaseNumber = @CaseNumber

    UNION

    SELECT
        CD.CaseNumber,
        CD.UpdatedOnUTC AS EventTimeUTC,
		FORMAT(SWITCHOFFSET(CD.UpdatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS EventTimeIST,
        'Action Taken' AS EventType,
        NULL AS AssignmentId,
        CD.CallDetailId AS CallDetailId
    FROM dbo.[T_CallDetails] CD WITH(NOLOCK) WHERE CD.CaseNumber = @CaseNumber

	UNION

	SELECT
			EA.CaseNumber,
			EA.ClosedOnUTC AS EventTimeUTC,
			FORMAT(SWITCHOFFSET(EA.ClosedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') AS EventTimeIST,
			'Admin - Ticket Closed' AS EventType,
			0 AS AssignmentId,
			NULL AS CallDetailId
	FROM dbo.[T_EngineerAssignment] EA WITH(NOLOCK) WHERE EA.CaseNumber = @CaseNumber AND AdminClosureRemarks IS NOT NULL
	
)

SELECT
	ROW_NUMBER() OVER (PARTITION BY TT.CaseNumber ORDER BY TT.EventTimeUTC) AS SlNo,
	TT.CaseNumber,
	TT.EventTimeUTC,
	TT.EventTimeIST,
	TT.EventType,
	TT.AssignmentId,
	TT.CallDetailId,
	CA.CallAction,
	EAH.AssignmentHistoryId,
	EAH.AssignedTo,
	EAH.AssignedToEmail,
	EAH.AssignedBy,
	EAH.AssignedByEmail,
	CD.UpdatedBy,
	CD.UpdatedByEmail,
	CD.CustomerName,
	CD.UnitSlNo,
	CD.PassId,
	CD.TaskStartDateTimeIST,
	CD.TaskEndDateTimeIST,
	CD.CloserRemarks,
	CD.PartConsumptionTypeId,
	(SELECT PCT.PartConsumptionType FROM dbo.[M_PartConsumptionType] PCT WITH(NOLOCK) WHERE PCT.PartConsumptionTypeId = CD.PartConsumptionTypeId) AS PartConsumptionType,
	CD.SONo,
	CD.RequiredPart,
	CD.RequiredSparePartNo,
	CD.RequiredPartName,
	CD.FaultyPartCTNo,
	CD.FailureId,
	CD.IssueDescription,
	CD.TroubleshootingStep,
	CD.FirstPartConsumptionTypeId,
	(SELECT PCT.PartConsumptionType FROM dbo.[M_PartConsumptionType] PCT WITH(NOLOCK) WHERE PCT.PartConsumptionTypeId = CD.FirstPartConsumptionTypeId) AS FirstPartConsumptionType,
	CD.FirstPartSONo,
	CD.FirstRequiredPartName,
	CD.ReceivedPartConsumptionTypeId,
	(SELECT PCT.PartConsumptionType FROM dbo.[M_PartConsumptionType] PCT WITH(NOLOCK) WHERE PCT.PartConsumptionTypeId = CD.ReceivedPartConsumptionTypeId) AS ReceivedPartConsumptionType,
	CD.ReceivedPartName,
	EA.AdminClosureRemarks,
	EA.ClosedBy,
	EA.ClosedByEmail,
	EA.TicketId
FROM TicketTimeline TT

LEFT JOIN dbo.[T_EngineerAssignmentHistory] EAH WITH(NOLOCK) ON EAH.AssignmentHistoryId = TT.AssignmentId

LEFT JOIN dbo.[T_EngineerAssignment] EA WITH(NOLOCK) ON TT.CaseNumber = EA.CaseNumber AND TT.AssignmentId = 0

LEFT JOIN dbo.[T_CallDetails] CD WITH(NOLOCK) ON CD.CallDetailId = TT.CallDetailId

LEFT JOIN dbo.[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = CD.CallActionId

ORDER BY TT.EventTimeUTC DESC;

------------------------------

SELECT 
	[DocumentId]
	,CD.[CallDetailId]
	,CD.[DocumentTypeId]
	,DT.DocumentType
	,[DocumentName]
	,[MimeType]
	,[DocumentUrlPath]
	,[IsActive]
	,[InternalName]
	,[CreatedOnUCT]
	,CD.[UpdatedOnUTC]
FROM [dbo].[T_CallDocuments] CD WITH(NOLOCK)

INNER JOIN [dbo].[M_DocumentType] DT WITH(NOLOCK) ON DT.DocumentTypeId = CD.DocumentId

INNER JOIN [dbo].[T_CallDetails] CD2 WITH(NOLOCK) ON CD2.CallDetailId = CD.CallDetailId AND CD2.CaseNumber = @CaseNumber

ORDER BY CD.[CallDetailId] DESC

END