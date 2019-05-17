using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScheduleRevealTool
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private readonly MainWindow mainWindow;

        public ControlWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void AddTestRun_Click(object sender, RoutedEventArgs e)
        {
            Run run = new Run
            {
                Game = "Test Game",
                Category = "Any%",
                Runners = "Someone",
                Platform = "NES",
                Estimate = "00:15:00",
                Time = "19.02.2019 14:12"
            };

            mainWindow.RevealNextRun(run);
        }
    }
}
