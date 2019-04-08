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
    static class QuestingConstants
    {
        #region rects

        public static readonly RECT rectIdleMode = new RECT()
        {
            Left = 374,
            Top = 512,
            Right = 571,
            Bottom = 535
        };

        #endregion

        #region points

        public static readonly Point pointPageQuesting = new Point()
        {
            X = 237,
            Y = 473
        };

        public static readonly Point pointCompleteQuest = new Point()
        {
            X = 717,
            Y = 170
        };

        public static readonly Point pointIdleMode = new Point()
        {
            X = 477,
            Y = 568
        };

        #endregion
    }

    public class Questing
    {
        NGUIdleMasterWindow window;

        public Questing(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void CompleteQuest()
        {
            window.Log($"CompleteQuest");

            window.Click(QuestingConstants.pointPageQuesting, false, true);

            window.Click(QuestingConstants.pointCompleteQuest, false, true);
            window.Click(QuestingConstants.pointCompleteQuest, false, true);

            if (window.OCRTextSearch(QuestingConstants.rectIdleMode, true).Contains("OFF") || window.Locked)
            {
                window.Click(QuestingConstants.pointIdleMode, false, true);
            }
        }
    }
}
