using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;

namespace ScheduleRevealTool
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private readonly MainWindow mainWindow;

        private readonly DispatcherTimer timer = new DispatcherTimer();

        public ControlWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            timer.Tick += Timer_Tick;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public ObservableCollection<Run> RunCollection { get; set; } = new ObservableCollection<Run>();

        private void LoadSchedule_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Tab Seperated Values|*.tsv"
            };

            bool? res = dialog.ShowDialog();

            if (res != true)
                return;

            mainWindow.Clear();
            RunCollection.Clear();

            IEnumerable<string> lines = File.ReadAllLines(dialog.FileName);

            // Skip Header Line
            lines = lines.Skip(1);

            foreach (string line in lines)
            {
                string[] elements = line.Split('\t');

                if (elements.Length != 7)
                {
                    MessageBox.Show("Invalid data line in run tsv.");
                    RunCollection.Clear();
                    return;
                }

                Run run = new Run()
                {
                    Time = elements[0].Trim(),
                    Runners = elements[1].Trim(),
                    Game = elements[2].Trim(),
                    Category = elements[3].Trim(),
                    Estimate = elements[5].Trim(),
                    Platform = elements[6].Trim(),
                    Presented = false
                };

                if (elements[4].Trim() != "")
                    run.Category += " (" + elements[4].Trim() + ")";

                if (run.Game == "")
                    continue;

                RunCollection.Add(run);

                Console.WriteLine(run.Game);
            }
        }

        private void PresentNextRun_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                timer.Start();
            }

            RevealNextRun();
        }

        private void PreviewIntegerEnforce(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            e.Handled = !int.TryParse(tb.Text + e.Text, out int _);
        }

        private void PreviewDoubleEnforce(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            e.Handled = !double.TryParse(tb.Text + e.Text, out double _);
        }

        private void TimerInputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimerStatusLabel == null)
                return;

            string txt = TimerInputText.Text;

            if (txt == "" || txt == "0" || !int.TryParse(txt, out int interval))
            {
                timer.Stop();
                TimerStatusLabel.Content = "Stopped";
            }
            else
            {
                TimerStatusLabel.Content = "Running every " + interval + "s";
                timer.Stop();
                timer.Interval = TimeSpan.FromSeconds(interval);
                timer.Start();
            }
        }

        private double SpeedMul { get; set; } = 1.0;

        private void SpeedInputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(SpeedInputText.Text, out double speed))
                SpeedMul = speed;
            else
                SpeedMul = 1.0;

            // Safety, someone puts in 100, and it will take forever to be ready again.
            if (SpeedMul > 2.0)
                SpeedMul = 2.0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RevealNextRun();
        }

        private void RevealNextRun()
        {
            foreach (Run run in RunCollection)
            {
                if (!run.Presented)
                {
                    if (mainWindow.RevealNextRun(run, SpeedMul))
                        run.Presented = true;
                    return;
                }
            }

            mainWindow.RevealNextRun(null, SpeedMul);
            TimerStatusLabel.Content = "Finished";
            timer.Stop();
        }

        private void OmnibarTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                mainWindow.UpdateOmniBar(OmnibarTextBox.Text);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Really Exit?", "Exit?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void ScrollOverList_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ScrollDurationText.Text, out double duration))
                duration = 120.0;

            mainWindow.ScrollOverList(duration, 5.0);
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            TimerStatusLabel.Content = "Stopped";
        }

        private void StartCountdown_Click(object sender, RoutedEventArgs e)
        {
            double v = 0.0;
            if (CountdownTimerText.Text != "")
                v = double.Parse(CountdownTimerText.Text);

            mainWindow.SetCountdown(v);
        }
    }
}
