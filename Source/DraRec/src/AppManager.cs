/*
 * Author : Brian Tu (RTU)
 * Last modify : -
 * 
 * This file contains the AppManager reading the state of other apps. */

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DRnamespace
{
    public class AppManager
    {
        private Process[] ProcessList;
        private int process_id = 0;
        private StringBuilder TargetName = new StringBuilder();
        private uint TargetId = 0;
        private bool isLocking = true;
        private IntPtr hw;

        NotifyIcon notify;

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

        [DllImport("User32.dll")]
        private static extern uint GetWindowModuleFileNameA(IntPtr hwnd, char[] pszFileName,uint cchFileNameMax);

        public AppManager(bool locking, NotifyIcon ntf)
        {
            isLocking = locking;
            notify = ntf;
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
            return TargetName.ToString();
        }

        public bool IsTargetActive()
        {
            return !isLocking || TargetName.ToString() == ActiveWindow();
        }

        public void SetWindowsToForground()
        {
            SetForegroundWindow(Process.GetCurrentProcess().Handle);
        }

        public void SetTargetToNextWindow()
        {
            try
            {
                hw = GetWindow(GetActiveWindow(), 2);
                GetWindowThreadProcessId(hw, out TargetId);
                Process p = Process.GetProcessById((int)TargetId);
                TargetName.Clear();
                TargetName.Append(p.ProcessName);
            }
            catch (Exception e)
            {
                notify.ShowBalloonTip(1000, "", "Error 001 : "+e.Message, ToolTipIcon.Info);
            }
        }

        public string ActiveWindow()
        {
            string name = "";
            try
            {
                IntPtr hw = GetForegroundWindow();
                GetWindowThreadProcessId(hw, out uint id);
                name = Process.GetProcessById((int)id).ProcessName;
            }
            catch (Exception e)
            {
                notify.ShowBalloonTip(1000, "", "Error 002 : " + e.Message, ToolTipIcon.Info);
            }

            return name;
        }

        public bool IsTargetAvalible()
        {
            return TargetId != 0 && TargetId != Process.GetCurrentProcess().Id;
        }

        public String NextProcess()
        {
            ProcessList = Process.GetProcesses();
            process_id = process_id % ProcessList.Length;
            return ProcessList[process_id++].ProcessName;
        }
    }
}