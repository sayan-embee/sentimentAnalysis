CREATE PROCEDURE [dbo].[usp_Ticket_Insert]
(
	@CaseNumber VARCHAR(50) = NULL,
    @FromEmail VARCHAR(100) = NULL,
    @ToEmail VARCHAR(100) = NULL,
    @CaseSubject NVARCHAR(500) = NULL,
    @WorkOrderNumber VARCHAR(50) = NULL,
    @ServiceAccount VARCHAR(100) = NULL,
    @ContactName VARCHAR(100) = NULL,
    @ContactEmail VARCHAR(100) = NULL,
    @ContactPhone VARCHAR(100) = NULL,
    @ServiceDeliveryStreetAddress NVARCHAR(200) = NULL,
    @ServiceDeliveryCity NVARCHAR(50) = NULL,
    @ServiceDeliveryCountry NVARCHAR(50) = NULL,
    @PostalCode VARCHAR(50) = NULL,
    @SerialNumber NVARCHAR(50) = NULL,
    @ProductName NVARCHAR(100) = NULL,
    @ProductNumber VARCHAR(50) = NULL,
    @OTCCode NVARCHAR(100) = NULL,
    @PartNumber NVARCHAR(100) = NULL,
    @PartDescription NVARCHAR(1000) = NULL,
    @EmailSubject NVARCHAR(500) = NULL,
    @EmailDate VARCHAR(50) = NULL,
	@PartNumber2 NVARCHAR(100) = NULL,
	@PartDescription2 NVARCHAR(1000) = NULL,
	@PartNumber3 NVARCHAR(100) = NULL,
	@PartDescription3 NVARCHAR(1000) = NULL
)

AS
BEGIN

	BEGIN TRANSACTION

    INSERT INTO [dbo].[T_TicketDetails] 
    (
        [CaseNumber],
        [FromEmail],
        [ToEmail],
        [CaseSubject],
        [WorkOrderNumber],
        [ServiceAccount],
        [ContactName],
        [ContactEmail],
        [ContactPhone],
        [ServiceDeliveryStreetAddress],
        [ServiceDeliveryCity],
        [ServiceDeliveryCountry],
        [PostalCode],
        [SerialNumber],
        [ProductName],
        [ProductNumber],
        [OTCCode],
        [PartNumber],
        [PartDescription],
        [EmailSubject],
        [EmailDate],
        [CreatedOn],
		[PartNumber2],
        [PartDescription2],
		[PartNumber3],
        [PartDescription3]
    )
    VALUES 
    (
        @CaseNumber,
        @FromEmail,
        @ToEmail,
        @CaseSubject,
        @WorkOrderNumber,
        @ServiceAccount,
        @ContactName,
        @ContactEmail,
        @ContactPhone,
        @ServiceDeliveryStreetAddress,
        @ServiceDeliveryCity,
        @ServiceDeliveryCountry,
        @PostalCode,
        @SerialNumber,
        @ProductName,
        @ProductNumber,
        @OTCCode,
        @PartNumber,
        @PartDescription,
        @EmailSubject,
        @EmailDate,
        GETUTCDATE(),
		@PartNumber2,
		@PartDescription2,
		@PartNumber3,
		@PartDescription3
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
    @CaseNumber				   AS ReferenceNo

END