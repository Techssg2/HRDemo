CREATE TABLE [dbo].[TrainingRequestCostCenters] (
    [Id]                 UNIQUEIDENTIFIER   NOT NULL,
    [TrainingRequestId]  UNIQUEIDENTIFIER     NOT NULL,
    [BudgetCode]         NVARCHAR(20) NULL,
    [CostCenterCode]     NVARCHAR(50)   NULL,
    [Amount]         DECIMAL(13)                NULL ,
    [VATPercentage]         DECIMAL(3, 1)                NULL ,
    [VAT]         DECIMAL(13)                NULL ,
    [Currency] NVARCHAR(255) NULL, 
    [Type]               NVARCHAR (20)     NULL,
    [BudgetBalanced] DECIMAL(13)  NULL, 
    [BudgetPlan]         NVARCHAR(50)   NULL, 
    [TotalBudget] DECIMAL(13)  NULL, 

    CONSTRAINT [PK_dbo.TrainingRequestCostCenters] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TrainingRequestCostCenters_TrainingRequests] FOREIGN KEY ([TrainingRequestId]) REFERENCES [dbo].[TrainingRequests] ([Id])
);

