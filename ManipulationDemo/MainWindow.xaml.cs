using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ManipulationDemo.Properties;

namespace ManipulationDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.RemoveIcon();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private Storyboard StylusDownStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.StylusDown");
        private Storyboard StylusMoveStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.StylusMove");
        private Storyboard StylusUpStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.StylusUp");
        private Storyboard TouchDownStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.TouchDown");
        private Storyboard TouchMoveStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.TouchMove");
        private Storyboard TouchUpStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.TouchUp");
        private Storyboard MouseDownStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.MouseDown");
        private Storyboard MouseMoveStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.MouseMove");
        private Storyboard MouseUpStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.MouseUp");
        private Storyboard ManipulationStartedStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.ManipulationStarted");
        private Storyboard ManipulationDeltaStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.ManipulationDelta");
        private Storyboard ManipulationCompletedStoryboard => (Storyboard)IndicatorPanel.FindResource("Storyboard.ManipulationCompleted");

        private readonly DispatcherTimer _timer;

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                var value = typeof(InputManager).Assembly
                    .TypeOf("StylusLogic")
                    .Get<bool>("IsStylusAndTouchSupportEnabled");
                IsStylusAndTouchSupportEnabledRun.Text = value.ToString();
            }
            catch (Exception ex)
            {
                IsStylusAndTouchSupportEnabledRun.Text = ex.ToString();
            }
            try
            {
                PimcManagerTabletCountRun.Text = new TouchTabletCollection().Count.ToString();
            }
            catch (Exception ex)
            {
                PimcManagerTabletCountRun.Text = ex.ToString();
            }
        }

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            StylusDownStoryboard.Begin();
        }

        private void OnStylusMove(object sender, StylusEventArgs e)
        {
            StylusMoveStoryboard.Begin();
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            StylusUpStoryboard.Begin();
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            TouchDownStoryboard.Begin();
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            TouchMoveStoryboard.Begin();
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            TouchUpStoryboard.Begin();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownStoryboard.Begin();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveStoryboard.Begin();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseUpStoryboard.Begin();
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ManipulationStartedStoryboard.Begin();
        }

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            ManipulationStartedStoryboard.Begin();
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            ManipulationDeltaStoryboard.Begin();
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ManipulationCompletedStoryboard.Begin();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = (HwndSource) PresentationSource.FromVisual(this);
            source?.AddHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (UnnecessaryMsgs.Contains(msg))
            {
                return IntPtr.Zero;
            }
            HwndMsgTextBlock.Text += $"{(WindowMessages) msg}{Environment.NewLine}";
            return IntPtr.Zero;
        }

        private static readonly Lazy<List<int>> UnnecessaryMsgsLazy =
            new Lazy<List<int>>(() => Settings.Default.IgnoredMsgs.Split(',').Select(int.Parse).ToList());

        private static List<int> UnnecessaryMsgs => UnnecessaryMsgsLazy.Value;
    }
}
