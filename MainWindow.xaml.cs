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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScheduleRevealTool
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer countdownTimer;
        private DateTime countdownEnd = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();

            NextRunControl.Opacity = 0.0;
            CountdownViewBox.Opacity = 0.0;

            countdownTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(100),
                DispatcherPriority.Render,
                CountdownTimer_Tick,
                Application.Current.Dispatcher);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Delay(TimeSpan delay, Action action)
        {
            Task.Delay(delay).ContinueWith(t => Dispatcher.Invoke(action));
        }

        private void Delay(double delayMs, Action action)
        {
            Delay(TimeSpan.FromMilliseconds(delayMs), action);
        }

        private static readonly DependencyProperty ScrollOffsetProperty
            = DependencyProperty.Register(
                "ScrollOffset",
                typeof(double),
                typeof(MainWindow),
                new FrameworkPropertyMetadata(
                    0.0,
                    new PropertyChangedCallback(OnScrollOffsetChanged)));

        public double ScrollOffset
        {
            get { return (double)GetValue(ScrollOffsetProperty); }
            set { SetValue(ScrollOffsetProperty, value); }
        }

        private static void OnScrollOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MainWindow win = obj as MainWindow;
            if (win == null)
                return;

            win.MainCardsScroll.ScrollToVerticalOffset((double)args.NewValue);
        }

        public void ScrollOverList(double durationSec = 30.0, double pauseSec = 5.0)
        {
            DoubleAnimation scrollUp = new DoubleAnimation(MainCardsScroll.VerticalOffset, 0.0, TimeSpan.FromSeconds(durationSec / 2));
            scrollUp.Completed += (s, e) =>
            {
                Delay(pauseSec * 1000, () =>
                {
                    DoubleAnimation scrollDown = new DoubleAnimation(0.0, MainCardsScroll.ScrollableHeight, TimeSpan.FromSeconds(durationSec / 2));
                    BeginAnimation(ScrollOffsetProperty, scrollDown);
                });
            };

            BeginAnimation(ScrollOffsetProperty, scrollUp);
        }

        public void AddRunToList(Run run, double speedMul = 1.0)
        {
            if (speedMul < 0.01)
                speedMul = 0.01;

            if (run == null || run.Game == "")
                return;

            ScheduleItemControl ctrl = new ScheduleItemControl();
            ctrl.FromRun(NextRunControl.ToRun());

            var margin = ctrl.Margin;
            margin.Bottom = 5;
            ctrl.Margin = margin;

            ctrl.Opacity = 0.0;

            MainCardsStack.Children.Add(ctrl);
            UpdateLayout();

            DoubleAnimation scrollDown = new DoubleAnimation(MainCardsScroll.VerticalOffset, MainCardsScroll.ScrollableHeight, TimeSpan.FromMilliseconds(2000 * speedMul));
            scrollDown.EasingFunction = new BounceEase { EasingMode = EasingMode.EaseOut };
            scrollDown.Completed += (s, e) =>
            {
                DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2000 * speedMul));
                fadeIn.BeginTime = TimeSpan.FromMilliseconds(500 * speedMul);
                ctrl.BeginAnimation(OpacityProperty, fadeIn);
            };

            BeginAnimation(ScrollOffsetProperty, scrollDown);
        }

        bool inProgress = false;

        public bool RevealNextRun(Run run, double speedMul = 1.0)
        {
            if (speedMul < 0.01)
                speedMul = 0.01;

            if (inProgress)
                return false;
            inProgress = true;

            Run prevRun = NextRunControl.ToRun();
            if (prevRun != null && prevRun.Game != "")
            {
                prevRun.PropertyChanged -= CurRunPropChanged;
                AddRunToList(prevRun, speedMul);
            }

            if (run != null && run.Game != "")
                run.PropertyChanged += CurRunPropChanged;

            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(2000 * speedMul));
            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2000 * speedMul));
            fadeIn.BeginTime = TimeSpan.FromMilliseconds(2500 * speedMul);
            fadeOut.BeginTime = TimeSpan.FromMilliseconds(2000 * speedMul);
            fadeOut.Completed += (s, e) =>
            {
                if (run == null || run.Game == "")
                {
                    inProgress = false;
                    NextRunControl.Clear();
                }
                else
                {
                    NextRunControl.FromRun(run);

                    NextRunControl.BeginAnimation(OpacityProperty, fadeIn);
                    inProgress = false;
                }
            };

            NextRunControl.BeginAnimation(OpacityProperty, fadeOut);

            return true;
        }

        private void CurRunPropChanged(object sender, PropertyChangedEventArgs _)
        {
            Run run = (Run)sender;
            if (!run.Presented)
            {
                run.PropertyChanged -= CurRunPropChanged;

                inProgress = true;

                DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(250));
                fadeOut.Completed += (s, e) =>
                {
                    inProgress = false;
                    NextRunControl.Clear();
                };
                NextRunControl.BeginAnimation(OpacityProperty, fadeOut);
            }
        }

        public void Clear()
        {
            inProgress = true;

            DoubleAnimation fadeOutRun = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000));
            DoubleAnimation fadeOutList = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000));

            fadeOutRun.Completed += (s, e) =>
            {
                NextRunControl.Clear();
                inProgress = false;
            };

            fadeOutList.Completed += (s, e) =>
            {
                MainCardsStack.Children.Clear();
                UpdateLayout();
                MainCardsScroll.ScrollToTop();

                MainCardsScroll.BeginAnimation(OpacityProperty, null);
                MainCardsScroll.Opacity = 1.0;
            };

            NextRunControl.BeginAnimation(OpacityProperty, fadeOutRun);
            MainCardsScroll.BeginAnimation(OpacityProperty, fadeOutList);
        }

        public void UpdateOmniBar(string text, double speedMul = 1.0)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(2500 * speedMul));
            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2500 * speedMul));
            fadeOut.Completed += (s, e) =>
            {
                OmnibarTextBox.Text = text;

                Delay(2500 * speedMul, () => {
                    OmnibarTextBox.BeginAnimation(OpacityProperty, fadeIn);
                });
            };
            OmnibarTextBox.BeginAnimation(OpacityProperty, fadeOut);
        }

        public void SetCountdown(double seconds)
        {
            countdownTimer.Stop();

            countdownEnd = DateTime.Now.AddSeconds(seconds);
            CountdownTimer_Tick(null, null);

            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2500));
            fadeIn.Completed += (s, e) =>
            {
                countdownEnd = DateTime.Now.AddSeconds(seconds);
                countdownTimer.Start();
            };

            CountdownViewBox.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan diff = countdownEnd - DateTime.Now;

            if (diff.TotalMilliseconds <= 0)
            {
                countdownTimer.Stop();

                DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(2500));
                CountdownViewBox.BeginAnimation(OpacityProperty, fadeOut);

                return;
            }

            CountdownTextBox.Text = Math.Floor(diff.TotalMinutes).ToString("00") + ":" + diff.Seconds.ToString("00");
        }
    }
}
