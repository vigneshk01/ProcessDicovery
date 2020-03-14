using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ProcessDiscovery
{

    public class class1
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private static uint processID = 0;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 23;
        //private static readonly string dbConStr = ConfigurationManager.ConnectionStrings["ProcessDiscoveryDB"].ConnectionString;
        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);
        private static Dictionary<string, Tuple<double, string, string, int>> applhashdict;
        //private static bool isNewAppl;
        private static string prevValue = null;

        private static string GetActiveWindowTitle()
        {
            try
            {

                IntPtr handle = GetForegroundWindow();
                int nChars = GetWindowTextLength(handle) + 1;
                StringBuilder Buff = new StringBuilder(nChars);

                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    GetWindowThreadProcessId(handle, out processID);
                    Process processName = Process.GetProcessById(Convert.ToInt32(processID));
                    return Buff.ToString();

                }
                else
                {
                    IntPtr parentHandle = GetParent(handle);
                    int nPChars = GetWindowTextLength(parentHandle) + 1;
                    StringBuilder PBuff = new StringBuilder(nChars);
                    if (GetWindowText(handle, PBuff, nPChars) > 0)

                    {
                        GetWindowThreadProcessId(handle, out processID);
                        Process processName = Process.GetProcessById(Convert.ToInt32(processID));
                        return PBuff.ToString();
                    }
                    else
                    {
                        GetWindowThreadProcessId(handle, out processID);
                        Process processName = Process.GetProcessById(Convert.ToInt32(processID));
                        return "PROCESS NAME" + processName.ProcessName;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "Null";
            }
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string ActiveWindowName = GetActiveWindowTitle();
            if (prevValue != ActiveWindowName)
            {
                string activatedTime = DateTime.Now.ToString();
                Console.WriteLine("Foreground changed to " + "---" + ActiveWindowName + " ---- " + activatedTime + " ----- " + processID);
                if (applhashdict.ContainsKey(ActiveWindowName))
                {
                    applhashdict.Remove(ActiveWindowName);
                }
                applhashdict.Add(ActiveWindowName, new Tuple<double, string, string, int>(2, activatedTime, activatedTime, 1));
                prevValue = ActiveWindowName;
                //isNewAppl = true;
            }
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                applhashdict = new Dictionary<string, Tuple<double, string, string, int>>();
                //Debug.WriteLine(GetActiveWindowTitle()); 
                IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
                MessageBox.Show("Tracking focus, close message box to exit.");
                UnhookWinEvent(m_hhook);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }                       
        }
    }
}