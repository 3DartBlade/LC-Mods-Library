using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraPlus
{
    internal class StartMsg
    {
        internal static void Msg()
        {
            CLog.L("");
            CLog.L("================== Keybinds ==================");
            CLog.L("Note: You should unbind these");
            CLog.L("");
            CLog.L("Right click     | zoom");
            CLog.L("Scroll (Zoom)   | adjust zoom amount");
            CLog.L("================== Keybinds =================");
            CLog.L("");
        }
    }
}
