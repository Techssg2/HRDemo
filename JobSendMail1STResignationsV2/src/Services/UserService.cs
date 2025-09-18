using JobSendMail1STResignationsV2.src.Entity;
using JobSendMail1STResignationsV2.src.Enums;
using JobSendMail1STResignationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Services
{
    public class UserService
    {
        private SQLQuery<UserEntity> userQuery;

        public UserService()
        {
            userQuery = new SQLQuery<UserEntity>();
        }
        public List<UserEntity> GetUsers(Guid departmentId, int role)
        {
            List<UserEntity> r_list = new List<UserEntity>();
            try
            {
                string selectData = string.Format(@"
                SELECT 
                    u.[Id]
                    ,u.[FullName]
                    ,u.[Email]
                    ,u.[LoginName]
                    ,u.[SAPCode]
                    ,u.[Type]
                    ,u.[IsActivated]
                    ,u.[Role]
                    ,u.[IsDeleted]
                    ,u.[Created]
                    ,u.[CreatedBy]
                    ,u.[Modified]
                    ,u.[ModifiedBy]
                    ,u.[CreatedById]
                    ,u.[ModifiedById]
                    ,u.[CreatedByFullName]
                    ,u.[ModifiedByFullName]
                    ,u.[AppService]
                    ,u.[ProfilePictureId]
                    ,u.[StartDate]
                    ,u.[QuotaDataJson]
                    ,u.[RedundantPRD]
                    ,u.[Gender]
                    ,u.[CheckAuthorizationUSB]
                    ,u.[IsTargetPlan]
                    ,u.[IsNotTargetPlan]
                    ,u.[HasTrackingLog]
                    ,u.[IsFromIT]
                FROM [dbo].[Users] u
                INNER JOIN [dbo].[UserDepartmentMappings] udm ON u.[Id] = udm.[UserId]
                WHERE u.[Email] IS NOT NULL
                AND u.[Email] <> ''
                AND udm.[DepartmentId] = '{0}'
                AND udm.[Role] = {1}
            ", departmentId, role);
                r_list = userQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetUsers:" + ex);
            }
            return r_list;
        }
        public List<UserEntity> GetUsersByDepartmentAndRole(Guid departmentId, Group departmentType, bool isHeadCount = true)
        {
            List<UserEntity> r_list = new List<UserEntity>();
            try
            {
                string selectData = string.Format(@"
                SELECT 
                    u.[Id]
                    ,u.[FullName]
                    ,u.[Email]
                    ,u.[LoginName]
                    ,u.[SAPCode]
                    ,u.[Type]
                    ,u.[IsActivated]
                    ,u.[Role]
                    ,u.[IsDeleted]
                    ,u.[Created]
                    ,u.[CreatedBy]
                    ,u.[Modified]
                    ,u.[ModifiedBy]
                    ,u.[CreatedById]
                    ,u.[ModifiedById]
                    ,u.[CreatedByFullName]
                    ,u.[ModifiedByFullName]
                    ,u.[AppService]
                    ,u.[ProfilePictureId]
                    ,u.[StartDate]
                    ,u.[QuotaDataJson]
                    ,u.[RedundantPRD]
                    ,u.[Gender]
                    ,u.[CheckAuthorizationUSB]
                    ,u.[IsTargetPlan]
                    ,u.[IsNotTargetPlan]
                    ,u.[HasTrackingLog]
                    ,u.[IsFromIT]
                FROM [dbo].[Users] u
                INNER JOIN [dbo].[UserDepartmentMappings] udm ON u.[Id] = udm.[UserId]
                WHERE u.[IsActivated] = 1
                AND u.[IsDeleted] = 0
                AND u.[Email] IS NOT NULL
                AND u.[Email] <> ''
                AND udm.[DepartmentId] = '{0}'
                AND udm.[Role] = {1}
                AND udm.[IsHeadCount] = {2}
                ", departmentId, (int)departmentType, isHeadCount ? 1 : 0);

                r_list = userQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetUsersByDepartmentAndRole:" + ex);
            }
            return r_list;
        }
    }
}
