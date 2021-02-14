﻿using System;
using System.Collections.Generic;

namespace DigimonWorld2MapTool.Utility
{
    public class TextConversion
    {
        private static readonly Dictionary<byte, string> CharacterLookupTable = new Dictionary<byte, string>()
        {
            {0x00, "0"},
            {0x01, "1"},
            {0x02, "2"},
            {0x03, "3"},
            {0x04, "4"},
            {0x05, "5"},
            {0x06, "6"},
            {0x07, "7"},
            {0x08, "8"},
            {0x09, "9"},
            {0x0A, "A"},
            {0x0B, "B"},
            {0x0C, "C"},
            {0x0D, "D"},
            {0x0E, "E"},
            {0x0F, "F"},
            {0x10, "G"},
            {0x11, "H"},
            {0x12, "I"},
            {0x13, "J"},
            {0x14, "K"},
            {0x15, "L"},
            {0x16, "M"},
            {0x17, "N"},
            {0x18, "O"},
            {0x19, "P"},
            {0x1A, "Q"},
            {0x1B, "R"},
            {0x1C, "S"},
            {0x1D, "T"},
            {0x1E, "U"},
            {0x1F, "V"},
            {0x20, "W"},
            {0x21, "X"},
            {0x22, "Y"},
            {0x23, "Z"},
            {0x24, "a"},
            {0x25, "b"},
            {0x26, "c"},
            {0x27, "d"},
            {0x28, "e"},
            {0x29, "f"},
            {0x2A, "g"},
            {0x2B, "h"},
            {0x2C, "i"},
            {0x2D, "j"},
            {0x2E, "k"},
            {0x2F, "l"},
            {0x30, "m"},
            {0x31, "n"},
            {0x32, "o"},
            {0x33, "p"},
            {0x34, "q"},
            {0x35, "r"},
            {0x36, "s"},
            {0x37, "t"},
            {0x38, "u"},
            {0x39, "v"},
            {0x3A, "w"},
            {0x3B, "x"},
            {0x3C, "y"},
            {0x3D, "z"},
            {0x41, "<SQUARE>"},
            {0x44, "?"},
            {0x45, "!"},
            {0x46, "/"},
            {0x49, "-"},
            {0x54, ","},
            {0x55, "."},
            {0x56, ""},
            {0x5B, "PLUS SIGN"},
            {0xFB, "<Input X>"},
            {0xFC, "<NEW BOX>"},
            {0xFD, " "},
            {0xFE, "<ENTER>"},
        };

        /// <summary>
        /// Note: All value in this list are prefixed by 0xF0
        /// </summary>
        private static readonly Dictionary<byte, string> SpecialLookupTable = new Dictionary<byte, string>()
        {
            {0x00, "<Akira>"},
            {0x06, "<Digimon>"},
            {0x07, "<you>"},
            {0x08, "<the>"},
            {0x09, "<Digi-Beetle>"},
            {0x0A, "<Domain>"},
            {0x0B, "<Guard>"},
            {0x0C, "<Tamer>"},
            {0x0D, "<here>"},
            {0x0E, "<have>"},
            {0x0F, "<Knights>"},
            {0x10, "<and>"},
            {0x11, "<thing>"},
            {0x12, "<Security>"},
            {0x13, "<that>"},
            {0x14, "<Bertran>"},
            {0x15, "<Tournament>"},
            {0x16, "<Crimson>"},
            {0x18, "<something>"},
            {0x19, "<Item>"},
            {0x1A, "<Falcon>"},
            {0x1B, "<for>"},
            {0x1C, "<That's>"},
            {0x1D, "<Commander>"},
            {0x1E, "<Blood>"},
            {0x1F, "<Leader>"},
            {0x20, "<Attendant>"},
            {0x21, "<Cecilia>"},
            {0x22, "<all>"},
            {0x23, "<mission>"},
            {0x24, "<this>"},
            {0x26, "<Archive>"},
            {0x27, "<Black>"},
            {0x28, "<I'll>"},
            {0x29, "<are>"},
            {0x2A, "<Sword>"},
            {0x2B, "<right>"},
            {0x2C, "<Digivolve>"},
            {0x2D, "<enter>"},
            {0x2E, "<What>"},
            {0x2F, "<will>"},
            {0x30, "<come>"},
            {0x31, "<You>"},
            {0x32, "<Coliseum>"},
            {0x33, "<about>"},
            {0x34, "<don't>"},
            {0x35, "<anything>"},
            {0x37, "<Parts>"},
            {0x38, "<where>"},
            {0x39, "<The>"},
            {0x3A, "<know>"},
            {0x3B, "<Leomon>"},
            {0x3C, "<want>"},
            {0x3D, "<Oldman>"},
            {0x3E, "<like>"},
            {0x3F, "<need>"},
            {0x40, "<Chief>"},
            {0x41, "<with>"},
            {0x42, "<Thank>"},
            {0x43, "<strange>" },
            {0x44, "<Island>"},
            {0x45, "<can>"},
            {0x46, "<really>"},
            {0x47, "<Blue>"},
            {0x48, "<time>"},
        };

        /// <summary>
        /// These colour changes are always prefixed by 0xF4
        /// </summary>
        private static readonly Dictionary<byte, string> SpeakerLookupTable = new Dictionary<byte, string>()
        {
            {0x34, "<Text_Yellow>" },
            {0x30, "<Text_White>" },
        };

        /// <summary>
        /// These portraits are always prefixed by 0xF9
        /// </summary>
        private static readonly Dictionary<byte, string> PortraitLookupTable = new Dictionary<byte, string>()
        {
            {0x00, "[Portrait right]" },
            {0x01, "[Portrait left]" }, //This is a guess
        };

        /// <summary>
        /// Convert the hex value of DW2 text to the ASCII representation using the text map found here https://docs.google.com/spreadsheets/d/1UiDU4MsSfxO1vhpK6err1KsLRZM53JUOuYqYhfEFp8o/edit#gid=1279970913
        /// </summary>
        /// <param name="input">The bytes that need to be converted</param>
        /// <returns>Input bytes converted to ASCII text</returns>
        public static string DigiBytesToString(byte[] input)
        {
            string converted = "";
            bool skipByte = false;

            converted += $"[{input[0]:X2} {input[1]:X2} {input[2]:X2} {input[3]:X2}]\n";
            converted += $"[{input[4]:X2} {input[5]:X2} {input[6]:X2} {input[7]:X2}]\n";
            converted += $"[{input[8]:X2} {input[9]:X2} {input[10]:X2} {input[11]:X2}]\n";

            for (int i = 12; i < input.Length; i++)
            {
                if (skipByte)
                {
                    skipByte = false;
                    continue;
                }

                if (input[i] == 0xF0)
                {
                    if (SpecialLookupTable.ContainsKey(input[i + 1]))
                        converted += $"{SpecialLookupTable[input[i + 1]]}";
                    else
                        converted += "[F0 Unknown]";

                    skipByte = true;
                    continue;
                }

                if (input[i] == 0xF4)
                {
                    if (SpeakerLookupTable.ContainsKey(input[i + 1]))
                        converted += $"{SpeakerLookupTable[input[i + 1]]}";
                    else
                        converted += "[F9 Unknown]";

                    skipByte = true;
                    continue;
                }

                if (input[i] == 0xF9)
                {
                    if (PortraitLookupTable.ContainsKey(input[i + 1]))
                        converted += $"{PortraitLookupTable[input[i + 1]]}";
                    else
                        converted += "[F4 Unknown]";

                    skipByte = true;
                    continue;
                }

                if (CharacterLookupTable.ContainsKey(input[i]))
                    converted += $"{CharacterLookupTable[input[i]]}";
                else
                    converted += $"[{input[i]:X2}]";

                if(input[i] == 0xFB)
                    converted += "\n";
            }

            return converted;
        }

        public static string ByteArrayToHexString(byte[] data, char seperator = ' ')
        {
            string result = "";
            foreach (var item in data)
            {
                result += $"{item:X2}{seperator}";
            }
            return result;
        }
    }
}
