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
    static class FightBossConstants
    {
        #region rects

        public static readonly RECT rectCurrentBoss = new RECT()
        {
            Left = 792,
            Top = 133,
            Right = 864,
            Bottom = 149
        };

        #endregion

        #region points

        public static readonly Point pointPageFightBoss = new Point()
        {
            X = 237,
            Y = 84
        };

        public static readonly Point pointNuke = new Point()
        {
            X = 629,
            Y = 120
        };

        public static readonly Point pointStop = new Point()
        {
            X = 629,
            Y = 175
        };

        public static readonly Point pointFight = new Point()
        {
            X = 628,
            Y = 227
        };

        #endregion

        public static readonly List<int> WorldStages = new List<int>()
        {
            4,7,17,37,48,58,58,66,66,74,82,82,90,100,100,108,116,116,124,132,137
        };
    }

    public class FightBoss
    {
        NGUIdleMasterWindow window;


        private int lastBoss = 0;

        public FightBoss(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public bool Nuke()
        {
            int oldBoss = GetCurrentBoss();

            if (oldBoss < lastBoss)
            {
                lastBoss = -1;
            }

            window.Log($"Nuke Boss");
            window.Click(FightBossConstants.pointPageFightBoss, false, true);
            window.Click(FightBossConstants.pointNuke, false, false);
            window.Click(FightBossConstants.pointFight, false, false);
            window.Wait(5);
            int newBoss = GetCurrentBoss();

            if (newBoss != lastBoss)
            {
                window.Log($"New Boss: {newBoss, 3}");
            }
            int lastBossForStage = lastBoss;
            lastBoss = newBoss;

            return newStage(lastBossForStage, newBoss);
        }

        private bool newStage(int oldBoss, int newBoss)
        {
            if (oldBoss != -1 && newBoss != -1)
            {
                return FightBossConstants.WorldStages.Where(x => x > oldBoss && x <= newBoss).ToList().Count() > 0;
            }
            else
            {
                return true;
            }
        }

        public void Fight()
        {
            window.Log($"Fight Boss");
            window.Click(FightBossConstants.pointPageFightBoss, false, true);
            window.Click(FightBossConstants.pointFight, false, false);
        }

        public int GetCurrentBoss()
        {
            if (!window.Locked)
            {
                window.Click(FightBossConstants.pointPageFightBoss, false, true);

                string input = window.OCRTextSearch(FightBossConstants.rectCurrentBoss, false);

                int start = input.IndexOf("Boss");
                
                if (start < 0 || input.Length < start + 4 + 1)
                {
                    return -1;
                }

                start += 4;
                
                input = input.Substring(start);

                BigInteger boss = window.Parse(input);

                return (int)boss - 1;
            }
            else
            {
                return -1;
            }
        }
    }
}