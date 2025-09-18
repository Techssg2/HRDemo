CREATE TABLE [dbo].[TrainingRequestParticipants] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [TrainingRequestId] UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId] UNIQUEIDENTIFIER,
    [SapCode] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Email]  NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(50) NOT NULL,
    [Position] NVARCHAR(255) NOT NULL,
    [Department] NVARCHAR(255),
    CONSTRAINT [PK_dbo.TrainingRequestParticipants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TrainingRequestParticipants_TrainingRequests] FOREIGN KEY ([TrainingRequestId]) REFERENCES [dbo].[TrainingRequests] ([Id])
);

