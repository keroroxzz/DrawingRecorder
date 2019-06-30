using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    public class AppManager
    {
        private string TargetName = "";
        private uint TargetId = 0;
        private bool isLocking = true;
        private IntPtr hw;

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern IntPtr ShowWindow(IntPtr hWnd,int nCmdShow);

        [DllImport("User32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("User32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public AppManager(bool locking)
        {
            isLocking = locking;
        }

        public void ActiveLock()
        {
            isLocking = true;
        }

        public void Unlock()
        {
            isLocking = false;
        }

        public bool Lock()
        {
            return isLocking;
        }

        public string Target()
        {
            return TargetName;
        }

        public bool IsTargetActive()
        {
            return !isLocking || TargetName == ActiveWindow();
        }

        public void SetWindowsToForground()
        {
            SetForegroundWindow(Process.GetCurrentProcess().Handle);
        }

        public void SetTargetToNextWindow()
        {
            hw = GetWindow(GetActiveWindow(), 2);
            GetWindowThreadProcessId(hw, out TargetId);
            Process p = Process.GetProcessById((int)TargetId);
            TargetName = p.ProcessName;

        }

        public string ActiveWindow()
        {
            uint id = 0;
            IntPtr hw = GetForegroundWindow();
            GetWindowThreadProcessId(hw, out id);
            Process p = Process.GetProcessById((int)id);
            return p.ProcessName;
        }

        public bool IsTargetAvalible()
        {
            return TargetId != 0 && TargetId != Process.GetCurrentProcess().Id;
        }
    }
}