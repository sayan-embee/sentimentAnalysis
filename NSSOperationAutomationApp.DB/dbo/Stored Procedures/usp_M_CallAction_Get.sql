CREATE PROCEDURE [dbo].[usp_M_CallAction_Get]
  (
	@Id INT = NULL
  )
  AS
  BEGIN

	SELECT
		[CallActionId]
		,[CallAction]
	FROM dbo.[M_CallAction] CA WITH(NOLOCK) 
	WHERE @Id IS NULL OR CA.CallActionId = @Id

  END