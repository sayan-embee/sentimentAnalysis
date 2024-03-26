CREATE PROCEDURE [dbo].[usp_SentimentAnalysis_Get]
(
	@Id INT = NULL
  )
  AS
  BEGIN

	SELECT
		[AutoId]
      ,[SummaryText]
      ,[Sentiment]
      ,[Reason]
      ,[TranscribeText]
      ,[FileRefId]
      ,[FileName]
      ,[FileInternalName]
      ,[FileUrl]
      ,[ContentType]
      ,[IsActive]
      ,[CreatedOnIST]
	FROM dbo.[T_SentimentDetails]  WITH(NOLOCK) 
	WHERE @Id IS NULL OR [AutoId] = @Id

  END