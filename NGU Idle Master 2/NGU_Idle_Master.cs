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
    public class NguIdleMaster : IDisposable
    {
        public Config config { get; set; }

        public NGUIdleMasterWindow window { get; private set; }
        public SpendEXP spendEXP { get; private set; }
        public Rebirth rebirth { get; private set; }
        public FightBoss fightBoss { get; private set; }
        public MoneyPit moneyPit { get; private set; }
        public Adventure adventure { get; private set; }
        public Inventory inventory { get; private set; }
        public Augmentation augmentation { get; private set; }
        public AdvTraining advTraining { get; private set; }
        public TimeMachine timeMachine { get; private set; }
        public BloodMagic bloodMagic { get; private set; }
        public Wandoos wandoos { get; private set; }
        public NGU ngu { get; private set; }
        public Yggdrasil yggdrasil { get; private set; }
        public GoldDiggers goldDiggers { get; private set; }
        public Questing questing { get; private set; }

        public bool Stop { get; set; }
        public ManualResetEvent mre = new ManualResetEvent(false);

        private bool is_disposed = false;

        public NguIdleMaster(Config config)
        {
            this.config = config;

            window = new NGUIdleMasterWindow(config.TesseractPath, config.LogPath);
            spendEXP = new SpendEXP(window);
            rebirth = new Rebirth(window);
            fightBoss = new FightBoss(window);
            moneyPit = new MoneyPit(window);
            adventure = new Adventure(window);
            inventory = new Inventory(window);
            augmentation = new Augmentation(window);
            advTraining = new AdvTraining(window);
            timeMachine = new TimeMachine(window);
            bloodMagic = new BloodMagic(window);
            wandoos = new Wandoos(window);
            ngu = new NGU(window);
            yggdrasil = new Yggdrasil(window);
            goldDiggers = new GoldDiggers(window);
            questing = new Questing(window);
        }

        ~NguIdleMaster()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!is_disposed)
            {
                if (disposing)
                {
                    window?.Dispose();
                    window = null;
                    spendEXP = null;
                    rebirth = null;
                    fightBoss = null;
                    moneyPit = null;
                    adventure = null;
                    inventory = null;
                    augmentation = null;
                    advTraining = null;
                    timeMachine = null;
                    bloodMagic = null;
                    wandoos = null;
                    ngu = null;
                    yggdrasil = null;
                    goldDiggers = null;
                    questing = null;

                    window?.Dispose();
                    window = null;
                }

                this.is_disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Rebirth(int challenge)
        {
            yggdrasil.HarvestAbove1();

            goldDiggers.SetDiggersForFight();

            fightBoss.Nuke();

            goldDiggers.LevelDiggers();
            moneyPit.Throw();
            moneyPit.Spin();

            window.Log($"Boss before Rebirth: {fightBoss.GetCurrentBoss(), 3}");
            if (challenge == 0)
            {
                rebirth.DoRebirth();
            }
            else
            {
                rebirth.DoChallenge(challenge);
            }

            window.Wait(1);
        }

        public void NukeAndAdventure(bool isChallenge, Timer timerMergeBoost, Timer timerAllocateEnergyMagic, bool withoutRebirth)
        {
            if (fightBoss.Nuke())
            {
                if (isChallenge)
                {
                    adventure.Farm(config.MaxFarmForGold, fightBoss.GetCurrentBoss());
                    
                    inventory.Loadout(2);

                    timeMachine.SetSpeedAndMultiplier();

                    timerAllocateEnergyMagic.Elapsed = true;
                }
                else
                {
                    adventure.Farm(config.MaxFarmForGold, fightBoss.GetCurrentBoss());

                    Timer timerOneMob = new Timer(new TimeSpan(0, 0, 5));
                    inventory.Loadout(2);

                    timeMachine.SetSpeedAndMultiplier();

                    inventory.Merge(true, config.inventarSlotsMerge);
                    inventory.Boost(true, config.inventarSlotsBoost);
                    timerMergeBoost.Restart();

                    window.Wait((timerOneMob.RemainingMilliseconds + 1) / 1000);

                    inventory.Loadout(1);

                    adventure.Farm(config.FarmStage, fightBoss.GetCurrentBoss());
                    
                    timeMachine.SetSpeedAndMultiplier();

                    timerAllocateEnergyMagic.Elapsed = true;
                }

                if (withoutRebirth)
                {
                    window.SendString("rt", false);
                }
            }
        }

        public void Runs()
        {
            bool withoutChallenge = config.Challenge == 0;

            window.Log($"Starting Runs - Challenge: {config.Challenge}");

            if ((!withoutChallenge && rebirth.GetCurrentChallenge() == 0) || (config.Challenge == 8 && rebirth.GetCurrentChallenge() == -1))
            {
                if (rebirth.getRunTime().TotalMinutes < 3)
                {
                    window.Wait(3 * 60 - (int)rebirth.getRunTime().TotalSeconds);
                }

                Rebirth(config.Challenge);
            }

            while (!Stop && (withoutChallenge || rebirth.GetCurrentChallenge() != 0))
            {
                Run();
                if (config.Challenge == 8 && rebirth.GetCurrentChallenge() == -1)
                {
                    break;
                }

                if (config.RebirthTime != new TimeSpan() && (withoutChallenge || (rebirth.GetCurrentChallenge() != 0 && rebirth.GetCurrentChallenge() != 8)))
                {
                    Rebirth(0);
                }
            }
        }

        public void Run()
        {
            int waitTime = 60 * 60 * 1000;
            bool withoutRebirth = config.RebirthTime == new TimeSpan();
            if (!withoutRebirth)
            {
                waitTime = (int)config.RebirthTime.TotalMilliseconds / 60;
            }
            else if (config.Challenge == 7)
            {
                waitTime = (int)new TimeSpan(0, 30, 0).TotalMilliseconds / 60;
            }

            bool isChallenge = (config.Challenge != 0 && config.Challenge != 8);

            window.Log($"Starting Run - Targeted runtime: {config.RebirthTime.ToString()}");

            List<Timer> timers = new List<Timer>();

            Timer timerMaintenance = new Timer(new TimeSpan(0, 1, 0));
            timers.Add(timerMaintenance);
            Timer timerMergeBoost = new Timer(new TimeSpan(0, 5, 0));
            timers.Add(timerMergeBoost);
            Timer timerFightBoss = new Timer(waitTime * 6);
            timers.Add(timerFightBoss);
            Timer timerAllocateEnergyMagic = new Timer(waitTime);
            timers.Add(timerAllocateEnergyMagic);
            
            timers.ForEach(x => x.Elapsed = true);

            bool fiveMinAllocationDone = config.RebirthTime.TotalMinutes > 10;
            bool timeMachieneUnlocked = false;
            
            TimeSpan runTime = rebirth.getRunTime();
            while (!Stop && (withoutRebirth || runTime < config.RebirthTime))
            {
                if (!timeMachieneUnlocked)
                {
                    NukeAndAdventure(isChallenge, timerMergeBoost, timerAllocateEnergyMagic, withoutRebirth);

                    if (!timeMachine.check())
                    {
                        inventory.Loadout(2);
                        
                        while (!augmentation.check() && config.Challenge != 2 && rebirth.getRunTime() < new TimeSpan(0, 30, 0))
                        {
                            NukeAndAdventure(true, timerMergeBoost, timerAllocateEnergyMagic, false);

                            wandoos.CapWandoos();
                        }

                        wandoos.CapWandoos();
                        window.Wait(10);
                        augmentation.SetAugmentations();

                        while (!timeMachine.check() && config.Challenge != 11 && rebirth.getRunTime() < new TimeSpan(0,30,0))
                        {
                            NukeAndAdventure(true, timerMergeBoost, timerAllocateEnergyMagic, false);

                            wandoos.CapWandoos();
                        }

                        NukeAndAdventure(isChallenge, timerMergeBoost, timerAllocateEnergyMagic, withoutRebirth);

                        timeMachine.SetSpeedAndMultiplier();

                        if (rebirth.getRunTime().TotalMilliseconds < (config.RebirthTime.TotalMilliseconds / 6) * 1)
                        {
                            timerAllocateEnergyMagic.Elapsed = true;
                        }
                        else
                        {
                            timerAllocateEnergyMagic.Restart();
                        }
                    }
                    else if (!withoutRebirth)
                    {
                        wandoos.CapWandoos();
                        timeMachine.SetSpeedAndMultiplier();
                        window.Wait(1);
                        augmentation.SetAugmentations();

                        if (rebirth.getRunTime().TotalMilliseconds < (config.RebirthTime.TotalMilliseconds / 6) * 1)
                        {
                            timerAllocateEnergyMagic.Elapsed = true;
                        }
                        else
                        {
                            timerAllocateEnergyMagic.Restart();
                        }
                    }

                    timeMachieneUnlocked = true;

                    runTime = rebirth.getRunTime();
                    continue;
                }

                if (!fiveMinAllocationDone && runTime > new TimeSpan(0, 5, 0))
                {
                    timerAllocateEnergyMagic.Elapsed = true;

                    fiveMinAllocationDone = true;
                }

                if (timerMaintenance.Elapsed)
                {
                    if (config.Exp.ShouldSpenExp)
                    {
                        while (spendEXP.AllocateEXP(config.Exp.EnergyCap, config.Exp.EnergyPower, config.Exp.EnergyBars,
                                                    config.Exp.MagicCap, config.Exp.MagicPower, config.Exp.MagicBars,
                                                    config.Exp.ThirdCap, config.Exp.ThirdPower, config.Exp.ThirdBars) > 0)
                        {
                            timerAllocateEnergyMagic.Elapsed = true;
                        }
                    }
                    else
                    {
                        spendEXP.GetEXP();
                    }

                    //questing.CompleteQuest();

                    goldDiggers.LevelDiggers();
                    moneyPit.Throw();

                    moneyPit.Spin();

                    bloodMagic.IronPill();

                    yggdrasil.HarvestAllMax();
                    
                    timerMaintenance.Restart();
                    runTime = rebirth.getRunTime();
                    continue;
                }

                if (timerMergeBoost.Elapsed)
                {
                    inventory.Merge(true, config.inventarSlotsMerge);
                    inventory.Boost(true, config.inventarSlotsBoost);

                    timerMergeBoost.Restart();
                    runTime = rebirth.getRunTime();
                    continue;
                }

                if (timerAllocateEnergyMagic.Elapsed)
                {
                    if (withoutRebirth && !isChallenge)
                    {
                        //kein rebirth
                        bloodMagic.setRituals(config.Rituals);
                        wandoos.CapWandoos();
                        ngu.SetNGUs();
                    }
                    else
                    {
                        int mode = 0;

                        if (runTime.TotalMilliseconds < (config.RebirthTime.TotalMilliseconds / 6) * 1)
                        {
                            //erstees sechstel
                            mode = 1;
                        }
                        else if (runTime.TotalMilliseconds > (config.RebirthTime.TotalMilliseconds / 6) * 5 || isChallenge)
                        {
                            //letztes sechstel
                            mode = 3;
                        }
                        else
                        {
                            //dazwischen
                            mode = 2;
                        }

                        if (mode == 1)
                        {
                            //erstes sechstel

                            if (!isChallenge)
                            {
                                goldDiggers.SetDiggersForFarm();
                            }
                            else
                            {
                                goldDiggers.SetDiggersForFight();
                            }

                            window.SendString("rt", false);
                            timeMachine.SetSpeedAndMultiplier();
                        }
                        else if (mode == 2)
                        {
                            //zweites bis einschließlich fünftes sechstel

                            if (!isChallenge)
                            {
                                goldDiggers.SetDiggersForFarm();
                            }
                            else
                            {
                                goldDiggers.SetDiggersForWandoos();
                            }

                            window.SendString("rt", false);

                            bloodMagic.setRituals(config.Rituals);

                            if (!isChallenge)
                            {
                                ngu.SetNGUs();
                            }
                            else
                            {
                                wandoos.CapWandoos();
                                augmentation.SetAugmentations();
                                ngu.SetNGUs();
                            }
                        }
                        else if (mode == 3)
                        {
                            //letztes sechstel

                            goldDiggers.SetDiggersForWandoos();

                            window.SendString("rt", false);
                            bloodMagic.setRituals(config.Rituals);
                            wandoos.CapWandoos();
                            augmentation.SetAugmentations();
                            ngu.SetNGUs();
                        }
                    }

                    timerAllocateEnergyMagic.Restart();
                    runTime = rebirth.getRunTime();
                    continue;
                }

                if (timerFightBoss.Elapsed)
                {
                    NukeAndAdventure(isChallenge, timerMergeBoost, timerAllocateEnergyMagic, withoutRebirth);

                    timerFightBoss.Restart();
                    runTime = rebirth.getRunTime();
                    continue;
                }

                if (isChallenge && rebirth.GetCurrentChallenge() == 0 && runTime.TotalMinutes > 3)
                {
                    break;
                }

                if (config.Snipe.ShouldSnipe)
                {
                    adventure.Farm(config.Snipe.SnipeStage, config.Snipe.SnipeSeconds, config.Snipe.SnipeBossOnly, config.FarmStage, fightBoss.GetCurrentBoss());
                }

                window.Wait(Math.Max(1, timers.Min(x => x.RemainingMilliseconds) / 1000));

                runTime = rebirth.getRunTime();
            }
        }
    }
}
