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
    static class RebirthConstants
    {
        #region points

        public static readonly Point pointPageAdvTraining = new Point()
        {
            X = 192,
            Y = 234
        };

        #endregion

        #region rects

        public static readonly RECT rectRunTime = new RECT()
        {
            Left = 31,
            Top = 394 + 4,
            Right = 155,
            Bottom = 406 + 7
        };

        public static readonly RECT rectCurrentChallenge = new RECT()
        {
            Left = 471,
            Top = 93,
            Right = 755,
            Bottom = 113
        };

        public static readonly RECT rectCurrentChallengeTime = new RECT()
        {
            Left = 441,
            Top = 110,
            Right = 755,
            Bottom = 124
        };

        public static readonly RECT rectCurrentChallengeObjectivee = new RECT()
        {
            Left = 488,
            Top = 126,
            Right = 755,
            Bottom = 139
        };

        #endregion

        #region points

        public static readonly Point pointPageRebirth = new Point()
        {
            X = 98,
            Y = 425
        };

        public static readonly Point pointPageChallenges = new Point()
        {
            X = 717,
            Y = 527
        };

        public static readonly Point pointRebirth = new Point()
        {
            X = 554,
            Y = 528
        };

        public static readonly Point pointConfirm = new Point()
        {
            X = 442,
            Y = 323
        };

        public static readonly Point pointBasic = new Point()
        {
            X = 377,
            Y = 190
        };

        public static readonly Point pointNoAugs = new Point()
        {
            X = 377,
            Y = 220
        };

        public static readonly Point point24h = new Point()
        {
            X = 377,
            Y = 250
        };

        public static readonly Point point100lvl = new Point()
        {
            X = 377,
            Y = 280
        };

        public static readonly Point pointNoEquip = new Point()
        {
            X = 377,
            Y = 310
        };

        public static readonly Point pointTroll = new Point()
        {
            X = 377,
            Y = 340
        };

        public static readonly Point pointNoRebirth = new Point()
        {
            X = 377,
            Y = 370
        };

        public static readonly Point pointLaserSword = new Point()
        {
            X = 377,
            Y = 400
        };

        public static readonly Point pointBlind = new Point()
        {
            X = 377,
            Y = 430
        };

        public static readonly Point pointNoNGU = new Point()
        {
            X = 377,
            Y = 460
        };

        public static readonly Point pointNoTM = new Point()
        {
            X = 377,
            Y = 490
        };

        #endregion

    }

    public class Rebirth
    {
        NGUIdleMasterWindow window;

        DateTime runStartTime = DateTime.Now;

        TimeSpan lastTimeSpan = new TimeSpan();

        public Rebirth(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public TimeSpan getRunTime()
        {
            if (window.Locked)
            {
                DateTime currentTime = DateTime.Now;

                TimeSpan timeSpan = currentTime - runStartTime;

                return timeSpan;
            }

            try
            {
                DateTime currentTime = DateTime.Now;
                string input = window.OCRTextSearch(RebirthConstants.rectRunTime, true);

                string days = string.Empty;

                if (input.Contains("days"))
                {
                    days = input.Substring(0, input.IndexOf("days")).Trim();
                    input = input.Substring(input.IndexOf("days") + 4).Trim();
                }
                if (input.Contains("."))
                {
                    input = input.Substring(0, input.LastIndexOf("."));
                }

                switch (input.Length)
                {
                    case 1:
                        input = $"00:00:0{input}";
                        break;
                    case 2:
                        input = $"00:00:{input}";
                        break;
                    case 4:
                        input = $"00:0{input}";
                        break;
                    case 5:
                        input = $"00:{input}";
                        break;
                }

                TimeSpan timeSpan = TimeSpan.Parse(input);
                if (!string.IsNullOrWhiteSpace(days))
                {
                    timeSpan = timeSpan + TimeSpan.FromDays(int.Parse(days));
                }

                if (timeSpan != lastTimeSpan)
                {
                    lastTimeSpan = timeSpan;
                    runStartTime = currentTime - timeSpan;
                }
                else
                {
                    timeSpan = DateTime.Now - runStartTime;
                }

                return timeSpan;
            }
            catch
            {
                TimeSpan timeSpan = DateTime.Now - runStartTime;

                return timeSpan;
            }
        }

        public int GetCurrentChallenge()
        {
            if (window.Locked)
            {
                return -1;
            }

            window.Click(RebirthConstants.pointPageRebirth, false, true);
            window.Click(RebirthConstants.pointPageChallenges, false, true);
            string input = window.OCRTextSearch(RebirthConstants.rectCurrentChallenge, false);

            if (input.ToLower().Contains("none"))
            {
                return 0;
            }
            else if (input.ToLower().Contains("basic"))
            {
                return 1;
            }
            else if (input.ToLower().Contains("augs"))
            {
                return 2;
            }
            else if (input.ToLower().Contains("hour"))
            {
                return 3;
            }
            else if (input.ToLower().Contains("level"))
            {
                return 4;
            }
            else if (input.ToLower().Contains("equipment"))
            {
                return 5;
            }
            else if (input.ToLower().Contains("troll"))
            {
                return 6;
            }
            else if (input.ToLower().Contains("rebirth"))
            {
                return 7;
            }

            else if (input.ToLower().Contains("laser"))
            {
                return 8;
            }
            else if (input.ToLower().Contains("blind"))
            {
                return 9;
            }
            else if (input.ToLower().Contains("ngu"))
            {
                return 10;
            }
            else if (input.ToLower().Contains("tm"))
            {
                return 11;
            }

            else
            {
                return -1;
            }
        }

        public TimeSpan GetCurrentChallengeTime()
        {
            window.Click(RebirthConstants.pointPageRebirth, false, true);
            window.Click(RebirthConstants.pointPageChallenges, false, true);
            string input = window.OCRTextSearch(RebirthConstants.rectCurrentChallengeTime, false);

            if (string.IsNullOrWhiteSpace(input))
            {
                return new TimeSpan();
            }

            string days = string.Empty;

            if (input.Contains("days"))
            {
                days = input.Substring(0, input.IndexOf("days")).Trim();
                input = input.Substring(input.IndexOf("days") + 4).Trim();
            }

            TimeSpan timeSpan = TimeSpan.Parse(input);
            timeSpan = timeSpan + TimeSpan.FromDays(int.Parse(days));

            return timeSpan;
        }

        public int GetCurrentChallengeObjective()
        {
            window.Click(RebirthConstants.pointPageRebirth, false, true);
            window.Click(RebirthConstants.pointPageChallenges, false, true);
            string input = window.OCRTextSearch(RebirthConstants.rectCurrentChallengeTime, false);

            if(string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            return int.Parse(input);
        }

        public void DoRebirth()
        {
            window.Log($"Rebirth");

            window.Click(RebirthConstants.pointPageRebirth, false, true);
            window.Click(RebirthConstants.pointRebirth, false, true);
            window.Click(RebirthConstants.pointConfirm, false, true);
            runStartTime = DateTime.Now;
        }

        public void DoChallenge(int challenge)
        {
            window.Click(RebirthConstants.pointPageRebirth, false, true);
            window.Click(RebirthConstants.pointPageChallenges, false, true);

            if (GetCurrentChallenge() == 0 || (GetCurrentChallenge() == -1 && challenge == 8))
            {
                switch (challenge)
                {
                    case 1:
                        window.Click(RebirthConstants.pointBasic, false, true);
                        break;
                    case 2:
                        window.Click(RebirthConstants.pointNoAugs, false, true);
                        break;
                    case 3:
                        window.Click(RebirthConstants.point24h, false, true);
                        break;
                    case 4:
                        window.Click(RebirthConstants.point100lvl, false, true);
                        break;
                    case 5:
                        window.Click(RebirthConstants.pointNoEquip, false, true);
                        break;
                    case 6:
                        window.Click(RebirthConstants.pointTroll, false, true);
                        break;
                    case 7:
                        window.Click(RebirthConstants.pointNoRebirth, false, true);
                        break;
                    case 8:
                        window.Click(RebirthConstants.pointLaserSword, false, true);
                        break;
                    case 9:
                        window.Click(RebirthConstants.pointBlind, false, true);
                        break;
                    case 10:
                        window.Click(RebirthConstants.pointNoNGU, false, true);
                        break;
                    case 11:
                        window.Click(RebirthConstants.pointNoTM, false, true);
                        break;
                    default:
                        return;
                }

                window.Click(RebirthConstants.pointConfirm, false, true);
                runStartTime = DateTime.Now;
            }
        }
    }
}