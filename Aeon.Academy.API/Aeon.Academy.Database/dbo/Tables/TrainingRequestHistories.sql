CREATE TABLE [dbo].[TrainingRequestHistories] (
    [Id]                UNIQUEIDENTIFIER   NOT NULL,
    [TrainingRequestId] UNIQUEIDENTIFIER   NOT NULL,
    [Created]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedById]       UNIQUEIDENTIFIER   NOT NULL,
    [CreatebBy]         NVARCHAR (255)     NOT NULL,
    [CreatedByFullName] NVARCHAR (255)     NOT NULL,
    [ReferenceNumber]   NVARCHAR (50)      NOT NULL,
    [Comment]           NVARCHAR (MAX)     NULL,
    [Action]            NVARCHAR (50)      NOT NULL,
    [StepNumber]        INT                NULL, 
    [AssignedToDepartmentName] NVARCHAR (50) NULL,
    [StartDate] DATETIMEOFFSET (7) NULL  ,
    [DueDate] DATETIMEOFFSET (7) NULL  ,
    [RoundNumber] INT NULL DEFAULT 1,

    CONSTRAINT [PK_dbo.TrainingRequestHistories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TrainingRequestHistories_TrainingRequests] FOREIGN KEY ([TrainingRequestId]) REFERENCES [dbo].[TrainingRequests] ([Id])
);

