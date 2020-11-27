﻿using System;
using System.Collections.Generic;
using System.IO;
using static DigimonWorld2MapVisualizer.BinReader;

namespace DigimonWorld2MapVisualizer
{
    public class Domain
    {
        private static readonly string FilePathToMapDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}Maps\\";

        public static byte[] DomainData { get; private set; }
        public readonly string DomainName;

        public List<DomainFloor> floorsInThisDomain = new List<DomainFloor>();

        public Domain(string domainFilename)
        {
            DomainData = ReadDomainMapDataFile(domainFilename);
            if (DomainData == null)
            {
                Program.DungeonFileSelector();
                return;
            }

            bool searchingDomainFloors = true;
            do
            {
                var floorHeaderBasePointerDecimalAddress = GetPointer(floorsInThisDomain.Count * 4);

                if (floorHeaderBasePointerDecimalAddress == 0)
                {
                    searchingDomainFloors = false;
                    continue;
                }

                floorsInThisDomain.Add(new DomainFloor(floorHeaderBasePointerDecimalAddress));
            }
            while (searchingDomainFloors);

            Program.watch.Stop();
            Console.WriteLine($"\nVisualization took {Program.watch.ElapsedMilliseconds}ms\n");
            Program.FinishUpVisualization();
        }

        /// <summary>
        /// Read the entire contents of a map file
        /// </summary>
        /// <param name="domainFilename">The name of the dungeon file to load</param>
        /// <returns>String array containing all the hex values in the binary</returns>
        /// <remarks>Technically we could use reader.ReadBytes(int.MaxValue), however this may cause an OutOfMemoryException on 32-bit systems.</remarks>
        private byte[] ReadDomainMapDataFile(string domainFilename)
        {
            if (File.Exists(FilePathToMapDirectory + domainFilename))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(FilePathToMapDirectory + domainFilename, FileMode.Open)))
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    reader.BaseStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error; File {domainFilename} was not found in directory:" +
                                  $"\n{FilePathToMapDirectory}" +
                                  $"\nPlease check if the \\Maps\\ folder exists and contains the DUNGxxxx.BIN file(s).");
                Console.ForegroundColor = ConsoleColor.Gray;
                return null;
            }
        }
    }
}
