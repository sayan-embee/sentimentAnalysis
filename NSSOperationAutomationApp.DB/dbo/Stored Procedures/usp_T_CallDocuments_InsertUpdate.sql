CREATE PROCEDURE [dbo].[usp_T_CallDocuments_InsertUpdate]
(
	@udt_CallDocuments dbo.[udt_T_CallDocuments] READONLY
)
AS
BEGIN
	
	DECLARE @SuccessMsg VARCHAR(100);
	DECLARE @ErrorMsg VARCHAR(100);
	DECLARE @CallDetailId BIGINT = 0;

	BEGIN TRY
	-----------------

	BEGIN TRANSACTION
	-----------------

		IF EXISTS (SELECT * FROM @udt_CallDocuments CD)
		BEGIN

			SET @CallDetailId = (SELECT TOP 1 CD.CallDetailId FROM @udt_CallDocuments CD)

			INSERT INTO dbo.[T_CallDocuments]
			(
				CallDetailId,
				DocumentTypeId,
				DocumentName,
				MimeType,
				DocumentUrlPath,
				IsActive,
				InternalName,
				CreatedOnUCT
			)
			SELECT
				CD.CallDetailId,
				CD.DocumentTypeId,
				CD.DocumentName,
				CD.MimeType,
				CD.DocumentUrlPath,
				1,
				CD.InternalName,
				GETUTCDATE()
			FROM @udt_CallDocuments CD
			WHERE CD.DocumentId = 0 AND ISNULL(CD.DocumentUrlPath, '') != '';
			
			UPDATE tbl_CD
			SET tbl_CD.IsActive = 0,
			tbl_CD.UpdatedOnUTC = GETUTCDATE()
			FROM dbo.[T_CallDocuments] tbl_CD, @udt_CallDocuments udt_CD
			WHERE udt_CD.DocumentId > 0
			AND udt_CD.IsActive = 0
			AND udt_CD.CallDetailId = tbl_CD.CallDetailId
			AND udt_CD.DocumentId = tbl_CD.DocumentId

		END
		
		SET @SuccessMsg = 'DB execution successful - CallDocuments updated';

	END TRY
	-----------------
	BEGIN CATCH
	-----------------
	ROLLBACK TRANSACTION
	SET @ErrorMsg = 'DB execution failed - Update CallDocuments'
    SELECT
        @ErrorMsg												AS [Message],
        ERROR_MESSAGE()		                                    AS ErrorMessage,
        0					                                    AS [Status],
        ''				                                        AS Id
        RETURN
	END CATCH
	-----------------
	

	COMMIT TRANSACTION
	-----------------
	SELECT
		@SuccessMsg												AS [Message],
		''						                                AS ErrorMessage,
		1					                                    AS [Status],
		@CallDetailId											AS Id
	RETURN
END
