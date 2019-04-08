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
    static class YggdrasilConstants
    {
        #region points

        public static readonly Point pointPageYggdrasil = new Point()
        {
            X = 201,
            Y = 384
        };

        public static readonly Point pointHarvestAllMax = new Point()
        {
            X = 810,
            Y = 455
        };

        public static readonly Point pointHarvestAbove1 = new Point()
        {
            X = 810,
            Y = 498
        };

        #endregion
    }

    public class Yggdrasil
    {
        NGUIdleMasterWindow window;

        public Yggdrasil(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void HarvestAllMax()
        {
            window.Log($"HarvestAllMax");

            window.Click(YggdrasilConstants.pointPageYggdrasil, false, true);
            window.Click(YggdrasilConstants.pointHarvestAllMax, false, true);
        }

        public void HarvestAbove1()
        {
            window.Log($"HarvestAllAbove1");

            window.Click(YggdrasilConstants.pointPageYggdrasil, false, true);
            window.Click(YggdrasilConstants.pointHarvestAbove1, false, true);
        }
    }
}