using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace URWME
{
    public partial class MinimapForm : Form
    {
        public ReadWriteMem RWMain;
        public Player player;
        public Map map;
        public PipeClient pipeClient;

        private byte[] LastMapBytes;

        public Bitmap WorldMapImage;
        public Bitmap LocalMapImage;
        public Bitmap DisplayMapImage;
        public Point LastPlayerLocation;

        private float _scale;
        private IntPtr _externalHwnd;
        private Timer _positionTimer;

        // --- Win32 imports ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X, Y;
        }

        public MinimapForm()
        {
            InitializeComponent();
        }

        private async void MinimapForm_Load(object sender, EventArgs e)
        {
            pipeClient = new PipeClient(data => { ProcessPipeData(data); });
            pipeClient.Start();
            //DefaultData.URW.WaitForInputIdle();
            RWMain = new ReadWriteMem(DefaultData.URW);

            await SignatureThread();
            //DefaultData.Load();

            player = new Player(RWMain);
            map = new Map(RWMain);

            AttachToExternalWindow();

            //await pipeClient.Send("test");
        }

        private void ProcessPipeData(string data)
        {
            //MessageBox.Show(data);
            switch (data)
            {
                case "hide":
                    Visible = false;
                    Hide();
                    break;
                case "show":
                    UpdatePosition(true);
                    Visible = true;
                    Show();
                    break;
                case "exit":
                    Application.Exit();
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> SignatureThread()
        {
            int Locations = await RWMain.FindSignatureAsync("C6 05 ?? ?? ?? ?? 01 89 3D ?? ?? ?? ?? 89 35 ?? ?? ?? ?? 88 1D ?? ?? ?? ??");
            Address.PC_LocationX = RWMain.Read<int>(Locations + 9, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_LocationY = RWMain.Read<int>(Locations + 15, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Type = RWMain.Read<int>(await RWMain.FindSignatureAsync("C3 83 3D ?? ?? ?? ?? 02 C6 05 ?? ?? ?? ?? 02 75 30") + 10, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Tiles = RWMain.Read<int>(await RWMain.FindSignatureAsync("C6 80 ?? ?? ?? ?? B4 05 01 0C 00 00 41") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.Item_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("B9 2B 00 00 00 BF ?? ?? ?? ?? 81 C6 ?? ?? ?? ?? F3 A5 5F 5E C3") + 12, (byte)0) - RWMain.ProcBaseAddress;
            //Address.Static_Item_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("81 C7 ?? ?? ?? ?? F3 A5 8B 4D FC 5F 5E 33 CD 5B") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.NPC_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("BF ?? ?? ?? ?? B9 5A 01 00 00 81 C6 ?? ?? ?? ?? F3 A5") + 1, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Objects = RWMain.Read<int>(await RWMain.FindSignatureAsync("0F 11 04 8D ?? ?? ?? ?? 0F 10 47 10 0F 11 04 8D ?? ?? ?? ?? 89 04 8D ?? ?? ?? ??") + 4, (byte)0) - RWMain.ProcBaseAddress;
            //Address.PC_ViewingInventory = RWMain.Read<int>(await RWMain.FindSignatureAsync("89 0D ?? ?? ?? ?? 33 D2 B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F 28 05 ?? ?? ?? ?? 8D 84 24 30 07 00 00") + 2, (byte)0) - RWMain.ProcBaseAddress;
            //Address.UI_Index = RWMain.Read<int>(await RWMain.FindSignatureAsync("89 35 ?? ?? ?? ?? 83 3D ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? E9 ?? ?? ?? ?? 8D 87 CF FB FF FF") + 2, (byte)0) - RWMain.ProcBaseAddress;

            Address.Update();
            //MessageBox.Show(DefaultData.URWVersion);
            //MessageBox.Show(Address.Map_Objects.ToString("X") + " ");
            Console.WriteLine("Addresses loaded from signatures.");

            return false;
        }

        public void AttachToExternalWindow()
        {
            _externalHwnd = DefaultData.URW.MainWindowHandle;

            // Set parent
            SetParent(this.Handle, _externalHwnd);
            Hide();
            Visible = false;

            // Position immediately
            // Start timer to keep tracking position
            _positionTimer = new Timer();
            _positionTimer.Interval = 250; // update 4x per second
            _positionTimer.Tick += (s, e) => UpdatePosition();
            _positionTimer.Start();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private void UpdatePosition(bool ForcedUpdate = false)
        {
            if (_externalHwnd == IntPtr.Zero)
                return;

            // Get external window rect
            if (GetWindowRect(_externalHwnd, out RECT rect))
            {
                // Position this form at the top-right of the external window
                int x = rect.Left;                 // left edge
                int y = rect.Bottom - 320; // bottom edge minus form height

                this.Location = new Point(0, 0);
            }

            // Local function to draw a centered player block
            void DrawPlayerBlock(Graphics g, Point location, int blockSize, Color color, Bitmap bitmap)
            {
                int halfBlock = blockSize / 2;

                int x = location.X - halfBlock;
                int y = location.Y - halfBlock;

                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x + blockSize > bitmap.Width) x = bitmap.Width - blockSize;
                if (y + blockSize > bitmap.Height) y = bitmap.Height - blockSize;

                g.FillRectangle(new SolidBrush(color), x, y, blockSize, blockSize);
            }

            // --- redraw minimap if player moved ---
            if (LastPlayerLocation != player.Location || ForcedUpdate)
            {
                if (map.IsLocal)
                {
                    if (LastMapBytes != map.Tiles.Buffer)
                    {
                        LastMapBytes = map.Tiles.Buffer;
                        LocalMapImage = map.GetImageLocal();
                    }

                    DisplayMapImage = (Bitmap)LocalMapImage.Clone();

                    using (Graphics g = Graphics.FromImage(DisplayMapImage))
                    {
                        DrawPlayerBlock(g, Point.Add(player.Location, new Size(-72, -72)), 5, Color.Red, DisplayMapImage);
                    }

                    BackgroundImage = DisplayMapImage;
                    this.Size = new Size(214, 214);
                    pipeClient.Send("Cloned localmap and set to background");
                }
                else
                {
                    if (LastMapBytes != map.Tiles.Buffer)
                    {
                        LastMapBytes = map.Tiles.Buffer;
                        WorldMapImage = map.GetImageOverworld();
                    }

                    DisplayMapImage = (Bitmap)WorldMapImage.Clone();

                    using (Graphics g = Graphics.FromImage(DisplayMapImage))
                    {
                        DrawPlayerBlock(g, player.Location, 50, Color.Red, DisplayMapImage);
                    }

                    BackgroundImage = DisplayMapImage;
                    this.Size = new Size(256, 170);
                    pipeClient.Send("Cloned worldmap and set to background");
                }
                pipeClient.Send(LastPlayerLocation.ToString() + " " + player.Location.ToString());
                LastPlayerLocation = player.Location;
            }
            //else { await pipeClient.Send(LastPlayerLocation.ToString() + " " + player.Location.ToString()); }
        }


    }
}
