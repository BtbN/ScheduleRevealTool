using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace ScheduleRevealTool
{
    public class ClockTicker : INotifyPropertyChanged
    {
        public ClockTicker()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Timer_Tick(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
        }

        public DateTime Now
        {
            get => DateTime.Now;
        }
    }
}
