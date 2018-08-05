using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ManipulationDemo
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //var current = Process.GetCurrentProcess();
                //var processes = Process.GetProcessesByName(current.ProcessName);
                //processes = processes.Where(x => x.Id != current.Id).ToArray();
                //foreach (var process in processes)
                //{
                //    var hwnd = process.MainWindowHandle;
                //    //
                //    ForceWindowToForeground(hwnd);
                //    // SetWindowLong hwnd , GWL_STYLE -16 , WS_MAXIMIZE 0x01000000L
                //}

                var hwnd = FindWindow(null, "触摸以监视");
                ShowWindow(hwnd, 9);
                //SetForegroundWindow(hwnd);
                //SetWindowLong(hwnd, -16, 0x01000000);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }


        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);

        /// <summary>
        /// 窗口放在最前
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        private static void ForceWindowToForeground(IntPtr hWnd)
        {
            const uint SW_SHOW = 9;
            BringWindowToTop(hWnd);
            ShowWindow(hWnd, SW_SHOW);
        }

        /// <summary>
        /// 改变指定窗口的属性
        /// </summary>
        /// <param name="window">窗口句柄</param>
        /// <param name="index">指定将设定的大于等于0的偏移值。有效值的范围从0到额外类的存储空间的字节数减4：例如若指定了12或多于12个字节的额外窗口存储空间，则应设索引位8来访问第三个4字节，同样设置0访问第一个4字节，4访问第二个4字节。要设置其他任何值，可以指定下面值之一
        /// <para>
        /// GWL_EXSTYLE             -20    设定一个新的扩展风格。 </para>
        /// <para>GWL_HINSTANCE     -6     设置一个新的应用程序实例句柄。</para>
        /// <para>GWL_ID            -12    设置一个新的窗口标识符。</para>
        /// <para>GWL_STYLE         -16    设定一个新的窗口风格。</para>
        /// <para>GWL_USERDATA      -21    设置与窗口有关的32位值。每个窗口均有一个由创建该窗口的应用程序使用的32位值。</para>
        /// <para>GWL_WNDPROC       -4    为窗口设定一个新的处理函数。</para>
        /// <para>GWL_HWNDPARENT    -8    改变子窗口的父窗口,应使用SetParent函数</para>
        /// </param>
        /// <param name="value">指定的替换值</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void bringToFront(string title)
        {
            // Get a handle to the Calculator application.
            IntPtr handle = FindWindow(null, title);

            // Verify that Calculator is a running process.
            if (handle == IntPtr.Zero)
            {
                return;
            }

            // Make Calculator the foreground application
            SetForegroundWindow(handle);
        }

    }
}
