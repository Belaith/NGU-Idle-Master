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
using System.Diagnostics;

namespace NGU_Idle_Master
{
    static class AdventureConstants
    {
        #region points

        public static readonly Point pointPageAdventure = new Point()
        {
            X = 240,
            Y = 145
        };

        public static readonly Point pointLeftArrow = new Point()
        {
            X = 330,
            Y = 232
        };

        public static readonly Point pointRightArrow = new Point()
        {
            X = 941,
            Y = 229
        };

        public static readonly Point pointITOPODEnter1 = new Point()
        {
            X = 412,
            Y = 232
        };

        public static readonly Point pointITOPODOptimalFloor = new Point()
        {
            X = 713,
            Y = 216
        };

        public static readonly Point pointITOPODEnter2 = new Point()
        {
            X = 634,
            Y = 304
        };

        public static readonly Point pointCrown = new Point()
        {
            X = 719,
            Y = 284
        };

        public static readonly Point pointHealthMob = new Point()
        {
            X = 714,
            Y = 417
        };

        public static readonly Point pointHealthPlayer = new Point()
        {
            X = 553,
            Y = 417
        };

        public static readonly Point pointIdleMode = new Point()
        {
            X = 377,
            Y = 109
        };

        #endregion

        #region colors

        public static readonly Color colorPlayerHealthMax = Color.FromArgb(218, 48, 48);

        public static readonly Color colorMobHealthAlive = Color.FromArgb(217, 48, 48);

        public static readonly Color colorMobHealthDead = Color.FromArgb(235, 235, 235);

        public static readonly Color colorCrown = Color.FromArgb(247, 239, 41);


        #endregion

    }

    public class Adventure
    {
        NGUIdleMasterWindow window;

        public Adventure(NGUIdleMasterWindow window)
        {
            this.window = window;
        }

        public void ITOPOD()
        {
            window.Click(AdventureConstants.pointPageAdventure, false, true);
            window.Click(AdventureConstants.pointITOPODEnter1, false, true);
            window.Click(AdventureConstants.pointITOPODOptimalFloor, false, true);
            window.Click(AdventureConstants.pointITOPODEnter2, false, true);
        }

        public void Farm(int stage, int currentBoss)
        {

            if (stage <= 0 || window.Locked || currentBoss > FightBossConstants.WorldStages[stage])
            {
                Farm(stage);
            }
            else
            {
                Farm(-1);
            }
        }

        public void Farm(int stage)
        {
            window.Log($"Farming Stage: {stage}");

            window.Click(AdventureConstants.pointPageAdventure, false, true);

            if (stage == 0)
            {
                ITOPOD();
            }
            else if (stage < 0)
            {
                window.Click(AdventureConstants.pointRightArrow, true, true);
                for (int i = -1; i > stage; i--)
                {
                    window.Click(AdventureConstants.pointLeftArrow, false, true);
                }
            }
            else
            {
                window.Click(AdventureConstants.pointLeftArrow, true, true);
                for (int i = 0; i < stage; i++)
                {
                    window.Click(AdventureConstants.pointRightArrow, false, true);
                }
            }
        }

        public void Farm(int stage, int seconds, bool bossOnly, int endStage, int currentBoss)
        {
            if (window.Locked)
            {
                return;
            }

            window.Log($"Sniping Stage: {stage} BossOnly: {bossOnly} - For {seconds} seconds");

            window.Click(AdventureConstants.pointPageAdventure, false, true);
            window.Click(AdventureConstants.pointLeftArrow, true, true);

            window.SendString("q", false);
            
            if (!Console.IsOutputRedirected)
            {
                Console.Write($"{seconds,5}");
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Stopwatch swContinue = new Stopwatch();

            while (sw.ElapsedMilliseconds < seconds * 1000 && !window.Locked)
            {
                //Wait for Player HP
                swContinue.Restart();
                while (window.GetPixelColor(AdventureConstants.pointHealthPlayer, true) != AdventureConstants.colorPlayerHealthMax && !window.Locked)
                {
                    if (swContinue.ElapsedMilliseconds > 20000)
                    {
                        window.Click(AdventureConstants.pointLeftArrow, true, true);
                        break;
                    }

                    if (!Console.IsOutputRedirected)
                    {
                        Console.Write(new String('\b', 5) + $"{(seconds * 1000 - sw.ElapsedMilliseconds) / 1000,5}");
                    }

                    Thread.Sleep(5);
                }
                if (swContinue.ElapsedMilliseconds > 20000)
                {
                    continue;
                }

                if (stage < 0)
                {
                    window.Click(AdventureConstants.pointRightArrow, true, true);
                    for (int i = -1; i > stage; i--)
                    {
                        window.Click(AdventureConstants.pointLeftArrow, false, true);
                    }
                }
                else
                {
                    for (int i = 1; i < stage; i++)
                    {
                        window.Click(AdventureConstants.pointRightArrow, false, true);
                    }
                }

                //Wait for Mob
                swContinue.Restart();
                while (window.GetPixelColor(AdventureConstants.pointHealthMob, true) != AdventureConstants.colorMobHealthAlive && !window.Locked)
                {
                    if (swContinue.ElapsedMilliseconds > 6000)
                    {
                        window.Click(AdventureConstants.pointLeftArrow, true, true);
                        break;
                    }

                    if (!Console.IsOutputRedirected)
                    {
                        Console.Write(new String('\b', 5) + $"{(seconds * 1000 - sw.ElapsedMilliseconds) / 1000,5}");
                    }

                    Thread.Sleep(5);
                }
                if (swContinue.ElapsedMilliseconds > 6000)
                {
                    continue;
                }

                if (bossOnly && window.GetPixelColor(AdventureConstants.pointCrown, true) != AdventureConstants.colorCrown)
                {
                    window.Click(AdventureConstants.pointLeftArrow, true, true);
                }
                else
                {
                    //Wait for Mob death
                    swContinue.Restart();
                    while (window.GetPixelColor(AdventureConstants.pointHealthMob, true) != AdventureConstants.colorMobHealthDead && !window.Locked)
                    {
                        if (swContinue.ElapsedMilliseconds > 60000)
                        {
                            window.Click(AdventureConstants.pointLeftArrow, true, true);
                            break;
                        }

                        window.SendString("ghfytrewdsax", false);

                        if (!Console.IsOutputRedirected)
                        {
                            Console.Write(new String('\b', 5) + $"{(seconds * 1000 - sw.ElapsedMilliseconds) / 1000,5}");
                        }

                        Thread.Sleep(5);
                    }
                    if (swContinue.ElapsedMilliseconds > 60000)
                    {
                        continue;
                    }

                    window.Click(AdventureConstants.pointLeftArrow, true, true);
                }
            }

            if (!Console.IsOutputRedirected)
            {
                Console.Write(new String('\b', 5));
            }

            window.SendString("q", false);
            Farm(endStage, currentBoss);
        }
    }
}