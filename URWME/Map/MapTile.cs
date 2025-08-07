using System.Drawing;

namespace URWME // Unreal World MemoryManager
{
    public class MapTile
    {
        public int Index = 0;
        ReadWriteMem RWMain;

        public MapTile(ReadWriteMem RW, int i)
        {
            RWMain = RW;
            Index = i;
        }

        private int BaseAddress
        {
            get { return Index + Address.Map_Tiles; }
        }

        public byte ID
        {
            get { return RWMain.Read<byte>(BaseAddress); }
            set { RWMain.Write(BaseAddress, value); }
        }

        public Point Location
        {
            get { return new Point(Index % 3073, Index / 3073); }
        }

        public string Sprite
        {
            get { return DefaultData.DefaultMapTiles[ID][1].ToString(); }
        }

        public Color Color
        {
            get { return (Color)DefaultData.DefaultMapTiles[ID][0]; }
        }
    }

}
