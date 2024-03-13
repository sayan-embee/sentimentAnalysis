CREATE PROCEDURE [dbo].[usp_T_CallDocuments_Get]
  (
	@DocumentId BIGINT = NULL,
	@CallDetailId BIGINT = NULL
  )
  AS
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
			,[CreatedOnUCT]
			,[UpdatedOnUTC]
		FROM [dbo].[T_CallDocuments] CD WITH(NOLOCK)

		INNER JOIN [dbo].[M_DocumentType] DT WITH(NOLOCK) ON DT.DocumentTypeId = CD.DocumentId

		WHERE @DocumentId IS NULL OR CD.DocumentId = @DocumentId
		AND @CallDetailId IS NULL OR CD.CallDetailId = @CallDetailId
		ORDER BY DocumentId DESC

  END