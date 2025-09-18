using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;
using SyncOrgchartJob.Service;

namespace SyncOrgchartJob
{
    internal abstract class Program
    {
        private static string STR_DeparmentPreFix_CacheKey = "Department_";
        public static void Main(string[] args)
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                string keyCache = $"{STR_DeparmentPreFix_CacheKey}";
                var cacheObj = cache.Get(keyCache);

                bool rollback = false;
                bool.TryParse(ConfigurationManager.AppSettings["IsRollback"], out rollback);
                if (rollback)
                {
                    Console.WriteLine("🔄 ROLLBACK MODE ENABLED");
                    Console.Write("Vui lòng nhập ID rollback (hoặc nhấn Enter để hủy): ");
                    var inputId = Console.ReadLine();
                    
                    bool isValidId = int.TryParse(inputId, out int rollbackId);
                    if (!isValidId) {
                        Console.WriteLine("❌  ID không hợp lệ. Vui lòng nhập ID hợp lệ.");
                        Console.WriteLine("Rollback đã bị hủy.");
                        return;
                    }
                    else
                    {
                        // Thực hiện rollback theo ID
                        Console.Write($"Bạn có chắc chắn muốn rollback ID \"{rollbackId}\" không? (Y/N): ");
                        string confirm = Console.ReadLine();

                        if (confirm?.Trim().ToUpper() == "Y")
                        {
                            try
                            {
                                Console.Write($"Rollback đang được thực hiện cho ID: {rollbackId}");
                                RollbackService service = new RollbackService();
                                service.RunAsync(rollbackId).GetAwaiter().GetResult();
                                Console.WriteLine($"✅ Rollback completed for ID: {rollbackId}");
                                ClearCacheHR();
                            }
                            catch (Exception ex)
                            {
                                Logger.Write($"❌ Rollback ID {rollbackId} failed: {ex.Message}", true);
                                throw ex;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Rollback đã bị hủy.");
                        }
                    }
                }
                else
                {
                    int index = 1;
                    for (int i = 0; i < index; i++)
                    {
                        Console.WriteLine("=========================");
                        Console.WriteLine($"🕐 [{DateTime.Now}] === Run #{i + 1} ===");

                        var stopwatch = Stopwatch.StartNew();

                        try
                        {
                            Console.WriteLine("===========================================");
                            Console.WriteLine("🔁 (1) BACKUP JOB...");
                            Console.WriteLine("-------------------------------------------");
                            Logger.Write("===========================================");
                            Logger.Write("🔁 (1) BACKUP JOB...");
                            Logger.Write("-------------------------------------------");

                            try
                            {
                                var backupService = new BackupService(); 
                                backupService.RunAsync().GetAwaiter().GetResult();
                                Console.WriteLine("✅ (1) BACKUP JOB COMPLETED");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ (1) BACKUP JOB FAILED: {ex.Message}");
                                Logger.Write($"❌ (1) BACKUP JOB FAILED: {ex.Message}");
                                throw new Exception($"Backup job failed: {ex.Message}");
                            }
                            
                            Console.WriteLine("===========================================");
                            Console.WriteLine("🔁 (1) STARTING SYNC JOB...");
                            Console.WriteLine("-------------------------------------------");
                            Logger.Write("===========================================");
                            Logger.Write("🔁 (1) STARTING SYNC JOB...");
                            Logger.Write("-------------------------------------------");

                            int headerId = 0;
                            try
                            {
                                var syncService = new SyncService(); 
                                headerId = syncService.RunAsync().GetAwaiter().GetResult();
                                Console.WriteLine("✅ (2) SYNC JOB COMPLETED");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ (2) SYNC JOB FAILED: {ex.Message}");
                                Logger.Write($"❌ (2) SYNC JOB FAILED: {ex.Message}");
                                throw new Exception($"Backup job failed: {ex.Message}");
                            }
                            
                            if (headerId <= 0)
                            {
                                Console.WriteLine("❌ No data to apply, skipping apply job.");
                                Logger.Write("❌ No data to apply, skipping apply job.");
                                break;
                            }

                            Console.WriteLine("===========================================");
                            Console.WriteLine("🚀 (2) STARTING APPLY JOB...");
                            Console.WriteLine("-------------------------------------------");
                            Logger.Write("===========================================");
                            Logger.Write("🚀 (2) STARTING APPLY JOB...");
                            Logger.Write("-------------------------------------------");

                            try
                            {
                                var applyService = new ApplyService();
                                applyService.RunAsync(headerId).GetAwaiter().GetResult();
                                Console.WriteLine("✅ (3) APPLY JOB COMPLETED");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ (3) APPLY JOB FAILED: {ex.Message}");
                                Logger.Write($"❌ (3) APPLY JOB FAILED: {ex.Message}");
                                throw new Exception($"Apply job failed: {ex.Message}");
                            }

                            Console.WriteLine("===========================================");
                            Console.WriteLine("🎉 ALL JOBS FINISHED");
                            Logger.Write($"===========================================");
                            Logger.Write($"🎉 ALL JOBS FINISHED");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Unhandled error: " + ex.Message);
                            Logger.Write("❌ Unhandled error: " + ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            throw new Exception($"Unhandled error: {ex.Message}");
                        }

                        stopwatch.Stop();
                        Console.WriteLine($"✅ Run #{i + 1} completed in {stopwatch.Elapsed.TotalSeconds:N2} seconds");
                        Console.WriteLine("=========================\n");
                        Logger.Write($"✅ Run #{i + 1} completed in {stopwatch.Elapsed.TotalSeconds:N2} seconds");
                        Logger.Write("=========================\n");
                    }
                    
                    Console.WriteLine($"🎉 All {index} runs finished.");

                    try
                    {
                        ClearCacheHR();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write($"❌ Failed to clear cache department: {ex.Message}", true);
                        Console.WriteLine($"❌ Failed to clear cache department: {ex.Message}");
                        throw new Exception($"Failed to clear cache department: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled error: " + ex.Message);
                    throw new Exception($"Unhandled error: {ex.Message}");
            }
        }
        
        private static void ClearCacheHR()
        {
            IntergrationService intergrationService = new IntergrationService();
            bool rs = intergrationService.ClearCacheDepartment().GetAwaiter().GetResult();
            if (rs)
            {
                Console.WriteLine("✅ Cache department HR cleared successfully.");
                Logger.Write("✅ Cache department HR cleared successfully.", true);
            }
            else
            {
                Console.WriteLine("❌ HR Failed to clear cache department.");
                Logger.Write("❌ HR Failed to clear cache department.",true);
            }
            rs = intergrationService.ClearCacheDepartmentIT().GetAwaiter().GetResult();
            if (rs)
            {
                Console.WriteLine("✅ IT Cache department cleared successfully.");
                Logger.Write("✅ IT Cache department cleared successfully.", true);
            }
            else
            {
                Console.WriteLine("❌ IT Failed to clear cache department.");
                Logger.Write("❌ IT Failed to clear cache department.",true);
            }
        }
    }
}