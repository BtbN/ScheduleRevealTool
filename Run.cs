using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleRevealTool
{
    public class Run : INotifyPropertyChanged
    {
        private string game;
        private string category;
        private string runners;
        private string platform;
        private string estimate;
        private string time;
        private bool presented;

        public string Game
        {
            get => game;
            set
            {
                if (value != game)
                {
                    game = value;
                    NotifyChanged();
                }
            }
        }

        public string Category
        {
            get => category;
            set
            {
                if (value != category)
                {
                    category = value;
                    NotifyChanged();
                }
            }
        }

        public string Runners
        {
            get => runners;
            set
            {
                if (value != runners)
                {
                    runners = value;
                    NotifyChanged();
                }
            }
        }

        public string Platform
        {
            get => platform;
            set
            {
                if (value != platform)
                {
                    platform = value;
                    NotifyChanged();
                }
            }
        }

        public string Estimate
        {
            get => estimate;
            set
            {
                if (value != estimate)
                {
                    estimate = value;
                    NotifyChanged();
                }
            }
        }

        public string Time
        {
            get => time;
            set
            {
                if (value != time)
                {
                    time = value;
                    NotifyChanged();
                }
            }
        }

        public bool Presented
        {
            get => presented;
            set
            {
                if (value != presented)
                {
                    presented = value;
                    NotifyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
