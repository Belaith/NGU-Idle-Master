using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace NGU_Idle_Master
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct POWERBROADCAST_SETTING
    {
        public Guid PowerSetting;
        public uint DataLength;
        public byte Data;
    }

    public partial class DummyWindowForPowerBroadcast : Window
    {
        public delegate void SessionSwitchHandler(object sender, Microsoft.Win32.SessionSwitchEventArgs e);
        public event SessionSwitchHandler OnSessionSwitch;

        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport(@"User32", SetLastError = true, EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        public static Guid GUID_CONSOLE_DISPLAY_STATE = new Guid(0x6fe69556, 0x704a, 0x47a0, 0x8f, 0x24, 0xc2, 0x8d, 0x93, 0x6f, 0xda, 0x47);
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        public const int WM_POWERBROADCAST = 0x0218;
        public const int PBT_POWERSETTINGCHANGE = 0x8013;

        private HwndSource _HwndSource;
        private readonly IntPtr _ScreenStateNotify;

        public DummyWindowForPowerBroadcast()
        {
            this.Visibility = Visibility.Collapsed;

            var wih = new WindowInteropHelper(this);
            var hwnd = wih.EnsureHandle();
            _ScreenStateNotify = RegisterPowerSettingNotification(hwnd, ref GUID_CONSOLE_DISPLAY_STATE, DEVICE_NOTIFY_WINDOW_HANDLE);
            _HwndSource = HwndSource.FromHwnd(hwnd);
            _HwndSource.AddHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (OnSessionSwitch == null)
            {
                return IntPtr.Zero;
            }

            // handler of console display state system event 
            if (msg == WM_POWERBROADCAST)
            {
                if (wParam.ToInt32() == PBT_POWERSETTINGCHANGE)
                {
                    var s = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(lParam, typeof(POWERBROADCAST_SETTING));
                    if (s.PowerSetting == GUID_CONSOLE_DISPLAY_STATE)
                    {
                        Microsoft.Win32.SessionSwitchReason reason = new Microsoft.Win32.SessionSwitchReason();

                        switch (s.Data)
                        {
                            case (0x0):
                                reason = Microsoft.Win32.SessionSwitchReason.SessionLock;
                                break;
                            case (0x1):
                            case (0x2):
                                reason = Microsoft.Win32.SessionSwitchReason.SessionUnlock;
                                break;
                        }

                        Microsoft.Win32.SessionSwitchEventArgs args = new Microsoft.Win32.SessionSwitchEventArgs(reason);
                        OnSessionSwitch(this, args);
                    }
                }
            }

            return IntPtr.Zero;
        }

        ~DummyWindowForPowerBroadcast()
        {
            // unregister for console display state system event 
            _HwndSource.RemoveHook(HwndHook);
            UnregisterPowerSettingNotification(_ScreenStateNotify);
        }
    }
}
