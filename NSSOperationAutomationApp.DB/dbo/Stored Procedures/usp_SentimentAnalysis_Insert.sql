CREATE PROCEDURE [dbo].[usp_SentimentAnalysis_Insert]
(
	@SummaryText  nvarchar(MAX) = NULL,
    @Sentiment nvarchar(50) = null,
    @Reason nvarchar(MAX) = null,
    @TranscribeText nvarchar(MAX) = NULL,
    @RefId nvarchar(50) = null,
    @FileName nvarchar(250) = null,
    @FileInternalName nvarchar(300) = null,
    @FileUrl nvarchar(500) = null,
    @ContentType nvarchar(50) = null    
)

AS
BEGIN

	BEGIN TRANSACTION

    INSERT INTO [dbo].[T_SentimentDetails] 
    (
        
    [SummaryText] , 
    [Sentiment] , 
    [Reason] , 
    [TranscribeText] , 
    [FileRefId] , 
    [FileName] , 
    [FileInternalName] , 
    [FileUrl], 
    [ContentType],
    IsActive,
    CreatedOnIST
    )
    VALUES 
    (       
	@SummaryText,
    @Sentiment,
    @Reason,
    @TranscribeText,
    @RefId,
    @FileName,
    @FileInternalName,
    @FileUrl,
    @ContentType,
    1,
    DATEADD(MINUTE, 330, GETUTCDATE())
    )

    IF @@ERROR<>0
    BEGIN
	    ROLLBACK TRANSACTION
	    SELECT 
		    ''                      AS [SuccessMessage],
		    'Create failed'		    AS ErrorMessage,
		    0						AS [Status],
		    ''						AS ReferenceNo
	    RETURN
    END

    COMMIT TRANSACTION
    SELECT 
    'Create executed'          AS [SuccessMessage],
    ''						   AS ErrorMessage,
    1					       AS [Status],
    @RefId				       AS ReferenceNo

END
