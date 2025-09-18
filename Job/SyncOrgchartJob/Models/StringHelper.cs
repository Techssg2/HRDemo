namespace SyncOrgchartJob.Models
{
    public static class StringHelper
    {
        public static string ConvertToSAP(this string value)
        {
            return $"00{value}";
        }
    }
}