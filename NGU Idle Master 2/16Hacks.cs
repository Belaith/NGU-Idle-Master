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

        public static readonly Point pointPageHacks1 = new Point()
        {
            X = 353,
            Y = 192
        };

        public static readonly Point pointPageHacks2 = new Point()
        {
            X = 413,
            Y = 192
        };

        public static readonly Point pointHack1 = new Point()
        {
            X = 574,
            Y = 243
        };

        public static readonly Point pointHack2 = new Point()
        {
            X = 899,
            Y = 243
        };

        public static readonly Point pointHack3 = new Point()
        {
            X = 574,
            Y = 336
        };

        public static readonly Point pointHack4 = new Point()
        {
            X = 899,
            Y = 336
        };

        public static readonly Point pointHack5 = new Point()
        {
            X = 574,
            Y = 429
        };

        public static readonly Point pointHack6 = new Point()
        {
            X = 899,
            Y = 429
        };

        public static readonly Point pointHack7 = new Point()
        {
            X = 574,
            Y = 522
        };

        public static readonly Point pointHack8 = new Point()
        {
            X = 899,
            Y = 522
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

            window.Click(HacksConstants.pointPageHacks1, false, true);
            window.Click(HacksConstants.pointHack1, false, false);
            window.Click(HacksConstants.pointHack2, false, false);
            window.Click(HacksConstants.pointHack3, false, false);
            window.Click(HacksConstants.pointHack4, false, false);
            window.Click(HacksConstants.pointHack5, false, false);
            window.Click(HacksConstants.pointHack6, false, false);
            window.Click(HacksConstants.pointHack7, false, false);
            window.Click(HacksConstants.pointHack8, false, false);

            window.Click(HacksConstants.pointPageHacks2, false, true);
            window.Click(HacksConstants.pointHack1, false, false);
            window.Click(HacksConstants.pointHack2, false, false);
            window.Click(HacksConstants.pointHack3, false, false);
            window.Click(HacksConstants.pointHack4, false, false);
            window.Click(HacksConstants.pointHack5, false, false);
            window.Click(HacksConstants.pointHack6, false, false);
            window.Click(HacksConstants.pointHack7, false, false);
            window.Click(HacksConstants.pointHack8, false, false);
        }
    }
}
