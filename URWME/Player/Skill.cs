using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class Skill
    {
        public int Index = 0;
        public static int IndexMax
        {
            get { return SkillNames.Count - 1; }
        }

        ReadWriteMem RWMain;
        public Skill(ReadWriteMem RW, int i)
        {
            RWMain = RW;
            Index = i;
            SkillNames = GetSkillNames();
        }

        public Skill(ReadWriteMem RW, string n)
        {
            RWMain = RW;
            for (int i = 0; i < SkillNames.Count; i++)
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
                if (Index < SkillNames.Count)
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

        public List<string> GetSkillNames()
        {
            int Index = 0;
            List<string> SkillsFound = new List<string>();
            while (true)
            {
                string SkillName = RWMain.Read<string>(Address.PC_SkillNames + (Index*12), 12);
                if (string.IsNullOrEmpty(SkillName)) { return SkillsFound; }
                else { SkillsFound.Add(SkillName.ToTitleCase()); Index++; }
            }
        }

        public static List<string> SkillNames = new List<string>()
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
