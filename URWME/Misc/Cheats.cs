using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class Cheats
    {
        ReadWriteMem RWMain;
        Player player;
        public MapObjectHandler MapObjectCheats;
        public ItemStructHandler ItemStructCheats;

        private bool _noNeeds;
        private bool _noWeight;
        private bool _treeVision;
        private bool _noInjuries;
        private bool _cannibalism;
        private bool _freezeNPC;
        private bool _freezeTime;
        private bool _checksum;

        private byte[] _hungerGain;
        private byte[] _thirstGain;
        private byte[] _fatigueGain;
        private byte[] _nutritionLoss;
        private byte[] _energyLoss;
        private byte[] _weightChange;
        private byte[] _isInTreeCheck;
        private byte[] _temperatureChange;
        private byte[] _starvationLoss;
        private byte[] _attrbiuteChecksum = new byte[] { 0x72, 0x83 };
        private byte[] _skillChecksum = new byte[]{ 0x43 };

        public Cheats(ReadWriteMem RW)
        {
            RWMain = RW;
            player = new Player(RWMain);
            MapObjectCheats = new MapObjectHandler(RWMain);
            ItemStructCheats = new ItemStructHandler(RWMain);
            Checksum = true;
            Initialize();
        }

        public void Initialize()
        {
            _hungerGain = RWMain.Read<byte[]>(Code.PC_HungerGain, 6);
            _thirstGain = RWMain.Read<byte[]>(Code.PC_ThirstGain, 6);
            _fatigueGain = RWMain.Read<byte[]>(Code.PC_FatigueGain, 8);
            _nutritionLoss = RWMain.Read<byte[]>(Code.PC_NutritionLoss, 5);
            _energyLoss = RWMain.Read<byte[]>(Code.PC_EnergyLoss, 5);
            _weightChange = RWMain.Read<byte[]>(Code.PC_WeightChange, 8);
            _isInTreeCheck = RWMain.Read<byte[]>(Code.PC_IsInTreeCheck, 6);
            _temperatureChange = RWMain.Read<byte[]>(Code.PC_TemperatureChange, 8);
            _starvationLoss = RWMain.Read<byte[]>(Code.PC_IsInTreeCheck, 6);
        }

        public void Cleanup()
        {
            NoNeeds = false;
            NoCarryWeight = false;
            TreeVision = false;
            Cannibalism = false;
            Checksum = false;
        }

        public bool NoNeeds
        {
            get { return _noNeeds; }
            set
            {
                if (_noNeeds != value)
                {
                    _noNeeds = value;
                    if (_noNeeds)
                    {
                        RWMain.Write(Code.PC_HungerGain, Enumerable.Repeat((byte)144, 6).ToArray());
                        RWMain.Write(Code.PC_ThirstGain, Enumerable.Repeat((byte)144, 6).ToArray());
                        RWMain.Write(Code.PC_NutritionLoss, Enumerable.Repeat((byte)144, 5).ToArray());
                        RWMain.Write(Code.PC_EnergyLoss, Enumerable.Repeat((byte)144, 5).ToArray());
                        RWMain.Write(Code.PC_FatigueGain, Enumerable.Repeat((byte)144, 8).ToArray());
                        RWMain.Write(Code.PC_TemperatureChange, Enumerable.Repeat((byte)144, 8).ToArray());
                        player.Temperature = 37f;
                        player.Energy = 0;
                        player.Fatigue = 0f;
                        player.Thirst = 0;
                        player.Hunger = 255;
                        player.Nutrition = 5000;
                        if (!_cannibalism) { player.Starvation = 0; }
                    }
                    else
                    {
                        RWMain.Write(Code.PC_HungerGain, _hungerGain);
                        RWMain.Write(Code.PC_ThirstGain, _thirstGain);
                        RWMain.Write(Code.PC_NutritionLoss, _nutritionLoss);
                        RWMain.Write(Code.PC_EnergyLoss, _energyLoss);
                        RWMain.Write(Code.PC_FatigueGain, _fatigueGain);
                        RWMain.Write(Code.PC_TemperatureChange, _temperatureChange);
                    }
                    SendKeys.Send(".");
                }
            }
        }

        public bool NoCarryWeight
        {
            get { return _noWeight; }
            set
            {
                if (_noWeight != value)
                {
                    _noWeight = value;
                    if (_noWeight)
                    {
                        RWMain.Write(Code.PC_WeightChange, Enumerable.Repeat((byte)144, 8).ToArray());
                        player.InventoryWeight = 0f;
                        SendKeys.Send(".");
                    }
                    else
                    {
                        RWMain.Write(Code.PC_WeightChange, _weightChange);
                        player.InventoryWeight = player.Inventory.Items.GetWeight();
                        SendKeys.Send(".");
                    }
                }
            }
        }

        public bool Cannibalism
        {
            get { return _cannibalism; }
            set
            {
                if (_cannibalism != value)
                {
                    _cannibalism = value;
                    if (_cannibalism)
                    {
                        RWMain.Write(Code.PC_StarvationLoss, Enumerable.Repeat((byte)144, 6).ToArray());
                        player.Starvation = 2;
                        SendKeys.Send(".");
                    }
                    else
                    {
                        RWMain.Write(Code.PC_StarvationLoss, _starvationLoss);
                        player.Starvation = 0;
                        SendKeys.Send(".");
                    }
                }
            }
        }

        public bool TreeVision
        {
            get { return _treeVision; }
            set
            {
                if (_treeVision != value)
                {
                    _treeVision = value;
                    if (_treeVision)
                    {
                        RWMain.Write(Code.PC_IsInTreeCheck, Enumerable.Repeat((byte)144, 6).ToArray());
                        player.IsInTree = true;
                    }
                    else
                    {
                        RWMain.Write(Code.PC_IsInTreeCheck, _isInTreeCheck);
                    }
                }
            }
        }

        public bool Checksum
        {
            get { return _checksum; }
            set
            {
                if (_checksum != value)
                {
                    _checksum = value;
                    if (_checksum)
                    {
                        RWMain.Write(Code.PC_SkillChecksum, (byte)144);
                        RWMain.Write(Code.PC_AttributeChecksum, Enumerable.Repeat((byte)144, 2).ToArray());
                    }
                    else
                    {
                        RWMain.Write(Code.PC_SkillChecksum, _skillChecksum);
                        RWMain.Write(Code.PC_AttributeChecksum, _attrbiuteChecksum);
                    }
                }
            }
        }

        public void MovePlayerTo(Point Destination)
        {
            player.Location = Destination;
        }


        public void FacePlayerTowards(Point destination)
        {
            double dx = destination.X - player.Location.X;
            double dy = destination.Y - player.Location.Y;

            // Flip dy for clockwise angle (Y increases downward on screen)
            double angle = Math.Atan2(dy, dx) * (180.0 / Math.PI);

            // Invert the angle by 180 degrees
            angle += 180;

            // Normalize to [-180, 180)
            angle = NormalizeAngle(angle);

            player.SetDirectionToAngle(angle);

            // Local normalization function
            double NormalizeAngle(double a)
            {
                while (a < -180) a += 360;
                while (a >= 180) a -= 360;
                return a;
            }
        }






        public void MoveItemsTo(Point Destination)
        {
            MapObjectCheats.MoveAllItemsToTarget(Destination);
        }

        public void ShowItemID(Point Target)
        {
            //ItemBuffered itemBuffered = new ItemBuffered(RWMain, 0);
            //itemBuffered.ReadArray();
            ItemStructCheats.GetStaticItems();
            //MessageBox.Show("").ToString());
            //MessageBox.Show(MapObjectCheats.GetObjectID(Target).ToString());
        }

        public class MapObjectHandler
        {
            public MapObjectBuffered mapObjectBuffered;
            ReadWriteMem RWMain;
            public MapObjectHandler(ReadWriteMem RW)
            {
                RWMain = RW;
                mapObjectBuffered = new MapObjectBuffered(RWMain, 0);
            }

            public List<int> GetObjectsAt(Point Location)
            {
                List<int> Return = new List<int>();
                for (mapObjectBuffered.Index = 0; mapObjectBuffered.Index < mapObjectBuffered.Count; mapObjectBuffered.Index++)
                {
                    if (mapObjectBuffered.Location == Location)
                    {
                        Return.Add(mapObjectBuffered.Index);
                    }
                }
                return Return;
            }

            public void MoveObject(int Obj, Point Destination)
            {
                mapObjectBuffered.Index = Obj;
                mapObjectBuffered.Location = Destination;
            }

            public void MoveObjects(List<int> Objs, Point Destination)
            {
                Dictionary<int, int> StackCount = new Dictionary<int, int>();
                foreach (int i in Objs)
                {
                    mapObjectBuffered.Index = i;
                    if (StackCount.ContainsKey(mapObjectBuffered.ID))
                    {
                        int Count = mapObjectBuffered.Count;
                        mapObjectBuffered.Count = 0;
                        mapObjectBuffered.Index = StackCount[mapObjectBuffered.ID];
                        mapObjectBuffered.Count += Count;
                    }
                    else
                    {
                        mapObjectBuffered.Location = Destination;
                        StackCount.Add(mapObjectBuffered.ID, mapObjectBuffered.Index);
                    }
                }
            }

            public List<int> GetObjectsByType<T>()
            {
                List<int> Return = new List<int>();
                int ObjCount = mapObjectBuffered.ObjectCount;

                for (mapObjectBuffered.Index = 0; mapObjectBuffered.Index < ObjCount; mapObjectBuffered.Index++)
                {
                    if (mapObjectBuffered.ObjectType != null)
                    {
                        if (mapObjectBuffered.ObjectType == typeof(T))
                        {
                            Return.Add(mapObjectBuffered.Index);
                        }
                    }
                    else
                    {
                        //MessageBox.Show(mapObjectBuffered.ID.ToString());
                    }
                }
                return Return;
            }

            public int GetObjectID(Point Target)
            {
                int ObjCount = mapObjectBuffered.ObjectCount;
                StringBuilder sbBuilder = new StringBuilder();
                for (mapObjectBuffered.Index = 0; mapObjectBuffered.Index < ObjCount; mapObjectBuffered.Index++)
                {
                    sbBuilder.AppendLine(mapObjectBuffered.ID.ToString() + " - " + mapObjectBuffered.State);
                    if (mapObjectBuffered.Location == Target)
                    {
                        return mapObjectBuffered.ID;
                    }
                }
                File.WriteAllText("IDsScan.txt", sbBuilder.ToString());
                return 0;
            }

            public void MoveAllItemsToTarget(Point Destination)
            {
                mapObjectBuffered.ReadArray();
                MoveObjects(GetObjectsByType<Item>(), Destination);
                mapObjectBuffered.WriteArray();
            }

        }

        public class ItemStructHandler
        {
            public ItemBuffered itemBuffered;
            ReadWriteMem RWMain;
            public ItemStructHandler(ReadWriteMem RW)
            {
                RWMain = RW;
                itemBuffered = new ItemBuffered(RWMain, 0);
            }

            public void GetStaticItems()
            {
                itemBuffered.ReadArrayStatic();
                int ObjCount = itemBuffered.ObjectCountStatic;
                StringBuilder sbBuilder = new StringBuilder();
                for (itemBuffered.Index = 0; itemBuffered.Index < ObjCount; itemBuffered.Index++)
                {
                    if (!string.IsNullOrEmpty(itemBuffered.Name))
                    {
                        sbBuilder.AppendLine(itemBuffered.Index.ToString() + " - " + itemBuffered.Name);
                    }
                }
                File.WriteAllText("ItemsScanned.txt", sbBuilder.ToString());
            }
        }
    }

}
