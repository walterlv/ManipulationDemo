using System;
using System.Diagnostics;
using System.IO;
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
            try
            {
                var hwnd = FindWindow(null, "触摸以监视");
                ShowWindow(hwnd, 9);
                Task.Run(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                    Control(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                        "删除当前文件夹.bat"));
                    Environment.Exit(0);
                });
            }
            catch (Exception)
            {
            }



            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);

        private static void Control(string str)
        {
            var processStartInfo = new ProcessStartInfo()
            {
                Verb = "runas", // 如果程序是管理员权限，那么运行 cmd 也是管理员权限
                FileName = str,
                UseShellExecute = false,
                CreateNoWindow = true, // 如果需要隐藏窗口，设置为 true 就不显示窗口
                //Arguments = "/c " + str //+ " &exit",
            };

            Process.Start(processStartInfo);
        }
    }


}
