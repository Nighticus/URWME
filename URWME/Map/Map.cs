using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class Map
    {
        ReadWriteMem RWMain;
        public TileArray Tiles;
        public Dictionary<int, object[]> TileData;

        public Map(ReadWriteMem RW)
        {
            RWMain = RW;
            Tiles = new TileArray(RWMain);
            TileData = DefaultData.DefaultMapTiles;
        }

        public bool IsLocal
        {
            get
            {
                byte Result = RWMain.Read<byte>(Address.Map_Type);
                if (Result == 2) { return true; }
                return false;
            }
        }

        public string RegionName
        {
            get { return RWMain.Read<string>(Address.Map_RegionName, 28); }
        }

        public Bitmap GetImageOverworld()
        {
            DirectBitmap Return = new DirectBitmap(3073, 2049);
            byte[] MapTiles = Tiles.Buffer;
            for (int i = 0; i < MapTiles.Length; i++)
            {
                int X = i % 3073; int Y = i / 3073;
                int TileID = MapTiles[i];

                if (TileData.ContainsKey(TileID))
                {
                    Return.SetPixel(X, Y, (Color)TileData[TileID][0]);
                }
                else { Return.SetPixel(X, Y, Color.Red); }
            }
            return Return.Bitmap;
        }

        public Bitmap GetImageOverworldPLM() // Fog of war
        {
            DirectBitmap Return = new DirectBitmap(3073, 2049);
            byte[] MapTiles = Tiles.FileBufferPLM;
            for (int i = 0; i < MapTiles.Length; i++)
            {
                int X = i % 3073; int Y = i / 3073;
                int TileID = MapTiles[i];

                if (TileData.ContainsKey(TileID))
                {
                    Return.SetPixel(X, Y, (Color)TileData[TileID][0]);
                }
                else { Return.SetPixel(X, Y, Color.Black); }
            }
            return Return.Bitmap;
        }

        public Bitmap GetImageOverworldDAT() // No Fog
        {
            DirectBitmap Return = new DirectBitmap(3073, 2049);
            byte[] MapTiles = Tiles.FileBufferDAT;
            for (int i = 0; i < MapTiles.Length; i++)
            {
                int X = i % 3073; int Y = i / 3073;
                int TileID = MapTiles[i];

                if (TileData.ContainsKey(TileID))
                {
                    Return.SetPixel(X, Y, (Color)TileData[TileID][0]);
                }
                else { Return.SetPixel(X, Y, Color.Black); }
            }
            return Return.Bitmap;
        }

        public void GetFullImageOverworld()
        {
            List<Bitmap> ReturnBitmaps = new List<Bitmap>();
            //Bitmap Return = new Bitmap(3073 * 18, 2049 * 20);
            Bitmap CurrentTileBmp;

            Bitmap GroundBmp = new Bitmap(Image.FromFile(TileData[250][1].ToString()));
            GroundBmp.MakeTransparent(GroundBmp.GetPixel(1, 1));
            GroundBmp = CropBMP(GroundBmp.Clone(new Rectangle(1, 1, GroundBmp.Height - 2, GroundBmp.Height - 2), System.Drawing.Imaging.PixelFormat.DontCare));
            //GroundBmp = new Bitmap(GroundBmp, new Size(18, 20));

            Dictionary<string, Bitmap> StoredCropData = new Dictionary<string, Bitmap>();

            byte[] MapTiles = Tiles.Buffer;
            for (int SectionX = 0; SectionX < 3; SectionX++)
            {
                for (int SectionY = 0; SectionY < 2; SectionY++)
                {
                    Bitmap CurrentSection = new Bitmap(1024 * 18, 1024 * 20);
                    Rectangle Area = new Rectangle(1024 * 18 * SectionX, 1024 * 20 * SectionY, 1024 * 18, 1024 * 20);
                    using (Graphics G = Graphics.FromImage(CurrentSection))
                    {
                        for (int z = 0; z < 2; z++)
                        {
                            int Index = (SectionX * 1024) + ((SectionY * 1024) * 3072);
                            for (int i = Index; i < MapTiles.Length; i++)
                            {
                                int X = i % 3073; int Y = i / 3073;
                                int TileID = MapTiles[i];
                                int CurrentX = X - (SectionX * 1024), CurrentY = Y - (SectionY * 1024);
                                Point Target = new Point((X * 18), (Y * 20));
                                Point SectionTarget = new Point(CurrentX * 18, CurrentY * 20);

                                if (Area.Contains(Target))
                                {
                                    switch (z)
                                    {
                                        case 0: // ground/water layer
                                            if (DefaultData.DefaultWaterTiles.ContainsKey(TileID))
                                            {
                                                if (!StoredCropData.ContainsKey(DefaultData.DefaultWaterTiles[TileID][1].ToString()))
                                                {
                                                    CurrentTileBmp = new Bitmap(Image.FromFile(DefaultData.DefaultWaterTiles[TileID][1].ToString()));
                                                    CurrentTileBmp.MakeTransparent(CurrentTileBmp.GetPixel(1, 1));
                                                    CurrentTileBmp = CropBMP(CurrentTileBmp.Clone(new Rectangle(1, 1, CurrentTileBmp.Height - 2, CurrentTileBmp.Height - 2), System.Drawing.Imaging.PixelFormat.DontCare));
                                                    //CurrentTileBmp = new Bitmap(CurrentTileBmp, new Size(18, 20));
                                                    StoredCropData.Add(DefaultData.DefaultWaterTiles[TileID][1].ToString(), CurrentTileBmp);

                                                    G.DrawImage(CurrentTileBmp, SectionTarget);
                                                }
                                                else
                                                {
                                                    G.DrawImage(StoredCropData[DefaultData.DefaultWaterTiles[TileID][1].ToString()], SectionTarget);
                                                }
                                            }
                                            else if (DefaultData.DefaultGroundTiles.ContainsKey(TileID))
                                            {
                                                if (!StoredCropData.ContainsKey(DefaultData.DefaultGroundTiles[TileID][1].ToString()))
                                                {
                                                    CurrentTileBmp = new Bitmap(Image.FromFile(DefaultData.DefaultGroundTiles[TileID][1].ToString()));
                                                    CurrentTileBmp.MakeTransparent(CurrentTileBmp.GetPixel(1, 1));
                                                    CurrentTileBmp = CropBMP(CurrentTileBmp.Clone(new Rectangle(1, 1, CurrentTileBmp.Height - 2, CurrentTileBmp.Height - 2), System.Drawing.Imaging.PixelFormat.DontCare));
                                                    //CurrentTileBmp = new Bitmap(CurrentTileBmp, new Size(18, 20));
                                                    StoredCropData.Add(DefaultData.DefaultGroundTiles[TileID][1].ToString(), CurrentTileBmp);

                                                    G.DrawImage(CurrentTileBmp, SectionTarget);
                                                }
                                                else
                                                {
                                                    G.DrawImage(StoredCropData[DefaultData.DefaultGroundTiles[TileID][1].ToString()], SectionTarget);
                                                }
                                            }
                                            else
                                            {
                                                G.DrawImage(GroundBmp, SectionTarget);
                                            }
                                            break;
                                        case 1: // overlay layer Left off here
                                            if (!DefaultData.DefaultWaterTiles.ContainsKey(TileID) && TileData.ContainsKey(TileID))
                                            {
                                                if (!StoredCropData.ContainsKey(TileData[TileID][1].ToString()))
                                                {
                                                    CurrentTileBmp = new Bitmap(Image.FromFile(TileData[TileID][1].ToString()));
                                                    CurrentTileBmp.MakeTransparent(CurrentTileBmp.GetPixel(1, 1));
                                                    CurrentTileBmp = CropBMP(CurrentTileBmp.Clone(new Rectangle(1, 1, CurrentTileBmp.Height - 2, CurrentTileBmp.Height - 2), System.Drawing.Imaging.PixelFormat.DontCare));
                                                    //CurrentTileBmp = new Bitmap(CurrentTileBmp, new Size(18, 20));
                                                    StoredCropData.Add(TileData[TileID][1].ToString(), CurrentTileBmp);

                                                    if (CurrentTileBmp.Width > 20)
                                                    {
                                                        SectionTarget.X -= (CurrentTileBmp.Width - 20) / 2;
                                                    }

                                                    if (CurrentTileBmp.Height > 18)
                                                    {
                                                        SectionTarget.Y -= (CurrentTileBmp.Height - 18) / 2;
                                                    }

                                                    G.DrawImage(CurrentTileBmp, SectionTarget);
                                                }
                                                else
                                                {
                                                    if (StoredCropData[TileData[TileID][1].ToString()].Width > 20)
                                                    {
                                                        SectionTarget.X -= (StoredCropData[TileData[TileID][1].ToString()].Width - 20) / 2;
                                                    }

                                                    if (StoredCropData[TileData[TileID][1].ToString()].Height > 18)
                                                    {
                                                        SectionTarget.Y -= (StoredCropData[TileData[TileID][1].ToString()].Height - 18) / 2;
                                                    }

                                                    G.DrawImage(StoredCropData[TileData[TileID][1].ToString()], SectionTarget);
                                                }
                                            }
                                            break;
                                        case 2: // tree layer
                                            break;
                                        case 3: // structure layer
                                            break;
                                        case 4: // tile layer
                                            break;

                                    }
                                }
                            }
                        }
                    }
                    CurrentSection.Save(string.Format("testMapgenB{1}{0}.png", SectionX, SectionY));
                    CurrentSection.Dispose();
                    //ReturnBitmaps;
                }
            }
        }

        public Bitmap CropBMP(Bitmap BmpOriginal)
        {
            Point min = new Point(int.MaxValue, int.MaxValue);
            Point max = new Point(int.MinValue, int.MinValue);

            for (int x = 0; x < BmpOriginal.Width; ++x)
            {
                for (int y = 0; y < BmpOriginal.Height; ++y)
                {
                    Color pixelColor = BmpOriginal.GetPixel(x, y);
                    if (pixelColor.A != 0)
                    {
                        //MessageBox.Show(pixelColor.ToString());
                        if (x < min.X) min.X = x;
                        if (y < min.Y) min.Y = y;

                        if (x > max.X) max.X = x;
                        if (y > max.Y) max.Y = y;
                    }
                }
            }

            // Create a new bitmap from the crop rectangle
            //MessageBox.Show(max.X + "");
            Rectangle cropRectangle = new Rectangle(min.X, min.Y, (max.X - min.X) + 1, (max.Y - min.Y) + 1);
            Bitmap newBitmap = new Bitmap(cropRectangle.Width, cropRectangle.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(BmpOriginal, 0, 0, cropRectangle, GraphicsUnit.Pixel);
            }
            return newBitmap;
        }

        public Bitmap GetImageLocalOld()
        {
            DirectBitmap Return = new DirectBitmap(3073, 2049);
            byte[] MapTiles = Tiles.Buffer;
            for (int i = 0; i < MapTiles.Length; i++)
            {
                int X = i % 3073; int Y = i / 3073;
                int TileID = MapTiles[i];
                if (X >= 72 && Y >= 72 && X <= 391 && Y <= 391)
                {
                    if (TileData.ContainsKey(TileID))
                    {
                        Return.SetPixel(X, Y, (Color)TileData[TileID][0]);
                    }
                    else { Return.SetPixel(X, Y, Color.Black); }
                }
            }
            return Return.Bitmap;
        }

        public Bitmap GetImageLocal()
        {
            int minX = 72;
            int minY = 72;
            int maxX = 391;
            int maxY = 391;

            int newWidth = (maxX - minX) + 1;
            int newHeight = (maxY - minY) + 1;

            DirectBitmap Return = new DirectBitmap(newWidth, newHeight);
            byte[] MapTiles = Tiles.Buffer;

            for (int y = minY; y <= maxY; y++)
            {
                int rowStartIndex = y * 3073;

                for (int x = minX; x <= maxX; x++)
                {
                    int index = rowStartIndex + x;
                    int tileID = MapTiles[index];

                    int newX = x - minX;
                    int newY = y - minY;

                    if (TileData.ContainsKey(tileID))
                    {
                        Return.SetPixel(newX, newY, (Color)TileData[tileID][0]);
                    }
                    else
                    {
                        Return.SetPixel(newX, newY, Color.Black);
                    }
                }
            }

            return Return.Bitmap;
        }


        public Bitmap GetImageLocalFog()
        {
            int minX = 72;
            int minY = 72;
            int maxX = 391;
            int maxY = 391;

            int newWidth = (maxX - minX) + 1;
            int newHeight = (maxY - minY) + 1;

            DirectBitmap Return = new DirectBitmap(newWidth, newHeight);
            byte[] MapTiles = Tiles.FogBuffer;

            for (int y = minY; y <= maxY; y++)
            {
                int rowStartIndex = y * 3073;

                for (int x = minX; x <= maxX; x++)
                {
                    int index = rowStartIndex + x;
                    int tileID = MapTiles[index];

                    int newX = x - minX;
                    int newY = y - minY;

                    if (TileData.ContainsKey(tileID))
                    {
                        MessageBox.Show(index.ToString());
                        Return.SetPixel(newX, newY, (Color)TileData[tileID][0]);
                    }
                    else
                    {
                        Return.SetPixel(newX, newY, Color.Black);
                    }
                }
            }

            return Return.Bitmap;
        }

        public class TileArray
        {
            ReadWriteMem RWMain;
            public TileArray(ReadWriteMem RW)
            {
                RWMain = RW;
            }

            public MapTile this[int Index]
            {
                get { return new MapTile(RWMain, Index); }
            }

            public byte[] Buffer
            {
                get { return RWMain.Read<byte[]>(Address.Map_Tiles, 6293502); }
                set { RWMain.Write(Address.Map_Tiles, value); }
            }
            
            public byte[] FogBuffer
            {
                get { return RWMain.Read<byte[]>(Address.Map_TilesFog2, 6293502); }
                set { RWMain.Write(Address.Map_TilesFog, value); }
            }

            public byte[] FileBufferDAT
            {
                get
                {
                    return File.ReadAllBytes(DefaultData.GameDirectory + "\\TEST6\\WORLD.DAT");
                }
            }

            public byte[] FileBufferPLM
            {
                get
                {
                    return File.ReadAllBytes(DefaultData.GameDirectory + "\\TEST6\\WORLD.PLM");
                }
            }

        }
    }

}
