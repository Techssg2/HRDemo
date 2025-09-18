CREATE TABLE [dbo].[ReasonOfTrainingRequests] (
    [Id]                 UNIQUEIDENTIFIER   NOT NULL,
    [Value]              NTEXT              NOT NULL,
    [CreatedDate]        DATETIME           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_dbo.ReasonOfTrainingRequests] PRIMARY KEY CLUSTERED ([Id] ASC)
);

