using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMVVM.Threading
{
    internal static class NativeMethods
    {
        internal const uint MWMO_INPUTAVAILABLE = 0x0004;

        internal const uint
            QS_KEY = 0x0001,
            QS_MOUSEMOVE = 0x0002,
            QS_MOUSEBUTTON = 0x0004,
            QS_POSTMESSAGE = 0x0008,
            QS_TIMER = 0x0010,
            QS_PAINT = 0x0020,
            QS_SENDMESSAGE = 0x0040,
            QS_HOTKEY = 0x0080,
            QS_ALLPOSTMESSAGE = 0x0100,
            QS_MOUSE = QS_MOUSEMOVE | QS_MOUSEBUTTON,
            QS_INPUT = QS_MOUSE | QS_KEY,
            QS_ALLEVENTS = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY,
            QS_ALLINPUT = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE,
            QS_EVENT = 0x2000;

        [DllImport("user32.dll")]
        internal static extern uint GetQueueStatus(uint flags);
    }
}
