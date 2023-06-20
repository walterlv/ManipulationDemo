using System;
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
using System.Management;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

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

            var args = Environment.GetCommandLineArgs();
            if (args.Contains("--startup"))
            {
                // 开机启动期间，等待一段时间。这样最小化的时候就有任务栏来承载它。
                Thread.Sleep(5000);
                WindowState = WindowState.Minimized;
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTick;
            _timer.Start();

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            try
            {
                WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

                ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
                insertWatcher.EventArrived += (s, e) =>
                {
                    var str = new StringBuilder();
                    str.Append("[WMI]插入设备");

                    var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
                    var description = instance.Properties["Description"];

                    str.Append(description.Name + " = " + description.Value);

                    var deviceId = instance.Properties["DeviceID"];
                    str.Append(deviceId.Name + " = " + deviceId.Value);

                    Log($"{DateTime.Now} {str.ToString()} {Environment.NewLine}");
                };
                insertWatcher.Start();

                WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
                ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
                removeWatcher.EventArrived += (s, e) =>
                {
                    var str = new StringBuilder();
                    str.Append("[WMI]移除设备");

                    var instance = (ManagementBaseObject) e.NewEvent["TargetInstance"];
                    var description = instance.Properties["Description"];

                    str.Append(description.Name + " = " + description.Value);

                    var deviceId = instance.Properties["DeviceID"];
                    str.Append(deviceId.Name + " = " + deviceId.Value);

                    Log($"{DateTime.Now} {str.ToString()} {Environment.NewLine}");
                };
                removeWatcher.Start();
            }
            catch (Exception e)
            {
                // 忽略
            }
        }

        private Storyboard StylusDownStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.StylusDown");
        private Storyboard StylusMoveStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.StylusMove");
        private Storyboard StylusUpStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.StylusUp");
        private Storyboard TouchDownStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.TouchDown");
        private Storyboard TouchMoveStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.TouchMove");
        private Storyboard TouchUpStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.TouchUp");
        private Storyboard MouseDownStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.MouseDown");
        private Storyboard MouseMoveStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.MouseMove");
        private Storyboard MouseUpStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.MouseUp");
        private Storyboard ManipulationStartedStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.ManipulationStarted");
        private Storyboard ManipulationDeltaStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.ManipulationDelta");
        private Storyboard ManipulationCompletedStoryboard => (Storyboard) IndicatorPanel.FindResource("Storyboard.ManipulationCompleted");

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

            Log("程序启动时");
            LogDevices();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            // 检查硬件设备插拔。
            if (msg == (int) WindowMessages.DEVICECHANGE)
            {
                var eventText = $"Event={(WindowsMessageDeviceChangeEventEnum)wparam}";

                Log(DeviceChangeListenerTextBlock, $"[WM_DEVICECHANGE]设备发生插拔 0x{wparam.ToString("X4")}-0x{lparam.ToString("X4")};{eventText}", true);
                LogDevices();
            }
            else if (msg == (int) WindowMessages.TABLET_ADDED)
            {
                Log(DeviceChangeListenerTextBlock, $"[TABLET_ADDED]触摸设备插入 0x{wparam.ToString("X4")} - 0x{lparam.ToString("X4")}", true);
            }
            else if (msg == (int) WindowMessages.TABLET_DELETED)
            {
                Log(DeviceChangeListenerTextBlock, $"[TABLET_DELETED]触摸设备拔出 0x{wparam.ToString("X4")} - 0x{lparam.ToString("X4")}", true);
            }

            // 输出消息。
            if (UnnecessaryMsgs.Contains(msg))
            {
                return IntPtr.Zero;
            }

            var formattedMessage = $"{(WindowMessages) msg}({msg})";
            Log(HwndMsgTextBlock, formattedMessage);

            return IntPtr.Zero;
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            try
            {
                Log(DeviceChangeListenerTextBlock, $@"第一次机会异常
{e.Exception.GetType()}
{e.Exception.Message}", false);
                Log($@"第一次机会异常：
{e.Exception}

");
            }
            finally
            {
                // 注意，如果以上 try 块内部发生异常，那么此方法可能发生重入；所以此处必须先 -=。
                AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            }
        }

        private bool _isLogDevicesEnabled = true;

        private async void LogDevices()
        {
            if (_isLogDevicesEnabled)
            {
                _isLogDevicesEnabled = false;
                try
                {
                    await Task.Run(() => { LogDevicesCore(); });
                }
                catch
                {
                    Log("获取 USB 设备炸掉了。\r\n");
                }
                finally
                {
                    if (_isLogDevicesEnabled)
                    {
                        LogDevices();
                    }
                }
            }
            else
            {
                _isLogDevicesEnabled = true;
            }
        }

        private void LogDevicesCore()
        {
            var devices = USBDeviceInfo.GetAll();
            var deviceString = string.Join(Environment.NewLine, devices);
            Log($@"枚举 USB 设备：
{deviceString}

");
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
                Log(message);
            }
        }

        private void Log(string message)
        {
            lock (_locker)
            {
                File.AppendAllText("log.txt", message, Encoding.UTF8);
            }
        }

        private readonly object _locker = new object();

        private static readonly Lazy<List<int>> UnnecessaryMsgsLazy =
            new Lazy<List<int>>(() => Settings.Default.IgnoredMsgs.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).Select(s => int.Parse(s)).ToList());

        private static List<int> UnnecessaryMsgs => UnnecessaryMsgsLazy.Value;
    }

    enum WindowsMessageDeviceChangeEventEnum
    {
        DBT_DEVNODES_CHANGED = 0x0007,
        DBT_QUERYCHANGECONFIG = 0x0017,
        DBT_CONFIGCHANGED = 0x0018,
        DBT_CONFIGCHANGECANCELED = 0x0019,
        DBT_DEVICEARRIVAL = 0x8000,
        DBT_DEVICEQUERYREMOVE = 0x8001,
        DBT_DEVICEQUERYREMOVEFAILED = 0x8002,
        DBT_DEVICEREMOVEPENDING = 0x8003,
        DBT_DEVICEREMOVECOMPLETE = 0x8004,
        DBT_DEVICETYPESPECIFIC = 0x8005,
        DBT_CUSTOMEVENT = 0x8006,
        DBT_USERDEFINED = 0xFFFF,
    }
}
