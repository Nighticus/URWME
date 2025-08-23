using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Http;
using System.Security.Policy;

namespace URWME
{

    public partial class Form1 : Form
    {

        public ReadWriteMem RWMain;
        public Player player;
        public WorldTime worldTime;
        public Item item;
        public ItemStructHandler itemStruct;
        public Cheats cheats;
        public Map map;
        public PipeServer pipeServer;

        FontDialog fontDialog = new FontDialog();

        //public ConsoleCommands consoleCommands;
        private GlobalMouseHook _mouseHook;
        private HttpListener httpListener;
        public CommandEngine engine;

        public Point LastTargetPosition = new Point();

        private const int FixedWidth = 800;
        private const int FixedHeight = 600;

        public Form1()
        {
            InitializeComponent();
        }

        public static Size GetSizeAR43(int containerWidth, int containerHeight)
        {
            const double aspectRatio = 4.0 / 3.0;

            int width, height;

            if ((containerWidth / aspectRatio) <= containerHeight)
            {
                width = containerWidth;
                height = (int)(containerWidth / aspectRatio);
            }
            else
            {
                width = (int)(containerHeight * aspectRatio);
                height = containerHeight;
            }

            return new Size(width, height);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //Address.UpdateIntegers(Address.ParseFile("Addresses.txt"));
            //ConsoleOverlayHandler.FreeConsole();
            //Extensions.AllocConsole();
            //DefaultData.URW.WaitForInputIdle();
            RWMain = new ReadWriteMem(DefaultData.URW);
            //MessageBox.Show(GetSizeAR43(1707, 897).ToString());

            await SignatureThread();
            //DefaultData.Load();

            player = new Player(RWMain);
            worldTime = new WorldTime(RWMain);
            item = new Item(RWMain, 0);
            itemStruct = new ItemStructHandler(RWMain);
            cheats = new Cheats(RWMain);
            map = new Map(RWMain);

            _mouseHook = new GlobalMouseHook(ShowContextMenu);
            //consoleCommands = new ConsoleCommands(RWMain);

            //HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
            //HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Cleanup();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Cleanup();

            Load_HttpListener();
            Load_CommandEngine();
            pipeServer = new PipeServer();
            pipeServer.Start();

            SetupUI();

            //Thread CmdLineThread = new Thread(new ThreadStart(CmdLineHandler)) { IsBackground = true };
            //CmdLineThread.Start();

            //RWMain.Write(Address.PC_Name, "!test2");

            //var jsonData = "{\"items\":[{\"name\":\"Sword\",\"quantity\":1}]}";
            //var client = new HttpClient();
            //var result = client.PostAsync("http://localhost:8080/", new StringContent(jsonData, Encoding.UTF8, "application/json")).Result;
            //Console.WriteLine($"Response: {result.StatusCode}"); 
        }

        private async void SetupUI()
        {
            string name = Properties.Settings.Default.MenuFontName;
            float size = Properties.Settings.Default.MenuFontSize;
            FontStyle style = (FontStyle)Properties.Settings.Default.MenuFontStyle;
            
            if (!string.IsNullOrEmpty(name) && size > 0)
            {
                cmsMenu.Font = new Font(name, size, style);
            }

            //tsmiCheatMenu.Image = await Extensions.GetImageFromUrlAsync("https://www.unrealworld.fi/urwicon.png");
            //map.Tiles.FogBuffer = map.Tiles.Buffer;
            //map.GetImageOverworldPLM().Save("test4.bmp");
            //RWMain.Write(0x3F7FC11 - 0x3F8E59, map.Tiles.FileBufferDAT);
            //RWMain.Write(0x4581019 - 0x3F8E59, map.Tiles.FileBufferPLM);
            //MessageBox.Show(Address.Map_Tiles.ToString("X"));
            //DescribeStruct();
        }
         
        private async Task<bool> SignatureThread()
        {
            int Locations = await RWMain.FindSignatureAsync("C6 05 ?? ?? ?? ?? 01 89 3D ?? ?? ?? ?? 89 35 ?? ?? ?? ?? 88 1D ?? ?? ?? ??");
            Address.PC_LocationX = RWMain.Read<int>(Locations + 9, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_LocationY = RWMain.Read<int>(Locations + 15, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_InTree = RWMain.Read<int>(Locations + 21, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_TribeName = RWMain.Read<int>(await RWMain.FindSignatureAsync("BE ?? ?? ?? ?? 88 1D ?? ?? ?? ?? 8B 14 85 ?? ?? ?? ??") + 1, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_Direction = RWMain.Read<int>(await RWMain.FindSignatureAsync("8B 35 ?? ?? ?? ?? 84 C0 74 08 8D 4E FE") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Type = RWMain.Read<int>(await RWMain.FindSignatureAsync("C3 83 3D ?? ?? ?? ?? 02 C6 05 ?? ?? ?? ?? 02 75 30") + 10, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Tiles = RWMain.Read<int>(await RWMain.FindSignatureAsync("C6 80 ?? ?? ?? ?? B4 05 01 0C 00 00 41") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_StartLocationX = RWMain.Read<int>(await RWMain.FindSignatureAsync("A3 ?? ?? ?? ?? FF 15 ?? ?? ?? ?? 99") + 1, (byte)0) - RWMain.ProcBaseAddress;
            Address.Item_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("B9 2B 00 00 00 BF ?? ?? ?? ?? 81 C6 ?? ?? ?? ?? F3 A5 5F 5E C3") + 12, (byte)0) - RWMain.ProcBaseAddress;
            //Address.Static_Item_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("81 C7 ?? ?? ?? ?? F3 A5 8B 4D FC 5F 5E 33 CD 5B") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.NPC_Struct = RWMain.Read<int>(await RWMain.FindSignatureAsync("BF ?? ?? ?? ?? B9 5A 01 00 00 81 C6 ?? ?? ?? ?? F3 A5") + 1, (byte)0) - RWMain.ProcBaseAddress;
            Address.PC_Temperature = RWMain.Read<int>(await RWMain.FindSignatureAsync("F3 0F 11 05 ?? ?? ?? ?? 76 33 C7 05 ?? ?? ?? ?? 00 00 28 42") + 4, (byte)0) - RWMain.ProcBaseAddress;
            Address.Map_Objects = RWMain.Read<int>(await RWMain.FindSignatureAsync("0F 11 04 8D ?? ?? ?? ?? 0F 10 47 10 0F 11 04 8D ?? ?? ?? ?? 89 04 8D ?? ?? ?? ??") + 4, (byte)0) - RWMain.ProcBaseAddress;
            //Address.PC_ViewingInventory = RWMain.Read<int>(await RWMain.FindSignatureAsync("89 0D ?? ?? ?? ?? 33 D2 B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F 28 05 ?? ?? ?? ?? 8D 84 24 30 07 00 00") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.UI_Index = RWMain.Read<int>(await RWMain.FindSignatureAsync("89 35 ?? ?? ?? ?? 83 3D ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? E9 ?? ?? ?? ?? 8D 87 CF FB FF FF") + 2, (byte)0) - RWMain.ProcBaseAddress;
            Address.UI_Inventory = RWMain.Read<int>(await RWMain.FindSignatureAsync("BF ?? ?? ?? ?? F3 A5 8B C8 E8 ?? ?? ?? ?? 89 44 24 4C") + 1, (byte)0) - RWMain.ProcBaseAddress;



            Code.PC_HungerGain = await RWMain.FindSignatureAsync("88 15 ?? ?? ?? ?? 66 0F 6E 45 F8 0F 5B C0") - RWMain.ProcBaseAddress;
            Code.PC_FatigueGain = await RWMain.FindSignatureAsync("F3 0F 11 05 ?? ?? ?? ?? 72 3B C7 05 ?? ?? ?? ?? 00 00 C8 42") - RWMain.ProcBaseAddress;
            Code.PC_WeightChange = await RWMain.FindSignatureAsync("F3 0F 11 25 ?? ?? ?? ?? 0F 28 C4 5F") - RWMain.ProcBaseAddress;
            Code.PC_IsInTreeCheck = await RWMain.FindSignatureAsync("88 0D ?? ?? ?? ?? 80 3D ?? ?? ?? ?? 02 C7 05 ?? ?? ?? ?? FF FF FF FF") - RWMain.ProcBaseAddress;
            Code.PC_StarvationLoss = await RWMain.FindSignatureAsync("88 1D ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 3D ?? ?? ?? ?? 01") - RWMain.ProcBaseAddress;
            Code.PC_AttributeChecksum = await RWMain.FindSignatureAsync("80 3D ?? ?? ?? ?? 12 72 05 83 F8 1E ?? ??") + 12 - RWMain.ProcBaseAddress;
            Code.PC_SkillChecksum = await RWMain.FindSignatureAsync("80 BE ?? ?? ?? ?? 5F ?? ?? ?? 83 C2 0C") + 9 - RWMain.ProcBaseAddress;

            Address.Update();
            Code.Update();
            //MessageBox.Show(DefaultData.URWVersion);
            //MessageBox.Show(Address.Map_Objects.ToString("X") + " ");
            Console.WriteLine("Addresses loaded from signatures.");

            return false;
        }

        private void DescribeStruct()
        {
            File.WriteAllText("jsonoutput.txt", itemStruct.GetItems().ToJson());
            //itemStruct.GetItems().ToJson();
        }

        private void Load_CommandEngine()
        {
            var instances = new Dictionary<string, object>
            {
                { "Player", player },
                { "Item", item },
                { "ItemStructHandler", itemStruct },
                { "WorldTime", worldTime }
                //{ "ConsoleCommands", consoleCommands }
            };

            engine = new CommandEngine(instances);
        }

        private void Load_HttpListener()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/"); // Listen on this URL
            httpListener.Start();

            new Thread(new ThreadStart(HandleIncomingRequests)) { IsBackground = true }.Start();
        }

        private void HandleIncomingRequests()
        {
            this.Invoke(new Action(() => Console.WriteLine("Request Handler Started")));
            try
            {
                while (true)
                {
                    var context = httpListener.GetContext();
                    var request = context.Request;

                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // Allow all origins
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS"); // Allow specified methods
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type"); // Allow specified headers

                    if (request.HttpMethod == "OPTIONS")
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.Close();
                        continue; // Skip further processing
                    }

                    if (request.HttpMethod == "GET")
                    {
                        string getRequestData = request.QueryString["request"];
                        this.Invoke(new Action(() => Console.WriteLine("Received request: " + getRequestData)));
                        // Example inventory data

                        string json = System.Text.Json.JsonSerializer.Serialize(engine.ExecuteCommand(getRequestData));

                        // Set response headers

                        // Send response
                        context.Response.ContentType = "application/json";
                        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(json);
                        using (var writer = new System.IO.StreamWriter(context.Response.OutputStream))
                        {
                            try
                            {
                                writer.Write(json);
                            }
                            catch { writer.Close(); }
                        }
                        context.Response.StatusCode = (int)HttpStatusCode.OK; // Send OK status
                    }

                    if (request.HttpMethod == "POST")
                    {
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string jsonData = reader.ReadToEnd();
                            this.Invoke(new Action(() => Console.WriteLine("Received JSON data: " + jsonData)));
                            this.Invoke(new Action(() => Console.WriteLine(engine.ExecuteCommand(jsonData))));
                        }

                        // Send a response back to the client
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.Close();
                    }
                }
            }
            catch { }
        }

        private void UpdateReceivedData(string data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateReceivedData), data);
            }
            else
            {
                // Display the received data in a TextBox or any other UI element
                //receivedDataTextBox.Text = data;
            }
        }

        private void CmdLineHandler()
        {
            this.Invoke(new Action(() => ConsoleOverlayHandler.OverlayWindow()));
            this.Invoke(new Action(() => ConsoleOverlayHandler.ToggleWindow()));

            //DescribeStruct();

            while (Thread.CurrentThread.IsAlive)
            {
                Console.WriteLine(engine.ExecuteCommand(Console.ReadLine()));
            }
        }

        private void ShowContextMenu()
        {
            if (cmsMenu.InvokeRequired)
            {
                cmsMenu.BeginInvoke(new Action(ShowContextMenu));
                return;
            }

            LastTargetPosition = player.GetMouseTile();
            cmsMenu.Show(Cursor.Position);
        }


        public void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Key == Keys.T && player.IsTargetting)
            {
                player.Location = player.GetMouseTile();
            }
            ConsoleOverlayHandler.ToggleWindow();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure the hook is unhooked

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Cleanup()
        {
            cheats.Cleanup();
            _mouseHook.Dispose();
            httpListener.Stop();
            httpListener.Close();
            pipeServer.Send("exit");
        }

        private ToolStripMenuItem CreateMenuItem(string text, Action onClick)
        {
            var item = new ToolStripMenuItem(text)
            {
                Tag = onClick
            };
            item.Click += tsmiItem_Click;
            return item;
        }

        private void cmsMenu_Opening(object sender, CancelEventArgs e)
        {
            //List<string> URWFiles = new List<string>(Directory.GetFiles(DefaultData.GameDirectory));
            //string[] Filters = new[] { "diy_", "menudef_", "mod_menudef_" };

            bool Targetting = player.IsTargetting;
            bool ViewingInventory = player.IsViewingInventory;
            bool MapType = map.IsLocal;
            tsmiTeleport.Visible = Targetting;
            tsmiDirection.Visible = Targetting;
            tsmiMoveMenu.Visible = Targetting;
            tsmiMoveTargetItemsMenu.Visible = Targetting && cheats.MapObjectCheats.GetObjectsByTypeAt<Item>(LastTargetPosition).Count > 0;
            tsmiMoveTargetNPCMenu.Visible = Targetting && cheats.MapObjectCheats.GetObjectsByTypeAt<NPC>(LastTargetPosition).Count > 0;
            tsmiRevealWM.Visible = !MapType;
            tsmiScreenshotWM.Visible = !MapType;
            
            tsmiCraftingMenu.Visible = false;
        }

        private void tsmiItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiItem = sender as ToolStripMenuItem;
            switch (tsmiItem.Name)
            {
                case "tsmiCharacterEditor":
                    Process.Start(new ProcessStartInfo("CustomScripts\\CharacterEditor.html") { UseShellExecute = true });
                    break;
                case "tsmiItemEditor":
                    Process.Start(new ProcessStartInfo("CustomScripts\\ItemEditor.html") { UseShellExecute = true });
                    break;
                case "tsmiInventoryEditor":
                    Process.Start(new ProcessStartInfo("CustomScripts\\InventoryEditor.html") { UseShellExecute = true });
                    break;
                case "tsmiInjuryEditor":
                    Process.Start(new ProcessStartInfo("CustomScripts\\SpriteEditor.html") { UseShellExecute = true });
                    break;
                case "tsmiEditItem":
                    break;
                case "tsmiEditNPC":
                    break;
                case "tsmiEditObject":
                    break;
                case "tsmiTeleportToHere":
                    MessageBox.Show(LastTargetPosition.ToString());
                    break;
                case "tsmiTeleport":
                    //MessageBox.Show(cheats.MapObjectCheats.GetObjectsByType<Item>().Count.ToString() + " ");
                    cheats.MovePlayerTo(LastTargetPosition);
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    //cheats.ShowItemID(LastTargetPosition);
                    break;
                case "tsmiDirection":
                    cheats.FacePlayerTowards(LastTargetPosition);
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiGenerateCT":
                    CheatTableBuilder.GenerateCheatTableFile(
                        groupName: DefaultData.URWVersion,
                        addressClassType: typeof(Address), // <<< PASS YOUR CLASS TYPE HERE
                        processName: "urw.exe",
                        outputFileName: $"URW_{DefaultData.URWVersion}.ct" // Optional file name
                    );
                    break;
                case "tsmiFont":
                    fontDialog.Font = cmsMenu.Font;
                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        cmsMenu.Font = fontDialog.Font;

                        Properties.Settings.Default.MenuFontName = fontDialog.Font.Name;
                        Properties.Settings.Default.MenuFontSize = fontDialog.Font.Size;
                        Properties.Settings.Default.MenuFontStyle = (int)fontDialog.Font.Style;
                        Properties.Settings.Default.Save();

                        DefaultData.FocusWindow();
                    }
                    break;
                case "tsmiRevealWM":
                    cheats.RevealMap();
                    break;
                case "tsmiScreenshotWM":
                    cheats.map.GetImageOverworld().Save($"{player.Name}_WorldMap");
                    break;
                case "tsmiMoveTargetItemsUnderneath":
                    //cheats.MapObjectCheats.MoveItemsToPlayer(LastTargetPosition);
                    cheats.MapObjectCheats.MoveObjectsTo<Item>(LastTargetPosition, player.Location);
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiMoveTargetItemsNextTo":
                    cheats.MapObjectCheats.MoveObjectsTo<Item>(LastTargetPosition, Point.Add(player.Location, (Size)player.DirectionOffset));
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiMoveAllItemsUnderneath":
                    cheats.MapObjectCheats.MoveAllObjectsTo<Item>(player.Location);
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiMoveAllItemsNextTo":
                    cheats.MapObjectCheats.MoveAllObjectsTo<Item>(Point.Add(player.Location, (Size)player.DirectionOffset));
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiMoveAllItemsTo":
                    cheats.MapObjectCheats.MoveAllObjectsTo<Item>(LastTargetPosition);
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiMoveTargetNPCNextTo":
                    cheats.MapObjectCheats.MoveObjectsTo<NPC>(LastTargetPosition, Point.Add(player.Location, (Size)player.DirectionOffset));
                    DefaultData.FocusWindow();
                    SendKeys.Send(".");
                    break;
                case "tsmiCloseMenu":
                    break;
                default:
                    break;
            }
            DefaultData.FocusWindow();
        }

        private async void tsmiItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiItem = sender as ToolStripMenuItem;
            switch (tsmiItem.Name)
            {
                case "tsmiNoInjuries":
                    break;
                case "tsmiNoCarryWeight":
                    cheats.NoCarryWeight = tsmiNoCarryWeight.Checked;
                    break;
                case "tsmiNoNeeds":
                    cheats.NoNeeds = tsmiNoNeeds.Checked;
                    break;
                case "tsmiTreeVision":
                    cheats.TreeVision = tsmiTreeVision.Checked;
                    break;
                case "tsmiCannibalism":
                    cheats.Cannibalism = tsmiCannibalism.Checked;
                    break;
                case "tsmiFreezeNPC":
                    break;
                case "tsmiFreezeTime":
                    break;
                case "tsmiChecksum":
                    cheats.Checksum = tsmiChecksum.Checked;
                    break;
                case "tsmiMinimap":
                    cheats.Minimap = tsmiMinimap.Checked;
                    if (tsmiMinimap.Checked) { await pipeServer.Send("show"); }
                    else if (!tsmiMinimap.Checked) { await pipeServer.Send("hide"); }
                    break;
                default:
                    break;
            }
            DefaultData.FocusWindow();
        }

        private void cmsMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.T) { tsmiTeleport.PerformClick(); }
        }
    }

    public static class Address
    {
        [CheatType("4 Bytes")] public static int PC_ActivityTimeSpent { get; set; } = 0xA2C591C;
        [CheatType("Array of byte", byteLength: 12)] public static int PC_Attributes { get; set; } = 0xA31FFD2;
        [CheatType("Array of byte", byteLength: 100)] public static int PC_Items_IDs { get; set; } = 0xA31FAF8;
        [CheatType("Array of byte", byteLength: 100)] public static int PC_Items_Counts { get; set; } = 0xA31FC88;
        [CheatType("Byte")] public static int PC_IsTargetting { get; set; } = 0x14F1132;
        [CheatType("Byte")] public static int PC_Starvation { get; set; } = 0xA31FFD0;
        [CheatType("Float")] public static int PC_Temperature { get; set; } = 0xA320B3C;
        [CheatType("4 Bytes")] public static int PC_Thirst { get; set; } = 0xA31FEFC;
        [CheatType("Float")] public static int PC_Bloodloss { get; set; } = 0xA31FFB4;
        [CheatType("Byte")] public static int PC_Direction { get; set; } = 0xA378F40;
        [CheatType("4 Bytes")] public static int PC_Height { get; set; } = 0xA31FFC4;
        [CheatType("4 Bytes")] public static int PC_Weight { get; set; } = 0xA31FFC0;
        [CheatType("Float")] public static int PC_Items_Weight { get; set; } = 0xA31FFB8;
        [CheatType("Byte")] public static int PC_InTree { get; set; } = 0x5F97DAA;
        [CheatType("4 Bytes")] public static int PC_Energy { get; set; } = 0xA31FF00;
        [CheatType("Float")] public static int PC_Fatigue { get; set; } = 0xA31FFB0;
        [CheatType("String", length: 12)] public static int PC_Name { get; set; } = 0xA31F510;
        [CheatType("Byte")] public static int PC_Gender { get; set; } = 0xA31FFD1;
        [CheatType("4 Bytes")] public static int PC_Hunger { get; set; } = 0xA320B10;
        [CheatType("Array of byte", byteLength: 20)] public static int PC_Injuries { get; set; } = 0xA31FF08;
        [CheatType("Byte")] public static int PC_TribeID { get; set; } = 0xA31F51D;
        [CheatType("String", length: 18)] public static int PC_TribeName { get; set; } = 0xA31F51E;
        [CheatType("String", length: 28)] public static int PC_PortraitPath { get; set; } = 0xA31FA45;
        [CheatType("4 Bytes")] public static int PC_LocationX { get; set; } = 0xA2C76C8;
        [CheatType("4 Bytes")] public static int PC_LocationY { get; set; } = 0xA320B68;
        [CheatType("4 Bytes")] public static int PC_TargetingX { get; set; } = 0x238092C;
        [CheatType("4 Bytes")] public static int PC_TargetingY { get; set; } = 0x2981D6C;
        [CheatType("4 Bytes")] public static int PC_LastTutorial { get; set; } = 0xA320700;
        [CheatType("4 Bytes")] public static int PC_Nutrition { get; set; } = 0xA31FFA4;
        [CheatType("Byte")] public static int PC_Phobia { get; set; } = 0xA31FFCC;
        [CheatType("Byte")] public static int PC_Physique { get; set; } = 0xA31FFC8;
        [CheatType("Array of byte", byteLength: 29)] public static int PC_Skills { get; set; } = 0xA31F7BA;
        [CheatType("Array of byte", byteLength: 29*12)] public static int PC_SkillNames { get; set; } = 0xA325BE0;
        [CheatType("4 Bytes")] public static int PC_SkillPoints { get; set; } = 0x1D91EC;
        [CheatType("Double")] public static int PC_PickupMultiplier { get; set; } = 0x1D7830;
        [CheatType("4 Bytes")] public static int PC_StartLocationX { get; set; } = 0x1D6C10;
        [CheatType("4 Bytes")] public static int PC_StartLocationY { get; set; } = 0x1D6C14;

        // View states
        [CheatType("Byte")] public static int PC_ViewingMenu { get; set; } = 0x5F20438;
        [CheatType("Byte")] public static int PC_ViewingInventory { get; set; } = 0x619AE2C;
        [CheatType("Byte")] public static int PC_ViewingRecipes { get; set; } = 0x6186C8C;
        [CheatType("Byte")] public static int PC_ViewingWorld { get; set; } = 0x61870F4;

        // Map
        [CheatType("Array of byte", byteLength: 10)] public static int Map_Tiles { get; set; } = 0x8EE918;
        [CheatType("Array of byte", byteLength: 10)] public static int Map_TilesFog { get; set; } = 0x41881C0;
        [CheatType("Array of byte", byteLength: 10)] public static int Map_TilesFog2 { get; set; } = 0x3B86DB8;
        [CheatType("Array of byte", byteLength: 10)] public static int Map_Elevation { get; set; } = 0x2ED510;
        [CheatType("Byte")] public static int Map_Type { get; set; } = 0x5F83CF3;
        [CheatType("Array of byte", byteLength: 36)] public static int Map_Objects { get; set; } = 0xA2CDD7C;
        [CheatType("String", length: 28)] public static int Map_RegionName { get; set; } = 0x3553E70;
        [CheatType("String", length: 28)] public static int Map_RegionNameB { get; set; } = 0x1B3BB9;

        // Time
        [CheatType("4 Bytes")] public static int World_TimeTick { get; set; } = 0xA31FFA8;
        [CheatType("Byte")] public static int World_TimeHour { get; set; } = 0xA31FFAC;
        [CheatType("Byte")] public static int World_TimeMinute { get; set; } = 0xA31FFAD;
        [CheatType("Byte")] public static int World_TimeDay { get; set; } = 0xA31FFD4;
        [CheatType("Byte")] public static int World_TimeMonth { get; set; } = 0xA31FFE0;
        [CheatType("4 Bytes")] public static int World_TimeYear { get; set; } = 0xA31FFE1;

        // Structs
        [CheatType("Array of byte", byteLength: 10)] public static int NPC_Struct { get; set; } = 0x5D45C78;
        [CheatType("Array of byte", byteLength: 172)] public static int Item_Struct { get; set; } = 0x5516538;
        [CheatType("Array of byte", byteLength: 172)] public static int Static_Item_Struct { get; set; } = 0x54F54D0;
        [CheatType("Array of byte", byteLength: 36)] public static int Tile_Struct { get; set; } = 0x1E2370;

        // UI & Mouse
        [CheatType("4 Bytes")] public static int UI_MapRenderSizeX { get; set; } = 0x61B0464;
        [CheatType("4 Bytes")] public static int UI_MapRenderSizeY { get; set; } = 0x61B0468;
        [CheatType("4 Bytes")] public static int UI_MouseXCoord { get; set; } = 0x5F49C74;
        [CheatType("4 Bytes")] public static int UI_MouseYCoord { get; set; } = 0x5F49844;
        [CheatType("4 Bytes")] public static int UI_VisibleRadiusX { get; set; } = 0x61B6A94;
        [CheatType("4 Bytes")] public static int UI_VisibleRadiusY { get; set; } = 0x61B0450;
        [CheatType("Float")] public static int UI_ZoomLevel { get; set; } = 0x61B6A88;
        [CheatType("Float")] public static int UI_TileScaleW { get; set; } = 0x61B6A88;
        [CheatType("Float")] public static int UI_TileScaleH { get; set; } = 0x61B6A88;
        [CheatType("4 Bytes")] public static int UI_Menu_CharacterLoadList { get; set; } = 0x5F28BF0;
        [CheatType("4 Bytes")] public static int UI_Menu_SelectionIndex { get; set; } = 0x5F49940;
        [CheatType("4 Bytes")] public static int UI_Menu_SelectionTag { get; set; } = 0x5F49A48;
        [CheatType("4 Bytes")] public static int UI_Menu_SelectedMenu { get; set; } = 0x5F498C8;
        [CheatType("4 Bytes")] public static int UI_Menu_SelectionList { get; set; } = 0x5F43DA0;
        [CheatType("4 Bytes")] public static int UI_Menu_SelectionListB { get; set; } = 0x6194800;
        [CheatType("Byte")] public static int UI_Menu_IsMenu { get; set; } = 0x5F49C7C;
        [CheatType("4 Bytes")] public static int UI_Index { get; set; } = 0x5F4FF88;

        [CheatType("Array of byte", byteLength: 36)] public static int UI_Inventory { get; set; } = 0x2EA280;
        [CheatType("Byte")] public static int UI_Menu_IsCrafting { get; set; } = 0x61B08D0;
        [CheatType("Byte")] public static int UI_Menu_IsSkills { get; set; } = 0x61B097C;
        [CheatType("String", length: 28)] public static int MsgStruct { get; set; } = 0xA28C7FA;
        [CheatType("Array of byte", byteLength: 10)] public static int TileBaseArray { get; set; } = 0xA297BD8;

        public static void Update()
        {
            PC_ActivityTimeSpent = PC_LocationX - 0x1DA0;
            MsgStruct = PC_LocationX - 0x3AED0;       // Old
            TileBaseArray = PC_LocationX - 0x2FAF0;   // Old

            // Offsets based on PC_LocationY (Could also be based on PC_TribeName with different offsets)
            //PC_Temperature = PC_LocationY - 0x2C;
            PC_Hunger = PC_Temperature - 0x2C;

            // Offsets based on PC_InTree
            //PC_ViewingInventory = PC_InTree + 0x1DDF26; // Old
            PC_ViewingRecipes = PC_InTree + 0x1EFEE2;   // Old (or PC_ViewingInventory + 0x11FBC)
            PC_ViewingWorld = PC_InTree + 0x1F034A;     // Old (or PC_ViewingRecipes + 0x468)
            UI_MapRenderSizeX = PC_InTree + 0x2186BA;
            UI_VisibleRadiusX = PC_InTree + 0x2186A2;   // Closer to UI_MapRenderSizeX - 0x18
            UI_Menu_SelectionListB = PC_InTree + 0x1FCA56;
            UI_Menu_IsCrafting = PC_InTree + 0x218B26;  // Closer to UI_MapRenderSizeY + 0x468
            UI_Menu_IsSkills = PC_InTree + 0x218BD2;    // Closer to UI_Menu_IsCrafting + 0xAC

            // Offsets based on PC_TribeName (Most PC/World stats are here)
            PC_Attributes = PC_TribeName + 0xAB4;
            PC_Items_IDs = PC_TribeName + 0x5DA;
            PC_Items_Counts = PC_TribeName + 0x76A;
            PC_Starvation = PC_TribeName + 0xAB2;
            PC_Thirst = PC_TribeName + 0x9DE;
            PC_Bloodloss = PC_TribeName + 0xA96;
            PC_Height = PC_TribeName + 0xAA6;
            PC_Weight = PC_TribeName + 0xAA2;
            PC_Items_Weight = PC_TribeName + 0xA9A;
            PC_Energy = PC_TribeName + 0x9E2;
            PC_Fatigue = PC_TribeName + 0xA92;
            PC_Name = PC_TribeName - 0xE;
            PC_Gender = PC_TribeName + 0xAB3;
            PC_Injuries = PC_TribeName + 0x9EA;
            PC_TribeID = PC_TribeName - 0x1;
            PC_PortraitPath = PC_TribeName + 0x527;
            PC_LastTutorial = PC_TribeName + 0x11E2;
            PC_Nutrition = PC_TribeName + 0xA86;
            PC_Phobia = PC_TribeName + 0xAAE;
            PC_Physique = PC_TribeName + 0xAAA;
            PC_Skills = PC_TribeName + 0x29C;
            PC_SkillNames = PC_TribeName + 0x12;
            World_TimeTick = PC_TribeName + 0xA8A;
            World_TimeHour = PC_TribeName + 0xA8E;
            World_TimeMinute = PC_TribeName + 0xA8F;
            World_TimeDay = PC_TribeName + 0xAB6;
            World_TimeMonth = PC_TribeName + 0xAC2;
            World_TimeYear = PC_TribeName + 0xAC3;

            // Offsets based on Map_Type (Some UI Elements)
            PC_ViewingMenu = Map_Type - 0x638BB;           // Old
            UI_MouseXCoord = Map_Type - 0x3A07F;
            UI_MouseYCoord = Map_Type - 0x3A4AF;           // Closer to UI_MouseXCoord - 0x430
            UI_Menu_CharacterLoadList = Map_Type - 0x5B103;
            UI_Menu_SelectionIndex = Map_Type - 0x3A3B3;   // Closer to UI_MouseYCoord + 0xFC
            UI_Menu_SelectionTag = Map_Type - 0x3A2AB;     // Closer to UI_Menu_SelectionIndex + 0x108
            UI_Menu_SelectedMenu = Map_Type - 0x3A42B;   // Closer to UI_Menu_SelectionIndex - 0x78
            UI_Menu_SelectionList = Map_Type - 0x3FF53;
            UI_Menu_IsMenu = Map_Type - 0x3A077;           // Closer to UI_MouseXCoord + 0x8

            // Offsets based on Map_Tiles
            PC_IsTargetting = Map_Tiles + 0xC0281A;

            // Offsets based on PC_StartLocationX
            PC_SkillPoints = PC_StartLocationX + 0x25DC;
            PC_PickupMultiplier = PC_StartLocationX - 0x53E0;
            PC_StartLocationY = PC_StartLocationX + 0x4;
            Map_Elevation = PC_StartLocationX + 0x116900;
            Map_RegionNameB = PC_StartLocationX - 0x23057; // Old
            Tile_Struct = PC_StartLocationX + 0xB760;

            // Offsets based on Item_Struct
            Map_RegionName = Item_Struct - 0x1FC26C8;     // Old
            //NPC_Struct = Item_Struct + 0x82F740;

            // Offsets based on other calculated values (for tighter coupling if desired)
            // These are alternatives/refinements to the above bases
            UI_MapRenderSizeY = UI_MapRenderSizeX + 0x4;
            // UI_VisibleRadiusX = UI_MapRenderSizeX - 0x18; // Example if preferred
            UI_VisibleRadiusY = UI_VisibleRadiusX + 0x4;

            
        }

        public static void UpdateIntegers(Dictionary<string, int> intValues)
        {
            Type type = typeof(Address);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(Int32) && intValues.ContainsKey(property.Name))
                {
                    if ((int)property.GetValue(typeof(int)) != intValues[property.Name])
                    {
                        property.SetValue(null, intValues[property.Name]);
                    }
                    else { }
                }
            }
        }

        public static Dictionary<string, int> ParseFile(string filePath)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            foreach (string s in File.ReadAllLines(filePath))
            {
                string[] splitLine = s.Split("=".ToCharArray(), StringSplitOptions.None);
                if (splitLine.Length == 2)
                {
                    string key = splitLine[0].TrimEnd(' ');
                    string value = splitLine[1].TrimStart(' ').TrimEnd(';');
                    values.Add(key, Convert.ToInt32(value, 16));
                }
            }

            return values;
        }

    }

    public static class Code
    {
        public static int PC_HungerGain { get; set; } = 0xD1B99; // 6 bytes
        public static int PC_ThirstGain { get; set; } = 0xD1B73; // 6 bytes
        public static int PC_EnergyLoss { get; set; } = 0xD1B34; // 5 bytes
        public static int PC_NutritionLoss { get; set; } = 0xD1B01; // 5 bytes
        public static int PC_FatigueGain { get; set; } = 0xD18D6; // 8 bytes
        public static int PC_TemperatureChange { get; set; } = 0xD1BBB; // 8 bytes
        public static int PC_WeightChange { get; set; } = 0xD6B58; // 8 bytes
        public static int PC_IsInTreeCheck { get; set; } = 0x1774B9; // 6 bytes

        public static int PC_StarvationLoss { get; set; } = 0x80AF6;

        public static int PC_AttributeChecksum { get; set; } = 0x168F93;
        public static int PC_SkillChecksum { get; set; } = 0x0;

        public static void Update()
        {
            PC_TemperatureChange = PC_HungerGain + 34;
            PC_NutritionLoss = PC_HungerGain - 152;
            PC_ThirstGain = PC_HungerGain - 38;
            PC_EnergyLoss = PC_HungerGain - 101;
        }
    }


    public static class ConsoleOverlayHandler
    {
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        // Import the necessary WinAPI functions
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        // Import the ShowWindow function from user32.dll
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // RECT structure for window dimensions
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Constants for SetWindowPos
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // Constants for ShowWindow
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        const int SW_MAXIMIZE = 3;
        const int SW_RESTORE = 9;


        private static bool isConsoleVisible = true;

        public static void OverlayWindow()
        {
            // Get the console window handle
            IntPtr consoleWindowHandle = GetConsoleWindow();

            // Find the handle of the parent window (replace with actual window name or class)
            IntPtr parentWindowHandle = DefaultData.URW.MainWindowHandle;

            if (parentWindowHandle != IntPtr.Zero)
            {
                //ConsoleHandler.SetFontSize(8, 16);
                // Set the console window as a child of the parent window
                SetParent(consoleWindowHandle, parentWindowHandle);

                SetWindowPos(consoleWindowHandle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                int width = 800;
                int height = 200;

                // Slightly resize the console window to force a redraw and bring it to the front
                MoveWindow(consoleWindowHandle, 0, 0, width + 1, height + 1, true);
                MoveWindow(consoleWindowHandle, 0, 0, width, height, true);

                Console.Title = "URWME Console";
                Console.WriteLine("Console window is now a child of the specified window.");
            }
            else
            {
                Console.WriteLine("Parent window not found.");
            }
        }

        public static void ToggleWindow()
        {
            IntPtr consoleWindowHandle = GetConsoleWindow();
            ShowWindow(consoleWindowHandle, isConsoleVisible ? SW_HIDE : SW_SHOW);
            SetForegroundWindow(isConsoleVisible ? DefaultData.URW.MainWindowHandle : consoleWindowHandle);
            isConsoleVisible = !isConsoleVisible;
            Console.WriteLine("Toggled Console:");
        }
    }

    public class GlobalMouseHook : IDisposable
    {
        // Delegate for the hook callback
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Hook-related variables
        private LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        // Mouse event constants
        private const int WH_MOUSE_LL = 14;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        // Delegate to pass from the main form (e.g., to show a context menu)
        private Action _onRightClick;

        public GlobalMouseHook(Action onRightClick)
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            _onRightClick = onRightClick; // Assign the action to trigger on right-click
        }

        // Set the global mouse hook
        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Hook callback function
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONUP || wParam == (IntPtr) WM_RBUTTONDOWN)
            {
                if (GetForegroundWindow() == DefaultData.URW.MainWindowHandle)
                {
                    Task.Run(() => _onRightClick?.Invoke());
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        // Dispose method to unhook the hook
        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        // Unhook Windows hook
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // Call next hook in the hook chain
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // Set Windows hook
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        // Get module handle
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }

    public class ExternalAppEmbedding
    {
        // Importing SetParent from user32.dll
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        // Importing FindWindow from user32.dll to find the external application window
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Importing SetWindowLongPtr for setting window styles
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private const int GWL_STYLE = -16; // Index for window style
        private const int WS_CHILD = 0x40000000; // Style for child window

        public void EmbedControlIntoExternalApp(Control control)
        {
            // Step 1: Find the external application's window
            IntPtr externalAppHandle = DefaultData.URW.MainWindowHandle;

            if (externalAppHandle == IntPtr.Zero)
            {
                MessageBox.Show("Could not find URW window.");
                //return;
            }

            // Step 2: Set the external application window as the parent of your control
            IntPtr controlHandle = control.Handle;
            SetParent(controlHandle, externalAppHandle);

            // Step 3: (Optional) Modify window styles (if needed)
            // This ensures your control behaves like a child window of the external app.
            SetWindowLongPtr(controlHandle, GWL_STYLE, new IntPtr(WS_CHILD));

        }
    }



}
