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
            Top = 312 + 5,
            Right = 784,
            Bottom = 326 + 7
        };

        public static readonly RECT rectTotalCap = new RECT()
        {
            Left = 648,
            Top = 263 + 5,
            Right = 784,
            Bottom = 277 + 7
        };

        public static readonly RECT rectBasePower = new RECT()
        {
            Left = 482,
            Top = 312 + 5,
            Right = 618,
            Bottom = 326 + 7
        };

        public static readonly RECT rectTotalPower = new RECT()
        {
            Left = 482,
            Top = 263 + 5,
            Right = 618,
            Bottom = 277 + 7
        };

        public static readonly RECT rectBaseBar = new RECT()
        {
            Left = 805,
            Top = 312 + 5,
            Right = 941,
            Bottom = 326 + 7
        };

        public static readonly RECT rectTotalBar = new RECT()
        {
            Left = 805,
            Top = 263 + 5,
            Right = 941,
            Bottom = 277 + 7
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
            X = 355,
            Y = 119
        };

        public static readonly Point pointPageMagic = new Point()
        {
            X = 430,
            Y = 119
        };

        public static readonly Point pointFieldPower = new Point()
        {
            X = 552,
            Y = 530 + 7
        };

        public static readonly Point pointFieldCap = new Point()
        {
            X = 718,
            Y = 530 + 7
        };

        public static readonly Point pointFieldBar = new Point()
        {
            X = 866,
            Y = 530 + 7
        };

        public static readonly Point pointBuyPower = new Point()
        {
            X = 552,
            Y = 565 + 7
        };

        public static readonly Point pointBuyCap = new Point()
        {
            X = 718,
            Y = 565 + 7
        };

        public static readonly Point pointBuyBar = new Point()
        {
            X = 866,
            Y = 565 + 7
        };

        #endregion

        #region costs

        public static readonly BigInteger costECapPer10000 = 40;
        public static readonly BigInteger costEPowerPer10000 = 1500000;
        public static readonly BigInteger costEBarPer10000 = 800000;

        public static readonly BigInteger costMCapPer10000 = 120;
        public static readonly BigInteger costMPowerPer10000 = 4500000;
        public static readonly BigInteger costMBarPer10000 = 2400000;

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

        //public BigInteger AllocateEXP(int energyToManaRatio, double minutesTillFilled)
        //{
        //    BigInteger startExp = GetEXP();

        //    if (startExp == 0)
        //    {
        //        return startExp;
        //    }

        //    window.Click(pointPageSpendEXP, false, true);
        //    BigInteger eBaseCap = window.Parse(window.OCRTextSearch(rectBaseCap, false));
        //    BigInteger eTotalCap = window.Parse(window.OCRTextSearch(rectTotalCap, false));
        //    BigInteger eBasePower = window.Parse(window.OCRTextSearch(rectBasePower, false));
        //    BigInteger eTotalPower = window.Parse(window.OCRTextSearch(rectTotalPower, false));
        //    BigInteger eBaseBar = window.Parse(window.OCRTextSearch(rectBaseBar, false));
        //    BigInteger eTotalBar = window.Parse(window.OCRTextSearch(rectTotalBar, false));
            
        //    double eBaseToTotalCap = ((double)((eTotalCap * 10000) / eBaseCap)) / 10000;
        //    double eBaseToTotalPower = ((double)((eTotalPower * 10000) / eBasePower)) / 10000;
        //    double eBaseToTotalBar = ((double)((eTotalBar * 10000) / eBaseBar)) / 10000;

        //    BigInteger eBarNeeded = eBaseBar - (int)((double)(eTotalCap / (int)(50 * 60 * minutesTillFilled)) / eBaseToTotalBar);

        //    return 0;
        //}

        public BigInteger AllocateEXP(int eCapRatio, int ePowerRatio, int eBarRatio, int mCapRatio, int mPowerRatio, int mBarRatio)
        {
            BigInteger costPerUnit = ((eCapRatio * SpendEXPConstants.costECapPer10000) / 10000)
                                        + ((ePowerRatio * SpendEXPConstants.costEPowerPer10000) / 10000)
                                        + ((eBarRatio * SpendEXPConstants.costEBarPer10000) / 10000)
                                        + ((mCapRatio * SpendEXPConstants.costMCapPer10000) / 10000)
                                        + ((mPowerRatio * SpendEXPConstants.costMPowerPer10000) / 10000)
                                        + ((mBarRatio * SpendEXPConstants.costMBarPer10000) / 10000);

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

            BigInteger eCapCurrentUnits = eCap / eCapRatio;
            BigInteger ePowerCurrentUnits = ePower / ePowerRatio;
            BigInteger eBarCurrentUnits = eBar / eBarRatio;
            BigInteger mCapCurrentUnits = mCap / mCapRatio;
            BigInteger mPowerCurrentUnits = mPower / mPowerRatio;
            BigInteger mBarCurrentUnits = mBar / mBarRatio;

            BigInteger maxCurrentUnits = BigInteger.Max(eCapCurrentUnits, BigInteger.Max(ePowerCurrentUnits, BigInteger.Max(eBarCurrentUnits, BigInteger.Max(mCapCurrentUnits, BigInteger.Max(mPowerCurrentUnits, mBarCurrentUnits)))));
            BigInteger minCurrentUnits = BigInteger.Min(eCapCurrentUnits, BigInteger.Min(ePowerCurrentUnits, BigInteger.Min(eBarCurrentUnits, BigInteger.Min(mCapCurrentUnits, BigInteger.Min(mPowerCurrentUnits, mBarCurrentUnits)))));

            BigInteger targetUnits = minCurrentUnits + (startExp / costPerUnit);

            BigInteger eCapTarget = targetUnits * eCapRatio;
            BigInteger ePowerTarget = targetUnits * ePowerRatio;
            BigInteger eBarTarget = targetUnits * eBarRatio;
            BigInteger mCapTarget = targetUnits * mCapRatio;
            BigInteger mPowerTarget = targetUnits * mPowerRatio;
            BigInteger mBarTarget = targetUnits * mBarRatio;

            BigInteger eCapNeeded = eCapTarget - eCap;
            BigInteger ePowerNeeded = ePowerTarget - ePower;
            BigInteger eBarNeeded = eBarTarget - eBar;
            BigInteger mCapNeeded = mCapTarget - mCap;
            BigInteger mPowerNeeded = mPowerTarget - mPower;
            BigInteger mBarNeeded = mBarTarget - mBar;
            
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

            BigInteger endExp = GetEXP();
            
            return startExp - endExp;
        }
    }
}
