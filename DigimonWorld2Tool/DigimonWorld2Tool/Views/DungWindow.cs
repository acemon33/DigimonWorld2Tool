﻿using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DigimonWorld2Tool.Utility;
using DigimonWorld2Tool.FileFormats;
using System.Diagnostics;
using System.Collections.Generic;
using DigimonWorld2MapTool.Utility;
using DigimonWorld2Tool.Rendering;

namespace DigimonWorld2Tool.Views
{
    public partial class DungWindow : UserControl
    {
        public static DungWindow Instance { get; private set; }

        private string dungFileDir;
        public string DungFileDir
        {
            get => dungFileDir;
            private set
            {
                dungFileDir = value;
                DungDirLabel.Text = dungFileDir;
            }
        }

        private string[] dungFileNames;
        public string[] DungFileNames
        {
            get => dungFileNames;
            private set
            {
                dungFileNames = value;
                SelectDungComboBox.DataSource = dungFileNames;
            }
        }

        private byte[] loadedDungFileRawData;
        public byte[] LoadedDungFileRawData
        {
            get => loadedDungFileRawData;
            private set => loadedDungFileRawData = value;
        }

        private DUNG loadedDungFile;
        public DUNG LoadedDungFile
        {
            get => loadedDungFile;
            private set => loadedDungFile = value;
        }

        private DungFloorHeader loadedDungFloorHeader;
        public DungFloorHeader LoadedDungFloorHeader
        {
            get => loadedDungFloorHeader;
            private set => loadedDungFloorHeader = value;
        }

        private Button[] FloorLayoutButtons { get; set; } = new Button[8];
        private Button LastPressedFloorLayoutButton { get; set; }

        private Dictionary<int, int> DistinctFloorLayoutPointersOccurance { get; set; } = new Dictionary<int, int>();
        

        public DungWindow()
        {
            Instance = this;
            InitializeComponent();
            this.BackColor = (Color)Settings.Settings.BackgroundColour;
            this.ForeColor = (Color)Settings.Settings.TextColour;
            Colours.SetColourScheme(this.Controls);

            FloorLayoutButtons[0] = FloorLayoutButton1;
            FloorLayoutButtons[1] = FloorLayoutButton2;
            FloorLayoutButtons[2] = FloorLayoutButton3;
            FloorLayoutButtons[3] = FloorLayoutButton4;
            FloorLayoutButtons[4] = FloorLayoutButton5;
            FloorLayoutButtons[5] = FloorLayoutButton6;
            FloorLayoutButtons[6] = FloorLayoutButton7;
            FloorLayoutButtons[7] = FloorLayoutButton8;

            new DUNGLayoutRenderer(FloorLayoutPictureBox);

            LoadUserSettings();

            PostInit();
        }

        private void PostInit()
        {
            if (DungFileDir != "")
            {
                DungFileNames = GetFileNamesInDungDirectory();
                PopulateDomainNamesComboBox(SelectDungComboBox);
            }
        }

        private void LoadUserSettings()
        {
            DungFileDir = (string)Properties.Settings.Default["DefaultMapDataDirectory"];
        }

        /// <summary>
        /// Select the directory in which the dung files are stored
        /// </summary>
        private void SelectDungDirButton_Click(object sender, EventArgs e)
        {
            DungFileDir = GetDungFilesDirectory();
            DungFileNames = GetFileNamesInDungDirectory();
        }

        private string GetDungFilesDirectory()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowNewFolderButton = false;
                folderDialog.RootFolder = Environment.SpecialFolder.Personal;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default["DefaultMapDataDirectory"] = folderDialog.SelectedPath;
                    return folderDialog.SelectedPath;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get all the filenames of the files contained in <see cref="DungFileDir"/>
        /// </summary>
        private string[] GetFileNamesInDungDirectory()
        {
            if (DungFileDir == null || !Directory.Exists(DungFileDir))
                return null;

            string[] filePaths = Directory.GetFiles(DungFileDir);
            string[] fileNames = new string[filePaths.Length];

            for (int i = 0; i < fileNames.Length; i++)
                fileNames[i] = filePaths[i].Replace($"{DungFileDir}\\", "");

            string[] mappedFileNames = new string[fileNames.Length];
            for (int i = 0; i < mappedFileNames.Length; i++)
            {
                var a = Settings.Settings.DungMapping;
                var result = a.FirstOrDefault(o => o.Filename == fileNames[i]);
                if (result == null)
                    mappedFileNames[i] = fileNames[i];
                else
                    mappedFileNames[i] = result.DomainName;
            }

            return mappedFileNames;
        }

        /// <summary>
        /// Select the clicked entry in the combo box as file to load
        /// </summary>
        private void SelectDungComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDomainNamesComboBox((ComboBox)sender);
        }

        private void PopulateDomainNamesComboBox(ComboBox comboBox)
        {
            string selectedDomainName = (string)comboBox.SelectedItem;
            string selectedFileName = Settings.Settings.DungMapping.FirstOrDefault(o => o.DomainName == selectedDomainName)?.Filename;

            LoadDungFileRawData(selectedFileName);
        }

        /// <summary>
        /// Check if the filename exists in the directory <see cref="DungFileDir"/> and load its bytes
        /// </summary>
        /// <param name="filename">The name of the file to load</param>
        private void LoadDungFileRawData(string filename)
        {
            string filePath = $"{DungFileDir}\\{filename}";
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            LoadedDungFileRawData = File.ReadAllBytes(filePath);
            SerializeDungFile(LoadedDungFileRawData);
            PopulateSelectDungFloorComboBox();
        }

        private void SerializeDungFile(byte[] data)
        {
            LoadedDungFile = new DUNG(data);
        }

        private void PopulateSelectDungFloorComboBox()
        {
            SelectDungFloorComboBox.DataSource = LoadedDungFile.DungFloorHeaders.Select(floorHeader => floorHeader.FloorName).ToList();
        }

        private void SelectDungFloorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cBox = (ComboBox)sender;
            LoadSelectedFloorData((string)cBox.SelectedItem);

            FloorLayoutButton_Click(FloorLayoutButton1, null);
        }

        private void LoadSelectedFloorData(string floorName)
        {
            DistinctFloorLayoutPointersOccurance = new Dictionary<int, int>();
            LoadedDungFloorHeader = loadedDungFile.DungFloorHeaders.FirstOrDefault(o => o.FloorName == floorName);
            if (LoadedDungFloorHeader == null)
                return;

            foreach (var item in LoadedDungFloorHeader.DungFloorLayoutHeaders)
            {
                if (!DistinctFloorLayoutPointersOccurance.ContainsKey(item.FloorLayoutPointer))
                    DistinctFloorLayoutPointersOccurance.Add(item.FloorLayoutPointer, 1);
                else
                    DistinctFloorLayoutPointersOccurance[item.FloorLayoutPointer]++;
            }
            DisableUnusedFloorLayoutButtons(DistinctFloorLayoutPointersOccurance.Count);
        }

        private void DisableUnusedFloorLayoutButtons(int uniqueLayoutCount)
        {
            for (int i = 0; i < FloorLayoutButtons.Length; i++)
                FloorLayoutButtons[i].Visible = i < uniqueLayoutCount;
        }

        private void FloorLayoutButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (LastPressedFloorLayoutButton != null)
                LastPressedFloorLayoutButton.BackColor = (Color)Settings.Settings.ButtonBackgroundColour;

            button.BackColor = (Color)Settings.Settings.ButtonSelectedBackgroundColour;
            LastPressedFloorLayoutButton = button;

            var buttonId = FloorLayoutButtons.ToList().IndexOf(button);
            int distinctHeaderPointer = DistinctFloorLayoutPointersOccurance.ElementAt(buttonId).Key;
            var selectedFloorLayout = LoadedDungFloorHeader.DungFloorLayoutHeaders.FirstOrDefault(o => o.FloorLayoutPointer == distinctHeaderPointer);
            if (selectedFloorLayout == null)
                throw new NullReferenceException();

            DUNGLayoutRenderer.Instance.SetupFloorLayoutToDraw(selectedFloorLayout);
            DUNGLayoutRenderer.Instance.SetupDungFloorBitmap();
        }

        private void DrawGridCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FloorLayoutButton_Click(LastPressedFloorLayoutButton, null);
        }

        private void DisplayMousePositionOnGrid(object sender, MouseEventArgs e)
        {
            MousePositionLabel.Text = $"X: {(int)e.Location.X / DUNGLayoutRenderer.Instance.TileSizeWidth:D2} Y: {(int)e.Location.Y / DUNGLayoutRenderer.Instance.TileSizeHeight:D2}";
        }
    }
}