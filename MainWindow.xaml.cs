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

namespace ScheduleRevealTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NextRunControl.Opacity = 0.0;
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
                Delay(500, () => {
                    DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2000 * speedMul));
                    ctrl.BeginAnimation(OpacityProperty, fadeIn);
                });
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

            AddRunToList(NextRunControl.ToRun(), speedMul);

            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(2000 * speedMul));
            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(2000 * speedMul));
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

                    Delay(2500 * speedMul, () =>
                    {
                        NextRunControl.BeginAnimation(OpacityProperty, fadeIn);
                        inProgress = false;
                    });
                }
            };

            Delay(2000 * speedMul, () =>
            {
                NextRunControl.BeginAnimation(OpacityProperty, fadeOut);
            });

            return true;
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

        internal void UpdateOmniBar(string text, double speedMul = 1.0)
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
    }
}
