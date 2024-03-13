CREATE PROCEDURE usp_M_PartConsumptionType_Get
  (
	@Id INT = NULL
  )
  AS
  BEGIN

	SELECT 
		[PartConsumptionTypeId]
		,[PartConsumptionType]
	FROM [dbo].[M_PartConsumptionType] PCT WITH(NOLOCK) 
	WHERE @Id IS NULL OR PCT.PartConsumptionTypeId = @Id

  END