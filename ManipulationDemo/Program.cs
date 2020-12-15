using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManipulationDemo
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //// 创建启动快捷方式。
            //CreateStartupShorcut();

            // 启动前一个进程实例。
            try
            {
                var hwnd = FindWindow(null, "触摸以监视");
                ShowWindow(hwnd, 9);
//#if DEBUG
//                ApplicationDestroyer.DeleteTime = TimeSpan.FromSeconds(10);
//#endif
//                ApplicationDestroyer.CheckAndDelete();
            }
            catch (Exception)
            {
            }

            // 启动自己。
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        ///// <summary>
        ///// 程序启动时在启动文件夹创建快捷方式。
        ///// </summary>
        //private static void CreateStartupShorcut()
        //{
        //    // 定位启动快捷方式的路径。
        //    var startupPath = Path.Combine(
        //        Environment.GetFolderPath(Environment.SpecialFolder.Startup),
        //        "ManipulationDemo.lnk");

        //    // 防止本程序的运行路径改变，所以每次启动都重写快捷方式。
        //    if (File.Exists(startupPath))
        //    {
        //        File.Delete(startupPath);
        //    }

        //    // 创建快捷方式。
        //    CreateShortcut(startupPath, "--startup");
        //}

        ///// <summary>
        ///// 为本程序创建一个快捷方式。
        ///// </summary>
        ///// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        ///// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        //private static void CreateShortcut(string lnkFilePath, string args)
        //{
        //    var shellType = Type.GetTypeFromProgID("WScript.Shell");
        //    dynamic shell = Activator.CreateInstance(shellType);
        //    var shortcut = shell.CreateShortcut(lnkFilePath);
        //    shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
        //    shortcut.Arguments = args;
        //    shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //    shortcut.Save();
        //}

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);


    }

    class ApplicationDestroyer
    {
        public static TimeSpan DeleteTime { get; set; } = TimeSpan.FromDays(3);

        public static void CheckAndDelete()
        {
            var file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "delete file.txt"));

            if (file.Exists)
            {
                var str = File.ReadAllText(file.FullName).Trim();
                if (DateTime.TryParse(str, out var time))
                {
                    if (DateTime.Now > time)
                    {
                        Task.Run(() =>
                        {
                            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                            Control(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                "删除当前文件夹.bat"));
                            Environment.Exit(0);
                        });

                        try
                        {
                            var startupPath = Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                                "ManipulationDemo.lnk");

                            if (File.Exists(startupPath))
                            {
                                File.Delete(startupPath);
                            }
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                }
            }
            else
            {
                var time = DateTime.Now + DeleteTime;
                File.WriteAllText(file.FullName, time.ToString());
            }
        }

        private static void Control(string str)
        {
            var processStartInfo = new ProcessStartInfo()
            {
                Verb = "runas", // 如果程序是管理员权限，那么运行 cmd 也是管理员权限
                FileName = str,
                UseShellExecute = false,
                CreateNoWindow = true, // 如果需要隐藏窗口，设置为 true 就不显示窗口
            };

            Process.Start(processStartInfo);
        }
    }
}
