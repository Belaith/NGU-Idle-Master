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
        public bool ShouldSpenExp { get; set; }
        public int Rituals { get; set; }
        public int MaxFarmForGold { get; set; }
        public bool Snipe { get; set; }
        public int SnipeStage { get; set; }
        public int SnipeSeconds { get; set; }
        public bool SnipeBossOnly { get; set; }
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
