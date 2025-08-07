using System;

namespace URWME // Unreal World MemoryManager
{
    public class Skill
    {
        public int Index = 0;
        public static int IndexMax
        {
            get { return SkillNames.Length - 1; }
        }

        ReadWriteMem RWMain;
        public Skill(ReadWriteMem RW, int i)
        {
            RWMain = RW;
            Index = i;
        }

        public Skill(ReadWriteMem RW, string n)
        {
            RWMain = RW;
            for (int i = 0; i < SkillNames.Length; i++)
            {
                if (SkillNames[i] == n)
                {
                    Index = i;
                    break;
                }
                else { Index = -1; }
            }

            if (Index == -1) { throw new Exception("No skill by that name exists. Please check name."); }
        }

        public string Name
        {
            get
            {
                if (Index < SkillNames.Length)
                {
                    return SkillNames[Index];
                }
                else return "Skill index out of range, no name was found.";
            }
        }

        public byte Level
        {
            get
            {
                return RWMain.Read<byte>(Address.PC_Skills + Index);
            }
            set
            {
                RWMain.Write(Address.PC_Skills + Index, value);
            }
        }

        public static string[] SkillNames = new string[] 
        {
            "Dodge",
            "Agriculture",
            "Shield",
            "Knife",
            "Sword",
            "Club",
            "Axe",
            "Flail",
            "Spear",
            "Bow",
            "Crossbow",
            "Unarmed",
            "Physician",
            "Climbing",
            "Stealth",
            "Cookery",
            "Skiing",
            "Timbercraft",
            "Swimming",
            "Fishing",
            "Weatherlore",
            "Carpentry",
            "Herblore",
            "Hideworking",
            "Tracking",
            "Trapping",
            "Building",
            "Textilecraft",
            "Netmaking",
            "Bowyer"
        };

    }

}
