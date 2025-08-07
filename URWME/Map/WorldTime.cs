namespace URWME // Unreal World MemoryManager
{
    public class WorldTime
    {
        ReadWriteMem RWMain;
        public WorldTime(ReadWriteMem RW)
        {
            RWMain = RW;
        }

        public byte Minute
        {
            get { return RWMain.Read<byte>(Address.World_TimeMinute); }
            set { RWMain.Write(Address.World_TimeMinute, value); }
        }
        public byte Hour
        {
            get { return RWMain.Read<byte>(Address.World_TimeHour); }
            set { RWMain.Write(Address.World_TimeHour, value); }
        }
        public byte Day
        {
            get { return RWMain.Read<byte>(Address.World_TimeDay); }
            set { RWMain.Write(Address.World_TimeDay, value); }
        }
        public byte Month
        {
            get { return RWMain.Read<byte>(Address.World_TimeMonth); }
            set { RWMain.Write(Address.World_TimeMonth, value); }
        }
        public byte Year
        {
            get { return RWMain.Read<byte>(Address.World_TimeYear); }
            set { RWMain.Write(Address.World_TimeYear, value); }
        }

        public string Help
        {
            get { return HelpGenerator.GenerateHelp<WorldTime>(); }
        }
    }

}
