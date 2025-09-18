CREATE TABLE [dbo].[TrainingSurveyQuestions] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[TrainingReportId] UNIQUEIDENTIFIER NOT NULL,
	[SurveyQuestion] NVARCHAR (50) NOT NULL,
	[ParentQuestion] NVARCHAR (50) NULL,
	[Value] NVARCHAR (255) NOT NULL,
	CONSTRAINT [PK_dbo.TrainingSurveyQuestions] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_TrainingSurveyQuestions_TrainingReports] FOREIGN KEY ([TrainingReportId]) REFERENCES [dbo].[TrainingReports] ([Id])
);