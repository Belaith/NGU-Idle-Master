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
    static class GoldDiggersConstants
    {
        #region points

        public static readonly Point pointPageGoldDiggers = new Point()
        {
            X = 201,
            Y = 415
        };
        public static readonly Point pointPage1 = new Point()
        {
            X = 348,
            Y = 116
        };
        public static readonly Point pointPage2 = new Point()
        {
            X = 411,
            Y = 116
        };
        public static readonly Point pointPage3 = new Point()
        {
            X = 478,
            Y = 116
        };

        public static readonly Point pointClearActiveDiggers = new Point()
        {
            X = 850,
            Y = 116
        };
        
        public static readonly Point pointDiggerInput = new Point()
        {
            X = 452,
            Y = 192
        };
        public static readonly Point pointDiggerPlus = new Point()
        {
            X = 494,
            Y = 192
        };
        public static readonly Point pointDiggerMinus = new Point()
        {
            X = 523,
            Y = 192
        };
        public static readonly Point pointDiggerCap = new Point()
        {
            X = 557,
            Y = 192
        };
        public static readonly Point pointDiggerActive = new Point()
        {
            X = 347,
            Y = 247
        };
        public static readonly Point pointDiggerLevel = new Point()
        {
            X = 557,
            Y = 247
        };

        public static readonly Point pointDigger1Offset = new Point()
        {
            X = 0,
            Y = 0
        };
        public static readonly Point pointDigger2Offset = new Point()
        {
            X = 765 - 452,
            Y = 0
        };
        public static readonly Point pointDigger3Offset = new Point()
        {
            X = 0,
            Y = 435-247
        };
        public static readonly Point pointDigger4Offset = new Point()
        {
            X = 765 - 452,
            Y = 435 - 247
        };

        #endregion
    }

    public class GoldDiggers
    {
        NGUIdleMasterWindow window;

        public GoldDiggers(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void SetDiggersForFight()
        {
            window.Log($"SetDiggersForFight");

            List<KeyValuePair<int, int>> diggerList = new List<KeyValuePair<int, int>>();

            KeyValuePair<int, int> digger3 = new KeyValuePair<int, int>(3, 0);
            diggerList.Add(digger3);
            KeyValuePair<int, int> digger4 = new KeyValuePair<int, int>(4, 0);
            diggerList.Add(digger4);
            KeyValuePair<int, int> digger12 = new KeyValuePair<int, int>(12, 0);
            diggerList.Add(digger12);
            KeyValuePair<int, int> digger1 = new KeyValuePair<int, int>(1, 0);
            diggerList.Add(digger1);
            KeyValuePair<int, int> digger9 = new KeyValuePair<int, int>(9, 0);
            diggerList.Add(digger9);
            KeyValuePair<int, int> digger5 = new KeyValuePair<int, int>(5, 0);
            diggerList.Add(digger5);
            KeyValuePair<int, int> digger6 = new KeyValuePair<int, int>(6, 0);
            diggerList.Add(digger6);
            KeyValuePair<int, int> digger7 = new KeyValuePair<int, int>(7, 0);
            diggerList.Add(digger7);
            KeyValuePair<int, int> digger8 = new KeyValuePair<int, int>(8, 0);
            diggerList.Add(digger8);
            KeyValuePair<int, int> digger11 = new KeyValuePair<int, int>(11, 0);
            diggerList.Add(digger11);
            KeyValuePair<int, int> digger10 = new KeyValuePair<int, int>(10, 0);
            diggerList.Add(digger10);
            KeyValuePair<int, int> digger2 = new KeyValuePair<int, int>(2, 0);
            diggerList.Add(digger2);

            SetDiggers(diggerList);
        }

        public void SetDiggersForFarm()
        {
            window.Log($"SetDiggersForFarm");

            List<KeyValuePair<int, int>> diggerList = new List<KeyValuePair<int, int>>();

            KeyValuePair<int, int> digger1 = new KeyValuePair<int, int>(1, 0);
            diggerList.Add(digger1);
            KeyValuePair<int, int> digger4 = new KeyValuePair<int, int>(4, 0);
            diggerList.Add(digger4);
            KeyValuePair<int, int> digger9 = new KeyValuePair<int, int>(9, 0);
            diggerList.Add(digger9);
            KeyValuePair<int, int> digger12 = new KeyValuePair<int, int>(12, 0);
            diggerList.Add(digger12);
            KeyValuePair<int, int> digger5 = new KeyValuePair<int, int>(5, 0);
            diggerList.Add(digger5);
            KeyValuePair<int, int> digger6 = new KeyValuePair<int, int>(6, 0);
            diggerList.Add(digger6);
            KeyValuePair<int, int> digger7 = new KeyValuePair<int, int>(7, 0);
            diggerList.Add(digger7);
            KeyValuePair<int, int> digger8 = new KeyValuePair<int, int>(8, 0);
            diggerList.Add(digger8);
            KeyValuePair<int, int> digger11 = new KeyValuePair<int, int>(11, 0);
            diggerList.Add(digger11);
            KeyValuePair<int, int> digger10 = new KeyValuePair<int, int>(10, 0);
            diggerList.Add(digger10);
            KeyValuePair<int, int> digger2 = new KeyValuePair<int, int>(2, 0);
            diggerList.Add(digger2);
            KeyValuePair<int, int> digger3 = new KeyValuePair<int, int>(3, 0);
            diggerList.Add(digger3);

            SetDiggers(diggerList);
        }

        public void SetDiggersForWandoos()
        {
            window.Log($"SetDiggersForWandoos");

            List<KeyValuePair<int, int>> diggerList = new List<KeyValuePair<int, int>>();

            KeyValuePair<int, int> digger2 = new KeyValuePair<int, int>(2, 0);
            diggerList.Add(digger2);
            KeyValuePair<int, int> digger3 = new KeyValuePair<int, int>(3, 0);
            diggerList.Add(digger3);
            KeyValuePair<int, int> digger4 = new KeyValuePair<int, int>(4, 0);
            diggerList.Add(digger4);
            KeyValuePair<int, int> digger9 = new KeyValuePair<int, int>(9, 0);
            diggerList.Add(digger9);
            KeyValuePair<int, int> digger12 = new KeyValuePair<int, int>(12, 0);
            diggerList.Add(digger12);
            KeyValuePair<int, int> digger5 = new KeyValuePair<int, int>(5, 0);
            diggerList.Add(digger5);
            KeyValuePair<int, int> digger6 = new KeyValuePair<int, int>(6, 0);
            diggerList.Add(digger6);
            KeyValuePair<int, int> digger7 = new KeyValuePair<int, int>(7, 0);
            diggerList.Add(digger7);
            KeyValuePair<int, int> digger8 = new KeyValuePair<int, int>(8, 0);
            diggerList.Add(digger8);
            KeyValuePair<int, int> digger11 = new KeyValuePair<int, int>(11, 0);
            diggerList.Add(digger11);
            KeyValuePair<int, int> digger10 = new KeyValuePair<int, int>(10, 0);
            diggerList.Add(digger10);

            SetDiggers(diggerList);
        }

        public void SetDiggers(List<KeyValuePair<int, int>> diggerList)
        {
            window.Click(GoldDiggersConstants.pointPageGoldDiggers, false, false);

            window.Click(GoldDiggersConstants.pointClearActiveDiggers, false, false);

            int lastPage = 0;

            foreach (KeyValuePair<int, int> digger in diggerList)
            {
                int page = ((digger.Key - 1) / 4) + 1;

                if (page != lastPage)
                {
                    switch (page)
                    {
                        case 1:
                            window.Click(GoldDiggersConstants.pointPage1, false, false);
                            break;
                        case 2:
                            window.Click(GoldDiggersConstants.pointPage2, false, false);
                            break;
                        case 3:
                            window.Click(GoldDiggersConstants.pointPage3, false, false);
                            break;
                    }
                    lastPage = page;
                }
                
                Point offset = GoldDiggersConstants.pointDigger1Offset;

                int diggerOnPage = ((digger.Key - 1) % 4) + 1;

                switch (diggerOnPage)
                {
                    case 1:
                        offset = GoldDiggersConstants.pointDigger1Offset;
                        break;
                    case 2:
                        offset = GoldDiggersConstants.pointDigger2Offset;
                        break;
                    case 3:
                        offset = GoldDiggersConstants.pointDigger3Offset;
                        break;
                    case 4:
                        offset = GoldDiggersConstants.pointDigger4Offset;
                        break;
                }

                if (digger.Value == 0)
                {
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerCap.X, offset.Y + GoldDiggersConstants.pointDiggerCap.Y), false, false);
                }
                else if (digger.Value < 0)
                {
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerCap.X, offset.Y + GoldDiggersConstants.pointDiggerCap.Y), false, false);
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerActive.X, offset.Y + GoldDiggersConstants.pointDiggerActive.Y), false, false);
                    for (int i = digger.Value; i < 0; i++)
                    {
                        window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerMinus.X, offset.Y + GoldDiggersConstants.pointDiggerMinus.Y), false, false);
                    }
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerMinus.X, offset.Y + GoldDiggersConstants.pointDiggerMinus.Y), false, false);
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerPlus.X, offset.Y + GoldDiggersConstants.pointDiggerPlus.Y), false, false);
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerActive.X, offset.Y + GoldDiggersConstants.pointDiggerActive.Y), false, false);
                }
                else
                {
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerInput.X, offset.Y + GoldDiggersConstants.pointDiggerInput.Y), false, false);
                    window.SendString(digger.Value.ToString(), false);
                    window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerActive.X, offset.Y + GoldDiggersConstants.pointDiggerActive.Y), false, false);
                }
            }
        }

        public void LevelDiggers()
        {
            List<int> diggerList = new List<int>();
            diggerList.Add(12);
            diggerList.Add(11);
            diggerList.Add(10);
            diggerList.Add(9);
            diggerList.Add(8);
            diggerList.Add(7);
            diggerList.Add(6);
            diggerList.Add(5);
            diggerList.Add(4);
            diggerList.Add(3);
            diggerList.Add(2);
            diggerList.Add(1);

            LevelDiggers(diggerList);
        }

        public void LevelDiggers(List<int> diggerList)
        {
            window.Log($"LevelDiggers");

            window.Click(GoldDiggersConstants.pointPageGoldDiggers, false, false);

            int lastPage = 0;

            foreach (int digger in diggerList)
            {
                int page = ((digger - 1) / 4) + 1;

                if (page != lastPage)
                {
                    switch (page)
                    {
                        case 1:
                            window.Click(GoldDiggersConstants.pointPage1, false, false);
                            break;
                        case 2:
                            window.Click(GoldDiggersConstants.pointPage2, false, false);
                            break;
                        case 3:
                            window.Click(GoldDiggersConstants.pointPage3, false, false);
                            break;
                    }
                    lastPage = page;
                }

                Point offset = GoldDiggersConstants.pointDigger1Offset;

                int diggerOnPage = ((digger - 1) % 4) + 1;

                switch (diggerOnPage)
                {
                    case 1:
                        offset = GoldDiggersConstants.pointDigger1Offset;
                        break;
                    case 2:
                        offset = GoldDiggersConstants.pointDigger2Offset;
                        break;
                    case 3:
                        offset = GoldDiggersConstants.pointDigger3Offset;
                        break;
                    case 4:
                        offset = GoldDiggersConstants.pointDigger4Offset;
                        break;
                }

                window.Click(new Point(offset.X + GoldDiggersConstants.pointDiggerLevel.X, offset.Y + GoldDiggersConstants.pointDiggerLevel.Y), true, false);
            }
        }
    }
}