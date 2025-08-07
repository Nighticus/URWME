using System;

namespace URWME // Unreal World MemoryManager
{
    public class Attribute
    {
        public int Index = 0; // Offset[Index] == true index
        public static int IndexMax
        {
            get { return AttributeNames.Length - 1; }
        }

        ReadWriteMem RWMain;
        public Attribute(ReadWriteMem RW, int i)
        {
            RWMain = RW;
            Index = i;
        }

        public Attribute(ReadWriteMem RW, string n)
        {
            RWMain = RW;
            for (int i = 0; i < AttributeNames.Length; i++)
            {
                if (AttributeNames[i] == n)
                {
                    Index = i;
                    break;
                }
                else { Index = -1; }
            }

            if (Index == -1) { throw new Exception("No attribute by that name exists. Please check name."); }
        }

        public string Name
        {
            get
            {
                if (Index < AttributeNames.Length)
                {
                    return AttributeNames[Index];
                }
                else return "Attribute index out of range, no name was found.";
            }
        }

        public byte Level
        {
            get
            {
                return RWMain.Read<byte>(Address.PC_Attributes + Offset[Index]);
            }
            set
            {
                RWMain.Write(Address.PC_Attributes + Offset[Index], value);
            }
        }

        public static string[] AttributeNames = new string[]
        {
            "Strength",
            "Agility",
            "Dexterity",
            "Speed",
            "Endurance",
            "SmellTaste",
            "Eyesight",
            "Touch",
            "Will",
            "Intelligence",
            "Hearing"
        };

        private int[] Offset = new int[]
        {
            0,
            1,
            4,
            5,
            7,
            8,
            11,
            12,
            13,
            16,
            17
        };

    }

}
