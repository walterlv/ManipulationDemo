using System;
using System.Runtime.InteropServices;
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
    }
}
