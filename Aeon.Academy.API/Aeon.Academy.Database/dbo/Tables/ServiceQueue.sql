CREATE TABLE [dbo].[ServiceQueues] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [InstanceType] NVARCHAR (255) NOT NULL,
    [InstanceData] NTEXT NOT NULL,
	[ErrorMessage] NTEXT NOT NULL,
	[NumberOfCall]  INT DEFAULT(0),
    [Disabled]  INT DEFAULT(0),
    [Created] DATETIME NOT NULL DEFAULT(GETDATE()),
    [Modified] DATETIME NOT NULL DEFAULT(GETDATE()),    
    [ReferenceNumber] NVARCHAR(20) NULL, 
    [Status] NVARCHAR(20) NULL, 
    [Response] NTEXT NULL, 
    [SapCode] NVARCHAR(10) NULL, 
    [TrainingInvitationId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_EventQueue] PRIMARY KEY CLUSTERED ([Id] ASC)
);

