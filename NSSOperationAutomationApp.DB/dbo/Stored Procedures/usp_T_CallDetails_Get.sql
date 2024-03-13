CREATE PROCEDURE [dbo].[usp_T_CallDetails_Get]
  (
	@CallDetailId BIGINT = NULL,
	@AssignmentId BIGINT = NULL,
	@CaseNumber VARCHAR(50) = NULL,
	@UpdatedBy VARCHAR(100) = NULL,
	@UpdatedByEmail VARCHAR(50) = NULL,
    @FromDate VARCHAR(10) = NULL,
    @ToDate VARCHAR(10) = NULL
  )
  AS
  BEGIN

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

		LEFT JOIN [dbo].[M_PartConsumptionType] PCT WITH(NOLOCK) 
		ON (PCT.PartConsumptionTypeId = CD.PartConsumptionTypeId)
		OR (PCT.PartConsumptionTypeId = CD.FirstPartConsumptionTypeId)
		OR (PCT.PartConsumptionTypeId = CD.ReceivedPartConsumptionTypeId)

		WHERE (@CallDetailId IS NULL OR CD.CallDetailId = @CallDetailId)
			AND (@AssignmentId IS NULL OR CD.AssignmentId = @AssignmentId)
			AND (ISNULL(@CaseNumber,'')='' OR CD.CaseNumber LIKE @CaseNumber + '%')
			AND (ISNULL(@UpdatedBy,'')='' OR CD.UpdatedBy LIKE @UpdatedBy + '%')
			AND (ISNULL(@UpdatedByEmail,'') = '' OR CD.UpdatedByEmail = @UpdatedByEmail)
			AND (ISNULL(@FromDate,'')= '' OR CONVERT(DATE, SWITCHOFFSET(CD.UpdatedOnUTC, '+05:30')) >= CONVERT(DATE, @FromDate,103))
			AND (ISNULL(@ToDate ,'')= '' OR  CONVERT(DATE, SWITCHOFFSET(CD.UpdatedOnUTC, '+05:30')) <= CONVERT(DATE, @ToDate,103))
		ORDER BY CD.CallDetailId DESC


		IF(@CallDetailId IS NOT NULL AND @CallDetailId > 0)
		BEGIN

			SELECT 
				[DocumentId]
				,[CallDetailId]
				,CD.[DocumentTypeId]
				,DT.DocumentType
				,[DocumentName]
				,[MimeType]
				,[DocumentUrlPath]
				,[IsActive]
				,[InternalName]
				--,[CreatedOnUCT]
				,FORMAT(SWITCHOFFSET(CreatedOnUCT, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOnUCT
				--,[UpdatedOnUTC]
				,FORMAT(SWITCHOFFSET(UpdatedOnUTC, '+05:30'),'yyyy-MM-dd HH:mm:ss') as UpdatedOnUTC
			FROM [dbo].[T_CallDocuments] CD WITH(NOLOCK)

			INNER JOIN [dbo].[M_DocumentType] DT WITH(NOLOCK) ON DT.DocumentTypeId = CD.DocumentId

			WHERE CD.CallDetailId = @CallDetailId
			ORDER BY DocumentId DESC

		END
  END