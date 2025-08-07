using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URWME
{
    public class MapObjectBuffered
    {
        public int Index = 0;
        private ReadWriteBuffer RWBuffer;
        private ReadWriteMem RWMain;
        private MapObject MapObject;
        private const int SizeOfStruct = 36;

        public MapObjectBuffered(ReadWriteMem RW, int objectIndex)
        {
            RWMain = RW;
            Index = objectIndex;

            MapObject = new MapObject(RWMain, Index);
            RWBuffer = new ReadWriteBuffer(MapObject.DataArray);
        }

        public void WriteArray()
        {
            MapObject.DataArray = RWBuffer.CurrentBuffer;
        }

        public void ReadArray()
        {
            RWBuffer.CurrentBuffer = MapObject.DataArray;
        }

        public int ObjectCount
        {
            get { return MapObject.ObjectCount; }
        }

        private int BaseAddress
        {
            get { return (SizeOfStruct * Index); }
        }

        public int ID
        {
            get { return RWBuffer.Read<int>(BaseAddress + Offset.ID); }
            set { RWBuffer.Write(BaseAddress + Offset.ID, value); }
        }

        public int Count
        {
            get
            {
                switch (ObjectType)
                {
                    case Type itemType when itemType == typeof(Item):
                        return RWBuffer.Read<int>(BaseAddress + Offset.Count);
                    case Type npcType when npcType == typeof(NPC):
                        return RWBuffer.Read<int>(BaseAddress + Offset.Count);  // Example for NPC, adjust as needed
                    default:
                        return RWBuffer.Read<int>(BaseAddress + Offset.Count);
                }
            }
            set
            {
                switch (ObjectType)
                {
                    case Type itemType when itemType == typeof(Item):
                        RWBuffer.Write(BaseAddress + Offset.Count, value);
                        break;
                    case Type npcType when npcType == typeof(NPC):
                        RWBuffer.Write(BaseAddress + Offset.Count, value);  // Example for NPC, adjust as needed
                        break;
                    default:
                        RWBuffer.Write(BaseAddress + Offset.Count, value);
                        break;
                }
            }
        }

        public Point Location
        {
            get { return new Point(RWBuffer.Read<short>(BaseAddress + Offset.X), RWBuffer.Read<short>(BaseAddress + Offset.Y)); }
            set
            {
                RWBuffer.Write(BaseAddress + Offset.X, (short)value.X);
                RWBuffer.Write(BaseAddress + Offset.Y, (short)value.Y);
            }
        }

        public Type ObjectType
        {
            get
            {
                if (ID >= 0 && ID <= 366 || ID >= 50000 && ID <= 98000)
                {
                    return typeof(Item);
                }
                else if (ID >= 1000 && ID <= 2000)
                {
                    return typeof(NPC);
                }
                else
                {
                    return null;
                }
            }
        }

        public int State
        {
            get { return RWBuffer.Read<int>(BaseAddress + Offset.State); }
            set { RWBuffer.Write(BaseAddress + Offset.State, value); }
        }

        public static class Offset
        {
            public static int
            X = 0x0,
            Y = 0x2,
            State = 0x4,
            Count = 0x8,
            ID = 0xC;
        }
    }
}
