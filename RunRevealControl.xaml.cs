using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace ScheduleRevealTool
{
    public partial class RunRevealControl : UserControl
    {
        public RunRevealControl()
        {
            InitializeComponent();
        }

        public string Game
        {
            get
            {
                return GameText.Text;
            }
            set
            {
                GameText.Text = value;
            }
        }

        public string Category
        {
            get
            {
                return CategoryText.Text;
            }

            set
            {
                CategoryText.Text = value;
            }
        }

        public string Runners
        {
            get
            {
                return RunnerNamesText.Text;
            }
            set
            {
                RunnerNamesText.Text = value;
                if (value.Contains(",") || value.Contains("&") || value.ToLower().Contains(" vs."))
                    RunnerLabelLabel.Content = "Runners:";
                else
                    RunnerLabelLabel.Content = "Runner:";
            }
        }

        public string Estimate
        {
            get
            {
                return EstimateText.Text;
            }
            set
            {
                EstimateText.Text = value;
            }
        }

        public string Platform
        {
            get
            {
                return PlatformText.Text;
            }
            set
            {
                PlatformText.Text = value;
            }
        }

        public string Time
        {
            get
            {
                return TimeText.Text;
            }
            set
            {
                TimeText.Text = value;
            }
        }

        private Run curRun = null;

        public void Clear()
        {
            GameText.Clear();
            CategoryText.Clear();
            RunnerNamesText.Clear();
            PlatformText.Clear();
            EstimateText.Clear();
            TimeText.Clear();

            if (curRun != null)
            {
                curRun.PropertyChanged -= Run_PropertyChanged;
                curRun = null;
            }
        }

        public void FromRun(Run run)
        {
            Game = run.Game;
            Category = run.Category;
            Runners = run.Runners;
            Platform = run.Platform;
            Estimate = run.Estimate;
            Time = run.Time;

            if (curRun != null)
                curRun.PropertyChanged -= Run_PropertyChanged;
            curRun = run;
            curRun.PropertyChanged += Run_PropertyChanged;
        }

        private void Run_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Run run = (Run)sender;

            switch (e.PropertyName)
            {
                case "Game":
                    Game = run.Game;
                    break;
                case "Category":
                    Category = run.Category;
                    break;
                case "Runners":
                    Runners = run.Runners;
                    break;
                case "Platform":
                    Platform = run.Platform;
                    break;
                case "Estimate":
                    Estimate = run.Estimate;
                    break;
                case "Time":
                    Time = run.Time;
                    break;
                default:
                    break;
            }
        }

        public Run ToRun()
        {
            if (curRun != null)
                return curRun;

            return new Run
            {
                Game = Game,
                Category = Category,
                Runners = Runners,
                Platform = Platform,
                Estimate = Estimate,
                Time = Time
            };
        }
    }
}
