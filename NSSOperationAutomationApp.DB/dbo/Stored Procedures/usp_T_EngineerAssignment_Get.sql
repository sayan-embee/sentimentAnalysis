CREATE PROCEDURE [dbo].[usp_T_EngineerAssignment_Get]
  (
	@AssignmentId BIGINT = NULL,
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
	@CallStatusId INT = NULL,
	@CallActionId INT = NULL
  )
  AS
  BEGIN

		SELECT 
			[AssignmentId]
			,[CaseNumber]
			,[TicketId]
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

		WHERE (@AssignmentId IS NULL OR EA.AssignmentId = @AssignmentId)
			AND (ISNULL(@CaseNumber,'')='' OR EA.CaseNumber LIKE @CaseNumber + '%')
			AND (@TicketId IS NULL OR EA.TicketId = @TicketId)
			AND (ISNULL(@AssignedTo,'')='' OR EA.AssignedTo LIKE @AssignedTo + '%')
			AND (ISNULL(@AssignedToEmail,'') = '' OR EA.AssignedToEmail = @AssignedToEmail)
			AND (ISNULL(@FromDate,'')= '' OR CONVERT(DATE, SWITCHOFFSET(EA.AssignedOnUTC, '+05:30')) >= CONVERT(DATE, @FromDate,103))
			AND (ISNULL(@ToDate ,'')= '' OR  CONVERT(DATE, SWITCHOFFSET(EA.AssignedOnUTC, '+05:30')) <= CONVERT(DATE, @ToDate,103))
		ORDER BY EA.AssignmentId DESC

		IF(@AssignmentId IS NOT NULL AND @AssignmentId > 0)
		BEGIN

			SELECT
				[AssignmentHistoryId]
				  ,[AssignmentId]
				  ,[CaseNumber]
				  ,[TicketId]
				  ,[AssignedTo]
				  ,[AssignedToEmail]
				  ,[AssignedToADID]
				  --,[AssignedOnUTC]
				  ,FORMAT(SWITCHOFFSET(AssignedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as AssignedOnUTC
				  ,[AssignedBy]
				  ,[AssignedByEmail]
				  ,[AssignedByADID]
				  ,CASE 
						WHEN EAH.[CallActionId] IS NOT NULL THEN CA.CallAction
						ELSE 'Action Pending'
					END AS 'CallAction'
				  ,EAH.[CallStatusId]
				  ,CS.CallStatus
				  ,[ClosedBy]
				  ,[ClosedByEmail]
				  ,[ClosedByADID]
				  --,[ClosedOnUTC]
				  ,FORMAT(SWITCHOFFSET(ClosedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as ClosedOnUTC
				  ,[AdminClosureRemarks]
				  --,[CreatedOnUTC]
				  ,FORMAT(SWITCHOFFSET(CreatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOnUTC
		  FROM [dbo].[T_EngineerAssignmentHistory] EAH WITH(NOLOCK)

		  INNER JOIN [dbo].[M_CallStatus] CS WITH(NOLOCK) ON CS.CallStatusId = EAH.CallStatusId

		  LEFT JOIN [dbo].[M_CallAction] CA WITH(NOLOCK) ON CA.CallActionId = EAH.CallActionId

		  WHERE EAH.AssignmentId = @AssignmentId

		END

  END