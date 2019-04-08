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
    static class BloodMagicConstants
    {
        #region points

        public static readonly Point pointPageBloodMagic = new Point()
        {
            X = 192,
            Y = 294
        };

        public static readonly Point pointPageSpells = new Point()
        {
            X = 393,
            Y = 122
        };

        public static readonly Point pointIronPill = new Point()
        {
            X = 700,
            Y = 222
        };

        public static readonly Point pointCap = new Point()
        {
            X = 575,
            Y = 235
        };

        #endregion
    }

    public class BloodMagic
    {
        NGUIdleMasterWindow window;

        public BloodMagic(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public bool check()
        {
            Color color = window.GetPixelColor(BloodMagicConstants.pointPageBloodMagic, false);

            if (color == ColorTranslator.FromHtml("#FFFFFF") || color == ColorTranslator.FromHtml("#BA13A7"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IronPill()
        {
            window.Log($"Iron Pill");

            window.Click(BloodMagicConstants.pointPageBloodMagic, false, true);
            window.Click(BloodMagicConstants.pointPageSpells, false, true);
            window.Click(BloodMagicConstants.pointIronPill, false, true);
        }

        public void setRituals(int rituals)
        {
            window.Log($"setRituals");
            window.Click(BloodMagicConstants.pointPageBloodMagic, false, true);
            if (rituals > 0 && rituals < 9)
            {
                for (int i = rituals; i> 0; i--)
                {
                    window.Click(new Point(BloodMagicConstants.pointCap.X, BloodMagicConstants.pointCap.Y + (i-1) * 35), false, false);
                }
            }
        }
    }
}