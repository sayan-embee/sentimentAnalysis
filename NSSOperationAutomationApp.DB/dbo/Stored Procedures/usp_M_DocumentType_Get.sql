CREATE PROCEDURE usp_M_DocumentType_Get
  (
	@Id INT = NULL
  )
  AS
  BEGIN

	SELECT 
		[DocumentTypeId]
		,[DocumentType]
	FROM [dbo].[M_DocumentType] DT WITH(NOLOCK) 
	WHERE @Id IS NULL OR DT.DocumentTypeId = @Id

  END