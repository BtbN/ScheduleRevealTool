﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        private Run curRun = null;

        public void Clear()
        {
            game = "";
            category = "";
            runners = "";
            platform = "";
            estimate = "";
            time = "";

            TitleText.Clear();
            GameNameText.Clear();
            GameMetaText.Clear();

            if (curRun != null)
            {
                WeakEventManager<Run, PropertyChangedEventArgs>.RemoveHandler(curRun, "PropertyChanged", Run_PropertyChanged);
                curRun = null;
            }
        }

        public void FromRun(Run run)
        {
            Clear();

            if (run == null)
                return;

            curRun = run;

            game = run.Game;
            category = run.Category;
            runners = run.Runners;
            platform = run.Platform;
            estimate = run.Estimate;
            time = run.Time;

            UpdateTexts();

            WeakEventManager<Run, PropertyChangedEventArgs>.AddHandler(curRun, "PropertyChanged", Run_PropertyChanged);
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
                case "Presented":
                    if (!run.Presented)
                        Unpresent();
                    break;
                default:
                    break;
            }
        }

        private void Unpresent()
        {
            Panel parent = Parent as Panel;
            if (parent == null)
                return;

            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250));
            fadeOut.Completed += (s, e) => parent.Children.Remove(this);
            BeginAnimation(OpacityProperty, fadeOut);
        }

        public Run ToRun()
        {
            if (curRun != null)
                return curRun;

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
            TitleText.Text = time;
            GameNameText.Text = game;
            GameMetaText.Text = category + " / " + platform + " / " + runners + " / EST: " + estimate;
        }
    }
}
