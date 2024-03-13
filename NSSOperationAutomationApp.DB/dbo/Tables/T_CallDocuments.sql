CREATE TABLE [dbo].[T_CallDocuments] (
    [DocumentId]      BIGINT         IDENTITY (1, 1) NOT NULL,
    [CallDetailId]    BIGINT         NULL,
    [DocumentTypeId]  INT            NULL,
    [DocumentName]    VARCHAR (100)  NULL,
    [MimeType]        VARCHAR (100)   NULL,
    [DocumentUrlPath] NVARCHAR (MAX) NULL,
    [IsActive]        BIT            NULL,
    [InternalName]    VARCHAR (100)  NULL,
    [CreatedOnUCT]    DATETIME       NULL,
    [UpdatedOnUTC]    DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([DocumentId] ASC)
);

