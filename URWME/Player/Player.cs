using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace URWME // Unreal World MemoryManager
{
    public partial class Player
    {
        ReadWriteMem RWMain;
        public SkillArray SkillsArr;
        public AttributeArray AttributesArr;
        public InjuryArray Injuries;
        public Inventory Inventory;

        public Player(ReadWriteMem RW)
        {
            RWMain = RW;
            SkillsArr = new SkillArray(RWMain);
            AttributesArr = new AttributeArray(RWMain);
            Injuries = new InjuryArray(RWMain);
            Inventory = new Inventory(RWMain);
        }

        public string Name
        {
            get { return RWMain.Read<string>(Address.PC_Name, 12); }
            set { RWMain.Write(Address.PC_Name, value + '\0'); }
        }

        public string TribeName
        {
            get { return RWMain.Read<string>(Address.PC_TribeName, 28); }
            set { RWMain.Write(Address.PC_TribeName, value + '\0'); }
        }

        public string PortraitPath
        {
            get { return RWMain.Read<string>(Address.PC_PortraitPath, 20); }
            set { RWMain.Write(Address.PC_PortraitPath, value + '\0'); }
        }

        public string PortraitFullPath
        {
            get { return @$"{DefaultData.GameDirectory}truegfx\{PortraitPath}".Replace(@"\", @"/"); }
        }

        public byte TribeID
        {
            get { return RWMain.Read<byte>(Address.PC_TribeID); }
            set { RWMain.Write(Address.PC_TribeID, value); }
        }

        public byte Gender
        {
            get { return RWMain.Read<byte>(Address.PC_Gender); }
            set { RWMain.Write(Address.PC_Gender, value); }
        }

        public string GenderString
        {
            get { return (Gender == 1) ? "Male" : "Female"; }
        }

        public Point Location
        {
            get { return new Point(RWMain.Read<int>(Address.PC_LocationX), RWMain.Read<int>(Address.PC_LocationY)); }
            set { RWMain.Write(Address.PC_LocationX, value.X); RWMain.Write(Address.PC_LocationY, value.Y); }
        }

        public Point StartLocation
        {
            get { return new Point(RWMain.Read<int>(Address.PC_StartLocationX), RWMain.Read<int>(Address.PC_StartLocationY)); }
            set { RWMain.Write(Address.PC_StartLocationX, value.X); RWMain.Write(Address.PC_StartLocationY, value.Y); }
        }

        public int Direction
        {
            get { return RWMain.Read<int>(Address.PC_Direction); }
            set { RWMain.Write(Address.PC_Direction, value); }
        }

        public string DirectionString
        {
            get
            {
                return Direction switch
                {
                    4 => "West",
                    3 => "Northwest",
                    2 => "North",
                    1 => "Northeast",
                    0 => "East",
                    7 => "Southeast",
                    6 => "South",
                    5 => "Southwest",
                    _ => "Unknown"
                };
            }
        }

        public Point DirectionOffset => Direction switch
        {
            0 => new Point(1, 0),   // East
            1 => new Point(1, -1),  // Northeast
            2 => new Point(0, -1),  // North
            3 => new Point(-1, -1), // Northwest
            4 => new Point(-1, 0),  // West
            5 => new Point(-1, 1),  // Southwest
            6 => new Point(0, 1),   // South
            7 => new Point(1, 1),   // Southeast
            _ => Point.Empty
        };

        public float InventoryWeight // Inventory/Carried weight
        {
            get { return RWMain.Read<float>(Address.PC_Items_Weight); }
            set { RWMain.Write(Address.PC_Items_Weight, value); }
        }

        public float Temperature
        {
            get { return RWMain.Read<float>(Address.PC_Temperature); }
            set { RWMain.Write(Address.PC_Temperature, value); }
        }

        public float Fatigue
        {
            get { return RWMain.Read<float>(Address.PC_Fatigue); }
            set { RWMain.Write(Address.PC_Fatigue, value); }
        }

        public int Hunger
        {
            get { return RWMain.Read<int>(Address.PC_Hunger); }
            set { RWMain.Write(Address.PC_Hunger, value); }
        }

        public int Nutrition
        {
            get { return RWMain.Read<int>(Address.PC_Nutrition); }
            set { RWMain.Write(Address.PC_Nutrition, value); }
        }

        public uint CurrentObjective
        {
            get { return RWMain.Read<uint>(Address.PC_LastTutorial); }
            set { RWMain.Write(Address.PC_LastTutorial, value); }
        }

        public byte Starvation
        {
            get { return RWMain.Read<byte>(Address.PC_Starvation); }
            set { RWMain.Write(Address.PC_Starvation, value); }
        }

        public byte Phobia // Unused
        {
            get { return RWMain.Read<byte>(Address.PC_Phobia); }
            set { RWMain.Write(Address.PC_Phobia, value); }
        }

        public byte Physique // Unused
        {
            get { return RWMain.Read<byte>(Address.PC_Physique); }
            set { RWMain.Write(Address.PC_Physique, value); }
        }

        public byte Bloodloss
        {
            get { return RWMain.Read<byte>(Address.PC_Bloodloss); }
            set { RWMain.Write(Address.PC_Bloodloss, value); }
        }

        public byte SkillPoints
        {
            get { return RWMain.Read<byte>(Address.PC_SkillPoints); }
            set { RWMain.Write(Address.PC_SkillPoints, value); }
        }

        public int Energy
        {
            get { return RWMain.Read<int>(Address.PC_Energy); }
            set { RWMain.Write(Address.PC_Energy, value); }
        }

        public int Thirst
        {
            get { return RWMain.Read<int>(Address.PC_Thirst); }
            set { RWMain.Write(Address.PC_Thirst, value); }
        }

        public int Height
        {
            get { return RWMain.Read<int>(Address.PC_Height); }
            set { RWMain.Write(Address.PC_Height, value); }
        }

        public int Weight // Character weight
        {
            get { return RWMain.Read<int>(Address.PC_Weight); }
            set { RWMain.Write(Address.PC_Weight, value); }
        }

        public bool IsTargetting
        {
            get { return RWMain.Read<bool>(Address.PC_IsTargetting); }
            set { RWMain.Write(Address.PC_IsTargetting, value); }
        }

        public bool IsViewingWorld
        {
            get { return RWMain.Read<bool>(Address.PC_ViewingWorld); }
        }

        public bool IsViewingInventory
        {
            get { return RWMain.Read<bool>(Address.PC_ViewingInventory); }
        }

        public bool IsViewingMenu
        {
            get { return RWMain.Read<bool>(Address.PC_ViewingMenu); }
        }

        public bool IsViewingRecipes
        {
            get { return RWMain.Read<bool>(Address.PC_ViewingRecipes); }
        }

        public bool IsInTree
        {
            get { return RWMain.Read<bool>(Address.PC_InTree); }
            set { RWMain.Write(Address.PC_InTree, value); }
        }

        public Dictionary<string, object> Attributes
        {
            get
            {
                return AttributesArr.AsDictionary();
            }
        }

        public Dictionary<string, object> Skills
        {
            get
            {
                return SkillsArr.AsDictionary();
            }
        }

        public string Json()
        {
            return this.ToJson();
        }

        public class InjuryArray
        {
            ReadWriteMem RWMain;
            public InjuryArray(ReadWriteMem RW)
            {
                RWMain = RW;
            }

            public Injury this[int Index]
            {
                get { return new Injury(RWMain, Index); }
            }

            public byte[] Buffer
            {
                get { return RWMain.Read<byte[]>(Address.PC_Injuries, 120); }
                set { RWMain.Write(Address.PC_Injuries, value); }
            }

            public byte TotalDamage
            {
                get
                {
                    byte Total = 0;
                    for (int i = 0; i < 20; i++)
                    {
                        Total += this[i].Damage;
                    }
                    return Total;
                }
            }
        }

        public class SkillArray
        {
            ReadWriteMem RWMain;
            public SkillArray(ReadWriteMem RW)
            {
                RWMain = RW;
            }

            public Skill this[int Index]
            {
                get { return new Skill(RWMain, Index); }
            }

            public Skill this[string Name]
            {
                get { return new Skill(RWMain, Name); }
            }


            public Dictionary<string, object> AsDictionary()
            {
                Dictionary<string, object> Return = new Dictionary<string, object>();
                for (int i = 0; i < Skill.SkillNames.Count; i++)
                {
                    Return.Add(Skill.SkillNames[i], this[i]);
                }
                return Return;
            }

            public byte[] Buffer
            {
                get { return RWMain.Read<byte[]>(Address.PC_Skills, Skill.SkillNames.Count); }
                set { RWMain.Write(Address.PC_Skills, value); }
            }
        }

        public class AttributeArray
        {
            ReadWriteMem RWMain;
            public AttributeArray(ReadWriteMem RW)
            {
                RWMain = RW;
            }

            public Attribute this[int Index]
            {
                get { return new Attribute(RWMain, Index); }
            }

            public Attribute this[string Name]
            {
                get { return new Attribute(RWMain, Name); }
            }

            public Dictionary<string, object> AsDictionary()
            {
                Dictionary<string, object> Return = new Dictionary<string, object>();
                for (int i = 0; i < Attribute.AttributeNames.Length; i++)
                {
                    Return.Add(Attribute.AttributeNames[i], this[i]);
                }
                return Return;
            }

        }

        public Point GetMouseTile()
        {
            if (RWMain.Read<byte>(Address.PC_IsTargetting) == 1)
            {
                return new Point(RWMain.Read<int>(Address.PC_TargetingX), RWMain.Read<int>(Address.PC_TargetingY));
            }
            else { return Point.Empty; }
        }


        public Point GetMouseTileOld()
        {
            int CurrentMouseX = RWMain.Read<int>(Address.UI_MouseXCoord), CurrentMouseY = RWMain.Read<int>(Address.UI_MouseYCoord);
            int MapRenderX = RWMain.Read<int>(Address.UI_MapRenderSizeX), MapRenderY = RWMain.Read<int>(Address.UI_MapRenderSizeY);
            int RadiusX = RWMain.Read<int>(Address.UI_VisibleRadiusX), RadiusY = RWMain.Read<int>(Address.UI_VisibleRadiusY);
            byte IsLocal = RWMain.Read<byte>(Address.Map_Type);
            bool IsTargetting = RWMain.Read<byte>(Address.PC_IsTargetting) == 0;
            int TileSizeX = MapRenderX / ((RadiusX * 2) + 1);
            int TileSizeY = MapRenderY / ((RadiusY * 2) + 1);
            int TargetX, TargetY;
            Point CurrentLocation = Location;

            if (CurrentMouseX >= 14 && CurrentMouseY >= 84 && CurrentMouseX <= 464 && CurrentMouseY <= 544 && IsTargetting && MapRenderX == 450 && IsLocal == 2)
            {
                TargetX = (((int)CurrentMouseX - 14) / TileSizeX) - RadiusX;
                TargetY = (((int)CurrentMouseY - 84) / TileSizeY) - RadiusY;
                if (CurrentLocation.X + TargetX <= 328)
                {
                    if (CurrentLocation.X + TargetX >= 136)
                    {
                        CurrentLocation = new Point(CurrentLocation.X + TargetX, CurrentLocation.Y);
                    }
                    else
                    {
                        CurrentLocation = new Point(136, CurrentLocation.Y);
                    }
                }
                else
                {
                    CurrentLocation = new Point(328, CurrentLocation.Y);
                }
                if (CurrentLocation.Y + TargetY <= 328)
                {
                    if (CurrentLocation.Y + TargetY >= 136)
                    {
                        CurrentLocation = new Point(CurrentLocation.X, CurrentLocation.Y + TargetY);
                    }
                    else
                    {
                        CurrentLocation = new Point(CurrentLocation.X, 136);
                    }
                }
                else
                {
                    CurrentLocation = new Point(CurrentLocation.X, 328);
                }
            }
            return CurrentLocation;
        }

        public double GetMouseAngle()
        {
            // Calculate the distance between the mouse and the target position
            int CurrentMouseX = RWMain.Read<int>(Address.UI_MouseXCoord), CurrentMouseY = RWMain.Read<int>(Address.UI_MouseYCoord);
            if (CurrentMouseX >= 14 && CurrentMouseY >= 84 && CurrentMouseX <= 464 && CurrentMouseY <= 544)
            {
                double dx = 225 + 14 - CurrentMouseX;
                double dy = 230 + 84 - CurrentMouseY;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // Calculate the angle in radians
                double angleRadians = Math.Atan2(dy, dx);

                // Convert the angle from radians to degrees
                double angleDegrees = angleRadians * 180 / Math.PI;

                // Return the angle in degrees
                return angleDegrees;
            }
            else return 0;
        }

        public void SetDirectionToAngle(double angle)
        {
            if (angle >= -22.5 && angle < 22.5) { Direction = 4; } // W
            else if (angle >= 22.5 && angle < 67.5) { Direction = 3; } // NW
            else if (angle >= 67.5 && angle < 112.5) { Direction = 2; } // N
            else if (angle >= 112.5 && angle < 157.5) { Direction = 1; } // NE
            else if (angle >= 157.5 || angle < -157.5) { Direction = 0; } // E
            else if (angle >= -157.5 && angle < -112.5) { Direction = 7; } // SE
            else if (angle >= 112.5 || angle < -67.5) { Direction = 6; } // S
            else if (angle >= 67.5 || angle < -22.5) { Direction = 5; } // SW
        }

        public void ToggleChecksum(bool Switch = true)
        {
            if (Switch)
            {
                RWMain.Write(0x154693, (byte)0xFF); // Skills

                RWMain.Write(0x1546D3, (byte)0xFF); // Attributes
                for (int i = 0; i < 10; i++)
                {
                    RWMain.Write(0x1546D3 + (9 * i), (byte)0xFF);
                }
                RWMain.Write(0x1546D3 + (9 * 1), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 2), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 3), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 4), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 5), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 6), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 7), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 8), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 9), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 10), (byte)0xFF);
            }
            else
            {
                RWMain.Write(0x154693, (byte)0xFF);

                RWMain.Write(0x1546D3, (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 1), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 2), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 3), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 4), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 5), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 6), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 7), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 8), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 9), (byte)0xFF);
                RWMain.Write(0x1546D3 + (9 * 10), (byte)0xFF);
            }
        }

        public string Help()
        {
            return HelpGenerator.GenerateHelp<Item>();
        }
    }

}
