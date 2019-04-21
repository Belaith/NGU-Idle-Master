using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NGU_Idle_Master
{
    [Serializable]
    public class Config
    {
        public string TesseractPath { get; set; }
        public string LogPath { get; set; }
        public List<InventarSlot> inventarSlotsMerge { get; set; }
        public List<InventarSlot> inventarSlotsBoost { get; set; }
        public int Challenge { get; set; }
        public Exp Exp { get; set; }
        public int Rituals { get; set; }
        public int MaxFarmForGold { get; set; }
        public Snipe Snipe { get; set; }
        public int FarmStage { get; set; }


        [XmlIgnore]
        public TimeSpan RebirthTime { get; set; }
        [XmlElement(DataType = "duration", ElementName = "RebirthTime")]
        public string RebirthTimeXML
        {
            get { return RebirthTime.ToString(); }
            set { RebirthTime = TimeSpan.Parse(value); }
        }
    }

    [Serializable]
    public class Exp
    {
        [XmlAttribute]
        public bool ShouldSpenExp { get; set; }
        public long EnergyCap { get; set; }
        public long EnergyPower { get; set; }
        public long EnergyBars { get; set; }
        public long MagicCap { get; set; }
        public long MagicPower { get; set; }
        public long MagicBars { get; set; }
        public long ThirdCap { get; set; }
        public long ThirdPower { get; set; }
        public long ThirdBars { get; set; }
    }

    [Serializable]
    public class Snipe
    {
        [XmlAttribute]
        public bool ShouldSnipe { get; set; }
        public int SnipeStage { get; set; }
        public int SnipeSeconds { get; set; }
        public bool SnipeBossOnly { get; set; }
    }

    [Serializable]
    public class InventarSlot
    {
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int row { get; set; }
        [XmlAttribute]
        public int column { get; set; }

        public InventarSlot()
        {
        }

        public InventarSlot(int page, int row, int column)
        {
            this.page = page;
            this.row = row;
            this.column = column;
        }
    }
}
