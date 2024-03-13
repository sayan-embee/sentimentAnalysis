CREATE TABLE [dbo].[M_PartConsumptionType] (
    [PartConsumptionTypeId] INT          IDENTITY (1, 1) NOT NULL,
    [PartConsumptionType]   VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([PartConsumptionTypeId] ASC)
);

