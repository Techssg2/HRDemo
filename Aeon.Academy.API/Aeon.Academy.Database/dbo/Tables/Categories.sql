CREATE TABLE [dbo].[Categories] (
    [Id]                 UNIQUEIDENTIFIER   NOT NULL,
    [Name]               NVARCHAR (255)     NOT NULL,
    [ParentId]           UNIQUEIDENTIFIER   NULL,
    [IsActivated]        BIT                NOT NULL DEFAULT 1,
    [Created]            DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]          NVARCHAR (255)     NULL,
    [CreatedByFullName]  NVARCHAR (255)     NULL,
    [Modified]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedById]        UNIQUEIDENTIFIER   NULL,
    [ModifiedById]       UNIQUEIDENTIFIER   NOT NULL,
    [ModifiedBy]         NVARCHAR (255)     NULL,
    [ModifiedByFullName] NVARCHAR (255)     NULL,
    CONSTRAINT [PK_dbo.Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);

