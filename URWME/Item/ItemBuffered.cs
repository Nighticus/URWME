using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace URWME
{
    public class ItemBuffered
    {
        private ReadWriteBuffer RWBuffer;
        private ReadWriteMem RWMem;
        public int Index = 0;
        const int SizeOfStruct = 172;

        private Item Item;

        private int BaseAddress
        {
            get { return SizeOfStruct * Index; }
        }

        public ItemBuffered(ReadWriteMem RW, int index)
        {
            RWMem = RW;
            Index = index;

            Item = new Item(RWMem, index);
            RWBuffer = new ReadWriteBuffer(Item.DataArray);
        }

        public ItemBuffered(ReadWriteMem RW, ReadWriteBuffer RWB, int index)
        {
            RWMem = RW;
            RWBuffer = RWB;
            Index = index;
        }

        public List<ItemBuffered> GetItems()
        {
            List<ItemBuffered> FoundItems = new List<ItemBuffered>();
            List<string> FoundNames = new List<string>();
            for (int i = 0; i < ObjectCount; i++)
            {
                Index = i;
                if (!string.IsNullOrEmpty(Name) && !FoundNames.Contains(Name))
                {
                    if (Regex.IsMatch(Name, @"^[a-zA-Z0-9_ ]+$"))
                    {
                        FoundItems.Add(new ItemBuffered(RWMem, RWBuffer, i));
                        FoundNames.Add(Name);
                    }
                }
            }
            return FoundItems.OrderBy(x => x.Name).ToList(); ;
        }


        public string GetItemsJson
        {
            get
            {
                List<ItemBuffered> FoundItems = new List<ItemBuffered>();
                List<string> FoundNames = new List<string>();
                for (int i = 0; i < ObjectCount; i++)
                {
                    Index = i;
                    if (!string.IsNullOrEmpty(Name) && !FoundNames.Contains(Name))
                    {
                        if (Regex.IsMatch(Name, @"^[a-zA-Z0-9_ ]+$"))
                        {
                            FoundItems.Add(new ItemBuffered(RWMem, i));
                            FoundNames.Add(Name);
                        }
                    }
                }
                return JsonSerializer.Serialize(FoundItems.OrderBy(x => x.Name).ToList(), new JsonSerializerOptions { WriteIndented = true });
            }
        }

        public List<int> Copies
        {
            get
            {
                List<int> Return = new List<int>();
                string TargetName = Name; int i = Index;
                for (Index = 0; Index < ObjectCount; Index++)
                {
                    if (Name == TargetName)
                    {
                        Return.Add(Index + 50000);
                    }
                }
                Index = i;
                return Return;
            }
        }

        public void WriteArray()
        {
            Item.DataArray = RWBuffer.CurrentBuffer;
        }

        public void WriteArrayStatic()
        {
            Item.DataArrayStatic = RWBuffer.CurrentBuffer;
        }

        public void ReadArray()
        {
            RWBuffer.CurrentBuffer = Item.DataArray;
        }

        public void ReadArrayStatic()
        {
            RWBuffer.CurrentBuffer = Item.DataArrayStatic;
        }

        public int ObjectCount
        {
            get { return Item.ObjectCount; }
        }

        public int ObjectCountStatic
        {
            get { return Item.ObjectCountStatic; }
        }

        public string Name
        {
            get { return RWBuffer.ReadString(BaseAddress + Offset.Name, 28); }
            set { RWBuffer.WriteString(BaseAddress + Offset.Name, value); }
        }

        public string Sprite
        {
            get { return RWBuffer.ReadString(BaseAddress + Offset.Sprite, 28); }
            set { RWBuffer.WriteString(BaseAddress + Offset.Sprite, value); }
        }

        public string Group
        {
            get { return RWBuffer.ReadString(BaseAddress + Offset.Group, 28); }
            set { RWBuffer.WriteString(BaseAddress + Offset.Group, value); }
        }

        public int TypeID
        {
            get { return RWBuffer.Read<int>(BaseAddress + Offset.TypeID); }
            set { RWBuffer.Write(BaseAddress + Offset.TypeID, value); }
        }

        public byte Blunt
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Blunt);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Blunt, value); break;
                    default: break;
                }
            }
        }

        public byte Edge
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Edge);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Edge, value); break;
                    default: break;
                }
            }
        }

        public byte Point
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Point);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Point, value); break;
                    default: break;
                }
            }
        }

        public byte Tear
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Tear);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Tear, value); break;
                    default: break;
                }
            }
        }

        public byte Squeeze
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Squeeze);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Squeeze, value); break;
                    default: break;
                }
            }
        }

        public byte Warmth
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Warmth);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        RWBuffer.Write(BaseAddress + Offset.Warmth, value); break;
                    default: break;
                }
            }
        }

        // Protein = 0x62, Mature = 0x63, Sprout = 0x64, Wither = 0x65,
        public byte Carbohydrates
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Carb);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        RWBuffer.Write(BaseAddress + Offset.Carb, value); break;
                    default: break;
                }
            }
        }

        public byte Fat
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Fat);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        RWBuffer.Write(BaseAddress + Offset.Fat, value); break;
                    default: break;
                }
            }
        }

        public byte Protein
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Protein);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        RWBuffer.Write(BaseAddress + Offset.Protein, value); break;
                    default: break;
                }
            }
        }

        public byte Mature
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Mature);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWBuffer.Write(BaseAddress + Offset.Mature, value); break;
                    default: break;
                }
            }
        }

        public byte Sprout
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Sprout);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWBuffer.Write(BaseAddress + Offset.Sprout, value); break;
                    default: break;
                }
            }
        }

        public byte Wither
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWBuffer.Read<byte>(BaseAddress + Offset.Wither);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWBuffer.Write(BaseAddress + Offset.Wither, value); break;
                    default: break;
                }
            }
        }


        public float Weight
        {
            get
            {
                if (TypeID != ItemType.Container)
                {
                    return RWBuffer.Read<float>(BaseAddress + Offset.Weight);
                }
                else { return ContentWeight; }
            }
            set
            {
                if (TypeID != ItemType.Container)
                {
                    RWBuffer.Write(BaseAddress + Offset.Weight, value);
                }
                else { RWBuffer.Write(BaseAddress + Offset.ContentWeight, value); }
            }
        }

        public float Durability
        {
            get
            {
                if (TypeID != ItemType.Container)
                {
                    return RWBuffer.Read<float>(BaseAddress + Offset.Durability);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID != ItemType.Container)
                {
                    RWBuffer.Write(BaseAddress + Offset.Durability, value);
                }
                else { throw new Exception("Containers don't have a durability property, check item type before setting value."); }
            }
        }

        public float ContentWeight
        {
            get
            {
                if (TypeID == ItemType.Container)
                {
                    return RWBuffer.Read<float>(BaseAddress + Offset.ContentWeight);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID == ItemType.Container)
                {
                    RWBuffer.Write(BaseAddress + Offset.ContentWeight, value);
                }
                else { throw new Exception("Item is not a container, check item type before setting value."); }
            }
        }

        public float Capacity
        {
            get
            {
                if (TypeID == ItemType.Container)
                {
                    return RWBuffer.Read<float>(BaseAddress + Offset.Capacity);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID == ItemType.Container)
                {
                    RWBuffer.Write(BaseAddress + Offset.Capacity, value);
                }
                else { throw new Exception("Item is not a container, check item type before setting value."); }
            }
        }

        public byte ADBonus
        {
            get { return RWBuffer.Read<byte>(BaseAddress + Offset.ADBonus); }
            set { RWBuffer.Write(BaseAddress + Offset.ADBonus, value); }
        }

        public ArmorCoverage Armor
        {
            get { return (ArmorCoverage)RWBuffer.Read<short>(BaseAddress + Offset.ArmorFlags); }
            set { RWBuffer.Write(BaseAddress + Offset.ArmorFlags, (short)value); }
        }

        public HerbEffects ConsumptionEffects
        {
            get { return HerbEffects.None; }
            set { }
        }

        public byte Quality
        {
            get { return RWBuffer.Read<byte>(BaseAddress + Offset.Quality); }
            set { RWBuffer.Write(BaseAddress + Offset.Quality, value); }
        }

        public string QualityToString()
        {
            try
            {
                return DefaultData.Qualities(TypeID)[Quality];
            }
            catch { return ""; }
        }

        public float Value
        {
            get { return RWBuffer.Read<float>(BaseAddress + Offset.Value); }
            set { RWBuffer.Write(BaseAddress + Offset.Value, value); }
        }

        public byte OneHandPenalty
        {
            get { return RWBuffer.Read<byte>(BaseAddress + Offset.OneHandedPen); }
            set { RWBuffer.Write(BaseAddress + Offset.OneHandedPen, value); }
        }

        public byte Subtype
        {
            get { return RWBuffer.Read<byte>(BaseAddress + Offset.Subtype); }
            set { RWBuffer.Write(BaseAddress + Offset.Subtype, value); }
        }

        public byte RangedAccuracy
        {
            get { return RWBuffer.Read<byte>(BaseAddress + Offset.RangedAccuracy); }
            set { RWBuffer.Write(BaseAddress + Offset.RangedAccuracy, value); }
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

        public string Help()
        {
            return HelpGenerator.GenerateHelp<Item>();
        }
    }
}
