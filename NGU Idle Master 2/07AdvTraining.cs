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
    static class AdvTrainingConstants
    {
        #region points

        public static readonly Point pointPageAdvTraining = new Point()
        {
            X = 192,
            Y = 234
        };

        #endregion
    }

    public class AdvTraining
    {
        NGUIdleMasterWindow window;

        public AdvTraining(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public bool check()
        {
            if (window.GetPixelColor(AdvTrainingConstants.pointPageAdvTraining, false) == ColorTranslator.FromHtml("#FFFFFF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //TODO
    }
}