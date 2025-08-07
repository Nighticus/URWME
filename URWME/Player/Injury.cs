using System.Collections.Generic;
using System.Text;

namespace URWME // Unreal World MemoryManager
{
    public class Injury
    {
        public int Index = 0;
        ReadWriteMem RWMain;
        public Injury(ReadWriteMem RW, int i)
        {
            RWMain = RW;
            Index = i;
        }

        public byte Severity
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.Severity + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.Severity + Index, value); }
        }

        public byte Side
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.Side + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.Side + Index, value); }
        }

        public byte State
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.State + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.State + Index, value); }
        }

        public byte Location
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.Location + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.Location + Index, value); }
        }

        public byte Type
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.Type + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.Type + Index, value); }
        }

        public byte Damage
        {
            get { return RWMain.Read<byte>(Address.PC_Injuries + Offset.Damage + Index); }
            set { RWMain.Write(Address.PC_Injuries + Offset.Damage + Index, value); }
        }


        private class Offset
        {
            public static int
            Severity = 0x0,
            Side = 0x14,
            State = 0x28,
            Location = 0x3C,
            Type = 0x50,
            Damage = 0x64;

            public int this[int Index]
            {
                get
                {
                    return Index * 0x14;
                }
            }
        }

        #region NamingData
        public static readonly List<string> InjuryTypeNames = new List<string>() { "None", "Bruise", "Cut", "Puncture", "Burn", "Frost", "Tear/Bite", "Influenza", "Plague" };
        public static readonly List<string> InjurySeverityNames = new List<string>() { "None", "Minor", "Serious", "Severe" };
        private readonly string[] sInjurySeverityPrefix = { "Shallow", "Minor", "Serious", "Deep", "Grievous", "Severe" };
        private readonly string[] sInjuryTypePrefix = { "Bruise", "Fracture", "Crush", "Cut", "Puncture", "Burn", "Frost", "Frostbite", "Tear", "Bite" };
        public static readonly string[] InjuryLocationNames = { "None", "Eye", "Face/Skull", "Neck", "Shoulder", "Upper Arm", "Elbow", "Forearm", "Hand", "Thorax", "Abdomen", "Hip", "Groin", "Thigh", "Knee", "Calf", "Foot", "Tail", "Wing" };
        public static readonly string[] InjurySideNames = { "None", "Right", "Left", "Center" };
        public static readonly string[] InjuryStateNames = { "None", "Bleeding" };
        #endregion

        public string Name
        {
            get
            {
                string sSeverity = "", sType = "";
                if (Type == 1)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[1]; sType = sInjuryTypePrefix[0]; }
                    else if (Severity == 2)
                    { sSeverity = sInjurySeverityPrefix[2]; sType = sInjuryTypePrefix[1]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[5]; sType = sInjuryTypePrefix[2]; }
                }
                else if (Type == 2)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[0]; sType = sInjuryTypePrefix[3]; }
                    else if (Severity == 2)
                    { sSeverity = sInjurySeverityPrefix[2]; sType = sInjuryTypePrefix[3]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[3]; sType = sInjuryTypePrefix[3]; }
                }
                else if (Type == 3)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[1]; sType = sInjuryTypePrefix[4]; }
                    else if (Severity == 2)
                    { sSeverity = sInjurySeverityPrefix[2]; sType = sInjuryTypePrefix[4]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[3]; sType = sInjuryTypePrefix[4]; }
                }
                else if (Type == 4)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[1]; sType = sInjuryTypePrefix[5]; }
                    else if (Severity == 2)
                    { sSeverity = sInjurySeverityPrefix[2]; sType = sInjuryTypePrefix[5]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[4]; sType = sInjuryTypePrefix[5]; }
                }
                else if (Type == 5)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[1]; sType = sInjuryTypePrefix[6]; }
                    else if (Severity == 2)
                    { sSeverity = ""; sType = sInjuryTypePrefix[7]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[5]; sType = sInjuryTypePrefix[7]; }
                }
                else if (Type == 6)
                {
                    if (Severity == 1)
                    { sSeverity = sInjurySeverityPrefix[0]; sType = sInjuryTypePrefix[8]; }
                    else if (Severity == 2)
                    { sSeverity = sInjurySeverityPrefix[2]; sType = sInjuryTypePrefix[9]; }
                    else if (Severity == 3)
                    { sSeverity = sInjurySeverityPrefix[4]; sType = sInjuryTypePrefix[9]; }
                }
                else if (Type == 10) // Influenza
                {
                    sType = InjuryTypeNames[7];
                }
                else if (Type == 11) // Plague
                {
                    sType = InjuryTypeNames[8];
                }
                StringBuilder sbBuilder = new StringBuilder();
                if (sType != "" && InjuryLocationNames[Location] != "None")
                {
                    if (Type != 1)
                    {
                        sbBuilder.Append(sSeverity + " ");
                    }
                    else if (Type == 5)
                    {
                        if (Severity != 2)
                        {
                            sbBuilder.Append(sSeverity + " ");
                        }
                    }
                    sbBuilder.Append(sType + " in");
                    if (InjurySideNames[Side] != "None" && InjurySideNames[Side] != "Center")
                    {
                        sbBuilder.Append(" " + InjurySideNames[Side]);
                    }
                    else if (InjurySideNames[Side] == "None")
                    {
                        sbBuilder.Append(" Left");
                    }
                    sbBuilder.Append(" " + InjuryLocationNames[Location]);
                }
                else if (Type >= 10)
                {
                    sbBuilder.Append(sType);
                }
                else { sbBuilder.Append("None"); }
                return sbBuilder.ToString();
            }
        }

        public byte[] Buffer
        {
            get
            {
                List<byte> b = new List<byte>();
                Offset O = new Offset();
                for (int i = 0; i < 6; i++)
                {
                    b.Add(RWMain.Read<byte>(Address.PC_Injuries + O[i] + Index));
                }
                return b.ToArray();
            }
            set
            {
                Offset O = new Offset();
                for (int i = 0; i < 6; i++)
                {
                    RWMain.Write(Address.PC_Injuries + O[i] + Index, value[i]);
                }
            }
        }

    }

}
