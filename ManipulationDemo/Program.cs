using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ManipulationDemo
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 创建启动快捷方式。
            CreateStartupShorcut();

            // 启动前一个进程实例。
            try
            {
                var hwnd = FindWindow(null, "触摸以监视");
                ShowWindow(hwnd, 9);
            }
            catch (Exception)
            {
            }

            // 启动自己。
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        /// <summary>
        /// 程序启动时在启动文件夹创建快捷方式。
        /// </summary>
        private static void CreateStartupShorcut()
        {
            // 定位启动快捷方式的路径。
            var startupPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                "ManipulationDemo.lnk");

            // 防止本程序的运行路径改变，所以每次启动都重写快捷方式。
            if (File.Exists(startupPath))
            {
                File.Delete(startupPath);
            }

            // 创建快捷方式。
            CreateShortcut(startupPath, "--startup");
        }

        /// <summary>
        /// 为本程序创建一个快捷方式。
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        private static void CreateShortcut(string lnkFilePath, string args)
        {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            var shortcut = shell.CreateShortcut(lnkFilePath);
            shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
            shortcut.Arguments = args;
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            shortcut.Save();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);
    }
}
