namespace Aeon.HR.Data.Migrations
{
    using Aeon.HR.Data.Models;
    using Aeon.HR.Infrastructure.Constants;
    using Aeon.HR.Infrastructure.Enums;
    using Aeon.HR.Infrastructure.Utilities;
    using Aeon.HR.Infrastructure.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using Aeon.HR.Infrastructure.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<Aeon.HR.Data.HRDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Aeon.HR.Data.HRDbContext context)
        {

            //if (!context.ApplicantStatusRecruitments.Any())
            //{
            //    context.ApplicantStatusRecruitments.Add(new ApplicantStatusRecruitment
            //    {
            //        Id = Guid.NewGuid(),
            //        Code = "Initial",
            //        Arrangement = 1,
            //        Name = "Initial",
            //    });
            //}
            #region Jobgrade
            var jobGrades = new List<JobGrade>
            {
                new JobGrade
                {
                    Caption = "G1",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("ec1b3149-16be-4bfb-a150-710b72ffd3b5"),
                    Title = "G1",
                    ExpiredDayPosition = 11,
                    Grade = 1,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"

                },
                new JobGrade
                {
                    Caption = "G2",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("f8a9694f-d369-43f5-8625-9c38fa36abe4"),
                    Title = "Officer",
                    ExpiredDayPosition = 11,
                    Grade = 2,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G3",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("53e9bc34-3f92-45b3-b1ee-606e7ee6a77c"),
                    Title = "Staff",
                    ExpiredDayPosition = 24,
                    Grade = 3,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G4",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("5e7b0464-d2b2-446a-81d0-02281de68431"),
                    Title = "Staff 4",
                    ExpiredDayPosition = 22,
                    Grade = 4,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G5",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("a67cf0b7-44bd-437c-9b3e-5c95624bdbd4"),
                    Title = "Staff 5",
                    ExpiredDayPosition = 33,
                    Grade = 5,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G6",
                    Created = DateTime.Now,
                    Id = new Guid("333b2071-d76c-4fac-9d6d-d64e2f2217b5"),
                    Title = "Staff 6",
                    ExpiredDayPosition = 18,
                    Grade = 6,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G7",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("7958b0c4-9bd1-4aa4-8aa6-6f201e0a65f2"),
                    Title = "G7",
                    ExpiredDayPosition = 18,
                    Grade = 7,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G8",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("a2063572-81c8-40c4-9842-d6a86a5957d7"),
                    Title = "G8",
                    ExpiredDayPosition = 18,
                    Grade = 8,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                },
                new JobGrade
                {
                    Caption = "G9",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Id = new Guid("670761a9-92f5-4e63-945f-4176aa324923"),
                    Title = "G9",
                    ExpiredDayPosition = 18,
                    Grade = 9,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                    CreatedBy = "Edoc System",
                    ModifiedBy = "Edoc System"
                }
            };
            if (!context.JobGrades.Any())
            {

                context.JobGrades.AddRange(jobGrades);
            }
            #endregion
            #region REFERENCE NUMBER
            var assem = Assembly.GetExecutingAssembly();
            var autoNamingTypes = assem.GetTypes().Where(x => typeof(IAutoNumber).IsAssignableFrom(x));
            foreach (var autoNamingType in autoNamingTypes)
            {
                if (!context.ReferenceNumbers.Any(x => x.ModuleType == autoNamingType.Name))
                {
                    context.ReferenceNumbers.Add(new ReferenceNumber
                    {
                        Id = Guid.NewGuid(),
                        ModuleType = autoNamingType.Name,
                        CurrentNumber = 0,
                        IsNewYearReset = true,
                        Created = DateTime.Now,
                        CreatedBy = "System",
                        Modified = DateTime.Now,
                        ModifiedBy = "System",
                        Formula = autoNamingType.Name.Substring(0, 3).ToUpper() + @"-{AutoNumberLength:9}-{Year}"
                    });
                }
            }
            #endregion

            #region Workflow
            if (!context.WorkflowTemplates.Any())
            {
                var ass = Assembly.GetExecutingAssembly();
                var workflowTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));

                foreach (var workflowType in workflowTypes)
                {
                    #region Workflow
                    if (workflowType.Name == typeof(LeaveApplication).Name)
                    {
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                      new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G6",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },    new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G3",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for Store (G1,G2)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },    new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                 new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for Store (G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },    new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management (G4,G5,G6,G7,G8)"
                        });

                        //Long Term Leave
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                   new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                      new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G6",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G7",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for HQ Long Term (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G3",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G3",
                                     MaxJobGrade="G6",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for Store Long Term (G1,G2)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                  new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Leave Management for Store Long Term (G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="Is2ndApproval",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Leave Management Long Term (G4,G5,G6,G7,G8)"
                        });

                        //For assistant, order = 0 to increase priority
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management For Assistance"
                        });


                        //Cancel Workflow
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OnCancelled = "Completed",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Completed"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Cancel",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Completed",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Cancelled",
                                      DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Leave Management Revoke"
                        });
                    }
                    else if (workflowType.Name == typeof(MissingTimeClock).Name)
                    {
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Missing Timelock for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     JobGrade="G1",
                                     MaxJobGrade="G2",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G2",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G6",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Missing Timelock for Store (G1,G2)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Missing Timlock for Store (G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Missing Timelock (G4,G5,G6,G7,G8)"
                        });

                        //For assistant, order = 0 to increase priority
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Missing Timelock For Assistance"
                        });
                        //Cancel Workflow
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OnCancelled = "Completed",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Completed"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Cancel",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Completed",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Cancelled",
                                      DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Missing Timelock Revoke"
                        });
                    }
                    else if (workflowType.Name == typeof(ResignationApplication).Name)
                    {
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Review",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     TraversingFromRoot =true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4,
                                     RestrictedProperties= new List<RestrictedProperty>(){
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "suggestionForLastWorkingDay", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "isNotifiedLastWorkingDate", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "reasonDescription", IsRequired =false }

                                     }
                                   }
                                }
                            },
                            WorkflowName = "Resignation (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Review",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     TraversingFromRoot =true,
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G8",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4,
                                     RestrictedProperties= new List<RestrictedProperty>(){
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "suggestionForLastWorkingDay", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "isNotifiedLastWorkingDate", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "reasonDescription", IsRequired =false }

                                     }
                                   }
                                }
                            },
                            WorkflowName = "Resignation (G4,G5,G6,G7,G8)"
                        });

                        //For assistant, order = 0 to increase priority
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3,
                                     RestrictedProperties= new List<RestrictedProperty>(){
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "suggestionForLastWorkingDay", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "isNotifiedLastWorkingDate", IsRequired =false },
                                         new RestrictedProperty() {Name= " Suggestion for last working day", FieldPattern= "reasonDescription", IsRequired =false }

                                     }
                                   }
                                }
                            },
                            WorkflowName = "Resignation for Assistance"
                        });

                        //Cancel Workflow
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Completed"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Cancel",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Completed",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Cancelled",
                                      DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Resignation Revoke"
                        });

                    }
                    else if (workflowType.Name == typeof(ShiftExchangeApplication).Name)
                    {
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "Shift Exchange for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Shift Exchange for Store (G1,G2,G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Shift Exchange (G4,G5,G6,G7,G8)"
                        });

                        //For assistant, order = 0 to increase priority
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Shift Exchange For Assistance"
                        });

                        //Cancel Workflow
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OnCancelled = "Completed",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Completed"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Cancel",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Completed",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Cancelled",
                                      DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "Shift Exchange Revoke"
                        });
                    }
                    else if (workflowType.Name == typeof(OvertimeApplication).Name)
                    {
                        //Approve Plan
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "PLApproved",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "OT Plan for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },

                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "PLApproved",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "OT Plan for HQ (G4 and up)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "PLApproved",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "OT Plan for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "PLApproved",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=false,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "OT Plan for Store (G4 and up)"
                        });

                        //Approve Actual With Change
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Actual Hour Filled"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "OT Actual Approval for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                        "Actual Hour Filled"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 4,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 4,
                                     StepNumber = 3
                                   },

                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "OT Actual Approval for HQ (G4 and up)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                     "Actual Hour Filled"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G6",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "OT Actual Approval for HQ (G1,G2, G3)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G4","G5","G6","G7","G8"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                    "Actual Hour Filled"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 4,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 4,
                                     StepNumber = 3
                                   },

                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=false,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "OT for Store (G4 and up)"
                        });

                        //For assistant, order = 0 to increase priority
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                     new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "PLApproved",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "OT Plan for Assistance"
                        });

                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                   new WorkflowCondition()
                                    {

                                        FieldName="IsAssistant",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Actual Hour Filled"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                     new WorkflowStep(){
                                     StepName="Fill Actual Hour",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     SuccessVote = "Fill Actual Hour",
                                     OnSuccess = "Actual Hour Filled",
                                     RestrictedProperties=new List<RestrictedProperty>(){ new RestrictedProperty() { Name = "From Actual", FieldPattern= "fromActual",IsRequired=true },new RestrictedProperty() { Name = "To Actual", FieldPattern= "toActual",IsRequired=true } },
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="C & B Review",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                      IsHRHQ=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   }
                                }
                            },
                            WorkflowName = "OT Actual Approval for Assistance"
                        });
                        //Cancel Workflow
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OnCancelled = "Completed",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Completed"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="Cancel",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.CBDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Completed",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Cancelled",
                                      DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   }
                                }
                            },
                            WorkflowName = "OT Revoke"
                        });
                    }
                    else if (workflowType.Name == typeof(RequestToHire).Name)
                    {
                        #region RTH
                        //HQ
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm = Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="1st Approval",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement & Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     //OnFailure="Rejected",
                                     //FailureVote = "Reject",
                                     SuccessVote = "Acknowledge and Assign",
                                     OnSuccess = "Completed",
                                       JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } }
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G1 Budget"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="ReplacementFor",
                                        FieldValues= new List<string>(){
                                            "NewPosition"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     RequestorPerm = Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                    OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     ApproverPerm = Right.Edit,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     IncludeCurrentNode=true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },

                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                       JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     //OnFailure="Rejected",
                                     //FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G1  Budget and New Position"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Non_Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                          OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     IncludeCurrentNode=true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },

                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G1 Non Budget"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                       OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     TraversingFromRoot = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                       JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G2"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                      OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G7",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="4th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 7
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 9
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 10
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G3"
                        });

                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                      OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G7",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                    new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 9
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G4"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                       OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 8
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for HQ G5"
                        });


                        //Store
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 2,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                          OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                      StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G1 Replacement For"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                          OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                    StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G1 Budget"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 0,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                     new WorkflowCondition()
                                    {

                                        FieldName="ReplacementFor",
                                        FieldValues= new List<string>(){
                                            "NewPosition"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                          OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                      new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G1 Non Budget"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G1"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="HasBudget",
                                        FieldValues= new List<string>(){
                                            "Non_Budget"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                      new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                           StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G1 Non Budget"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                           StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                                                          RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G2"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G3"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                       OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType= Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },

                                        new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                        new WorkflowStep(){
                                     StepName="2st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="4th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 7
                                   },
                                   new WorkflowStep(){
                                   StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 9
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = false,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 10
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G3"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                         OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G7",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                  StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 8
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G4"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DeptDivisionId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="JobGradeCaption",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.Edit,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                      OnFailure="Cancelled",
                                     FailureVote = "Cancel",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Budget checker",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Review",
                                     OnSuccess = "Reviewed",
                                     DepartmentType = Group.Checker,
                                     NextDepartmentType= Group.Checker,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Department Name", FieldPattern= "departmentName",IsRequired =false } },
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IgnoreIfNoParticipant=false,
                                     IncludeCurrentNode=true,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     IncludeCurrentNode=true,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                   new WorkflowStep(){
                                     StepName="Recruiter Assignment",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = false,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Assign",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Assign To", FieldPattern= "assignTo", IsRequired =true } },
                                     StepNumber = 8
                                   }
                                }
                            },
                            WorkflowName = "Request To Hire for Store G5"
                        });
                        #endregion
                    }
                    else if (workflowType.Name == typeof(PromoteAndTransfer).Name)
                    {
                        #region PAT
                        //IsSameDepartment and HQ
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                      new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                    IncludeCurrentNode=true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   }
                                }
                            },
                            WorkflowName = "Promotion for HQ G1,G2"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G3","G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement ",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "Promotion for HQ G3,G4"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="2st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                    IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approve",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },

                                   new WorkflowStep(){
                                     StepName="Final Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                }
                            },
                            WorkflowName = "Promotion for HQ G5"
                        });
                        //IsSameDepartment and Store
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3","G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   }
                                }
                            },
                            WorkflowName = "Promotion for Store G1,G2,G3,G4"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },  new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement ",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approve",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },

                                   new WorkflowStep(){
                                     StepName="Final Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                }
                            },
                            WorkflowName = "Promotion for Store G5"
                        });

                        //Not Same Department and HQ
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    }, new WorkflowCondition()
                                    {

                                        FieldName="IsSameDepartment",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G8",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2st Approval",
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     IncludeCurrentNode=true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Promotion for HQ G1,G2(Other Department)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G3","G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                    new WorkflowStep(){
                                     StepName="3rd Approval",
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     TraversingFromRoot=true,
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="4th Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     OverwriteRequestedDepartment=true,
                                       RequestedDepartmentField="NewDeptOrLineId",
                                     IncludeCurrentNode=true,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                     ApproverPerm =  Right.View,   RequestedDepartmentField="NewDeptOrLineId",
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement ",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   }
                                }
                            },
                            WorkflowName = "Promotion for HQ G3,G4 (Other Department)"
                        });
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "False"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G7",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="2st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                     new WorkflowStep(){
                                     StepName="3rd Approval",
                                     OverwriteRequestedDepartment=true,   RequestedDepartmentField="NewDeptOrLineId",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                    new WorkflowStep(){
                                     StepName="4th Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                        RequestedDepartmentField="NewDeptOrLineId",
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },

                                    new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                        RequestedDepartmentField="NewDeptOrLineId",
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G8",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approve",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   },

                                   new WorkflowStep(){
                                     StepName="Final Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 9
                                   },
                                }
                            },
                            WorkflowName = "Promotion for HQ G5 (Other Department)"
                        });
                        //Not Same Department and Store
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G1","G2","G3","G4"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G5",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="2nd Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                      new WorkflowStep(){
                                     StepName="3rd Approval",
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     TraversingFromRoot=true,
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="4th Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                   new WorkflowStep(){
                                     StepName="5th Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 8
                                   }
                                }
                            },
                            WorkflowName = "Promotion for Store G1,G2,G3,G4"
                        });

                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "CurrentDepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="CurrentJobGrade",
                                        FieldValues= new List<string>(){
                                            "G5"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="IsStore",
                                        FieldValues= new List<string>(){
                                            "True"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                    new WorkflowStep(){
                                     StepName="Submit",
                                     IsStatusFollowStepName=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     ReverseJobGrade=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                    new WorkflowStep(){
                                     StepName="2nd Approval",
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     TraversingFromRoot=true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G6",
                                     MaxJobGrade="G9",
                                     IncludeCurrentNode=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="3rd Approval",
                                     AllowRequestToChange = true,
                                     OverwriteRequestedDepartment=true,
                                     RequestedDepartmentField="NewDeptOrLineId",
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G7",
                                     MaxJobGrade="G9",
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Acknowledgement ",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.HRDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Acknowledge",
                                     OnSuccess = "Acknowledged",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 5
                                   },
                                    new WorkflowStep(){
                                     StepName="HR Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approve",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 6
                                   },

                                   new WorkflowStep(){
                                     StepName="Final Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G9",
                                     MaxJobGrade="G9",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 7
                                   },
                                }
                            },
                            WorkflowName = "Promotion for Store G5"
                        });
                        #endregion
                    }
                    else if (workflowType.Name == typeof(Acting).Name)
                    {
                        context.WorkflowTemplates.Add(new WorkflowTemplate
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            IsActivated = true,
                            ItemType = workflowType.Name,
                            Modified = DateTime.Now,
                            Order = 1,
                            WorkflowData = new WorkflowData()
                            {
                                OverwriteRequestedDepartment = true,
                                RequestedDepartmentField = "DepartmentId",
                                StartWorkflowConditions = new List<WorkflowCondition>()
                                {
                                    new WorkflowCondition()
                                    {
                                        FieldName="RequestedJobGrade",
                                        FieldValues= new List<string>(){
                                            "G2","G3","G4","G5","G6"
                                        }
                                    },
                                    new WorkflowCondition()
                                    {

                                        FieldName="Status",
                                        FieldValues= new List<string>(){
                                            "Draft","Requested To Change"
                                        }
                                    }
                                },
                                Steps = new List<WorkflowStep>()
                                {
                                new WorkflowStep(){
                                     StepName="Submit",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="CreatedById",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Submit",
                                     OnSuccess = "Submitted",
                                     TraversingFromRoot=true,
                                     IgnoreIfNoParticipant=true,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 1
                                   },
                                   new WorkflowStep(){
                                     StepName="1st Approval",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.DepartmentLevel,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     Level =1,
                                     MaxLevel=2,
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 2
                                   },
                                   new WorkflowStep(){
                                     StepName="Perfomance Approval",
                                     IsStatusFollowStepName= false,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.PerfomanceDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G4",
                                     IsHRHQ = true,
                                     MaxJobGrade="G4",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 3
                                   },
                                   new WorkflowStep(){
                                     StepName="HR Manager Approval",
                                     IsStatusFollowStepName= true,
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.UpperDepartment,
                                     DueDateNumber=7,
                                     OnFailure="Rejected",
                                     FailureVote = "Reject",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     DepartmentType = Group.Member,
                                     NextDepartmentType= Group.Member,
                                     JobGrade="G5",
                                     MaxJobGrade="G5",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     StepNumber = 4
                                   },
                                   new WorkflowStep(){
                                     StepName="Appraiser 1",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="FirstAppraiserId",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Approved",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name= "Actual", FieldPattern= "actual", IsRequired =true } },
                                     StepNumber = 5
                                   },
                                    new WorkflowStep(){
                                     StepName="Appraiser 2",
                                     AllowRequestToChange = true,
                                     ApproverPerm =  Right.View,
                                     ParticipantType = ParticipantType.ItemUserField,
                                     DataField="SecondAppraiserId",
                                     DueDateNumber=7,
                                     OnFailure="",
                                     FailureVote = "",
                                     SuccessVote = "Approve",
                                     OnSuccess = "Completed",
                                     IgnoreIfNoParticipant=false,
                                     RequestorPerm = Right.View,
                                     ReturnToStepNumber = 0,
                                     RestrictedProperties= new List<RestrictedProperty>(){ new RestrictedProperty() {Name="Actual", FieldPattern= "actual", IsRequired =true } },
                                     StepNumber = 6
                                   }
                                }
                            },
                            WorkflowName = "Acting"
                        });
                    }
                    #endregion
                }
            }
            #endregion

            #region item list recruitment
            if (!context.ItemListRecruitments.Any())
            {
                var initItemListRecruitments = new List<ItemListRecruitment>
                {
                    new ItemListRecruitment
                    {
                        Id = Guid.NewGuid(),
                        Code = "Card",
                        Name = "Card Over",
                        Unit = "Cái"
                    },
                    new ItemListRecruitment
                    {
                        Id = Guid.NewGuid(),
                        Code = "LAPTOP",
                        Name = "Laptop Dell",
                        Unit = "Cái"
                    },
                    new ItemListRecruitment
                    {
                        Id = Guid.NewGuid(),
                        Code = "CPU",
                        Name = "CPU",
                        Unit = "Cái"
                    },
                    new ItemListRecruitment
                    {
                        Id = Guid.NewGuid(),
                        Code = "OCAMDIEM",
                        Name = "Ổ cắm điện",
                        Unit = "Cái"
                    }
                };
                context.ItemListRecruitments.AddRange(initItemListRecruitments);
                var initJobGradeItemListRecrutment = new List<JobGradeItemRecruitmentMapping>
            {
                new JobGradeItemRecruitmentMapping
                {
                    Id = Guid.NewGuid(),
                    JobGradeId = jobGrades[0].Id,
                    ItemListRecruitmentId = initItemListRecruitments[0].Id,
                    Created = DateTimeOffset.Now
                },
                new JobGradeItemRecruitmentMapping
                {
                    Id = Guid.NewGuid(),
                    JobGradeId = jobGrades[0].Id,
                    ItemListRecruitmentId = initItemListRecruitments[1].Id,
                    Created = DateTimeOffset.Now
                },
                new JobGradeItemRecruitmentMapping
                {
                    Id = Guid.NewGuid(),
                    JobGradeId = jobGrades[0].Id,
                    ItemListRecruitmentId = initItemListRecruitments[2].Id,
                    Created = DateTimeOffset.Now
                },
                new JobGradeItemRecruitmentMapping
                {
                    Id = Guid.NewGuid(),
                    JobGradeId = jobGrades[1].Id,
                    ItemListRecruitmentId = initItemListRecruitments[1].Id,
                    Created = DateTimeOffset.Now
                },
                new JobGradeItemRecruitmentMapping
                {
                    Id = Guid.NewGuid(),
                    JobGradeId = jobGrades[1].Id,
                    ItemListRecruitmentId = initItemListRecruitments[2].Id,
                    Created = DateTimeOffset.Now
                }
            };
                context.JobGradeItemRecruitmentMappings.AddRange(initJobGradeItemListRecrutment);
            }





            #endregion

            #region Master data
            if (!context.MasterDatas.Any())
            {
                #region Khởi tạo các reason type và các master data dành cho các reason đó
                var reasonMetadataTypes = new List<MetadataType>
            {
                new MetadataType
                {
                    Id = Guid.NewGuid(),
                    Name = "Missing Timeclock Reason Type",
                    Value = MetadataTypeConstants.MISSING_TIME_CLOCK_REASON_TYPE_CODE,
                    Created = DateTimeOffset.Now
                },
                new MetadataType
                {
                    Id = Guid.NewGuid(),
                    Name = "Overtime Reason Type",
                    Value = MetadataTypeConstants.OVERTIME_REASON_TYPE_CODE,
                    Created = DateTimeOffset.Now
                },
                new MetadataType
                {
                    Id = Guid.NewGuid(),
                    Name = "Resignation Reason Type",
                    Value = MetadataTypeConstants.RESIGNATION_REASON_TYPE_CODE,
                    Created = DateTimeOffset.Now
                },
                new MetadataType
                {
                    Id = Guid.NewGuid(),
                    Name = "Shift Exchange Reason Type",
                    Value = MetadataTypeConstants.SHIFT_EXCHANGE_REASON_TYPE_CODE,
                    Created = DateTimeOffset.Now
                },
                new MetadataType
                {
                    Id = Guid.NewGuid(),
                    Name = "Position",
                    Value = "Position",
                    Created = DateTimeOffset.Now
                }
            };
                context.MetadataTypes.AddRange(reasonMetadataTypes);
                #endregion
                var initApplicationMassterData = new List<MasterData>
            {
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Trung học phổ thông",
                    Name = "Trung học phổ thông",
                    Created = DateTimeOffset.Now.AddMinutes(1),
                    MetaDataTypeId = new Guid("b7687086-93b4-4d9d-a74c-94da3ebea98f"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Tốt nghiệp đại học",
                    Name = "Tốt nghiệp đại học",
                    Created = DateTimeOffset.Now.AddMinutes(2),
                    MetaDataTypeId = new Guid("b7687086-93b4-4d9d-a74c-94da3ebea98f"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "1",
                    Name = "1 tháng",
                    Description = "",
                    Created = DateTimeOffset.Now.AddMinutes(1),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074c2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "3",
                    Name = "3 tháng",
                    Created = DateTimeOffset.Now.AddMinutes(3),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074c2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "6",
                    Name = "6 tháng",
                    Created = DateTimeOffset.Now.AddMinutes(6),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074c2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "9",
                    Name = "9 tháng",
                    Created = DateTimeOffset.Now.AddMinutes(9),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074c2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "12",
                    Name = "12 tháng",
                    Created = DateTimeOffset.Now.AddMinutes(12),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074c2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "1",
                    Name = "Sau 30 ngày",
                    MetaDataTypeId = new Guid("11df2c4c-65d2-4314-a552-850f6808f6d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "2",
                    Name = "Sau 45 ngày",
                    MetaDataTypeId = new Guid("11df2c4c-65d2-4314-a552-850f6808f6d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "3",
                    Name = "Có thể làm việc ngay",
                    MetaDataTypeId = new Guid("11df2c4c-65d2-4314-a552-850f6808f6d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "1",
                    Name = "Chưa có kinh nghiệm",
                    Created = DateTimeOffset.Now.AddMinutes(1),
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "2",
                    Name = "1-2 năm",
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "3",
                    Name = "2-4 năm",
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "4",
                    Name = "4-8 năm",
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "5",
                    Name = "Trên 8 năm",
                    MetaDataTypeId = new Guid("b59b57c6-ca25-49a8-872e-f6e7d34074d2"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Ca hát",
                    Name = "Ca hát",
                    Created = DateTimeOffset.Now.AddMinutes(1),
                    MetaDataTypeId = new Guid("3dab2550-1b34-48b7-afc8-bc6611c555d6"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Nhảy múa",
                    Name = "Nhảy múa",
                    Created = DateTimeOffset.Now.AddMinutes(2),
                    MetaDataTypeId = new Guid("3dab2550-1b34-48b7-afc8-bc6611c555d6"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Thuyết trình",
                    Name = "Thuyết trình",
                    Created = DateTimeOffset.Now.AddMinutes(3),
                    MetaDataTypeId = new Guid("3dab2550-1b34-48b7-afc8-bc6611c555d6"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "1",
                    Name = "Nấu ăn",
                    Created = DateTimeOffset.Now.AddMinutes(3),
                    MetaDataTypeId = new Guid("11df2c4c-65d2-4314-a552-850f6808f6d7"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Spanish",
                    Name = "Tiếng Tây Ban Nha",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "English",
                    Name = "Tiếng Anh",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Portuguese",
                    Name = "Tiếng Bồ Đào Nha",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Russian",
                    Name = "Tiếng Nga",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "French",
                    Name = "Tiếng Pháp",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Japanese",
                    Name = "Tiếng Nhật",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "German",
                    Name = "Tiếng Đức",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Chinese",
                    Name = "Tiếng Trung",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Italian",
                    Name = "Tiếng Ý",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Thai",
                    Name = "Tiếng Thái",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Korean",
                    Name = "Tiếng Hàn",
                    MetaDataTypeId = new Guid("3553183a-23af-462d-afe4-e3bfd68648f1"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "5-day week",
                    Name = "Làm việc 5 ngày / tuần",
                    Created = DateTimeOffset.Now.AddMinutes(1),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "More Annual Leave",
                    Name = "Nhiều ngày phép năm",
                    Created = DateTimeOffset.Now.AddMinutes(2),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Higher Bonus",
                    Name = "Thưởng cao hơn",
                    Created = DateTimeOffset.Now.AddMinutes(3),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Money for Long Service",
                    Name = "Phúc lợi theo thâm niên",
                    Created = DateTimeOffset.Now.AddMinutes(4),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "More Social / Recreational Activities",
                    Name = "Nhiều hoạt động Xã hội/Giải trí",
                    Created = DateTimeOffset.Now.AddMinutes(5),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Personal Loans",
                    Name = "Hỗ trợ vay cá nhân",
                    Created = DateTimeOffset.Now.AddMinutes(6),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Family Medical Coverage",
                    Name = "Bảo hiểm Sức khỏe cho người thân",
                    Created = DateTimeOffset.Now.AddMinutes(7),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Good Pay",
                    Name = "Lương cao",
                    Created = DateTimeOffset.Now.AddMinutes(8),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Educational Sponsorship",
                    Name = "Hỗ trợ Giáo dục",
                    Created = DateTimeOffset.Now.AddMinutes(9),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Training & Development Opportunities",
                    Name = "Cơ hội Đào tạo & Phát triển",
                    Created = DateTimeOffset.Now.AddMinutes(10),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Career Advancement Opportunities",
                    Name = "Cơ hội Thăng tiến trong nghề nghiệp",
                    Created = DateTimeOffset.Now.AddMinutes(11),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Interesting / Challenging Job",
                    Name = "Công việc hấp dẫn/thách thức",
                    Created = DateTimeOffset.Now.AddMinutes(12),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Good Boss",
                    Name = "Quản lý tốt",
                    Created = DateTimeOffset.Now.AddMinutes(13),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Good Team-mates",
                    Name = "Đồng nghiệp tốt",
                    Created = DateTimeOffset.Now.AddMinutes(14),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Minimal Overtime",
                    Name = "Thời gian tăng ca ít",
                    Created = DateTimeOffset.Now.AddMinutes(15),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Good Work Environment / Physical Conditions",
                    Name = "Môi trường làm việc/Cơ sở vật chất tốt",
                    Created = DateTimeOffset.Now.AddMinutes(16),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
                new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "Rewards based on merit or performance",
                    Name = "Thưởng theo năng lực làm việc",
                    Created = DateTimeOffset.Now.AddMinutes(17),
                    MetaDataTypeId = new Guid("a89a19cb-4c6a-4c8b-b9bb-bd60be9e8a03"),
                },
            };
                context.MasterDatas.AddRange(initApplicationMassterData);

                #region mission time clock reason 
                var initValueMissingTimeClocks = new List<MasterData>
                {
                    new MasterData {
                        Id = Guid.NewGuid(),
                        Code = "MT0001",
                        Name = "Forget employee card",
                        MetaDataTypeId = reasonMetadataTypes[0].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                    },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "MT0002",
                    Name = "Losing employee card/broken",
                        MetaDataTypeId = reasonMetadataTypes[0].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "MT0003",
                    Name = "Forget scan time in/out",
                        MetaDataTypeId = reasonMetadataTypes[0].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "MT0004",
                    Name = "Working outsite",
                        MetaDataTypeId = reasonMetadataTypes[0].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "MT0005",
                    Name = "Others",
                        MetaDataTypeId = reasonMetadataTypes[0].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                }
                };
                context.MasterDatas.AddRange(initValueMissingTimeClocks);
                #endregion

                #region Overtime reason
                var initDataOvertimeReason = new List<MasterData>
                {
                    new MasterData {
                        Id = Guid.NewGuid(),
                        Code = "OT0001",
                        Name = "Bộ phận thiếu nhân sự",
                        MetaDataTypeId = reasonMetadataTypes[1].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                    },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "OT0002",
                    Name = "Hỗ trợ thời gian đông khách",
                        MetaDataTypeId = reasonMetadataTypes[1].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "OT0003",
                    Name = "Hỗ trợ thời gian ngày lễ",
                        MetaDataTypeId = reasonMetadataTypes[1].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "OT0004",
                    Name = "Hỗ trợ thời gian khai trương",
                        MetaDataTypeId = reasonMetadataTypes[1].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "OT0005",
                    Name = "Lý do khác (ghi rõ lý do)",
                        MetaDataTypeId = reasonMetadataTypes[1].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                }
                };
                context.MasterDatas.AddRange(initDataOvertimeReason);
                #endregion


                #region RESIGNATION REASONS
                var initDataResignationReason = new List<MasterData>
                {
                    new MasterData {
                        Id = Guid.NewGuid(),
                        Code = "RE0001",
                        Name = "Bộ phận thiếu nhân sự",
                        MetaDataTypeId = reasonMetadataTypes[2].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                    },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "RE0002",
                    Name = "Hỗ trợ thời gian đông khách",
                        MetaDataTypeId = reasonMetadataTypes[2].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "RE0003",
                    Name = "Hỗ trợ thời gian ngày lễ",
                        MetaDataTypeId = reasonMetadataTypes[2].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "RE0004",
                    Name = "Hỗ trợ thời gian khai trương",
                        MetaDataTypeId = reasonMetadataTypes[2].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "RE0005",
                    Name = "Lý do khác (ghi rõ lý do)",
                        MetaDataTypeId = reasonMetadataTypes[2].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                }
                };
                context.MasterDatas.AddRange(initDataResignationReason);
                #endregion

                #region SHIFTEXCHANGE_REASON
                var initDataShiftExchangeReason = new List<MasterData>
                {
                    new MasterData {
                        Id = Guid.NewGuid(),
                        Code = "SE0001",
                        Name = "Bộ phận thiếu nhân sự",
                        MetaDataTypeId = reasonMetadataTypes[3].Id,
                        Created = DateTimeOffset.Now,
                        CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                    },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "SE0002",
                    Name = "Hỗ trợ thời gian đông khách",
                        MetaDataTypeId = reasonMetadataTypes[3].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "SE0003",
                    Name = "Hỗ trợ thời gian ngày lễ",
                        MetaDataTypeId = reasonMetadataTypes[3].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "SE0004",
                    Name = "Hỗ trợ thời gian khai trương",
                        MetaDataTypeId = reasonMetadataTypes[3].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                },
                    new MasterData
                {
                    Id = Guid.NewGuid(),
                    Code = "SE0005",
                    Name = "Lý do khác (ghi rõ lý do)",
                        MetaDataTypeId = reasonMetadataTypes[3].Id,
                    Created = DateTimeOffset.Now,
                    CreatedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        ModifiedById = new Guid("812bb45f-c4a4-4750-a493-371b0cc23fa0"),
                        CreatedBy = "Edoc System",
                        ModifiedBy = "Edoc System"
                }
                };
                context.MasterDatas.AddRange(initDataShiftExchangeReason);
                #endregion
                var sqlContent = ReflectionHelper.ReadResourceFile<DbInitializer>("Aeon.HR.Data.sql_initializer.sql");
                context.Database.ExecuteSqlCommand(sqlContent);
            }
            #endregion

            #region EMAIL TEMPLATE
            if (!context.EmailTemplates.Any())
            {
                var listTemplate = new List<EmailTemplate>();
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.MainLayout.ToString(),
                    Body = "[MainContent]",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.ForApprover.ToString(),
                    Subject = "Request(s) are waiting for Approval",
                    Body = "<p>Dear [ApproverName],<br />You have the request of the following item(s) waiting for your approval:</p>" +
                    "<p>****FINANCE****</p>" +
                    "<p>[Edoc1BusinessName]</p>" +
                    "<p>****HUMAN RESOURCE****</p>" +
                    "<p>[Edoc2BusinessName]</p>" +
                    "<p>Please click to the link below for more details.</p>" +
                    "<p>Xin chào [ApproverName],<br />Có các phiếu đề xuất đang chờ bạn phê duyệt:</p>" +
                    "<p>****FINANCE****<br /><br />[Edoc1BusinessNameVN]</p>" +
                    "<p>****HUMAN RESOURCE****<br /><br /> [Edoc2BusinessNameVN]</p>" +
                    "<p>Vui lòng click vào link bên dưới để xem thêm chi tiết<br />" +
                    "<p>[Edoc1Link]</p>" +
                    "<p>[Edoc2Link]</p>"
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.ForCreatorApproved.ToString(),
                    Subject = "[BusinessName] - Request has been Completed",
                    Body = "<p>Dear [CreatorName],<br/>Your request of [BusinessName] has been completed. Click here for more details:<br />[Link].</p><p>Xin chào [CreatorName],<br/>" +
                    "[BusinessNameVN] của bạn đã được duyệt hoàn tất. Vui lòng truy cập vào đường dẫn bên dưới để xem thêm chi tiết :<br />[Link].</p>",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.ForCreatorRejected.ToString(),
                    Subject = "[BusinessName] - Request has been Rejected",
                    Body = "<p>Dear [CreatorName],<br/>Your request of [BusinessName] has been rejected. Please click to the link below for more details.</p><p>Xin chào [CreatorName],<br/>" +
                    "[BusinessNameVN] của bạn đã bị từ chối duyệt. Vui lòng truy cập vào đường dẫn bên dưới để xem thêm chi tiết.<br />[Link].</p>",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.ForCreatorRequestToChange.ToString(),
                    Subject = "[BusinessName] - Request has been Requested to Change",
                    Body = "<p>Dear [CreatorName],<br/>Your request of [BusinessName] has been requested to change. Please click to the link below for more details.</p><p>Xin chào [CreatorName],<br/>" +
                    "[BusinessNameVN] của bạn có yêu cầu thay đổi từ cấp quản lý. Vui lòng truy cập vào đường dẫn bên dưới để xem thêm chi tiết.<br />[Link].</p>",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.CanNotPushOTRecord.ToString(),
                    Subject = "[BusinessName] - Lack of Shift plan",
                    Body = "<p>Dear [CreatorName],<br/>Your request of [BusinessName] was denied because you have not had shift plan. Please contact HR for your shift plan and click to the link below for more details.</p><p>Xin chào [CreatorName],<br/>" +
                    "[BusinessNameVN] của bạn không được ghi nhận bởi vì bạn chưa có lịch ca làm việc. Vui lòng liên hệ phòng nhân sự để khai báo lịch ca làm việc và vui lòng truy cập vào đường dẫn bên dưới để biết thêm chị tiết.<br />[Link]</p>",
                });
                //listTemplate.Add(new EmailTemplate
                //{
                //    Id = Guid.NewGuid(),
                //    Name = EmailTemplateName.NewADAccount.ToString(),
                //    Subject = "Your account has been created",
                //    Body = "<p>Dear [FullName],<br/>" +
                //    "Your account [LoginName] has been created. Click here to access edoc site:<br />" +
                //    "[Link].</p>",
                //});
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.NewMSAccount.ToString(),
                    Subject = "Your account has been created",
                    Body = "<p>Dear [FullName],<br/>" +
                      "<p>Dear [FullName],<br/>Your account [LoginName] in eDoc has been created, your password is: [Password]. Please click to the following link to access the website. <br />" +
                      "To enhance the security related to login credentials, you are prompted to change the password when you login to the eDoc for the first time." +
                      "</p>" +
                      "<p>Xin chào [FullName],<br/>Bạn đã được tạo một account mới [LoginName] trên eDoc, password của account là: [Password]. Vui lòng click vào link bên dưới để truy vập hệ thống. " +
                      "Để tăng cường bảo mật liên quan đến thông tin đăng nhập, hãy thay đổi mật khẩu khi đăng nhập vào eDoc ở lần đăng nhập đầu tiên." +
                      "<br />[Link].</p>",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.ResetPassword.ToString(),
                    Subject = "Your password has been reset",
                    Body = "<p>Dear [FullName],<br/>Your account [LoginName] in eDoc has been reset, your password is: [Password]. Please click to the following link to access the website.</p>" +
                    "<p>Xin chào [FullName],<br/>Tên đăng nhập [LoginName] của bạn trên eDoc đã được cập nhật. Mật khẩu mới là: [Password]. Vui lòng click vào link bên dưới để truy vập hệ thống. <br />[Link].</p>",
                });
                //BTA - Email admin checker
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.BTASendEmailWhenNextStepAdminChecker.ToString(),
                    Subject = "EDOC2 - Request to Change/ Cancel Business Trip Application [BusinessTripApplicationNumber]",
                    Body = "<p>Notification from BTA System. Be inform that Submitter of Business Trip Application No.: <strong>[BusinessTripApplicationNumber]</strong> has sent a request to Cancel flight ticket(s). <br/>Please proceed to review and cancel flight(s) tickets</p>" +
                    "<p>Thông báo từ hệ thống BTA. Người lập phiếu cho Phiếu đăng ký công tác số: <strong>[BusinessTripApplicationNumber]</strong>. Đã gửi yêu cầu Hủy vé máy bay. <br/>Vui lòng phê duyệt và hủy các vé máy bay trong phiếu này.</p>",
                });
                listTemplate.Add(new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = EmailTemplateName.For1STResignation.ToString(),
                    Subject = "Request(s) are waiting for HR Review",
                    Body = "<p>Dear [ApproverName],<br />There are Resignation requests waiting for HR review:</p>" +
                    "<br/><p>[Edoc2RefereceNumber]</p>" +
                    "<p>Please click to the link below for more details.</p>" +
                    "<p>Xin chào [ApproverName],<br/>Có các đơn nghỉ việc đang chờ HR review:</p>" +
                    "<br/>[Edoc2RefereceNumber]</p>" +
                    "<p>Vui lòng click vào link bên dưới để xem thêm chi tiết<br />" +
                    "<p>[Edoc2AllResignationsLink]</p>"
                });
                context.EmailTemplates.AddRange(listTemplate);
            }
            #endregion
            #region Appriciation List
            if (!context.AppreciationListRecruitments.Any())
            {
                var appreciationList = new List<AppreciationListRecruitment>()
                {
                     new AppreciationListRecruitment
                    {
                        Id = new Guid("A023A7A8-84BA-4AF7-960F-C5E64B9DFDA7"),
                        Code = "APP1",
                        Name = "Pass",
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now,
                    },
                    new AppreciationListRecruitment
                    {
                        Id = new Guid("BFDAE0DB-022D-407E-99C0-89C23C921357"),
                        Code = "APP2",
                        Name = "Reject",
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now,
                    },
                    new AppreciationListRecruitment
                    {
                        Id = new Guid("0996C344-0EAD-4EDA-8631-1E9AAC24CE90"),
                        Code = "APP3",
                        Name = "Keep in view",
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now,
                    },new AppreciationListRecruitment
                    {
                        Id = new Guid("691EA70E-CCE5-41C5-BE23-F834DAFE8491"),
                        Code = "APP4",
                        Name = "Another Position",
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now,
                    }
                };
                context.AppreciationListRecruitments.AddRange(
                   appreciationList
                );
            }
            #endregion
            #region Applicant Status List
            if (!context.ApplicantStatusRecruitments.Any())
            {
                var applicationStatusList = new List<ApplicantStatusRecruitment>()
                {
                    new ApplicantStatusRecruitment
                    {
                        Id = new Guid("F17CE834-F0A7-4EDE-B724-4940E682A67A"),
                        Code = "APP-001",
                        Name = "Initial",
                        Arrangement = 1,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("98593602-C8BB-4553-8539-FC4C905DDE36"),
                        Code = "APP-002",
                        Name = "Accept Offer",
                        Arrangement = 2,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("F41AC937-170F-4C91-848D-0001FE4EE5A1"),
                        Code = "APP-003",
                        Name = "Interview Round 1",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("1739AAD6-F482-49D6-BD49-BA844C9D44AC"),
                        Code = "APP-004",
                        Name = "Interview Round 2",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("67C3A597-B028-4FBE-B651-D8404F2C56D9"),
                        Code = "APP-005",
                        Name = "Waiting for Onboarding",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("47583365-1053-46F5-98C5-87002298ABAB"),
                        Code = "APP-006",
                        Name = "Contract Signed",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("AC630E41-5090-4B52-B0C6-001B9B5FE22D"),
                        Code = "APP-007",
                        Name = "Reject to Interview",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("EA98B60A-E201-41EE-B941-78C06FDDDA32"),
                        Code = "APP-008",
                        Name = "Failed",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },new ApplicantStatusRecruitment
                    {
                        Id = new Guid("4AB16139-7918-442E-8F26-98A0E866D5E5"),
                        Code = "APP-009",
                        Name = "Signed Offer",
                        Arrangement = 3,
                        Created = DateTimeOffset.Now,
                        Modified = DateTimeOffset.Now
                    },
                };
            }
            //if (!context.TargetPlanPeriods.Any())
            //{
            //    var now = DateTimeOffset.Now;
            //    var currentMonth = now.Month + 1;
            //    var previousMonth = now.Month;
            //    var targetPlanPeriod = new TargetPlanPeriod
            //    {
            //        Id = Guid.NewGuid(),
            //        Created = DateTimeOffset.Now,
            //        Modified = DateTimeOffset.Now,
            //        CreatedBy = "AEON System",
            //        ModifiedBy = "AEON System",
            //        CreatedByFullName = "AEON System",
            //        ModifiedByFullName = "AEON System",
            //        FromDate = new DateTimeOffset(new DateTime(now.Year,previousMonth, 26)),
            //        ToDate = new DateTimeOffset(new DateTime(now.Year,currentMonth, 25)),
            //        Name = String.Format("{0}/{1}", previousMonth, now.Year)
            //    };
            //    context.TargetPlanPeriods.Add(targetPlanPeriod);
            //}
            #endregion

            #region Salary Dat configuration
            // trumhonai update
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "SalaryPeriodFrom") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "SalaryPeriodFrom", Value = 26, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "SalaryPeriodTo") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "SalaryPeriodTo", Value = 25, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "DeadlineOfSubmittingCABApplication") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "DeadlineOfSubmittingCABApplication", Value = 28, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "CreatedNewPeriodDate") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "CreatedNewPeriodDate", Value = 10, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "DeadlineOfSubmittingCABHQ") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "DeadlineOfSubmittingCABHQ", Value = 28, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "DeadlineOfSubmittingCABStore") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "DeadlineOfSubmittingCABStore", Value = 28, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "TimeOfSubmittingCABHQ") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "TimeOfSubmittingCABHQ", Value = 0, Created = DateTime.Now });
            }
            if (context.DaysConfigurations.FirstOrDefault(x => x.Name == "TimeOfSubmittingCABStore") == null)
            {
                context.DaysConfigurations.Add(new DaysConfiguration() { Id = Guid.NewGuid(), Name = "TimeOfSubmittingCABStore", Value = 0, Created = DateTime.Now });
            }
            var listNull = context.DaysConfigurations.Where(x => x.Name == null).ToList();
            context.DaysConfigurations.RemoveRange(listNull);
            //var salaryDay = new DaysConfiguration
            //{
            //    Id = new Guid("6c9fbcf1-d599-47ce-9eb8-bc31c5537c7a"),
            //    SalaryPeriodFrom = 26,
            //    SalaryPeriodTo = 25,
            //    DeadlineOfSubmittingCABApplication = 28
            //};
            //if (!context.DaysConfigurations.Any())
            //{
            //    context.DaysConfigurations.Add(salaryDay);
            //}
            #endregion

            #region times configruation
            var timeConfig = new MetadataType
            {
                Id = new Guid("96e49f3e-d3ac-488a-b982-ff53bba8f711"),
                Name = "Time Configuration",
                Value = "TIME_CONFIGURATION",
                Created = DateTimeOffset.Now,
                Modified = DateTimeOffset.Now

            };
            if (!context.MetadataTypes.Any(i=> i.Id == timeConfig.Id))
            {
                context.MetadataTypes.Add(timeConfig);
            }

            var timeInRound = new MasterData
            {
                Id = new Guid("D33BADFD-3AC5-4491-8A62-FF08F1AF7277"),
                Name = "Time In Round",
                Code = "30",
                MetaDataTypeId = new Guid("96E49F3E-D3AC-488A-B982-FF53BBA8F711"),
                IsDeleted = false,
                Created = DateTimeOffset.Now,
                Modified = DateTimeOffset.Now,
                ModifiedById = new Guid("5DD83D47-9045-44B6-AD2C-AF70F67C8C44"),
            };
            var timeOutRound = new MasterData
            {
                Id = new Guid("D33BADFD-3AC5-4491-8A62-FF08F1AF7278"),
                Name = "Time Out Round",
                Code = "15",
                MetaDataTypeId = new Guid("96E49F3E-D3AC-488A-B982-FF53BBA8F711"),
                IsDeleted = false,
                Created = DateTimeOffset.Now,
                Modified = DateTimeOffset.Now,
                ModifiedById = new Guid("5DD83D47-9045-44B6-AD2C-AF70F67C8C44"),
            };
            if (!context.MasterDatas.Any(i => i.Id == timeInRound.Id))
            {
                context.MasterDatas.Add(timeInRound);

            }
            if (!context.MasterDatas.Any(i => i.Id == timeOutRound.Id))
            {
                context.MasterDatas.Add(timeOutRound);
            }

            #endregion


            if (!context.Region.Any())
            {
                var region = new List<Region>();
                region.Add(new Region
                {
                    Id = new Guid("2D7D8F63-B9E1-4431-A537-9496ACDB5A37"),
                    RegionName = "North",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                region.Add(new Region
                {
                    Id = new Guid("D8547381-DE75-4C09-894C-E731D6E2C0DC"),
                    RegionName = "Middle",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                region.Add(new Region
                {
                    Id = new Guid("C239F821-2CC1-4073-A069-FA151E257179"),
                    RegionName = "South",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                context.Region.AddRange(region);
            }

            if (!context.BTAPolicy.Any())
            {
                var btaPolicyStore = new List<BTAPolicy>();

                //Store
                #region "Store"
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("FF261D6C-9ED3-43E0-9934-B5C608099FC1"),
                    JobGradeId = new Guid("ec1b3149-16be-4bfb-a150-710b72ffd3b5"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("91FE15FB-1DB9-4157-B70A-B605C5DB42D3"),
                    JobGradeId = new Guid("f8a9694f-d369-43f5-8625-9c38fa36abe4"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("D97CB658-656A-4C8B-98A3-2F7FFD9BFE8F"),
                    JobGradeId = new Guid("53e9bc34-3f92-45b3-b1ee-606e7ee6a77c"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("86DE822C-4F16-443F-AD05-F6BB7E977E12"),
                    JobGradeId = new Guid("5e7b0464-d2b2-446a-81d0-02281de68431"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("95DCE757-E73E-45C5-9922-AB5E7BAD6D5F"),
                    JobGradeId = new Guid("a67cf0b7-44bd-437c-9b3e-5c95624bdbd4"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("EE7D7779-E39D-4E6C-8A76-176052AEDE2B"),
                    JobGradeId = new Guid("333b2071-d76c-4fac-9d6d-d64e2f2217b5"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("E545B94E-64F0-4EB2-BCE3-42165FEFF356"),
                    JobGradeId = new Guid("7958b0c4-9bd1-4aa4-8aa6-6f201e0a65f2"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("E6983C16-08BA-4AF1-8969-E1CCC83A5987"),
                    JobGradeId = new Guid("a2063572-81c8-40c4-9842-d6a86a5957d7"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("EF1D5EAC-619E-4F5E-A175-D2B1903F4E2D"),
                    JobGradeId = new Guid("670761a9-92f5-4e63-945f-4176aa324923"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = true,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                #endregion

                //HQ
                #region "HQ"
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("ADCD4032-AFB0-400C-8756-4536CC8CDA5F"),
                    JobGradeId = new Guid("ec1b3149-16be-4bfb-a150-710b72ffd3b5"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("D42A7578-D73D-49A1-AEAF-9B91CACD56A7"),
                    JobGradeId = new Guid("f8a9694f-d369-43f5-8625-9c38fa36abe4"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("763692DD-B287-42BB-94AE-ED88BFF51805"),
                    JobGradeId = new Guid("53e9bc34-3f92-45b3-b1ee-606e7ee6a77c"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("D4D83B63-6CF7-42F3-A7BF-CEE7C350D1CA"),
                    JobGradeId = new Guid("5e7b0464-d2b2-446a-81d0-02281de68431"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("ACDDC472-7DD3-43ED-A9BA-FA256F15A908"),
                    JobGradeId = new Guid("a67cf0b7-44bd-437c-9b3e-5c95624bdbd4"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("FBD3A39D-955B-4FBE-9D62-C6F80E76D133"),
                    JobGradeId = new Guid("333b2071-d76c-4fac-9d6d-d64e2f2217b5"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("F76662D4-7F13-4E60-8FB8-E4A029A4E798"),
                    JobGradeId = new Guid("7958b0c4-9bd1-4aa4-8aa6-6f201e0a65f2"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("54141F88-FCBD-4A42-99F6-A8D8E2F863F3"),
                    JobGradeId = new Guid("a2063572-81c8-40c4-9842-d6a86a5957d7"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                btaPolicyStore.Add(new BTAPolicy
                {
                    Id = new Guid("2ABF5E97-3A49-4944-994A-E0A58D891651"),
                    JobGradeId = new Guid("670761a9-92f5-4e63-945f-4176aa324923"),
                    BudgetFrom = 0,
                    BudgetTo = 0,
                    IsStore = false,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                #endregion

                context.BTAPolicy.AddRange(btaPolicyStore);
            }
            if (!context.BookingContract.Any())
            {
                var bookingContract = new List<BookingContract>();

                bookingContract.Add(new BookingContract
                {
                    Id = new Guid("FE93C8E5-A63F-47F5-8B19-AD9A133867FD"),
                    FullName = "Phạm Thị Hiền",
                    EmailBookingContract = "admin.generalaffairs@aeon.com.vn",
                    PhoneNumber = "0763932523",
                    FirstName = "THI HIEN",
                    SurName = "PHAM",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = "sadmin",
                    ModifiedBy = "sadmin",
                    CreatedByFullName = "sadmin",
                    ModifiedByFullName = "sadmin",
                });
                context.BookingContract.AddRange(bookingContract);
            }
            context.SaveChanges();
        }
    }
}
