CREATE TABLE [dbo].[TrainingActionPlans] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[TrainingReportId] UNIQUEIDENTIFIER NOT NULL,
	[ActionPlanCode] NTEXT NOT NULL,
	[Quarter1] BIT,
	[Quarter2] BIT,
	[Quarter3] BIT,
	[Quarter4] BIT,
	CONSTRAINT [PK_dbo.TrainingActionPlans] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_TrainingActionPlans_TrainingReports] FOREIGN KEY ([TrainingReportId]) REFERENCES [dbo].[TrainingReports] ([Id])
);