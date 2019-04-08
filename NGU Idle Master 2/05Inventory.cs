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
    static class InventoryConstants
    {
        #region points

        public static readonly Point pointPageInventory = new Point()
        {
            X = 237,
            Y = 173
        };

        public static readonly Point pointPage1 = new Point()
        {
            X = 346,
            Y = 586
        };

        public static readonly Point pointPage2 = new Point()
        {
            X = 411,
            Y = 586
        };

        public static readonly Point pointPage3 = new Point()
        {
            X = 476,
            Y = 586
        };

        public static readonly Point pointLoadout1 = new Point()
        {
            X = 339,
            Y = 265
        };

        public static readonly Point pointInventory1x1 = new Point()
        {
            X = 350,
            Y = 325
        };


        public static readonly Point pointEquipAcc9 = new Point()
        {
            X = 375,
            Y = 75
        };

        public static readonly Point pointEquipAcc10 = new Point()
        {
            X = 375,
            Y = 125
        };

        public static readonly Point pointEquipAcc11 = new Point()
        {
            X = 375,
            Y = 175
        };

        public static readonly Point pointEquipAcc12 = new Point()
        {
            X = 375,
            Y = 225
        };


        public static readonly Point pointEquipAcc5 = new Point()
        {
            X = 425,
            Y = 75
        };

        public static readonly Point pointEquipAcc6 = new Point()
        {
            X = 425,
            Y = 125
        };

        public static readonly Point pointEquipAcc7 = new Point()
        {
            X = 425,
            Y = 175
        };

        public static readonly Point pointEquipAcc8 = new Point()
        {
            X = 425,
            Y = 225
        };


        public static readonly Point pointEquipAcc1 = new Point()
        {
            X = 475,
            Y = 75
        };

        public static readonly Point pointEquipAcc2 = new Point()
        {
            X = 475,
            Y = 125
        };

        public static readonly Point pointEquipAcc3 = new Point()
        {
            X = 475,
            Y = 175
        };

        public static readonly Point pointEquipAcc4 = new Point()
        {
            X = 475,
            Y = 225
        };


        public static readonly Point pointEquipHead = new Point()
        {
            X = 525,
            Y = 75
        };

        public static readonly Point pointEquipBody = new Point()
        {
            X = 525,
            Y = 125
        };

        public static readonly Point pointEquipPants = new Point()
        {
            X = 525,
            Y = 175
        };

        public static readonly Point pointEquipBoots = new Point()
        {
            X = 525,
            Y = 225
        };


        public static readonly Point pointEquipWeapon = new Point()
        {
            X = 575,
            Y = 125
        };

        public static readonly Point pointEquipCube = new Point()
        {
            X = 625,
            Y = 125
        };

        #endregion
    }


    public class Inventory
    {
        List<Point> equipPoints = new List<Point>();

        NGUIdleMasterWindow window;
        public Inventory(NGUIdleMasterWindow window)
        {
            this.window = window;

            equipPoints.Add(InventoryConstants.pointEquipAcc9);
            equipPoints.Add(InventoryConstants.pointEquipAcc10);
            equipPoints.Add(InventoryConstants.pointEquipAcc11);
            equipPoints.Add(InventoryConstants.pointEquipAcc12);
            equipPoints.Add(InventoryConstants.pointEquipAcc5);
            equipPoints.Add(InventoryConstants.pointEquipAcc6);
            equipPoints.Add(InventoryConstants.pointEquipAcc7);
            equipPoints.Add(InventoryConstants.pointEquipAcc8);
            equipPoints.Add(InventoryConstants.pointEquipAcc1);
            equipPoints.Add(InventoryConstants.pointEquipAcc2);
            equipPoints.Add(InventoryConstants.pointEquipAcc3);
            equipPoints.Add(InventoryConstants.pointEquipAcc4);
            equipPoints.Add(InventoryConstants.pointEquipHead);
            equipPoints.Add(InventoryConstants.pointEquipBody);
            equipPoints.Add(InventoryConstants.pointEquipPants);
            equipPoints.Add(InventoryConstants.pointEquipBoots);
            equipPoints.Add(InventoryConstants.pointEquipWeapon);
        }

        public void Merge(bool equip, List<InventarSlot> inventarSlots)
        {
            window.Log($"Merge");

            window.Click(InventoryConstants.pointPageInventory, false, true);

            int page = 0;

            if (equip)
            {
                foreach (Point point in equipPoints)
                {
                    window.Click(point, false, true);
                    window.SendString("d", true);
                }
            }

            foreach (InventarSlot inventarSlot in inventarSlots)
            {
                if (inventarSlot.page < 1 || inventarSlot.page > 3 || inventarSlot.row < 1 || inventarSlot.row > 5 || inventarSlot.column < 1 || inventarSlot.column > 12)
                {
                    continue;
                }

                if (inventarSlot.page != page)
                {
                    switch (inventarSlot.page)
                    {
                        case 1:
                            window.Click(InventoryConstants.pointPage1, false, true);
                            break;
                        case 2:
                            window.Click(InventoryConstants.pointPage2, false, true);
                            break;
                        case 3:
                            window.Click(InventoryConstants.pointPage3, false, true);
                            break;
                    }

                    page = inventarSlot.page;
                }

                Point point = new Point(InventoryConstants.pointInventory1x1.X + (inventarSlot.column - 1) * 50, InventoryConstants.pointInventory1x1.Y + (inventarSlot.row - 1) * 50);

                window.Click(point, false, true);
                window.SendString("d", true);
            }
        }

        public void Merge(bool equip)
        {
            Merge(equip, new List<InventarSlot>());
        }

        public void Boost(bool equip, List<InventarSlot> inventarSlots)
        {
            window.Log($"Boost");

            window.Click(InventoryConstants.pointPageInventory, false, true);

            int page = 0;

            if (equip)
            {
                foreach (Point point in equipPoints)
                {
                    window.Click(point, false, true);
                    window.SendString("a", true);
                }
            }

            foreach (InventarSlot inventarSlot in inventarSlots)
            {
                if (inventarSlot.page < 1 || inventarSlot.page > 3 || inventarSlot.row < 1 || inventarSlot.row > 5 || inventarSlot.column < 1 || inventarSlot.column > 12)
                {
                    continue;
                }

                if (inventarSlot.page != page)
                {
                    switch (inventarSlot.page)
                    {
                        case 1:
                            window.Click(InventoryConstants.pointPage1, false, true);
                            break;
                        case 2:
                            window.Click(InventoryConstants.pointPage2, false, true);
                            break;
                        case 3:
                            window.Click(InventoryConstants.pointPage3, false, true);
                            break;
                    }

                    page = inventarSlot.page;
                }

                Point point = new Point(InventoryConstants.pointInventory1x1.X + (inventarSlot.column - 1) * 50, InventoryConstants.pointInventory1x1.Y + (inventarSlot.row - 1) * 50);

                window.Click(point, false, true);
                window.SendString("a", true);
            }

            window.Click(InventoryConstants.pointEquipCube, true, true);
        }

        public void Boost(bool equip)
        {
            Boost(equip, new List<InventarSlot>());
        }

        public void Loadout(int loadout)
        {
            window.Log($"Change Loadout: {loadout}");

            if (loadout < 1 || loadout > 10)
            {
                return;
            }

            window.Click(InventoryConstants.pointPageInventory, false, true);

            Point point = new Point(InventoryConstants.pointLoadout1.X + (loadout - 1) * 30, InventoryConstants.pointLoadout1.Y);

            window.Click(point, false, true);
        }
    }
}