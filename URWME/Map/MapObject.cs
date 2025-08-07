using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace URWME // Unreal World MemoryManager
{
    public class MapObject
    {

        public int Index = 0;
        ReadWriteMem RWMain;
        public MapObject(ReadWriteMem RW, int ObjectIndex)
        {
            RWMain = RW;
            Index = ObjectIndex;
        }
        const int SizeOfStruct = 36;

        private int BaseAddress
        {
            get { return (SizeOfStruct * Index) + Address.Map_Objects; }
        }

        public byte[] DataArray
        {
            get
            {
                List<byte> result = new List<byte>();
                int offset = 0;

                while (true)
                {
                    byte[] chunk = RWMain.Read<byte[]>(Address.Map_Objects + offset, SizeOfStruct);

                    // Check if chunk is all zeroes (end marker)
                    bool isZeroChunk = chunk.All(b => b == 0x00);
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
                RWMain.Write(Address.Map_Objects, value);
            }
        }

        public int ObjectCount
        {
            get { return DataArray.Length / SizeOfStruct; }
        }


        public int ID
        {
            get { return RWMain.Read<int>(BaseAddress + Offset.ID); }
            set { RWMain.Write(BaseAddress + Offset.ID, value); }
        }

        public int Count
        {
            get { return RWMain.Read<int>(BaseAddress + Offset.Count); }
            set { RWMain.Write(BaseAddress + Offset.Count, value); }
        }

        public Point Location
        {
            get { return new Point(RWMain.Read<short>(BaseAddress + Offset.X), RWMain.Read<short>(BaseAddress + Offset.Y)); }
            set
            {
                RWMain.Write(BaseAddress + Offset.X, (short)value.X);
                RWMain.Write(BaseAddress + Offset.Y, (short)value.Y);
            }
        }

        public Type ObjectType
        {
            get
            {
                if (ID >= 50000 && ID <= new Item(RWMain, 0).ObjectCount)
                {
                    return typeof(Item);
                }
                else if (ID >= 1000 && ID <= 2000)
                {
                    return typeof(NPC);
                }
                else { return null; }
            }
        }

        public int State
        {
            get { return RWMain.Read<int>(BaseAddress + Offset.State); }
            set { RWMain.Write(BaseAddress + Offset.State, value); }
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
