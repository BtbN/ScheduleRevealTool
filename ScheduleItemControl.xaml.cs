using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScheduleRevealTool
{
    public partial class ScheduleItemControl : UserControl
    {
        public ScheduleItemControl()
        {
            InitializeComponent();
        }

        private string game;
        public string Game
        {
            get => game;
            set
            {
                game = value;
                UpdateTexts();
            }
        }

        private string category;
        public string Category
        {
            get => category;
            set
            {
                category = value;
                UpdateTexts();
            }
        }

        private string runners;
        public string Runners
        {
            get => runners;
            set
            {
                runners = value;
                UpdateTexts();
            }
        }

        private string estimate;
        public string Estimate
        {
            get => estimate;
            set
            {
                estimate = value;
                UpdateTexts();
            }
        }

        private string platform;
        public string Platform
        {
            get => platform;
            set
            {
                platform = value;
                UpdateTexts();
            }
        }

        private string time;
        public string Time
        {
            get => time;
            set
            {
                time = value;
                UpdateTexts();
            }
        }

        public void FromRun(Run run)
        {
            game = run.Game;
            category = run.Category;
            runners = run.Runners;
            platform = run.Platform;
            estimate = run.Estimate;
            time = run.Time;

            UpdateTexts();
        }

        public Run ToRun()
        {
            return new Run
            {
                Game = game,
                Category = category,
                Runners = runners,
                Platform = platform,
                Estimate = estimate,
                Time = time
            };
        }

        private void UpdateTexts()
        {
            TitleLabel.Content = time;
            ContentLabelUpper.Content = game;
            ContentLabelLower.Text = category + " / " + platform + " / " + runners + " / EST: " + estimate;
        }
    }
}
