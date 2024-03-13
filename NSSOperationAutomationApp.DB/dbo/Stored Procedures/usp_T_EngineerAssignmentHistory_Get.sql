CREATE PROCEDURE usp_T_EngineerAssignmentHistory_Get
  (
	@AssignmentId BIGINT = NULL,
    @FromDate VARCHAR(10) = NULL,
    @ToDate VARCHAR(10) = NULL
  )
  AS
  BEGIN

		SELECT 
			[AssignmentHistoryId]
			,[AssignmentId]
			,[CaseNumber]
			,[TicketId]
			,[AssignedTo]
			,[AssignedToEmail]
			,[AssignedToADID]
			,[AssignedOnUTC]
			,[AssignedBy]
			,[AssignedByEmail]
			,[AssignedByADID]
			,[CallActionId]
			,[CallStatusId]
			,[ClosedBy]
			,[ClosedByEmail]
			,[ClosedByADID]
			,[ClosedOnUTC]
			,[AdminClosureRemarks]
			,[CreatedOnUTC]
		FROM [dbo].[T_EngineerAssignmentHistory] EA WITH(NOLOCK)
		WHERE @AssignmentId IS NULL OR EA.AssignmentId = @AssignmentId
			AND (ISNULL(@FromDate,'')= '' OR CONVERT(DATE, SWITCHOFFSET(EA.AssignedOnUTC, '+05:30')) >= CONVERT(DATE, @FromDate,103))
			AND (ISNULL(@ToDate ,'')= '' OR  CONVERT(DATE, SWITCHOFFSET(EA.AssignedOnUTC, '+05:30')) <= CONVERT(DATE, @ToDate,103))
		ORDER BY EA.AssignmentHistoryId DESC

  END