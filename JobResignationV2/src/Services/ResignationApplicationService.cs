
using JobResignationV2.src.ModelEntity;
using JobResignationV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.Services
{
    public class ResignationApplicationService : SQLQuery<ResignationApplicationEntity>
    {
        public List<ResignationApplicationEntity> GetCompletedResignations(DateTime toDate)
        {
            try
            {
                string formattedDate = toDate.ToString("yyyy-MM-dd"); // format cho SQL DATE
                string query = $@" SELECT * FROM ResignationApplications WHERE Status = 'Completed' AND OfficialResignationDate <= '{formattedDate}'";

                var result = this.GetItemsByQuery(query);

                if (result != null && result.Any())
                {
                    Utilities.WriteLogError($"[INFO] GetCompletedResignations found {result.Count} record(s) up to {formattedDate}");
                }
                else
                {
                    Utilities.WriteLogError($"[WARN] GetCompletedResignations found no records up to {formattedDate}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetCompletedResignations failed for toDate: {toDate:yyyy-MM-dd}. Exception: {ex.Message}");
                return new List<ResignationApplicationEntity>();
            }
        }

    }
}
