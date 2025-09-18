
using JobPromoteAndTransfer.src.ModelEntity;
using JobPromoteAndTransferV2.src;
using JobPromoteAndTransferV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransfer.src.Services
{
    public class UserService : SQLQuery<UserEntity>
    {
        public UserEntity GetUserBySapCode(string sapCode)
        {
            try
            {
                sapCode = sapCode.Replace("'", "''"); 

                string query = $@"SELECT * FROM Users WHERE SapCode = '{sapCode}'";
                var users = this.GetItemsByQuery(query);
                var user = users.FirstOrDefault();

                if (user != null)
                {
                    Utilities.WriteLogError($"[INFO] GetUserBySapCode succeeded. SapCode: {sapCode}");
                }
                else
                {
                    Utilities.WriteLogError($"[WARN] GetUserBySapCode executed but no user found. SapCode: {sapCode}");
                }

                return user;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetUserBySapCode failed. SapCode: {sapCode}. Exception: {ex.Message}");
                return null;
            }
        }

        public UserEntity GetUserInfoSadmin()
        {
            try
            {
                string query = $@"SELECT * FROM Users WHERE LoginName = 'SAdmin'";
                var users = this.GetItemsByQuery(query);
                var user = users.FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetUserInfoSadmin failed. Login Name: SAdmin. Exception: {ex.Message}");
                return null;
            }
        }



        public bool UpdateInActivated(Guid userId)
        {
            try
            {
                string query = $@"UPDATE USERS SET IsActivated = 0 WHERE Id = '{userId}'";

                var isSuccess = this.ExecuteRunQuery(query);

                if (isSuccess)
                {
                    Utilities.WriteLogError($"[INFO] UpdateInActivated succeeded. UserId: {userId}");
                }
                else
                {
                    Utilities.WriteLogError($"[WARN] UpdateInActivated executed but returned false. UserId: {userId}");
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] UpdateInActivated failed. UserId: {userId}. Exception: {ex.Message}");
                return false;
            }
        }


    }
}
