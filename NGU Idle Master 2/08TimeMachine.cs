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
    static class TimeMachineConstants
    {
        #region points

        public static readonly Point pointPageTimeMachine = new Point()
        {
            X = 192,
            Y = 264
        };

        public static readonly Point pointMachineSpeed = new Point()
        {
            X = 541,
            Y = 241
        };

        public static readonly Point pointGoldMultiplier = new Point()
        {
            X = 541,
            Y = 341
        };

        #endregion
    }


    public class TimeMachine
    {
        NGUIdleMasterWindow window;

        public TimeMachine(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public bool check()
        {

            if (window.Locked || window.GetPixelColor(TimeMachineConstants.pointPageTimeMachine, true) == ColorTranslator.FromHtml("#FFFFFF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetSpeedAndMultiplier()
        {
            window.Log($"SetTimeMachine");

            window.Click(TimeMachineConstants.pointPageTimeMachine, false, true);

            window.SetInput(-1);

            window.Click(TimeMachineConstants.pointMachineSpeed, false, true);
            window.Click(TimeMachineConstants.pointGoldMultiplier, false, true);
        }

    }
}