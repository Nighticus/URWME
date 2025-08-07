using System.Collections.Generic;

namespace URWME // Unreal World MemoryManager
{
    public class NPC
    {
        ReadWriteMem RWMain;
        public int Index = 0;
        const int SizeOfStruct = 1384;

        private int BaseAddress
        {
            get { return (SizeOfStruct * (Index + 1)) + Address.NPC_Struct; }
        }

        public NPC(ReadWriteMem RW)
        {
            RWMain = RW;
        }

        public byte[] DataArray
        {
            get { return RWMain.Read<byte[]>(BaseAddress, SizeOfStruct); }
            set { RWMain.Write(BaseAddress, value); }
        }

        public string Name // Creature name
        {
            get { return RWMain.Read<string>(BaseAddress + Offset.Name, 38); }
            set { RWMain.Write(BaseAddress + Offset.Name, value); }
        }

        public string Nickname // Nickname; human assigned name.
        {
            get { return RWMain.Read<string>(BaseAddress + Offset.Nickname, 24); }
            set { RWMain.Write(BaseAddress + Offset.Nickname, value); }
        }

        public string Portrait // Portrait file name
        {
            get { return RWMain.Read<string>(BaseAddress + Offset.Portrait, 26); }
            set { RWMain.Write(BaseAddress + Offset.Portrait, value); }
        }

        public string Sprite // Sprite file name
        {
            get { return RWMain.Read<string>(BaseAddress + Offset.Sprite, 58); }
            set { RWMain.Write(BaseAddress + Offset.Sprite, value); }
        }

        public string[] Vocals
        {
            get
            {
                return new string[]
                {
                    RWMain.Read<string>(BaseAddress + Offset.Vocals, 8),
                    RWMain.Read<string>(BaseAddress + Offset.Vocals + 10, 8)
                };
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    RWMain.Write(BaseAddress + Offset.Vocals + (i * 10), value[i]);
                }
            }
        }

        public string[] Attacks
        {
            get
            {
                return new string[]
                {
                    RWMain.Read<string>(BaseAddress + Offset.Attacks, 10),
                    RWMain.Read<string>(BaseAddress + Offset.Attacks + 12, 10)
                };
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    RWMain.Write(BaseAddress + Offset.Attacks + (i * 12), value[i]);
                }
            }
        }

        public string[] Skills
        {
            get
            {
                return new string[]
                {
                    RWMain.Read<string>(BaseAddress + Offset.Skills, 10),
                    RWMain.Read<string>(BaseAddress + Offset.Skills + 12, 10),
                    RWMain.Read<string>(BaseAddress + Offset.Skills + 24, 10),
                    RWMain.Read<string>(BaseAddress + Offset.Skills + 36, 10)
                };
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    RWMain.Write(BaseAddress + Offset.Skills + (i * 12), value[i]);
                }
            }
        }

        public int[] ItemIDs
        {
            get
            {
                List<int> iIDs = new List<int>();
                for (int i = 0; i < 7; i++)
                {
                    int iID = RWMain.Read<int>(BaseAddress + Offset.Items + (i * 4));
                    if (iID != 0)
                    {
                        iIDs.Add(iID);
                    }
                    else { break; }
                }
                return iIDs.ToArray();
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] >= 50000)
                    {
                        RWMain.Write(BaseAddress + Offset.Items + (i * 4), value[i]);
                    }
                }
            }
        }

        public int CreatureID // Creature ID; Wolf, Lynx, Dog, Cow, etc.
        {
            get { return RWMain.Read<int>(BaseAddress + Offset.CreatureID); }
            set { RWMain.Write(BaseAddress + Offset.CreatureID, value); }
        }

        public int ClassID // Creature class; human, horse, canine, etc
        {
            get { return RWMain.Read<int>(BaseAddress + Offset.ClassID); }
            set { RWMain.Write(BaseAddress + Offset.ClassID, value); }
        }

        public float MeatWeight // ?
        {
            get { return RWMain.Read<float>(BaseAddress + Offset.MeatWeight); }
            set { RWMain.Write(BaseAddress + Offset.MeatWeight, value); }
        }

        public static class Offset
        {
            public static int
                ClassID = 0x0,
                CreatureID = 0x4,
                Name = 0x8,
                Nickname = 0x30,
                Portrait = 0x49,
                Sprite = 0x2B6,
                Vocals = 0xA8,
                //Vocal2 = 0xB2,
                Attacks = 0xDE,
                //Attack2 = 0xEA,
                Skills = 0x115,
                Items = 0xB4,
                MeatWeight = 0x94,
                IndexID = 0x52C;
        }
    }

}
