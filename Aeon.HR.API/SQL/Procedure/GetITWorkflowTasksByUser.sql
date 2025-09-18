USE [AEONHRPRODDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetITWorkflowTasksByUser]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy ra instance mới nhất theo ItemReferenceNumber
WITH LatestInstances AS (
    SELECT *
    FROM (
             SELECT
                 ins.*,
                 ROW_NUMBER() OVER (
                    PARTITION BY ins.ItemReferenceNumber 
                    ORDER BY ins.Modified DESC
                ) AS rn
             FROM ITWorkflowInstances ins
         ) AS sub
    WHERE rn = 1
)

SELECT a.*
FROM (
         -- Tasks đang xử lý (chưa completed)
         SELECT
             wfTa.Id,
             wfTa.Title,
             wfTa.ItemId,
             ISNULL((
                 SELECT pr.ShortUrl
                 FROM [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] pr
                 WHERE pr.Id = wfTa.ItemId
                 ), 
			case 
				when itt.ModuleCode = 'Facility' then 
					case 
						when wfTa.ReferenceNumber like 'AT-%' then '/Facility/#!/home/asset-transfermation/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						when wfTa.ReferenceNumber like 'AR-%' then '/Facility/#!/home/asset-registration/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						when wfTa.ReferenceNumber like 'AIM-%' then '/Facility/#!/home/asset-inspection-management/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						when wfTa.ReferenceNumber like 'AI-%' then '/Facility/#!/home/asset-inspection/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						when wfTa.ReferenceNumber like 'AH-%' then '/Facility/#!/home/asset-handover/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						when wfTa.ReferenceNumber like 'AMR-%' then '/Facility/#!/home/asset-maintenance-repair/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
						else null
						end
				else 
					null 
				end
			) AS Link,
             ISNULL((
                 SELECT 'Cancel Request'
                 FROM [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] pr
                 WHERE pr.Id = wfTa.ItemId
                 ), 
            CASE
				WHEN itt.ModuleCode = 'Facility' then 
				case
					when wfTa.ReferenceNumber like 'AT-%' then 'Asset Transfermation'
					when wfTa.ReferenceNumber like 'AR-%' then 'Asset Registration'
					when wfTa.ReferenceNumber like 'AIM-%' then 'Asset Inspection Management'
					when wfTa.ReferenceNumber like 'AI-%' then 'Asset Inspection'
					when wfTa.ReferenceNumber like 'AH-%' then 'Asset Handover'
					when wfTa.ReferenceNumber like 'AMR-%' then 'Asset Maintenance Repair'
				end
                when wfTa.ReferenceNumber LIKE 'DOC-%' THEN 'Document'
                ELSE ''
            END) AS ItemType,
             wfTa.ReferenceNumber,
             wfTa.DueDate,
             wfTa.Status,
             wfTa.Vote,
             itt.ModuleCode AS Module,
            wfTa.RequestedDepartmentId,
            wfTa.RequestedDepartmentCode,
            wfTa.RequestedDepartmentName,
            wfHis.ApproverId AS RequestorId,
            wfHis.Approver AS RequestorUserName,
            wfHis.ApproverFullName AS RequestorFullName,
            wfTa.IsCompleted,
            wfTa.ITWorkflowInstanceId,
            wfTa.Created,
            wfTa.Modified,
            wfTa.CreatedById,
            wfTa.CreatedBy,
            wfTa.CreatedByFullName,
            CAST(ISNULL(wfTa.IsParallelApprove, 0) AS BIT) AS IsParallelApprove,
            NULL AS ParallelStep,
            CAST(0 AS BIT) AS IsSignOff,
            CAST(
                CASE 
                    WHEN wfTa.ReferenceNumber LIKE 'F2-%' THEN 
                        ISNULL((
                            SELECT 
                                CASE WHEN pr.MultiBudget IS NOT NULL THEN 1 ELSE 0 END 
                            FROM [AEON.PRODUCT.Edocument].[dbo].[PurchaseRequests] pr 
                            WHERE pr.Id = wfTa.ItemId
                        ), 0)
                    ELSE 0
                END AS BIT
            ) AS IsMultibudget,
            CAST(0 AS BIT) AS IsManual,
            wfTa.AssignerId,
            wfTa.AssignerLoginName,
            NULL AS DocumentSetPurpose
         FROM ITWorkflowTasks wfTa
             LEFT JOIN ITWorkflowInstances ins ON wfTa.ITWorkflowInstanceId = ins.Id
             LEFT JOIN ITWorkflowHistories wfHis ON ins.Id = wfHis.ITInstanceId AND wfHis.StepNumber = 1
             LEFT JOIN ITWorkflowTemplates itt ON itt.Id = ins.ITTemplateId
             LEFT JOIN [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] eDoc1CR ON eDoc1CR.ItemId = wfTa.ItemId
         WHERE
             wfTa.IsCompleted = 0
           AND wfTa.AssignedToDepartmentId IS NOT NULL
           AND wfTa.AssignedToDepartmentGroup IS NOT NULL
           AND EXISTS (
             SELECT 1
             FROM UserDepartmentMappings udm
             INNER JOIN Users us ON udm.UserId = us.Id AND us.Id = @UserId
             INNER JOIN ITUserDepartmentMappings itUdm ON udm.Id = itUdm.Id
             INNER JOIN Departments de ON udm.DepartmentId = de.Id AND de.IsDeleted = 0
             LEFT JOIN ITModuleDepartmentGroupMappings itmdgm ON itmdgm.ITUserDepartmentMappingId = itUdm.Id
             LEFT JOIN ModuleDepartmentGroups mdg ON itmdgm.ModuleDepartmentGroupId = mdg.Id
           AND mdg.GroupCode = wfTa.AssignedToDepartmentGroup
             WHERE
             us.Id = wfTa.AssignedToId
            OR (
             udm.DepartmentId = wfTa.AssignedToDepartmentId
           AND (
             udm.Role = wfTa.AssignedToDepartmentGroup
            OR itUdm.GroupEdoc1 = wfTa.AssignedToDepartmentGroup
            OR mdg.GroupCode IS NOT NULL
             )
             )
             )

         UNION ALL

         -- Tasks bị yêu cầu chỉnh sửa nhưng đã bị terminated
         SELECT
             wfTa.Id,
             wfTa.Title,
             wfTa.ItemId,

             ISNULL((
             SELECT pr.ShortUrl
             FROM [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] pr
             WHERE pr.Id = wfTa.ItemId
             ),
             case
             when itt.ModuleCode = 'Facility' then
             case
             when wfTa.ReferenceNumber like 'AT-%' then '/Facility/#!/home/asset-transfermation/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             when wfTa.ReferenceNumber like 'AR-%' then '/Facility/#!/home/asset-registration/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             when wfTa.ReferenceNumber like 'AIM-%' then '/Facility/#!/home/asset-inspection-management/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             when wfTa.ReferenceNumber like 'AI-%' then '/Facility/#!/home/asset-inspection/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             when wfTa.ReferenceNumber like 'AH-%' then '/Facility/#!/home/asset-handover/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             when wfTa.ReferenceNumber like 'AMR-%' then '/Facility/#!/home/asset-maintenance-repair/item/' + wfTa.ReferenceNumber + '?id=' + CAST(wfTa.ItemId AS VARCHAR(36))
             else null
             end
             else
             null
             end
             ) AS Link,
             ISNULL((
             SELECT 'Cancel Request'
             FROM [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] pr
             WHERE pr.Id = wfTa.ItemId
             ),
             CASE
             WHEN itt.ModuleCode = 'Facility' then
             case
             when wfTa.ReferenceNumber like 'AT-%' then 'Asset Transfermation'
             when wfTa.ReferenceNumber like 'AR-%' then 'Asset Registration'
             when wfTa.ReferenceNumber like 'AIM-%' then 'Asset Inspection Management'
             when wfTa.ReferenceNumber like 'AI-%' then 'Asset Inspection'
             when wfTa.ReferenceNumber like 'AH-%' then 'Asset Handover'
             when wfTa.ReferenceNumber like 'AMR-%' then 'Asset Maintenance Repair'
             end
             when wfTa.ReferenceNumber LIKE 'DOC-%' THEN 'Document'
             ELSE ''
             END) AS ItemType,
             wfTa.ReferenceNumber,
             wfTa.DueDate,
             ins.ItemStatus,
             0 AS Vote,
             itt.ModuleCode AS Module,
             wfTa.RequestedDepartmentId,
             wfTa.RequestedDepartmentCode,
             wfTa.RequestedDepartmentName,
             itHis.ApproverId AS RequestorId,
             itHis.Approver AS RequestorUserName,
             itHis.ApproverFullName AS RequestorFullName,
             wfTa.IsCompleted,
             wfTa.ITWorkflowInstanceId,
             wfTa.Created,
             wfTa.Modified,
             wfTa.CreatedById,
             wfTa.CreatedBy,
             wfTa.CreatedByFullName,
             CAST(ISNULL(wfTa.IsParallelApprove, 0) AS BIT) AS IsParallelApprove,
             NULL AS ParallelStep,
             CAST(0 AS BIT) AS IsSignOff,
             CAST(
             CASE
             WHEN wfTa.ReferenceNumber LIKE 'F2-%' THEN
             ISNULL((
             SELECT CASE
             WHEN f2.MultiBudget IS NOT NULL THEN 1
             ELSE 0
             END
             FROM [AEON.PRODUCT.Edocument].[dbo].[PurchaseRequests] f2
             WHERE wfTa.ItemId = f2.Id
             ), 0)
             ELSE 0
             END AS BIT
             ) AS IsMultibudget,
             CAST(0 AS BIT) AS IsManual,
             wfTa.AssignerId,
             wfTa.AssignerLoginName,
             NULL AS DocumentSetPurpose
         FROM LatestInstances ins
             LEFT JOIN ITWorkflowHistories itHis ON itHis.ITInstanceId = ins.Id AND itHis.StepNumber = 1
             LEFT JOIN ITWorkflowTasks wfTa ON wfTa.ITWorkflowInstanceId = ins.Id
             AND wfTa.Status IN ('Waiting for Submit', 'Waiting for Draft', 'Waiting for Submited')
             LEFT JOIN ITWorkflowTemplates itt ON itt.Id = ins.ITTemplateId
             LEFT JOIN [AEON.PRODUCT.Edocument].[dbo].[CancelRequests] eDoc1CR ON eDoc1CR.ItemId = wfTa.ItemId
         WHERE
             ins.IsCompleted = 1
           AND ins.IsTerminated = 1
           AND NOT EXISTS (
             SELECT 1
             FROM LatestInstances insA
             WHERE
             ins.ItemReferenceNumber = insA.ItemReferenceNumber
           AND (
             (insA.IsCompleted = 1 AND insA.IsTerminated = 0)
            OR (insA.IsCompleted = 0 AND insA.IsTerminated = 0)
             )
             )
           AND ins.ItemStatus = 'Requested To Change'
           AND EXISTS (
             SELECT 1
             FROM ITWorkflowHistories itHis2
             WHERE
             ins.Id = itHis2.ITInstanceId
           AND itHis2.StepNumber = 1
           AND itHis2.ApproverId = @UserId
             )
     ) a
ORDER BY a.Modified DESC;
END;
