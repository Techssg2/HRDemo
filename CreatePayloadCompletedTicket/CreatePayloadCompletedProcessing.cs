using Aeon.CreatePayloadCompleted.Utilities;
using CreatePayloadCompleted.src;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace CreatePayloadCompleted
{
    public partial class CreatePayloadCompletedProcessing : ServiceBase
    {
        private Timer timer = null;
        public CreatePayloadCompletedProcessing()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            int excuteTimeMinute = 3;
            try
            {
                excuteTimeMinute = int.Parse(ConfigurationManager.AppSettings.Get("ExcuteTime"));
            } catch (Exception e)
            {
                Utilities.WriteLogError("Error: Can't not get ExcuteTime");
                Utilities.WriteLogError("ErrorMessage: " + e.Message);
            }
            Utilities.WriteLogError("excuteTimeMinute: " + excuteTimeMinute);
            timer.Interval = excuteTimeMinute * 30000; // Execute mỗi 5 minutes
            timer.Elapsed += Timer_Tick;
            timer.Enabled = true;
        }

        private async void Timer_Tick(object sender, ElapsedEventArgs args)
        {
            // Thoi gian
            try
            {
                CreatePayloadCompletedUpdate payloadCompleted = new CreatePayloadCompletedUpdate();
                await payloadCompleted.ProcessingAPI();
            } catch (Exception e)
            {
                Utilities.WriteLogError("Timer_Tick.Exception.Message: " + e.Message);
                Utilities.WriteLogError("Timer_Tick.Exception.StackTrace: " + e.StackTrace);
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = true;
        }
    }
}
