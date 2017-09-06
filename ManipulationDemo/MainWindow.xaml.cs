using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
    }
}
