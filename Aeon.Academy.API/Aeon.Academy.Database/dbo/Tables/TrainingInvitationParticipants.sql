CREATE TABLE [dbo].TrainingInvitationParticipants (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [TrainingInvitationId] UNIQUEIDENTIFIER NOT NULL,
    [ParticipantId] UNIQUEIDENTIFIER NOT NULL,
    [SapCode] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Email]  NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(50) NOT NULL,
    [Position] NVARCHAR(255) NOT NULL,
    [Department] NVARCHAR(255),
	EmailContent NTEXT NOT NULL,
	[Response] NVARCHAR(50) NULL,
	ReasonOfDecline NTEXT,
	StatusOfReport NVARCHAR(50) NULL,
    [DeptLine] NVARCHAR(255),
    [SapStatusCode] int,
    CONSTRAINT [PK_TrainingInvitationParticipants] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TrainingInvitationParticipants_TrainingInvitations] FOREIGN KEY ([TrainingInvitationId]) REFERENCES [dbo].[TrainingInvitations] ([Id])
)

