using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace URWME // Unreal World MemoryManager
{

    public static class DefaultData
    {
        public static Dictionary<int, object[]> DefaultMapTiles = new Dictionary<int, object[]>();
        public static Dictionary<int, object[]> DefaultWaterTiles = new Dictionary<int, object[]>();
        public static Dictionary<int, object[]> DefaultGroundTiles = new Dictionary<int, object[]>();
        public static Dictionary<int, object[]> DefaultOverlayTiles = new Dictionary<int, object[]>();
        public static Dictionary<int, object[]> DefaultTreeTiles = new Dictionary<int, object[]>();
        public static Dictionary<int, object[]> DefaultStructureTiles = new Dictionary<int, object[]>();
        private static string GameFolderDefault = "";
        private static string GameVersion = "";
        private static Process URWCurrentProcess = new Process();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void FocusWindow(nint Handle = 0)
        {
            if (Handle == 0) { SetForegroundWindow(URW.MainWindowHandle); }
            else { SetForegroundWindow(Handle); }
        }

        public static string SteamDirectory
        {
            get
            {
                return Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", "Key not found").ToString();
            }
        }

        public static string GameDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(GameFolderDefault))
                {
                    try
                    {
                        FileVersionInfo Info = URW.MainModule.FileVersionInfo;
                        GameFolderDefault = Info.FileName.Substring(0, Info.FileName.Length - 7);
                        return GameFolderDefault;
                    }
                    catch { return ""; } // Game not running
                }
                else { return GameFolderDefault; }
            }
        }

        public static List<string> CharacterFolders
        {
            get
            {
                List<string> Return = new List<string>();
                foreach (string Folder in Directory.GetDirectories(GameDirectory))
                {
                    if (Folder.Split('\\').Last().IsUpper() && !Folder.Contains("ANCESTORS"))
                    {
                        Return.Add(Folder);
                    }
                }
                return Return;
            }
        }

        public static bool IsUpper(this string s)
        {
            foreach (char c in s)
            {
                if (Char.IsLower(c)) { return false; }
            }
            return true;
        }

        public static Process URW
        {
            get
            {
                Process[] Procs = Process.GetProcessesByName("urw");
                if (Procs.Length > 0) // proc is running
                {
                    URWCurrentProcess = Procs[0];
                }
                else if (File.Exists(Environment.CurrentDirectory + "\\urw.exe")) // current directory owns proc
                {
                    ProcessStartInfo PSI = new ProcessStartInfo() { WorkingDirectory = Environment.CurrentDirectory, FileName = "urw.exe" };
                    Process.Start(PSI);
                    //URWCurrentProcess.WaitForInputIdle();
                    URWCurrentProcess = URW;
                }
                else // check for steam exe
                {
                    string sFilePath = SteamDirectory + @"\steamapps\common\UnRealWorld\urw.exe";
                    string sWorkingDirectory = SteamDirectory + @"\steamapps\common\UnRealWorld";
                    if (File.Exists(sFilePath))
                    {
                        ProcessStartInfo PSI = new ProcessStartInfo() { WorkingDirectory = sWorkingDirectory, FileName = sFilePath };
                        Process.Start(PSI);
                        //URWCurrentProcess.WaitForInputIdle();
                        URWCurrentProcess = URW;
                    }
                    else
                    {
                        try
                        {
                            Process.Start("steam://rungameid/351700");
                            URWCurrentProcess = URW;
                        }
                        catch (Exception E) { throw E; }
                    }
                }
                URWCurrentProcess.EnableRaisingEvents = true;
                URWCurrentProcess.Exited += (sender, e) =>
                {
                    //Console.WriteLine("External process exited.");
                    Environment.Exit(0);
                };
                //URWCurrentProcess.WaitForInputIdle();
                return URWCurrentProcess;
            }
        }

        public static Encoding URWEncoding = Encoding.GetEncoding("utf-8");
        public static string URWVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(GameVersion)) { return GameVersion; }
                else if (File.Exists(GameDirectory + "\\news.txt"))
                {
                    foreach (string Line in File.ReadAllLines(GameDirectory + "\\news.txt"))
                    {
                        if (Line.StartsWith("Version")) { GameVersion = Line.Split(' ')[1]; return GameVersion; }
                    }
                }
                return "News.txt not found.";
            }
        }

        public static void Load()
        {
            if (!string.IsNullOrEmpty(GameDirectory))
            {
                LoadDefaultTileData();
            }
            else
            {
                throw new Exception("Could not load default data. Please make sure game client is running.");
            }

        }

        public static Dictionary<byte, string> Qualities(int iType)
        {
            Dictionary<byte, string> Return = new Dictionary<byte, string>();
            switch (iType)
            {
                case ItemType.Armour: // 129, 130, 131/0, 132, 133
                    Return.Add(129, "Crude ");
                    Return.Add(130, "Rough ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Masterwork ");
                    break;
                case ItemType.Weapon:
                    Return.Add(129, "Crude ");
                    Return.Add(130, "Rough ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Masterwork ");
                    break;
                case ItemType.Carcass:
                    Return.Add(129, "Grisly ");
                    Return.Add(130, "Mangled ");
                    Return.Add(131, "Hacked ");
                    Return.Add(132, "Torn ");
                    Return.Add(133, "Harmed ");
                    Return.Add(0, "Decent ");
                    break;
                case ItemType.Container:
                    Return.Add(129, "Leaky ");
                    Return.Add(130, "Rough ");
                    Return.Add(131, "Decent");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Beautiful ");
                    break;
                case ItemType.Tool:
                    Return.Add(129, "Crude ");
                    Return.Add(130, "Rough ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Masterwork ");
                    break;
                case ItemType.Valuable:
                    Return.Add(129, "Poor ");
                    Return.Add(130, "Inferior ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Perfect ");
                    break;
                case ItemType.Vehicle:
                    Return.Add(129, "Poor ");
                    Return.Add(130, "Inferior ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Perfect ");
                    break;
                case ItemType.Plant:
                    Return.Add(129, "Awful ");
                    Return.Add(130, "Bland ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent ");
                    Return.Add(132, "Tasty ");
                    Return.Add(133, "Delicious ");
                    break;
                case ItemType.Skin:
                    Return.Add(129, "Ragged ");
                    Return.Add(130, "Harsh ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Superior ");
                    break;
                case ItemType.Timber:
                    Return.Add(129, "Poor ");
                    Return.Add(130, "Inferior ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Fine ");
                    Return.Add(133, "Perfect ");
                    break;
                case ItemType.Food:
                    Return.Add(129, "Awful ");
                    Return.Add(130, "Bland ");
                    Return.Add(131, "Decent ");
                    Return.Add(0, "Decent(0) ");
                    Return.Add(132, "Tasty ");
                    Return.Add(133, "Delicious ");
                    break;
                default:
                    //MessageBox.Show(iType.ToString());
                    break;
            }
            return Return;
        }

        private static void LoadDefaultTileData()
        {
            DefaultMapTiles.Add(3, new object[] { Color.FromArgb(0, 236, 0), GameDirectory + "truetile\\ter-lichpine.png" });
            DefaultMapTiles.Add(15, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-conifore.png" });
            DefaultMapTiles.Add(20, new object[] { Color.FromArgb(200, 200, 200), GameDirectory + "truetile\\ter-hicliff.png" });
            DefaultMapTiles.Add(21, new object[] { Color.FromArgb(255, 87, 247), GameDirectory + "truetile\\ter-pit.png" });
            DefaultMapTiles.Add(30, new object[] { Color.FromArgb(255, 21, 232), GameDirectory + "truetile\\ter-kotacamp.png" });
            DefaultMapTiles.Add(31, new object[] { Color.FromArgb(171, 87, 0), GameDirectory + "truetile\\ter-shelter.png" });
            DefaultMapTiles.Add(46, new object[] { Color.FromArgb(31, 31, 31), GameDirectory + "truetile\\ter-road.png" });
            DefaultMapTiles.Add(47, new object[] { Color.FromArgb(0, 0, 0), GameDirectory + "truetile\\ter-pasture.png" });
            DefaultMapTiles.Add(58, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-ground.png" });
            DefaultMapTiles.Add(60, new object[] { Color.FromArgb(255, 255, 255), GameDirectory + "truetile\\ter-cave.png" });
            DefaultMapTiles.Add(86, new object[] { Color.FromArgb(198, 0, 0), GameDirectory + "truetile\\ter-snghouse.png" });
            DefaultMapTiles.Add(91, new object[] { Color.FromArgb(150, 150, 150), GameDirectory + "truetile\\ter-hill.png" });
            DefaultMapTiles.Add(93, new object[] { Color.FromArgb(200, 200, 200), GameDirectory + "truetile\\ter-cliff.png" });
            DefaultMapTiles.Add(94, new object[] { Color.FromArgb(240, 240, 240), GameDirectory + "truetile\\ter-mount.png" });
            DefaultMapTiles.Add(176, new object[] { Color.FromArgb(50, 50, 100), GameDirectory + "truetile\\ter-ford.png" });
            DefaultMapTiles.Add(177, new object[] { Color.FromArgb(210, 210, 240), GameDirectory + "truetile\\ter-rapids.png" });
            DefaultMapTiles.Add(178, new object[] { Color.FromArgb(108, 121, 216), GameDirectory + "truetile\\ter-river.png" });
            DefaultMapTiles.Add(180, new object[] { Color.FromArgb(8, 20, 200), GameDirectory + "truetile\\ter-sea.png" });
            DefaultMapTiles.Add(181, new object[] { Color.FromArgb(0, 75, 215), GameDirectory + "truetile\\ter-shsea.png" });
            DefaultMapTiles.Add(182, new object[] { Color.FromArgb(8, 8, 216), GameDirectory + "truetile\\ter-dpwater.png" });
            DefaultMapTiles.Add(183, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-shwater.png" });
            DefaultMapTiles.Add(210, new object[] { Color.FromArgb(64, 0, 0), GameDirectory + "truetile\\ter-fortvill.png" });
            DefaultMapTiles.Add(211, new object[] { Color.FromArgb(128, 0, 0), GameDirectory + "truetile\\ter-snghouse.png" });
            DefaultMapTiles.Add(228, new object[] { Color.FromArgb(0, 80, 0), GameDirectory + "truetile\\ter-thicket.png" });
            DefaultMapTiles.Add(233, new object[] { Color.FromArgb(0, 40, 0), GameDirectory + "truetile\\ter-openmire.png" });
            DefaultMapTiles.Add(235, new object[] { Color.FromArgb(0, 128, 0), GameDirectory + "truetile\\ter-sprmire.png" });
            DefaultMapTiles.Add(236, new object[] { Color.FromArgb(0, 80, 0), GameDirectory + "truetile\\ter-pinemire.png" });
            DefaultMapTiles.Add(238, new object[] { Color.FromArgb(0, 200, 0), GameDirectory + "truetile\\ter-heath.png" });
            DefaultMapTiles.Add(239, new object[] { Color.FromArgb(244, 244, 131), GameDirectory + "truetile\\ter-village.png" });
            DefaultMapTiles.Add(244, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-water.png" });
            DefaultMapTiles.Add(247, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-water.png" });
            DefaultMapTiles.Add(250, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-ground.png" });
            DefaultMapTiles.Add(251, new object[] { Color.FromArgb(0, 240, 0), GameDirectory + "truetile\\ter-field.png" });
            DefaultMapTiles.Add(4, new object[] { Color.FromArgb(69, 65, 81), GameDirectory + "truetile\\ter-firepla.png" });
            DefaultMapTiles.Add(5, new object[] { Color.FromArgb(27, 24, 36), GameDirectory + "truetile\\ter-saustove.png" });
            DefaultMapTiles.Add(8, new object[] { Color.FromArgb(83, 87, 115), GameDirectory + "truetile\\ter-rowan.png" });
            DefaultMapTiles.Add(10, new object[] { Color.FromArgb(255, 255, 255), GameDirectory + "truetile\\ter-birch.png" });
            DefaultMapTiles.Add(12, new object[] { Color.FromArgb(43, 61, 29), GameDirectory + "truetile\\ter-yspruce.png" });
            DefaultMapTiles.Add(13, new object[] { Color.FromArgb(61, 78, 31), GameDirectory + "truetile\\ter-spruce.png" });
            DefaultMapTiles.Add(16, new object[] { Color.FromArgb(83, 87, 115), GameDirectory + "truetile\\ter-grove.png" });
            DefaultMapTiles.Add(17, new object[] { Color.FromArgb(98, 38, 6), GameDirectory + "truetile\\ter-ypine.png" });
            DefaultMapTiles.Add(18, new object[] { Color.FromArgb(112, 82, 66), GameDirectory + "truetile\\ter-pine.png" });
            DefaultMapTiles.Add(19, new object[] { Color.FromArgb(172, 117, 77), GameDirectory + "truetile\\ter-bunk.png" });
            DefaultMapTiles.Add(22, new object[] { Color.FromArgb(111, 69, 47), GameDirectory + "truetile\\ter-doorop.png" });
            DefaultMapTiles.Add(23, new object[] { Color.FromArgb(111, 69, 47), GameDirectory + "truetile\\ter-shutter.png" });
            DefaultMapTiles.Add(24, new object[] { Color.FromArgb(83, 87, 115), GameDirectory + "truetile\\ter-alder.png" });
            DefaultMapTiles.Add(28, new object[] { Color.FromArgb(172, 117, 77), GameDirectory + "truetile\\ter-bench.png" });
            DefaultMapTiles.Add(29, new object[] { Color.FromArgb(192, 115, 94), GameDirectory + "truetile\\ter-antnest.png" });
            DefaultMapTiles.Add(39, new object[] { Color.FromArgb(70, 71, 77), GameDirectory + "truetile\\ter-boulder.png" });
            DefaultMapTiles.Add(40, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });
            DefaultMapTiles.Add(41, new object[] { Color.FromArgb(95, 67, 31), GameDirectory + "truetile\\ter-kotafram.png" });
            DefaultMapTiles.Add(42, new object[] { Color.FromArgb(84, 59, 26), GameDirectory + "truetile\\ter-fence.png" });
            DefaultMapTiles.Add(43, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotadoor.png" });
            DefaultMapTiles.Add(44, new object[] { Color.FromArgb(84, 59, 26), GameDirectory + "truetile\\ter-fenceclg.png" });
            DefaultMapTiles.Add(45, new object[] { Color.FromArgb(84, 59, 26), GameDirectory + "truetile\\ter-fenceopg.png" });
            DefaultMapTiles.Add(81, new object[] { Color.FromArgb(95, 67, 31), GameDirectory + "truetile\\ter-cellar.png" });
            DefaultMapTiles.Add(82, new object[] { Color.FromArgb(59, 42, 20), GameDirectory + "truetile\\ter-prepsoil.png" });
            DefaultMapTiles.Add(95, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });
            DefaultMapTiles.Add(96, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });
            DefaultMapTiles.Add(136, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotaw.png" });
            DefaultMapTiles.Add(138, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotanwc.png" });
            DefaultMapTiles.Add(139, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotanec.png" });
            DefaultMapTiles.Add(140, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotaswc.png" });
            DefaultMapTiles.Add(141, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotasec.png" });
            DefaultMapTiles.Add(142, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotan.png" });
            DefaultMapTiles.Add(143, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotas.png" });
            DefaultMapTiles.Add(144, new object[] { Color.FromArgb(135, 135, 135), GameDirectory + "truetile\\ter-kotae.png" });
            DefaultMapTiles.Add(188, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-walle.png" });
            DefaultMapTiles.Add(191, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-wallnec.png" });
            DefaultMapTiles.Add(192, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-wallswc.png" });
            DefaultMapTiles.Add(196, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-walln.png" });
            DefaultMapTiles.Add(200, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-wallw.png" });
            DefaultMapTiles.Add(208, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-walls.png" });
            DefaultMapTiles.Add(217, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-wallsec.png" });
            DefaultMapTiles.Add(218, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-wallnwc.png" });
            DefaultMapTiles.Add(237, new object[] { Color.FromArgb(97, 120, 14), GameDirectory + "truetile\\ter-bushes.png" });
            DefaultMapTiles.Add(240, new object[] { Color.FromArgb(172, 117, 77), GameDirectory + "truetile\\ter-table.png" });
            DefaultMapTiles.Add(254, new object[] { Color.FromArgb(86, 54, 36), GameDirectory + "truetile\\ter-doorcl.png" });


            DefaultWaterTiles.Add(176, new object[] { Color.FromArgb(50, 50, 100), GameDirectory + "truetile\\ter-ford.png" });
            DefaultWaterTiles.Add(177, new object[] { Color.FromArgb(210, 210, 240), GameDirectory + "truetile\\ter-rapids.png" });
            DefaultWaterTiles.Add(178, new object[] { Color.FromArgb(108, 121, 216), GameDirectory + "truetile\\ter-river.png" });
            DefaultWaterTiles.Add(180, new object[] { Color.FromArgb(8, 20, 200), GameDirectory + "truetile\\ter-sea.png" });
            DefaultWaterTiles.Add(181, new object[] { Color.FromArgb(0, 75, 215), GameDirectory + "truetile\\ter-shsea.png" });
            DefaultWaterTiles.Add(182, new object[] { Color.FromArgb(8, 8, 216), GameDirectory + "truetile\\ter-dpwater.png" });
            DefaultWaterTiles.Add(183, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-shwater.png" });
            DefaultWaterTiles.Add(244, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-water.png" });
            DefaultWaterTiles.Add(247, new object[] { Color.FromArgb(8, 8, 244), GameDirectory + "truetile\\ter-water.png" });

            DefaultGroundTiles.Add(250, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-ground.png" });
            DefaultGroundTiles.Add(58, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-ground.png" });
            DefaultGroundTiles.Add(40, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });
            DefaultGroundTiles.Add(95, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });
            DefaultGroundTiles.Add(96, new object[] { Color.FromArgb(103, 103, 135), GameDirectory + "truetile\\ter-gcfloor.png" });

            DefaultOverlayTiles.Add(20, new object[] { Color.FromArgb(200, 200, 200), GameDirectory + "truetile\\ter-hicliff.png" });
            DefaultOverlayTiles.Add(21, new object[] { Color.FromArgb(255, 87, 247), GameDirectory + "truetile\\ter-pit.png" });
            DefaultOverlayTiles.Add(46, new object[] { Color.FromArgb(31, 31, 31), GameDirectory + "truetile\\ter-road.png" });
            DefaultOverlayTiles.Add(47, new object[] { Color.FromArgb(0, 0, 0), GameDirectory + "truetile\\ter-pasture.png" });
            DefaultOverlayTiles.Add(60, new object[] { Color.FromArgb(255, 255, 255), GameDirectory + "truetile\\ter-cave.png" });
            DefaultOverlayTiles.Add(91, new object[] { Color.FromArgb(150, 150, 150), GameDirectory + "truetile\\ter-hill.png" });
            DefaultOverlayTiles.Add(93, new object[] { Color.FromArgb(200, 200, 200), GameDirectory + "truetile\\ter-cliff.png" });
            DefaultOverlayTiles.Add(94, new object[] { Color.FromArgb(240, 240, 240), GameDirectory + "truetile\\ter-mount.png" });
            DefaultOverlayTiles.Add(251, new object[] { Color.FromArgb(0, 240, 0), GameDirectory + "truetile\\ter-field.png" });
            DefaultOverlayTiles.Add(82, new object[] { Color.FromArgb(59, 42, 20), GameDirectory + "truetile\\ter-prepsoil.png" });
            DefaultOverlayTiles.Add(237, new object[] { Color.FromArgb(97, 120, 14), GameDirectory + "truetile\\ter-bushes.png" });
            DefaultOverlayTiles.Add(29, new object[] { Color.FromArgb(192, 115, 94), GameDirectory + "truetile\\ter-antnest.png" });
            DefaultOverlayTiles.Add(39, new object[] { Color.FromArgb(70, 71, 77), GameDirectory + "truetile\\ter-boulder.png" });

            DefaultTreeTiles.Add(3, new object[] { Color.FromArgb(0, 236, 0), GameDirectory + "truetile\\ter-lichpine.png" });
            DefaultTreeTiles.Add(15, new object[] { Color.FromArgb(0, 160, 0), GameDirectory + "truetile\\ter-conifore.png" });
            DefaultTreeTiles.Add(228, new object[] { Color.FromArgb(0, 80, 0), GameDirectory + "truetile\\ter-thicket.png" });
            DefaultTreeTiles.Add(233, new object[] { Color.FromArgb(0, 40, 0), GameDirectory + "truetile\\ter-openmire.png" });
            DefaultTreeTiles.Add(235, new object[] { Color.FromArgb(0, 128, 0), GameDirectory + "truetile\\ter-sprmire.png" });
            DefaultTreeTiles.Add(236, new object[] { Color.FromArgb(0, 80, 0), GameDirectory + "truetile\\ter-pinemire.png" });
            DefaultTreeTiles.Add(238, new object[] { Color.FromArgb(0, 200, 0), GameDirectory + "truetile\\ter-heath.png" });
            DefaultTreeTiles.Add(8, new object[] { Color.FromArgb(83, 87, 115), GameDirectory + "truetile\\ter-rowan.png" });
            DefaultTreeTiles.Add(10, new object[] { Color.FromArgb(255, 255, 255), GameDirectory + "truetile\\ter-birch.png" });
            DefaultTreeTiles.Add(12, new object[] { Color.FromArgb(43, 61, 29), GameDirectory + "truetile\\ter-yspruce.png" });
            DefaultTreeTiles.Add(13, new object[] { Color.FromArgb(61, 78, 31), GameDirectory + "truetile\\ter-spruce.png" });
            DefaultTreeTiles.Add(16, new object[] { Color.FromArgb(83, 87, 115), GameDirectory + "truetile\\ter-grove.png" });
            DefaultTreeTiles.Add(17, new object[] { Color.FromArgb(98, 38, 6), GameDirectory + "truetile\\ter-ypine.png" });
            DefaultTreeTiles.Add(18, new object[] { Color.FromArgb(112, 82, 66), GameDirectory + "truetile\\ter-pine.png" });

            DefaultStructureTiles.Add(30, new object[] { Color.FromArgb(255, 21, 232), GameDirectory + "truetile\\ter-kotacamp.png" });
            DefaultStructureTiles.Add(31, new object[] { Color.FromArgb(171, 87, 0), GameDirectory + "truetile\\ter-shelter.png" });
            DefaultStructureTiles.Add(86, new object[] { Color.FromArgb(198, 0, 0), GameDirectory + "truetile\\ter-snghouse.png" });
            DefaultStructureTiles.Add(210, new object[] { Color.FromArgb(64, 0, 0), GameDirectory + "truetile\\ter-fortvill.png" });
            DefaultStructureTiles.Add(211, new object[] { Color.FromArgb(128, 0, 0), GameDirectory + "truetile\\ter-snghouse.png" });
            DefaultStructureTiles.Add(239, new object[] { Color.FromArgb(244, 244, 131), GameDirectory + "truetile\\ter-village.png" });

        }
    }

}
