//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Diagnostics;
using System.Text;

namespace Osrs.Text
{
    public static class CharUtils
    {
        [DebuggerStepThrough]
        public static bool IsNewLine(this char c)
        {
            return c == 0x000d || c == 0x000a || c == 0x2028 || c == 0x2029;
        }

        [DebuggerStepThrough]
        public static bool IsWhiteSpaceNotNewLine(this char c)
        {
            return char.IsWhiteSpace(c) && !(c == 0x000d || c == 0x000a || c == 0x2028 || c == 0x2029);
        }

        [DebuggerStepThrough]
        public static bool IsWhiteSpace(this char c)
        {
            return char.IsWhiteSpace(c);
        }

        [DebuggerStepThrough]
        public static bool IsEnglishVowel(this char c)
        {
            c = char.ToLowerInvariant(c);
            switch (c)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return true;
                default:
                    return false;
            }
        }

        [DebuggerStepThrough]
        public static bool IsEnglishConsonant(this char c)
        {
            c = char.ToLowerInvariant(c);
            switch (c)
            {
                case 't':
                case 'n':
                case 's':
                case 'h':
                case 'r':
                case 'd':
                case 'l':
                case 'c':
                case 'm':
                case 'w':
                case 'f':
                case 'b':
                case 'g':
                case 'y':
                case 'p':
                case 'j':
                case 'k':
                case 'q':
                case 'v':
                case 'x':
                case 'z':
                    return true;
                default:
                    return false;
            }
        }

        [DebuggerStepThrough]
        public static bool IsLegalHex(char c)
        {
            c = char.ToUpperInvariant(c);
            switch (c)
            {
                case '0':
                    return true;
                case '1':
                    return true;
                case '2':
                    return true;
                case '3':
                    return true;
                case '4':
                    return true;
                case '5':
                    return true;
                case '6':
                    return true;
                case '7':
                    return true;
                case '8':
                    return true;
                case '9':
                    return true;
                case 'A':
                    return true;
                case 'B':
                    return true;
                case 'C':
                    return true;
                case 'D':
                    return true;
                case 'E':
                    return true;
                case 'F':
                    return true;
            }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsLegalBinary(char c)
        {
            switch (c)
            {
                case '0':
                    return true;
                case '1':
                    return true;
            }
            return false;
        }
        [DebuggerStepThrough]
        public static byte CharToByte(char input)
        {
            int c = (int)input;
            if (c > byte.MaxValue)
            {
                return byte.MaxValue;
            }
            if (c < byte.MinValue)
            {
                return byte.MinValue;
            }
            return (byte)c;
        }
        [DebuggerStepThrough]
        public static byte HexToNibble(char input)
        {
            input = char.ToUpperInvariant(input);
            switch (input)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
            }
            return 0;
        }

        //single character representation of a byte
        [DebuggerStepThrough]
        public static char ByteToChar(byte input)
        {
            return (char)input;
        }

        //single character string representation of a byte
        [DebuggerStepThrough]
        public static string ByteToString(byte input)
        {
            return new string(new char[1] { (char)input });
        }

        //two character string representation of a byte
        [DebuggerStepThrough]
        public static string ByteToHex(byte input)
        {
            byte high = (byte)(input >> 4);
            byte low = (byte)(input & 15);
            char c0 = NibbleToHex(low);
            char c1 = NibbleToHex(high);
            StringBuilder sb = new StringBuilder();
            sb.Append(c1);
            sb.Append(c0);
            return sb.ToString();
        }

        //one character string representation of a byte
        [DebuggerStepThrough]
        public static char NibbleToHex(byte input)
        {
            switch (input)
            {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'C';
                case 13:
                    return 'D';
                case 14:
                    return 'E';
                case 15:
                    return 'F';
            }
            return '0';
        }

        [DebuggerStepThrough]
        public static string ByteToBinary(byte input)
        {
            StringBuilder sb = new StringBuilder();

            if ((byte)(input & 128) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 64) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 32) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 16) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 8) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 4) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 2) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            if ((byte)(input & 1) != 0)
                sb.Append("1");
            else
                sb.Append("0");

            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static char BitToChar(byte c)
        {
            if (c.Equals(0))
                return '0';
            return '1';
        }
    }
}
