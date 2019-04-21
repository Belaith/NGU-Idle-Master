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
    static class SpendEXPConstants
    {
        #region rects

        public static readonly RECT rectExp = new RECT()
        {
            Left = 340,
            Top = 80,
            Right = 900,
            Bottom = 105
        };

        public static readonly RECT rectBaseCap = new RECT()
        {
            Left = 648,
            Top = 312 - 8,
            Right = 784,
            Bottom = 326 - 6
        };

        public static readonly RECT rectTotalCap = new RECT()
        {
            Left = 648,
            Top = 263 - 8,
            Right = 784,
            Bottom = 277 - 6
        };

        public static readonly RECT rectBasePower = new RECT()
        {
            Left = 482,
            Top = 312 - 8,
            Right = 618,
            Bottom = 326 - 6
        };

        public static readonly RECT rectTotalPower = new RECT()
        {
            Left = 482,
            Top = 263 - 8,
            Right = 618,
            Bottom = 277 - 6
        };

        public static readonly RECT rectBaseBar = new RECT()
        {
            Left = 805,
            Top = 312 - 8,
            Right = 941,
            Bottom = 326 - 6
        };

        public static readonly RECT rectTotalBar = new RECT()
        {
            Left = 805,
            Top = 263 - 8,
            Right = 941,
            Bottom = 277 - 6
        };

        #endregion

        #region points

        public static readonly Point pointPageSpendEXP = new Point()
        {
            X = 92,
            Y = 454
        };

        public static readonly Point pointPageEnergy = new Point()
        {
            X = 405,
            Y = 119
        };

        public static readonly Point pointPageMagic = new Point()
        {
            X = 475,
            Y = 119
        };

        public static readonly Point pointPageThird = new Point()
        {
            X = 405,
            Y = 149
        };

        public static readonly Point pointFieldPower = new Point()
        {
            X = 552,
            Y = 530 - 6
        };

        public static readonly Point pointFieldCap = new Point()
        {
            X = 718,
            Y = 530 - 6
        };

        public static readonly Point pointFieldBar = new Point()
        {
            X = 866,
            Y = 530 - 6
        };

        public static readonly Point pointBuyPower = new Point()
        {
            X = 552,
            Y = 565 - 6
        };

        public static readonly Point pointBuyCap = new Point()
        {
            X = 718,
            Y = 565 - 6
        };

        public static readonly Point pointBuyBar = new Point()
        {
            X = 866,
            Y = 565 - 6
        };

        #endregion

        #region costs

        public static readonly BigInteger costEnergyCapPer10000 = 40;
        public static readonly BigInteger costEnergyPowerPer10000 = 1500000;
        public static readonly BigInteger costEnergyBarPer10000 = 800000;

        public static readonly BigInteger costMagicCapPer10000 = 120;
        public static readonly BigInteger costMagicPowerPer10000 = 4500000;
        public static readonly BigInteger costMagicBarPer10000 = 2400000;

        public static readonly BigInteger costThirdCapPer10000 = 4000000;
        public static readonly BigInteger costThirdPowerPer10000 = 150000000000;
        public static readonly BigInteger costThirdBarPer10000 = 80000000000;

        #endregion
    }

    public class SpendEXP
    {
        BigInteger currentExp = new BigInteger(-1);
        BigInteger gainedExp = new BigInteger(0);
        BigInteger allocatedExp = new BigInteger(0);

        NGUIdleMasterWindow window;

        public SpendEXP(NGUIdleMasterWindow window)
        {
            this.window = window;
            
            GetEXP();
        }

        public BigInteger GetEXP()
        {
            if (window.Locked)
            {
                return -1;
            }

            window.Click(SpendEXPConstants.pointPageSpendEXP, false, true);
            string input = window.OCRTextSearch(SpendEXPConstants.rectExp, false);

            int start = input.IndexOf("have ") + 5;
            int end = input.IndexOf(" EXP");
            int length = end - start;

            if (start < 1 || length < 1 || start + length > input.Length)
            {
                return new BigInteger(0);
            }

            input = input.Substring(start, length);

            BigInteger exp = window.Parse(input);

            string diffSinceLast = string.Empty;

            if (currentExp == -1)
            {
                window.Log($"Current Exp: {exp,5}");
            }
            else if (currentExp > -1 && currentExp != exp)
            {
                if (exp > currentExp)
                {
                    gainedExp += exp - currentExp;
                }
                else if (exp < currentExp)
                {
                    allocatedExp += currentExp - exp;
                }

                window.Log($"Current Exp: {exp,5} - GainedExp: {gainedExp,5} - SpendExp {allocatedExp,5} - DiffSinceLast: {exp - currentExp, 6}");
            }

            currentExp = exp;

            return exp;
        }
                
        public BigInteger AllocateEXP(long eCapRatio, long ePowerRatio, long eBarRatio, long mCapRatio, long mPowerRatio, long mBarRatio, long tCapRatio, long tPowerRatio, long tBarRatio)
        {

            bool spendThird = tCapRatio > 0 || tPowerRatio > 0 || tBarRatio > 0;
            BigInteger costPerUnit = (eCapRatio > 0 ? eCapRatio * SpendEXPConstants.costEnergyCapPer10000 / 10000 : 0)
                                        + (ePowerRatio > 0 ? ePowerRatio * SpendEXPConstants.costEnergyPowerPer10000 / 10000 : 0)
                                        + (eBarRatio > 0 ? eBarRatio * SpendEXPConstants.costEnergyBarPer10000 / 10000 : 0)
                                        + (mCapRatio > 0 ? mCapRatio * SpendEXPConstants.costMagicCapPer10000 / 10000 : 0)
                                        + (mPowerRatio > 0 ? mPowerRatio * SpendEXPConstants.costMagicPowerPer10000 / 10000 : 0)
                                        + (mBarRatio > 0 ? mBarRatio * SpendEXPConstants.costThirdBarPer10000 / 10000 : 0)
                                        + (tCapRatio > 0 ? tCapRatio * SpendEXPConstants.costThirdCapPer10000 / 10000 : 0)
                                        + (tPowerRatio > 0 ? tPowerRatio * SpendEXPConstants.costThirdPowerPer10000 / 10000 : 0)
                                        + (tBarRatio > 0 ? tBarRatio * SpendEXPConstants.costThirdBarPer10000 / 10000 : 0);

            BigInteger startExp = GetEXP();

            if (startExp < costPerUnit)
            {
                return 0;
            }

            window.Click(SpendEXPConstants.pointPageSpendEXP, false, true);
            BigInteger eCap = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseCap, false));
            BigInteger ePower = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBasePower, false));
            BigInteger eBar = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseBar, false));

            window.Click(SpendEXPConstants.pointPageMagic, false, true);
            BigInteger mCap = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseCap, false));
            BigInteger mPower = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBasePower, false));
            BigInteger mBar = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseBar, false));

            BigInteger tCap = 0;
            BigInteger tPower = 0;
            BigInteger tBar = 0;

            if (spendThird)
            {
                window.Click(SpendEXPConstants.pointPageThird, false, true);
                tCap = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseCap, false));
                tPower = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBasePower, false));
                tBar = window.Parse(window.OCRTextSearch(SpendEXPConstants.rectBaseBar, false));
            }

            BigInteger eCapCurrentUnits = eCap > 0 && eCapRatio > 0 ? eCap / eCapRatio : -1;
            BigInteger ePowerCurrentUnits = ePower > 0 && ePowerRatio > 0 ? ePower / ePowerRatio : -1;
            BigInteger eBarCurrentUnits = eBar > 0 && eBarRatio > 0 ? eBar / eBarRatio : -1;
            BigInteger mCapCurrentUnits = mCap > 0 && mCapRatio > 0 ? mCap / mCapRatio : -1;
            BigInteger mPowerCurrentUnits = mPower > 0 && mPowerRatio > 0 ? mPower / mPowerRatio : -1;
            BigInteger mBarCurrentUnits = mBar > 0 && mBarRatio > 0 ? mBar / mBarRatio : -1;
            BigInteger tCapCurrentUnits = tCap > 0 && tCapRatio > 0 ? tCap / tCapRatio : -1;
            BigInteger tPowerCurrentUnits = tPower > 0 && tPowerRatio > 0 ? tPower / tPowerRatio : -1;
            BigInteger tBarCurrentUnits = tBar > 0 && tBarRatio > 0 ? tBar / tBarRatio : -1;

            //todo funktion die das macht und -1 ignoriert
            BigInteger maxCurrentUnits = max(new BigInteger[] { eCapCurrentUnits, ePowerCurrentUnits, eBarCurrentUnits, mCapCurrentUnits, mPowerCurrentUnits, mBarCurrentUnits, tCapCurrentUnits, tPowerCurrentUnits, tBarCurrentUnits });
            BigInteger minCurrentUnits = min(new BigInteger[] { eCapCurrentUnits, ePowerCurrentUnits, eBarCurrentUnits, mCapCurrentUnits, mPowerCurrentUnits, mBarCurrentUnits, tCapCurrentUnits, tPowerCurrentUnits, tBarCurrentUnits });

            BigInteger targetUnits = minCurrentUnits + (startExp / costPerUnit);

            BigInteger eCapTarget = targetUnits * eCapRatio;
            BigInteger ePowerTarget = targetUnits * ePowerRatio;
            BigInteger eBarTarget = targetUnits * eBarRatio;
            BigInteger mCapTarget = targetUnits * mCapRatio;
            BigInteger mPowerTarget = targetUnits * mPowerRatio;
            BigInteger mBarTarget = targetUnits * mBarRatio;
            BigInteger tCapTarget = targetUnits * tCapRatio;
            BigInteger tPowerTarget = targetUnits * tPowerRatio;
            BigInteger tBarTarget = targetUnits * tBarRatio;

            BigInteger eCapNeeded = eCapTarget - eCap;
            BigInteger ePowerNeeded = ePowerTarget - ePower;
            BigInteger eBarNeeded = eBarTarget - eBar;
            BigInteger mCapNeeded = mCapTarget - mCap;
            BigInteger mPowerNeeded = mPowerTarget - mPower;
            BigInteger mBarNeeded = mBarTarget - mBar;
            BigInteger tCapNeeded = tCapTarget - tCap;
            BigInteger tPowerNeeded = tPowerTarget - tPower;
            BigInteger tBarNeeded = tBarTarget - tBar;

            window.Click(SpendEXPConstants.pointPageEnergy, false, true);
            if (eBarNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldBar, false, false);
                window.SendString(eBarNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyBar, false, false);
            }
            if (ePowerNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldPower, false, false);
                window.SendString(ePowerNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyPower, false, false);
            }
            if (eCapNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldCap, false, false);
                window.SendString(eCapNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyCap, false, false);
            }

            window.Click(SpendEXPConstants.pointPageMagic, false, true);
            if (mBarNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldBar, false, false);
                window.SendString(mBarNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyBar, false, false);
            }
            if (mPowerNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldPower, false, false);
                window.SendString(mPowerNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyPower, false, false);
            }
            if (mCapNeeded > 0)
            {
                window.Click(SpendEXPConstants.pointFieldCap, false, false);
                window.SendString(mCapNeeded.ToString(), false);
                window.Click(SpendEXPConstants.pointBuyCap, false, false);
            }

            if (spendThird)
            {
                window.Click(SpendEXPConstants.pointPageThird, false, true);
                if (mBarNeeded > 0)
                {
                    window.Click(SpendEXPConstants.pointFieldBar, false, false);
                    window.SendString(tBarNeeded.ToString(), false);
                    window.Click(SpendEXPConstants.pointBuyBar, false, false);
                }
                if (mPowerNeeded > 0)
                {
                    window.Click(SpendEXPConstants.pointFieldPower, false, false);
                    window.SendString(tPowerNeeded.ToString(), false);
                    window.Click(SpendEXPConstants.pointBuyPower, false, false);
                }
                if (mCapNeeded > 0)
                {
                    window.Click(SpendEXPConstants.pointFieldCap, false, false);
                    window.SendString(tCapNeeded.ToString(), false);
                    window.Click(SpendEXPConstants.pointBuyCap, false, false);
                }
            }

            BigInteger endExp = GetEXP();

            return startExp - endExp;
        }

        private BigInteger min(BigInteger[] numbers)
        {
            BigInteger min = 0;

            foreach (BigInteger number in numbers)
            {
                if (number > 0 && (min == 0 || number < min))
                {
                    min = number;
                }
            }

            return min;
        }

        private BigInteger max(BigInteger[] numbers)
        {
            BigInteger max = 0;

            foreach (BigInteger number in numbers)
            {
                if (number > max)
                {
                    max = number;
                }
            }

            return max;
        }
    }
}
