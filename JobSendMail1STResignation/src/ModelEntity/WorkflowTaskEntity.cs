using JobSendMail1STResignation.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.ModelEntity
{
    [TableAttribute(TableName = "WorkflowTasks")]
    public class WorkflowTaskEntity : BaseEntity
    {
        [Field(FieldName = "Id", Type = SqlDbType.UniqueIdentifier)]
        public Guid Id { get; set; }

        [Field(FieldName = "Title", Type = SqlDbType.NVarChar)]
        public string Title { get; set; }

        [Field(FieldName = "ItemId", Type = SqlDbType.UniqueIdentifier)]
        public Guid ItemId { get; set; }

        [Field(FieldName = "ItemType", Type = SqlDbType.NVarChar)]
        public string ItemType { get; set; }

        [Field(FieldName = "ReferenceNumber", Type = SqlDbType.NVarChar)]
        public string ReferenceNumber { get; set; }

        [Field(FieldName = "DueDate", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset DueDate { get; set; }

        [Field(FieldName = "AssignedToDepartmentId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? AssignedToDepartmentId { get; set; }

        [Field(FieldName = "AssignedToDepartmentGroup", Type = SqlDbType.Int)]
        public int AssignedToDepartmentGroup { get; set; }

        [Field(FieldName = "AssignedToId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? AssignedToId { get; set; }

        [Field(FieldName = "RequestedDepartmentId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? RequestedDepartmentId { get; set; }

        [Field(FieldName = "RequestedDepartmentCode", Type = SqlDbType.NVarChar)]
        public string RequestedDepartmentCode { get; set; }

        [Field(FieldName = "RequestedDepartmentName", Type = SqlDbType.NVarChar)]
        public string RequestedDepartmentName { get; set; }

        [Field(FieldName = "Status", Type = SqlDbType.NVarChar)]
        public string Status { get; set; }

        [Field(FieldName = "Vote", Type = SqlDbType.Int)]
        public int Vote { get; set; }

        [Field(FieldName = "RequestorId", Type = SqlDbType.UniqueIdentifier)]
        public Guid? RequestorId { get; set; }

        [Field(FieldName = "RequestorUserName", Type = SqlDbType.NVarChar)]
        public string RequestorUserName { get; set; }

        [Field(FieldName = "RequestorFullName", Type = SqlDbType.NVarChar)]
        public string RequestorFullName { get; set; }

        [Field(FieldName = "IsCompleted", Type = SqlDbType.Bit)]
        public bool IsCompleted { get; set; }

        [Field(FieldName = "IsTurnedOffSendNotification", Type = SqlDbType.Bit)]
        public bool IsTurnedOffSendNotification { get; set; }

        [Field(FieldName = "IsAttachmentFile", Type = SqlDbType.Bit)]
        public bool IsAttachmentFile { get; set; }

        [Field(FieldName = "WorkflowInstanceId", Type = SqlDbType.UniqueIdentifier)]
        public Guid WorkflowInstanceId { get; set; }

        [Field(FieldName = "Created", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Created { get; set; }

        [Field(FieldName = "CreatedBy", Type = SqlDbType.NVarChar)]
        public string CreatedBy { get; set; }

        [Field(FieldName = "Modified", Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset Modified { get; set; }

        [Field(FieldName = "ModifiedBy", Type = SqlDbType.NVarChar)]
        public string ModifiedBy { get; set; }

        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.Id = this.GetData<Guid>(sqlDataReader, "Id");
            this.Title = this.GetData<string>(sqlDataReader, "Title");
            this.ItemId = this.GetData<Guid>(sqlDataReader, "ItemId");
            this.ItemType = this.GetData<string>(sqlDataReader, "ItemType");
            this.ReferenceNumber = this.GetData<string>(sqlDataReader, "ReferenceNumber");
            this.DueDate = this.GetData<DateTimeOffset>(sqlDataReader, "DueDate");
            this.AssignedToDepartmentId = this.GetData<Guid?>(sqlDataReader, "AssignedToDepartmentId");
            this.AssignedToDepartmentGroup = this.GetData<int>(sqlDataReader, "AssignedToDepartmentGroup");
            this.AssignedToId = this.GetData<Guid?>(sqlDataReader, "AssignedToId");
            this.RequestedDepartmentId = this.GetData<Guid?>(sqlDataReader, "RequestedDepartmentId");
            this.RequestedDepartmentCode = this.GetData<string>(sqlDataReader, "RequestedDepartmentCode");
            this.RequestedDepartmentName = this.GetData<string>(sqlDataReader, "RequestedDepartmentName");
            this.Status = this.GetData<string>(sqlDataReader, "Status");
            this.Vote = this.GetData<int>(sqlDataReader, "Vote");
            this.RequestorId = this.GetData<Guid?>(sqlDataReader, "RequestorId");
            this.RequestorUserName = this.GetData<string>(sqlDataReader, "RequestorUserName");
            this.RequestorFullName = this.GetData<string>(sqlDataReader, "RequestorFullName");
            this.IsCompleted = this.GetData<bool>(sqlDataReader, "IsCompleted");
            this.IsTurnedOffSendNotification = this.GetData<bool>(sqlDataReader, "IsTurnedOffSendNotification");
            this.IsAttachmentFile = this.GetData<bool>(sqlDataReader, "IsAttachmentFile");
            this.WorkflowInstanceId = this.GetData<Guid>(sqlDataReader, "WorkflowInstanceId");
            this.Created = this.GetData<DateTimeOffset>(sqlDataReader, "Created");
            this.CreatedBy = this.GetData<string>(sqlDataReader, "CreatedBy");
            this.Modified = this.GetData<DateTimeOffset>(sqlDataReader, "Modified");
            this.ModifiedBy = this.GetData<string>(sqlDataReader, "ModifiedBy");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}