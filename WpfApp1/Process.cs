using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("User32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetProcessNameOfNextWindows()
        {
            uint id = 0;
            IntPtr hw = GetWindow(GetActiveWindow(), 2);
            GetWindowThreadProcessId(hw, out id);
            Process p = Process.GetProcessById((int)id);
            return p.ProcessName;
        }

        private string GetProcessNameOfForegroundWindows()
        {
            uint id = 0;
            IntPtr hw = GetForegroundWindow();
            GetWindowThreadProcessId(hw, out id);
            Process p = Process.GetProcessById((int)id);
            return p.ProcessName;
        }
    }
}