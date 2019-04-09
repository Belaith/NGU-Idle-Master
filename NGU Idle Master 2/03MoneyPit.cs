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
    static class MoneyPitConstants
    {
        #region points

        public static readonly Point pointPageMoneyPit = new Point()
        {
            X = 237,
            Y = 114
        };

        public static readonly Point pointPageDailySpin = new Point()
        {
            X = 830,
            Y = 250
        };

        public static readonly Point pointThrow = new Point()
        {
            X = 435,
            Y = 175
        };

        public static readonly Point pointSpin = new Point()
        {
            X = 720,
            Y = 570
        };

        public static readonly Point pointConfirm = new Point()
        {
            X = 442,
            Y = 323
        };

        #endregion
    }

    public class MoneyPit
    {
        NGUIdleMasterWindow window;

        public MoneyPit(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void Throw()
        {
            window.Log($"Bottomless Pit Throw");
            window.Click(MoneyPitConstants.pointPageMoneyPit, false, true);
            window.Click(MoneyPitConstants.pointThrow, false, true);
            window.Click(MoneyPitConstants.pointConfirm, false, true);
        }

        public void Spin()
        {
            window.Log($"Daily Spin");
            window.Click(MoneyPitConstants.pointPageMoneyPit, false, true);
            window.Click(MoneyPitConstants.pointPageDailySpin, false, true);
            window.Click(MoneyPitConstants.pointSpin, false, true);
        }
    }
}