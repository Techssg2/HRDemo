using Aeon.OvertimeUpdateCompleted.Utilities;
using OvertimeUpdateCompleted.src;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace OvertimeUpdateCompleted
{
    public partial class DoubleRoundProcessing : ServiceBase
    {
        private Timer timer = null;
        public DoubleRoundProcessing()
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

        private void Timer_Tick(object sender, ElapsedEventArgs args)
        {
            // Thoi gian
            List<string> refereceNumeberExcuted = new List<string>();
            try
            {
                DoubleRoundProcessingUpdate doubleRound = new DoubleRoundProcessingUpdate();
                doubleRound.ExcuteUpdate();
            } catch (Exception e)
            {
                refereceNumeberExcuted.Add(e.Message);
            } finally
            {
                if (refereceNumeberExcuted.Any())
                    refereceNumeberExcuted.ForEach(x => Utilities.WriteLogError(x));
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = true;
        }
    }
}
