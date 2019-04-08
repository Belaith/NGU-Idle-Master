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
    static class NGUConstants
    {
        #region points

        public static readonly Point pointPageNGU = new Point()
        {
            X = 192,
            Y = 354
        };

        public static readonly Point pointPageMagic = new Point()
        {
            X = 381,
            Y = 126
        };

        public static readonly Point pointCap = new Point()
        {
            X = 635,
            Y = 168
        };

        #endregion
    }

    public class NGU
    {
        NGUIdleMasterWindow window;

        public NGU(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void SetNGUs()
        {
            window.Log($"SetNGUs");

            window.Click(NGUConstants.pointPageNGU, false, true);
            window.Click(NGUConstants.pointCap, false, true);

            window.Click(NGUConstants.pointPageMagic, false, true);
            window.Click(NGUConstants.pointCap, false, true);
        }
    }
}