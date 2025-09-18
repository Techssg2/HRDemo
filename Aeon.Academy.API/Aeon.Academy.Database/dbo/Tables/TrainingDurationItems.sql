CREATE TABLE [dbo].TrainingDurationItems (
    [Id] UNIQUEIDENTIFIER NOT NULL,
	[TrainingRequestId] UNIQUEIDENTIFIER NOT NULL,
    TrainingMethod NVARCHAR(50) NOT NULL,
    [Duration] INT,
    [From] DATETIME,
    [To]  DATETIME,
    TrainingLocation NVARCHAR(225) NOT NULL,
    CONSTRAINT PK_TrainingDurationItems PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT FK_TrainingDurationItems_TrainingRequests FOREIGN KEY (TrainingRequestId) REFERENCES [dbo].[TrainingRequests] ([Id])
);

