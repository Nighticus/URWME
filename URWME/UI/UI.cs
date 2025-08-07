using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static URWME.Player;

namespace URWME
{
    public partial class UI
    {

        ReadWriteMem RWMain;


        public UI(ReadWriteMem RW)
        {
            RWMain = RW;
        }

        public int MapRenderSizeX
        {
            get { return RWMain.Read<int>(Address.UI_MapRenderSizeX); }
            set { RWMain.Write(Address.UI_MapRenderSizeX, value); }
        }

        public int MapRenderSizeY
        {
            get { return RWMain.Read<int>(Address.UI_MapRenderSizeY); }
            set { RWMain.Write(Address.UI_MapRenderSizeY, value); }
        }

        public int VisibleRadiusX
        {
            get { return RWMain.Read<int>(Address.UI_VisibleRadiusX); }
            set { RWMain.Write(Address.UI_VisibleRadiusX, value); }
        }

        public int VisibleRadiusY
        {
            get { return RWMain.Read<int>(Address.UI_VisibleRadiusY); }
            set { RWMain.Write(Address.UI_VisibleRadiusY, value); }
        }

        public float ZoomLevel
        {
            get { return RWMain.Read<float>(Address.UI_ZoomLevel); }
            set { RWMain.Write(Address.UI_ZoomLevel, value); }
        }

        public float TileScaleW
        {
            get { return RWMain.Read<float>(Address.UI_TileScaleW); }
            set { RWMain.Write(Address.UI_TileScaleW, value); }
        }

        public float TileScaleH
        {
            get { return RWMain.Read<float>(Address.UI_TileScaleH); }
            set { RWMain.Write(Address.UI_TileScaleH, value); }
        }

    }
}
