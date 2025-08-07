using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class Item
    {
        ReadWriteMem RWMain;
        public int Index { get; set; } = 0;
        const int SizeOfStruct = 172;

        private int BaseAddress
        {
            get { return (SizeOfStruct * Index) + Address.Item_Struct; }
        }

        [Description("The index in the player's inventory system that corresponds to a specific item. This is used to track the position of the item within the inventory.")]
        public int InventoryIndex
        {
            get
            {
                Dictionary<int, int> InventoryData = new Inventory(RWMain).Items.AsDictionary;
                int i = Array.IndexOf(InventoryData.Keys.ToArray(), ID);
                if (i != -1) { return i; }
                else { return InventoryData.Count; }
            }
        }

        public Item(ReadWriteMem RW, int index)
        {
            RWMain = RW;
            Index = index;
        }

        public List<Item> GetItems()
        {
            Item ValidItem = new Item(RWMain, 0);
            List<Item> FoundItems = new List<Item>();
            List<string> FoundNames = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                ValidItem.Index = i;
                if (!string.IsNullOrEmpty(ValidItem.Name) && !FoundNames.Contains(ValidItem.Name))
                {
                    if (Regex.IsMatch(ValidItem.Name, @"^[a-zA-Z0-9_ ]+$"))
                    {
                        FoundItems.Add(new Item(RWMain, i));
                        FoundNames.Add(ValidItem.Name);
                    }
                }
            }
            return FoundItems.OrderBy(x => x.Name).ToList(); ;
        }

        [Description("A list containing multiple instances of the item (if applicable). For example, if a player has several of the same item, each instance is stored in this list.")]
        public List<int> Copies
        {
            get
            {
                List<int> Return = new List<int>();
                for (int i = 0; i < 10000; i++)
                {
                    if (new Item(RWMain, i).Name == Name)
                    {
                        Return.Add(i + 50000);
                    }
                }
                return Return;
            }
        }

        [Description("A raw byte array used for storing data related to the item. This could be used for storing serialized data or additional properties that are not directly reflected in other fields.")]
        public byte[] DataArray
        {
            get
            {
                List<byte> result = new List<byte>();
                int offset = 0;

                while (true)
                {
                    byte[] chunk = RWMain.Read<byte[]>(Address.Item_Struct + offset, SizeOfStruct);

                    // Check if chunk is all zeroes (end marker)
                    bool isZeroChunk = chunk.All(b => b == (byte)0x00);
                    if (isZeroChunk)
                        break;

                    result.AddRange(chunk);
                    offset += SizeOfStruct;
                }

                return result.ToArray();
            }
            set
            {
                // Optional: stop at first all-zero struct in value, if needed
                //RWMain.Write(Address.Map_Objects, value);
            }
        }

        public byte[] DataArrayStatic
        {
            get
            {
                List<byte> result = new List<byte>();
                int offset = 0;

                while (true)
                {
                    byte[] chunk = RWMain.Read<byte[]>(Address.Static_Item_Struct + offset, SizeOfStruct);

                    // Check if chunk is all zeroes (end marker)
                    bool isZeroChunk = chunk.All(b => b == 0);
                    if (isZeroChunk)
                        break;

                    result.AddRange(chunk);
                    offset += SizeOfStruct;
                }

                return result.ToArray();
            }
            set
            {
                // Optional: stop at first all-zero struct in value, if needed
                //RWMain.Write(Address.Map_Objects, value);
            }
        }

        public int ObjectCount
        {
            get { return DataArray.Length / SizeOfStruct; }
        }

        public int ObjectCountStatic
        {
            get { return DataArrayStatic.Length / SizeOfStruct; }
        }

        [Description("The name of the item (e.g., 'Stone Axe,' 'Bear Hide'). This is how the item is identified in the game world.")]
        public string Name // Item Name
        {
            get
            {
                return RWMain.Read<string>(BaseAddress + Offset.Name, 28);
            }
            set { RWMain.Write(BaseAddress + Offset.Name, value); }
        }


        [Description("A name used for display in the game’s interface, especially in inventory lists or UI elements that show a list of items. It could be a more readable or user-friendly version of the item’s name.")]
        public string ListBoxName
        {
            get
            {
                if (Quantity > 1)
                {
                    return string.Format("{0}{1} ({2})", QualityToString().Replace("Decent ", "").Replace("Decent(0) ", ""), Name, Quantity);
                }
                else if (Quantity == 1) { return string.Format("{0}{1}", QualityToString().Replace("Decent ", "").Replace("Decent(0) ", ""), Name); }
                else { return ""; }
            }
        }

        public int QuantityAt(Point Location, int Amount = -1) // set/get amount at object location
        {
            int retvalue = 0;
            bool ItemFound = false;
            MapObject Obj = new MapObject(RWMain, 0);
            if (Amount > -1) // Set
            {
                for (int i = 0; i < 220000 / 36; i++)
                {
                    Obj.Index = i;
                    if (Obj.Location == Location && Obj.ID == ID)
                    {
                        ItemFound = true;
                        break;
                    }
                }
                if (ItemFound) { }
                else { }
                retvalue = Amount;
            }
            else // Get
            {
                for (int i = 0; i < 220000 / 36; i++)
                {
                    Obj.Index = i;
                    if (Obj.Location == Location && Obj.ID == ID)
                    {
                        ItemFound = true;
                        break;
                    }
                }
                if (ItemFound) { }
                else { }
            }
            return retvalue;
        }

        [Description("The number of items of a particular type that the player owns. For example, if the player has 5 arrows, the quantity would be 5.")]
        public int Quantity // set/get inventory amount
        {
            get
            {
                if (InventoryIndex != -1) return new Inventory(RWMain).Items.Counts[InventoryIndex * 4];
                else return 0;
            }
            set
            {
                if (InventoryIndex != -1)
                {
                    byte[] intBytes = BitConverter.GetBytes(value);
                    Inventory I = new Inventory(RWMain);
                    byte[] Counts = I.Items.Counts;
                    Array.Copy(intBytes, 0, Counts, InventoryIndex * 4, 4);
                    I.Items.Counts = Counts;

                    if (value <= 0)
                    {
                        byte[] IDs = I.Items.IDs;
                        Array.Copy(intBytes, 0, IDs, InventoryIndex * 4, 4);
                        I.Items.IDs = IDs;
                        I.Items.AsDictionary = I.Items.AsDictionary;
                    }
                }
            }
        }

        [Description("A reference to the sprite or graphic used to represent the item visually in the game world or inventory.")]
        public string Sprite // Item sprite path
        {
            get
            {
                return RWMain.Read<string>(BaseAddress + Offset.Sprite, 28);
            }
            set { RWMain.Write(BaseAddress + Offset.Sprite, value); }
        }

        [Description("A category or classification for the item, such as 'Weapon,' 'Food,' 'Tool,' 'Clothing,' etc. This helps in organizing items in the game.")]
        public string Group // Item group
        {
            get
            {
                return RWMain.Read<string>(BaseAddress + Offset.Group, 28);
            }
            set { RWMain.Write(BaseAddress + Offset.Group, value); }
        }

        [Description("A unique identifier for the item. This ID is used internally by the game to track and reference a specific item across different systems.")]
        public int ID // Index/ID
        {
            get
            {
                return Index + 50000;
            }
        }

        [Description("The identifier for the type of item. This is a numeric value that categorizes the item into a specific type, such as 'Weapon,' 'Armor,' 'Consumable,' etc.")]
        public int TypeID // Item type
        {
            get
            {
                return RWMain.Read<int>(BaseAddress + Offset.TypeID);
            }
            set { RWMain.Write(BaseAddress + Offset.TypeID, value); }
        }

        [Description("The blunt damage rating of a weapon or tool. This value affects how effective the item is for blunt-force impact, like hitting with a club or hammer.")]
        public byte Blunt
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Blunt);
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
                        RWMain.Write(BaseAddress + Offset.Blunt, value); break;
                    default: break;
                }
            }
        }

        [Description("The edge damage rating, relevant to cutting weapons like axes, swords, or knives. This value affects how well the item can cut or slice.")]
        public byte Edge
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Edge);
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
                        RWMain.Write(BaseAddress + Offset.Edge, value); break;
                    default: break;
                }
            }
        }

        [Description("The point damage rating, relevant to stabbing or piercing weapons like spears, arrows, or daggers. This value represents how effective the weapon is for piercing.")]
        public byte Point
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Point);
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
                        RWMain.Write(BaseAddress + Offset.Point, value); break;
                    default: break;
                }
            }
        }

        [Description("Likely relates to the damage potential for tearing or slashing actions, possibly for items like claws, knives, or other tools used for cutting or skinning.")]
        public byte Tear
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Tear);
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
                        RWMain.Write(BaseAddress + Offset.Tear, value); break;
                    default: break;
                }
            }
        }

        [Description("This might refer to the force or effectiveness of squeezing or crushing actions, such as with certain tools or actions that require exerting pressure (e.g., squeezing a sponge or crushing something).")]
        public byte Squeeze
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Squeeze);
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
                        RWMain.Write(BaseAddress + Offset.Squeeze, value); break;
                    default: break;
                }
            }
        }

        [Description("A value representing how much warmth an item (typically clothing or a shelter) provides. This affects the player’s ability to survive cold environments.")]
        public byte Warmth
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Skin:
                        return RWMain.Read<byte>(BaseAddress + Offset.Warmth);
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
                        RWMain.Write(BaseAddress + Offset.Warmth, value); break;
                    default: break;
                }
            }
        }


        // Protein = 0x62, Mature = 0x63, Sprout = 0x64, Wither = 0x65,
        [Description("The amount of carbohydrates in a food item. This would contribute to the player’s stamina, energy, or health based on their food intake.")]
        public byte Carbohydrates
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWMain.Read<byte>(BaseAddress + Offset.Carb);
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
                        RWMain.Write(BaseAddress + Offset.Carb, value); break;
                    default: break;
                }
            }
        }

        [Description("The amount of fat in a food item, which also contributes to the player’s energy reserves. Fatty foods are often high in energy.")]
        public byte Fat
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWMain.Read<byte>(BaseAddress + Offset.Fat);
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
                        RWMain.Write(BaseAddress + Offset.Fat, value); break;
                    default: break;
                }
            }
        }

        [Description("The amount of protein in a food item, contributing to the player’s health, muscle repair, or general well-being.")]
        public byte Protein
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                    case ItemType.Carcass:
                        return RWMain.Read<byte>(BaseAddress + Offset.Protein);
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
                        RWMain.Write(BaseAddress + Offset.Protein, value); break;
                    default: break;
                }
            }
        }

        [Description("The state of maturity for a food item, plant, or crop. This could indicate whether a food source is ready to be harvested or used.")]
        public byte Mature
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWMain.Read<byte>(BaseAddress + Offset.Mature);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWMain.Write(BaseAddress + Offset.Mature, value); break;
                    default: break;
                }
            }
        }

        [Description("Likely refers to the stage in the growth cycle of a plant or crop. The sprout stage is when a plant first begins to grow.")]
        public byte Sprout
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWMain.Read<byte>(BaseAddress + Offset.Sprout);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWMain.Write(BaseAddress + Offset.Sprout, value); break;
                    default: break;
                }
            }
        }

        [Description("Indicates whether a plant, food, or crop is wilting or in the process of deteriorating. A high value may indicate that the item is nearing the end of its usefulness.")]
        public byte Wither
        {
            get
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        return RWMain.Read<byte>(BaseAddress + Offset.Wither);
                    default: return 0;
                }
            }
            set
            {
                switch (TypeID)
                {
                    case ItemType.Food:
                    case ItemType.Plant:
                        RWMain.Write(BaseAddress + Offset.Wither, value); break;
                    default: break;
                }
            }
        }

        [Description("The weight of the item, measured in kilograms or other unit of mass. This value is important for inventory management, movement, and encumbrance.")]
        public float Weight
        {
            get
            {
                if (TypeID != ItemType.Container)
                {
                    return RWMain.Read<float>(BaseAddress + Offset.Weight);
                }
                else { return ContentWeight; }
            }
            set
            {
                if (TypeID != ItemType.Container)
                {
                    RWMain.Write(BaseAddress + Offset.Weight, value);
                }
                else { RWMain.Write(BaseAddress + Offset.ContentWeight, value); }
            }
        }

        [Description("The amount of wear and tear an item can take before breaking or degrading. Weapons and tools in the game have limited durability, affecting their effectiveness over time.")]
        public float Durability
        {
            get
            {
                if (TypeID != ItemType.Container)
                {
                    return RWMain.Read<float>(BaseAddress + Offset.Durability);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID != ItemType.Container)
                {
                    RWMain.Write(BaseAddress + Offset.Durability, value);
                }
                else { throw new Exception("Containers don't have a durability property, check item type before setting value."); }
            }
        }

        [Description("The weight of the contents within an item. For example, a container (e.g., a bag or basket) may have a weight, plus the weight of the items stored inside.")]
        public float ContentWeight
        {
            get
            {
                if (TypeID == ItemType.Container)
                {
                    return RWMain.Read<float>(BaseAddress + Offset.ContentWeight);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID == ItemType.Container)
                {
                    RWMain.Write(BaseAddress + Offset.ContentWeight, value);
                }
                else { throw new Exception("Item is not a container, check item type before setting value."); }
            }
        }

        [Description("The amount of space or volume an item can hold. For example, a container like a backpack or barrel has a capacity for storing items.")]
        public float Capacity
        {
            get
            {
                if (TypeID == ItemType.Container)
                {
                    return RWMain.Read<float>(BaseAddress + Offset.Capacity);
                }
                else { return -1; }
            }
            set
            {
                if (TypeID == ItemType.Container)
                {
                    RWMain.Write(BaseAddress + Offset.Capacity, value);
                }
                else { throw new Exception("Item is not a container, check item type before setting value."); }
            }
        }

        [Description("The 'Attack Damage' bonus. This value could indicate how much additional damage a weapon or item provides during combat.")]
        public byte ADBonus // Attack & defence bonus
        {
            get
            {
                return RWMain.Read<byte>(BaseAddress + Offset.ADBonus);
            }
            set { RWMain.Write(BaseAddress + Offset.ADBonus, value); }
        }

        [Description("Refers to the armor coverage of an item, affecting how much damage the player can absorb in combat. The ArmorCoverage would likely include values for different body parts (e.g., head, torso, legs).")]
        public ArmorCoverage Armor
        {
            get
            {
                return (ArmorCoverage)RWMain.Read<int>(BaseAddress + Offset.ArmorFlags);
            }
            set
            {
                RWMain.Write(BaseAddress + Offset.ArmorFlags, BitConverter.GetBytes((int)value));
            }
        }

        [Description("The effects an item (likely a herb or consumable) has on the player when consumed. This could include healing, poisoning, or status effects (e.g., stamina boost, fatigue reduction).")]
        public HerbEffects ConsumptionEffects
        {
            get { return HerbEffects.None; }
            set { }
        }

        [Description("The quality of the item, which could affect its effectiveness, durability, or value. A higher quality item is typically more durable, useful, and valuable.")]
        public byte Quality
        {
            get
            {
                return RWMain.Read<byte>(BaseAddress + Offset.Quality);
            }
            set { RWMain.Write(BaseAddress + Offset.Quality, value); }
        }

        public string QualityToString()
        {
            try
            {
                return DefaultData.Qualities(TypeID)[Quality];
            }
            catch { return ""; }
        }

        [Description("The market value or trade value of the item, which may vary depending on quality, rarity, and other factors. Items can be sold or traded for value.")]
        public float Value
        {
            get
            {
                return RWMain.Read<float>(BaseAddress + Offset.Value);
            }
            set { RWMain.Write(BaseAddress + Offset.Value, value); }
        }

        [Description("A penalty that applies if a two-handed weapon is used in one hand. This could affect the item’s effectiveness, damage, or accuracy when used incorrectly.")]
        public byte OneHandPenalty
        {
            get
            {
                return RWMain.Read<byte>(BaseAddress + Offset.OneHandedPen);
            }
            set { RWMain.Write(BaseAddress + Offset.OneHandedPen, value); }
        }

        [Description("An identifier for the specific subtype of the item. This could categorize items further within their primary type, such as different kinds of axes or bows.")]
        public byte Subtype
        {
            get
            {
                return RWMain.Read<byte>(BaseAddress + Offset.Subtype);
            }
            set { RWMain.Write(BaseAddress + Offset.Subtype, value); }
        }

        [Description("The accuracy of a ranged weapon, such as a bow or thrown object. A higher value indicates more precision when attacking at a distance.")]
        public byte RangedAccuracy
        {
            get
            {
                return RWMain.Read<byte>(BaseAddress + Offset.RangedAccuracy);
            }
            set { RWMain.Write(BaseAddress + Offset.RangedAccuracy, value); }
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
        public string Json()
        {
            return this.ToJson();
        }

        public string Help()
        {
            return HelpGenerator.GenerateHelp<Item>();
        }
    }

}
