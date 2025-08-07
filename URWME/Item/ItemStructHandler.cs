using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace URWME
{
    public class ItemStructHandler
    {
        private ReadWriteMem RWMem;
        private ReadWriteBuffer RWBuffer;
        public int Index = 0;
        const int SizeOfStruct = 172;
        const int SizeOfArray = 10000; // maybe more

        private Item Item;

        private int BaseAddress
        {
            get { return SizeOfStruct * Index; }
        }

        public ItemStructHandler(ReadWriteMem RW)
        {
            RWMem = RW;
            RWBuffer = new ReadWriteBuffer(Buffer);
        }

        public byte[] Buffer
        {
            get
            {
                return RWMem.Read<byte[]>(Address.Item_Struct, SizeOfStruct * SizeOfArray);
            }
        }
        public string Name
        {
            get { return RWBuffer.ReadString(BaseAddress + Offset.Name, 28); }
            set { RWBuffer.WriteString(BaseAddress + Offset.Name, value); }
        }

        public int TypeID
        {
            get { return RWBuffer.Read<int>(BaseAddress + Offset.TypeID); }
            set { RWBuffer.Write(BaseAddress + Offset.TypeID, value); }
        }

        public List<ItemRef> GetItems()
        {
            List<ItemRef> FoundItems = new List<ItemRef>();
            List<string> FoundNames = new List<string>();

            for (int i = 0; i < 10000; i++)
            {
                Index = i;

                if (!string.IsNullOrEmpty(Name) && !FoundNames.Contains(Name))
                {
                    if (Regex.IsMatch(Name, @"^[a-zA-Z0-9_ ]+$"))
                    {
                        FoundItems.Add(new ItemRef(Name, i, Enum.GetName(typeof(ItemTypeEnum), TypeID).ToString()));
                        FoundNames.Add(Name);
                    }
                }
            }
            return FoundItems.OrderBy(x => x.Name).ToList(); ;
        }

        public string GetItemsJson()
        {
            return GetItems().ToJson();
        }

        public static class Offset
        {
            public static int
                TypeID = 0,
                Name = 0x9,
                Group = 0x31,
                Sprite = 0x4C,
                SpriteIndex = 0x5A,
                Blunt = 0x60, Edge = 0x61, Point = 0x62, Tear = 0x63, Squeeze = 0x64, Warmth = 0x65,
                Carb = 0x60, Fat = 0x61, Protein = 0x62, Mature = 0x63, Sprout = 0x64, Wither = 0x65,
                Length = 0x64, Length2 = 0x65,
                ArmorFlags = 0x6A, // - 6B
                HerbFlags = 0x88, // - 8A
                Weight = 0x70, // Also capacity of container
                Capacity = 0x70,
                Durability = 0x74, // Also content's weight for container
                ContentWeight = 0x74,
                Quality = 0xA5,
                Hidetimer = 0x59, // ?
                Value = 0x5C,
                OneHandedPen = 0x6A,
                Subtype = 0x6B,
                RangedAccuracy = 0x6C,
                HerbKnown = 0x93,
                GroupPrefix = 0xA6,
                ADBonus = 0xA7,
                WaterObj = 0x94,
                ItemIDRef = 0x78;
        }

        public class ItemRef
        {
            public string Name { get; set; }

            public string TypeID { get; set; }
            public int ID { get; set; }

            public ItemRef(string name, int id, string typeID)
            {
                Name = name;
                ID = id;
                TypeID = typeID;
            }
        }
    }
}
