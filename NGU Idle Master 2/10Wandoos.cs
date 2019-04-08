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
    static class WandoosConstants
    {
        #region points

        public static readonly Point pointPageWandoos = new Point()
        {
            X = 192,
            Y = 324
        };

        public static readonly Point pointCapEnergy = new Point()
        {
            X = 633,
            Y = 257
        };

        public static readonly Point pointCapMagic = new Point()
        {
            X = 633,
            Y = 355
        };

        #endregion
    }

    public class Wandoos
    {
        NGUIdleMasterWindow window;

        public Wandoos(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void CapWandoos()
        {
            window.Log($"CapWandoos");

            window.Click(WandoosConstants.pointPageWandoos, false, true);

            window.SetInput(-1);

            window.Click(WandoosConstants.pointCapEnergy, false, true);
            window.Click(WandoosConstants.pointCapMagic, false, true);
        }
    }
}