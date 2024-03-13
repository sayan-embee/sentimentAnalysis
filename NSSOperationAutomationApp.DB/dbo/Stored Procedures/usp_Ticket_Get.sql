CREATE PROCEDURE [dbo].[usp_Ticket_Get]
(
	@CaseNumber VARCHAR(50) = NULL,
	@SerialNumber NVARCHAR(50) = NULL,
	@PartNumber NVARCHAR(100) = NULL,
    @FromDate VARCHAR(10) = NULL,
    @ToDate VARCHAR(10) = NULL
)
AS
BEGIN

	SELECT
        CaseNumber,
        FromEmail,
        ToEmail,
        CaseSubject,
        WorkOrderNumber,
        ServiceAccount,
        ContactName,
        ContactEmail,
        ContactPhone,
        ServiceDeliveryStreetAddress,
        ServiceDeliveryCity,
        ServiceDeliveryCountry,
        PostalCode,
        SerialNumber,
        ProductName,
        ProductNumber,
        OTCCode,
        PartNumber,
        PartDescription,
        EmailSubject,
        FORMAT(SWITCHOFFSET(EmailDate, '+05:30'),'yyyy-MM-dd HH:mm:ss') as EmailDate,
        FORMAT(SWITCHOFFSET(CreatedOn, '+05:30'),'yyyy-MM-dd HH:mm:ss') as CreatedOn,
        ISNULL(PartNumber2,'') AS PartNumber2,
        ISNULL(PartDescription2,'')  AS PartDescription2,
        ISNULL(PartNumber3,'') AS PartNumber3,
        ISNULL(PartDescription3,'') AS PartDescription3
    FROM [dbo].[T_TicketDetails] WITH(NOLOCK)
    WHERE
        (ISNULL(@CaseNumber,'')='' OR CaseNumber LIKE @CaseNumber + '%')
        AND (ISNULL(@SerialNumber,'') = '' OR SerialNumber LIKE @SerialNumber + '%')
        AND (ISNULL(@PartNumber,'')= '' OR PartNumber LIKE @PartNumber + '%')
        AND (ISNULL(@FromDate,'')= '' OR CONVERT(DATE, SWITCHOFFSET(EmailDate, '+05:30')) >= CONVERT(DATE, @FromDate,103))
        AND (ISNULL(@ToDate ,'')= '' OR  CONVERT(DATE, SWITCHOFFSET(EmailDate, '+05:30')) <= CONVERT(DATE, @ToDate,103))
	ORDER BY EmailDate DESC
END