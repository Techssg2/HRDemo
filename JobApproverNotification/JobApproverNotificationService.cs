using JobApproverNotification.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JobApproverNotification
{
    public partial class JobApproverNotificationService : ServiceBase
    {
        private Timer timer = new Timer();
        private bool IsProcess { get; set; }
        private readonly List<TimeSpan> _scheduledTimes = new List<TimeSpan>
        {
            //new TimeSpan(8, 0, 0),   // 8:00 AM
            //new TimeSpan(17, 0, 0)   // 5:00 PM
        };

        private readonly bool _testMode = true;
        private readonly int _testIntervalMinutes = 30;
        private DateTime _lastRunTime = DateTime.MinValue;

        public JobApproverNotificationService()
        {
            InitializeComponent();
            Utilities.WriteLogError("JobApproverNotificationService(): start");
        }

        protected override void OnStart(string[] args)
        {

            try
            {
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 60000; // 1 phút = 60,000ms
                timer.Enabled = true;
                timer.Start();

                //Utilities.WriteLogError($"Service started. Will run at 08:00 and 17:00 Vietnam time");

                if (_testMode)
                {
                    Utilities.WriteLogError($"Service started in TEST MODE. Will run every {_testIntervalMinutes} minutes");
                }
                else
                {
                    Utilities.WriteLogError($"Service started. Will run at 08:00 and 17:00 Vietnam time");
                }
            }
            catch (Exception e)
            {
                Utilities.WriteLogError("ex:" + e.Message);
            }
            base.OnStart(args);
        }

        private async void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //if (IsProcess) return;
            //try
            //{
            //    var vietnamTime = GetVietnamTime();
            //    var currentTime = vietnamTime.TimeOfDay;

            //    bool shouldRun = _scheduledTimes.Any(scheduledTime =>
            //    Math.Abs((currentTime - scheduledTime).TotalMinutes) < 1);
            //    if (shouldRun)
            //    {
            //        IsProcess = true;
            //        timer.Stop();

            //        Utilities.WriteLogError($"Starting job at: {DateTime.Now}");

            //        SendNotifications sendMail = new SendNotifications();
            //        await sendMail.StartJobSendMail();

            //        Utilities.WriteLogError($"Job completed at: {DateTime.Now}");
            //        timer.Start();
            //    }
            //    else
            //    {
            //        if (currentTime.Minutes % 30 == 0)
            //        {
            //            Utilities.WriteLogError($"Service running. Current time: {vietnamTime:HH:mm:ss}, Next run: {GetNextRunTime(vietnamTime):HH:mm:ss}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Utilities.WriteLogError("OnElapsedTime error: " + ex.Message + DateTime.Now);
            //}
            //finally
            //{
            //    IsProcess = false;
            //    Utilities.WriteLogError($"IsProcess set to false at: {DateTime.Now}");
            //}
            if (IsProcess) return;
            try
            {
                var vietnamTime = GetVietnamTime();
                bool shouldRun = false;

                if (_testMode)
                {
                    // Test mode: chạy mỗi 30 phút
                    if (_lastRunTime == DateTime.MinValue || (vietnamTime - _lastRunTime).TotalMinutes >= _testIntervalMinutes)
                    {
                        shouldRun = true;
                        _lastRunTime = vietnamTime;
                    }
                }
                else
                {
                    // Production mode: chạy 8h và 17h
                    var currentTime = vietnamTime.TimeOfDay;
                    shouldRun = _scheduledTimes.Any(scheduledTime =>
                        Math.Abs((currentTime - scheduledTime).TotalMinutes) < 1);
                }

                if (shouldRun)
                {
                    IsProcess = true;
                    timer.Stop();
                    Utilities.WriteLogError($"Starting job at: {vietnamTime:yyyy-MM-dd HH:mm:ss}");
                    SendNotifications sendMail = new SendNotifications();
                    await sendMail.StartJobSendMail();
                    Utilities.WriteLogError($"Job completed at: {GetVietnamTime():yyyy-MM-dd HH:mm:ss}");
                    timer.Start();
                }
                else
                {
                    if (vietnamTime.Minute % 30 == 0)
                    {
                        if (_testMode)
                        {
                            var nextRun = _lastRunTime == DateTime.MinValue ? vietnamTime : _lastRunTime.AddMinutes(_testIntervalMinutes);
                            Utilities.WriteLogError($"Service running in TEST MODE. Current time: {vietnamTime:HH:mm:ss}, Next run: {nextRun:HH:mm:ss}");
                        }
                        else
                        {
                            Utilities.WriteLogError($"Service running. Current time: {vietnamTime:HH:mm:ss}, Next run: {GetNextRunTime(vietnamTime):HH:mm:ss}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("OnElapsedTime error: " + ex.Message + " " + DateTime.Now);
            }
            finally
            {
                IsProcess = false;
                Utilities.WriteLogError($"IsProcess set to false at: {GetVietnamTime():yyyy-MM-dd HH:mm:ss}");
            }
        }

        private DateTime GetVietnamTime()
        {
            // UTC+7 for Vietnam
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);
        }

        private TimeSpan GetNextRunTime(DateTime currentVietnamTime)
        {
            var currentTime = currentVietnamTime.TimeOfDay;

            foreach (var scheduledTime in _scheduledTimes.OrderBy(t => t))
            {
                if (currentTime < scheduledTime)
                    return scheduledTime;
            }

            return _scheduledTimes.First();
        }

        protected override void OnStop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            Utilities.WriteLogError("Service stopped");
        }
    }
}
