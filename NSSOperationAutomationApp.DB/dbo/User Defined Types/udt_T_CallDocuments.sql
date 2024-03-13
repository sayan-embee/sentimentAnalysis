CREATE TYPE [dbo].[udt_T_CallDocuments] AS TABLE (
    [DocumentId]      BIGINT         NULL,
    [CallDetailId]    BIGINT         NULL,
    [DocumentTypeId]  INT            NULL,
    [DocumentName]    VARCHAR (100)  NULL,
    [MimeType]        VARCHAR (100)   NULL,
    [DocumentUrlPath] NVARCHAR (MAX) NULL,
    [IsActive]        BIT            NULL,
    [InternalName]    VARCHAR (50)   NULL
);
