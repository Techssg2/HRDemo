using JobPromoteAndTransfer.src.ModelEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransferV2.src.Services
{
    public class PromoteAndTransferService : JobPromoteAndTransferV2.src.SQLExcute.SQLQuery<PromoteAndTransferEntity>
    {

        public PromoteAndTransferService(){ }
        public List<PromoteAndTransferEntity> GetAllPromoteAndTransfer()
        {
            List<PromoteAndTransferEntity> users = new List<PromoteAndTransferEntity>();
            try
            {

                string query = $@"SELECT * FROM PromoteAndTransfers";
                users = this.GetItemsByQuery(query);

                Utilities.WriteLogError($"[INFO] Get {users.Count} records from PromoteAndTransfers.");

                return users;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetAllPromoteAndTransfer failed. Exception: {ex.Message}");
                return users;
            }
        }

    }
}
