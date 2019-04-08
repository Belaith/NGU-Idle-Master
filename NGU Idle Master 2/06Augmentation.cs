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
    static class AugmentationConstants
    {
        #region points

        public static readonly Point pointPageAugmentation = new Point()
        {
            X = 192,
            Y = 204
        };

        public static readonly Point pointAugmentation1 = new Point()
        {
            X = 546,
            Y = 271
        };

        public static readonly Point pointUpgrade1 = new Point()
        {
            X = 546,
            Y = 299
        };

        #endregion
    }

    public class Augmentation
    {
        NGUIdleMasterWindow window;

        public Augmentation(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public bool check()
        {
            if (window.GetPixelColor(AugmentationConstants.pointPageAugmentation, true) == ColorTranslator.FromHtml("#FFFFFF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetAugmentations()
        {
            window.Log($"SetAugmentations");

            window.Click(AugmentationConstants.pointPageAugmentation, false, true);
            window.SetInput(-2, false, true);
            window.Click(AugmentationConstants.pointAugmentation1, false, true);
            window.Click(AugmentationConstants.pointUpgrade1, false, true);
            window.SetInput(-1);
        }
    }
}