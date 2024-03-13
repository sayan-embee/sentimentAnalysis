CREATE PROCEDURE usp_M_CallStatus_Get
  (
	@Id INT = NULL
  )
  AS
  BEGIN

	SELECT 
		[CallStatusId]
		,[CallStatus]
	FROM [dbo].[M_CallStatus] CS WITH(NOLOCK) 
	WHERE @Id IS NULL OR CS.CallStatusId = @Id

  END