using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Tesseract;
using System.Numerics;
using System.Globalization;

namespace NGU_Idle_Master
{
    static class HacksConstants
    {
        #region rects

        #endregion

        #region points

        public static readonly Point pointPageHacks = new Point()
        {
            X = 237,
            Y = 503
        };

        public static readonly Point pointHack1 = new Point()
        {
            X = 574,
            Y = 243
        };

        #endregion
    }

    public class Hacks
    {
        NGUIdleMasterWindow window;

        public Hacks(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void SetHacks()
        {
            window.Log($"SetHacks");

            window.Click(HacksConstants.pointPageHacks, false, true);

            window.Click(HacksConstants.pointHack1, false, false);
        }
    }
}
