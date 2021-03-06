﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
                var collection = new TouchTabletCollection();
                PimcManagerTabletCountRun.Text = collection.Count.ToString();
                TabletDeviceCollectionRun.Text = string.Join($",{Environment.NewLine}",
                    collection.Select(x => $"{x.Name}({(x.IsMultiTouch ? "Multi-" : "")}{x.Kind})"));
            }
            catch (Exception ex)
            {
                PimcManagerTabletCountRun.Text = ex.ToString();
            }
            try
            {
                var builder = new StringBuilder();
                foreach (TabletDevice device in Tablet.TabletDevices)
                {
                    var deviceProperty = typeof(TabletDevice).GetProperty("TabletDeviceImpl",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                    var deviceImpl = deviceProperty is null ? device : deviceProperty.GetValue(device);
                    var info = deviceImpl.GetType().GetProperty("TabletSize",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);

                    var tabletSize = (Size) info.GetValue(deviceImpl, null);
                    if (device.Type == TabletDeviceType.Touch)
                    {
                        builder.Append(string.Format("{1}：{2} 点触摸，精度 {3}{0}", Environment.NewLine,
                            device.Name, device.StylusDevices.Count, tabletSize));
                    }
                    else
                    {
                        builder.Append(string.Format("{1}：{2} 个触笔设备，精度 {3}{0}", Environment.NewLine,
                            device.Name, device.StylusDevices.Count, tabletSize));
                    }
                }
                PhysicalSizeRun.Text = builder.ToString();
            }
            catch (Exception ex)
            {
                PhysicalSizeRun.Text = ex.ToString();
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
            // 检查硬件设备插拔。
            var isDeviceChanged = msg == 537;
            if (isDeviceChanged)
            {
                Log(DeviceChangeListenerTextBlock, $"设备发生插拔 0x{wparam.ToString("X4")} - 0x{lparam.ToString("X4")}", true);
            }

            // 输出消息。
            if (UnnecessaryMsgs.Contains(msg))
            {
                return IntPtr.Zero;
            }

            var formattedMessage = $"{(WindowMessages)msg}";
            Log(HwndMsgTextBlock, formattedMessage);

            return IntPtr.Zero;
        }

        private void Log(TextBlock block, string message, bool toFile = false)
        {
            message = $"{DateTime.Now} {message}{Environment.NewLine}";
            
            if (block.CheckAccess())
            {
                if (block.Text.Length > 2500)
                {
                    block.Text = block.Text.Substring(500);
                }

                block.Text += message;
            }
            else
            {
                block.Dispatcher.InvokeAsync(() => { block.Text += $"async: {message}"; });
            }

            if (toFile)
            {
                lock (_locker)
                {
                    File.AppendAllText("log.txt", message, Encoding.UTF8);
                }
            }
        }

        private readonly object _locker = new object();

        private static readonly Lazy<List<int>> UnnecessaryMsgsLazy =
            new Lazy<List<int>>(() => Settings.Default.IgnoredMsgs.Split(',').Select(int.Parse).ToList());

        private static List<int> UnnecessaryMsgs => UnnecessaryMsgsLazy.Value;
    }
}
