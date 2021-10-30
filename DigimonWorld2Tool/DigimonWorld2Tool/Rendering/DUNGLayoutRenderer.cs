﻿using DigimonWorld2MapTool.Utility;
using DigimonWorld2Tool.FileFormats;
using DigimonWorld2Tool.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DigimonWorld2Tool.Rendering
{
    class DUNGLayoutRenderer
    {
        public static DUNGLayoutRenderer Instance { get; private set; }

        private Bitmap CurrentDrawnMapLayoutBitmap { get; set; }

        public int TileSizeWidth { get; private set; }
        public int TileSizeHeight { get; private set; }

        private PictureBox FloorLayoutPictureBox { get; }

        private DungFloorLayoutHeader floorLayoutToDraw;
        public DungFloorLayoutHeader FloorLayoutToDraw
        {
            get => floorLayoutToDraw;
            private set => floorLayoutToDraw = value;
        }

        public enum TileType : byte
        {
            Room,
            Corridor,
            Water,
            Fire,
            Nature,
            Machine,
            Dark,
            Override,
            Empty,
        }

        public static readonly Dictionary<TileType, Color> TileTypeColour = new Dictionary<TileType, Color>
        {
            {TileType.Empty, Color.Black},
            {TileType.Room, Color.Gray},
            {TileType.Corridor, Color.DarkGray},
            {TileType.Water, Color.DarkBlue},
            {TileType.Fire, Color.DarkRed},
            {TileType.Nature, Color.DarkGreen},
            {TileType.Machine, Color.FromArgb(255, 184, 165, 24)},
            {TileType.Dark, Color.DarkMagenta},
        };

        public DUNGLayoutRenderer(PictureBox floorLayoutPictureBox)
        {
            Instance = this;
            this.FloorLayoutPictureBox = floorLayoutPictureBox;
        }

        public void SetupFloorLayoutToDraw(DungFloorLayoutHeader floorLayoutToDraw)
        {
            this.FloorLayoutToDraw = floorLayoutToDraw;
        }

        public void SetupDungFloorBitmap()
        {
            CurrentDrawnMapLayoutBitmap = new Bitmap(FloorLayoutPictureBox.Width, FloorLayoutPictureBox.Height);
            TileSizeWidth = CurrentDrawnMapLayoutBitmap.Width / 64;
            TileSizeHeight = CurrentDrawnMapLayoutBitmap.Height / 48;

            DrawFloorLayoutToBitmap();
            DrawFloorWarpsToBitmap();
            DrawFloorChestsToBitmap();
            DrawFloorTrapsToBitmap();
            DrawGridLayout();
            DrawFloorDigimonToBitmap();

            ApplyFloorBitmapToPictureBox();
        }

        private void DrawFloorLayoutToBitmap()
        {
            for (int i = 0; i < FloorLayoutToDraw.FloorLayoutData.Length; i++)
            {
                var xId = i % 32;
                var yId = (int)Math.Floor(i / 32d);

                var leftNiblet = FloorLayoutToDraw.FloorLayoutData[i].GetLeftHalfByte();
                var rightNiblet = FloorLayoutToDraw.FloorLayoutData[i].GetRightHalfByte();

                var rightTileType = (TileType)leftNiblet;
                var leftTileType = (TileType)rightNiblet;

                for (int x = 0; x < TileSizeWidth; x++)
                {
                    for (int y = 0; y < TileSizeHeight; y++)
                    {
                        CurrentDrawnMapLayoutBitmap.SetPixel(xId * 2 * TileSizeWidth + x, yId * TileSizeHeight + y, TileTypeColour[leftTileType]);
                        CurrentDrawnMapLayoutBitmap.SetPixel((xId * 2 + 1) * TileSizeWidth + x, yId * TileSizeHeight + y, TileTypeColour[rightTileType]);
                    }
                }
            }
        }

        private void DrawFloorWarpsToBitmap()
        {
            foreach (var warp in FloorLayoutToDraw.FloorLayoutWarps)
            {
                for (int x = 0; x < TileSizeWidth; x++)
                {
                    for (int y = 0; y < TileSizeHeight; y++)
                    {
                        CurrentDrawnMapLayoutBitmap.SetPixel(warp.X * TileSizeWidth + x, warp.Y * TileSizeHeight + y, Color.Cyan);
                    }
                }
            }
        }

        private void DrawFloorChestsToBitmap()
        {
            foreach (var chest in FloorLayoutToDraw.FloorLayoutChests)
            {
                for (int x = 0; x < TileSizeWidth; x++)
                {
                    for (int y = 0; y < TileSizeHeight; y++)
                    {
                        CurrentDrawnMapLayoutBitmap.SetPixel(chest.X * TileSizeWidth + x, chest.Y * TileSizeHeight + y, Color.FromArgb(255, 0, 255, 0));
                    }
                }
            }
        }

        private void DrawFloorTrapsToBitmap()
        {
            foreach (var trap in FloorLayoutToDraw.FloorLayoutTraps)
            {
                for (int x = 0; x < TileSizeWidth; x++)
                {
                    for (int y = 0; y < TileSizeHeight; y++)
                    {
                        CurrentDrawnMapLayoutBitmap.SetPixel(trap.X * TileSizeWidth + x, trap.Y * TileSizeHeight + y, Color.Yellow);
                    }
                }
            }
        }

        private void DrawFloorDigimonToBitmap()
        {
            foreach (var digimon in FloorLayoutToDraw.FloorLayoutDigimons)
            {
                for (int x = 0; x < TileSizeWidth; x++)
                {
                    for (int y = 0; y < TileSizeHeight; y++)
                    {
                        CurrentDrawnMapLayoutBitmap.SetPixel(digimon.X * TileSizeWidth + x, digimon.Y * TileSizeHeight + y, Color.FromArgb(255, 255, 100, 100));
                    }
                }
            }
        }

        private void DrawGridLayout()
        {
            if (!DungWindow.Instance.DrawGridCheckBox.Checked)
                return;

            for (int x = 0; x < 32 * 2; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    for (int i = 0; i < TileSizeWidth; i++)
                    {
                        for (int j = 0; j < TileSizeHeight; j++)
                        {
                            if (i == 0 || j == 0)
                            {
                                    CurrentDrawnMapLayoutBitmap.SetPixel((x * TileSizeWidth) + i, (y * TileSizeHeight) + j, Color.LightGray);
                                    CurrentDrawnMapLayoutBitmap.SetPixel((x * TileSizeWidth) + i, (y * TileSizeHeight) + j, Color.LightGray);
                                    continue;
                            }
                        }
                    }
                }
            }
            ApplyFloorBitmapToPictureBox();
        }

        private void ApplyFloorBitmapToPictureBox()
        {
            FloorLayoutPictureBox.Image = CurrentDrawnMapLayoutBitmap;
        }


    }
}