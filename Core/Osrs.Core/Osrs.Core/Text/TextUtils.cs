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
using Osrs.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Osrs.Text
{
    public enum StringOrdinalCompare
    {
        StartsWith,
        Contains,
        EndsWith
    }

    public static class TextUtils
    {
        private static readonly Regex n = new Regex(@"^-?\d+.?\d?$");
        private static readonly Regex i = new Regex(@"^-?\d+$");
        private static readonly Regex email = new Regex(@"^([a-zA-Z0-9_'+*$%\^&!\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9:]{2,4})+$");

        [DebuggerStepThrough]
        public static bool IsEmpty(this string s)
        {
            if (s == null)
                return false;
            return string.Empty.Equals(s);
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        [DebuggerStepThrough]
        public static bool IsNumeric(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return TextUtils.n.IsMatch(s);
        }

        [DebuggerStepThrough]
        public static bool IsInteger(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return TextUtils.i.IsMatch(s);
        }

        [DebuggerStepThrough]
        public static bool IsWhiteSpace(this string c)
        {
            if (string.IsNullOrEmpty(c))
                return false;
            char ch;
            int i = 0;
            for (; i < c.Length; i++)
            {
                ch = c[i];
                if (!char.IsWhiteSpace(ch))
                    return false;
            }
            return true;
        }

        [DebuggerStepThrough]
        public static bool IsByte(this string s)
        {
            try
            {
                byte.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsSByte(this string s)
        {
            try
            {
                sbyte.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsInt16(this string s)
        {
            try
            {
                short.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsInt32(this string s)
        {
            try
            {
                int.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsInt64(this string s)
        {
            try
            {
                long.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsUInt16(this string s)
        {
            try
            {
                ushort.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsUInt32(this string s)
        {
            try
            {
                uint.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsUInt64(this string s)
        {
            try
            {
                ulong.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsSingle(this string s)
        {
            try
            {
                float.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsDouble(this string s)
        {
            try
            {
                double.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsGuid(this string s)
        {
            try
            {
                Guid.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsDateTime(this string s)
        {
            try
            {
                DateTime.Parse(s);

                return true;
            }
            catch { }
            return false;
        }
        [DebuggerStepThrough]
        public static bool IsTimeSpan(this string s)
        {
            try
            {
                TimeSpan.Parse(s);

                return true;
            }
            catch { }
            return false;
        }

        [DebuggerStepThrough]
        public static byte AsByte(this string s, byte defaultValue)
        {
            byte res;
            if (byte.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static sbyte AsSByte(this string s, sbyte defaultValue)
        {
            sbyte res;
            if (sbyte.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static short AsInt16(this string s, short defaultValue)
        {
            short res;
            if (short.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static int AsInt32(this string s, int defaultValue)
        {
            int res;
            if (int.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static long AsInt64(this string s, long defaultValue)
        {
            long res;
            if (long.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static ushort AsUInt16(this string s, ushort defaultValue)
        {
            ushort res;
            if (ushort.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static uint AsUInt32(this string s, uint defaultValue)
        {
            uint res;
            if (uint.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static ulong AsUInt64(this string s, ulong defaultValue)
        {
            ulong res;
            if (ulong.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static float AsSingle(this string s, float defaultValue)
        {
            float res;
            if (float.TryParse(s, out res))
                return res;
            return defaultValue;
        }
        [DebuggerStepThrough]
        public static double AsDouble(this string s, double defaultValue)
        {
            double res;
            if (double.TryParse(s, out res))
                return res;
            return defaultValue;
        }

        [DebuggerStepThrough]
        public static bool IsValidEmailAddress(this string s)
        {
            return email.IsMatch(s);
        }

        [DebuggerStepThrough]
        public static T ToEnum<T>(this string value) where T : struct
        {
            return ToEnum<T>(value, true);
        }

        [DebuggerStepThrough]
        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                throw new StringEmptyOrNullException(value);
            Type t = typeof(T);
            if (!t.GetTypeInfo().IsEnum)
                throw new TypeMismatchException(t.ToString());
            return (T)Enum.Parse(t, value, ignoreCase);
        }

        [DebuggerStepThrough]
        public static byte[] HexToBytes(string hexInput)
        {
            if (string.IsNullOrEmpty(hexInput))
                return null;

            string buffer;
            if (hexInput.Length % 2 != 0)
                buffer = "0" + hexInput.ToUpperInvariant();
            else
                buffer = hexInput.ToUpperInvariant();
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (!(CharUtils.IsLegalHex(buffer[i]) && CharUtils.IsLegalHex(buffer[i + 1])))
                    return null;
                bytes.Add(HexToByte(buffer.Substring(i, 2)));
            }
            return bytes.ToArray();
        }

        [DebuggerStepThrough]
        public static byte[] BinaryToBytes(string binaryInput)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return null;
            string buffer;
            if (binaryInput.Length % 8 != 0)
            {
                StringBuilder sb = new StringBuilder();
                int left = 8 - (binaryInput.Length % 8);
                for (int i = 0; i < left; i++)
                    sb.Append('0');
                sb.Append(binaryInput);
                buffer = sb.ToString();
            }
            else
                buffer = binaryInput;
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < buffer.Length; i += 8)
            {
                if (!(CharUtils.IsLegalHex(buffer[i]) && CharUtils.IsLegalHex(buffer[i + 1]) && CharUtils.IsLegalHex(buffer[i + 2]) && CharUtils.IsLegalHex(buffer[i + 3]) &&
                    CharUtils.IsLegalHex(buffer[i + 4]) && CharUtils.IsLegalHex(buffer[i + 5]) && CharUtils.IsLegalHex(buffer[i + 6]) && CharUtils.IsLegalHex(buffer[i + 7])))
                    return null;
                bytes.Add(BinaryToByte(buffer.Substring(i, 8)));
            }
            return bytes.ToArray();
        }

        [DebuggerStepThrough]
        public static byte[] StringToBytes(string textInput)
        {
            if (string.IsNullOrEmpty(textInput))
                return null;
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < textInput.Length; i++)
                bytes.Add(CharUtils.CharToByte(textInput[i]));
            return bytes.ToArray();
        }

        [DebuggerStepThrough]
        public static string BytesToHex(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(CharUtils.ByteToHex(b));
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static string BytesToBinary(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(CharUtils.ByteToBinary(b));
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static string BytesToAscii(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append((char)b);
            return sb.ToString();
        }

        //one ascii character to a byte representation
        [DebuggerStepThrough]
        public static byte StringToByte(string input)
        {
            return CharUtils.CharToByte(input[0]);
        }

        //one ascii character (0..F) to byte representation
        [DebuggerStepThrough]
        public static byte HexToNibble(string input)
        {
            return CharUtils.HexToNibble(input[0]);
        }

        //two ascii characters (0..F) to byte representation
        [DebuggerStepThrough]
        public static byte HexToByte(string input)
        {
            byte c0 = CharUtils.HexToNibble(input[0]);
            c0 = (byte)(c0 << 4);
            byte c1 = CharUtils.HexToNibble(input[1]);
            return (byte)(c0 | c1);
        }

        //two ascii characters (0..1) to byte representation
        [DebuggerStepThrough]
        public static byte BinaryToPair(string input)
        {
            switch (input)
            {
                case "00":
                    return 0;
                case "01":
                    return 1;
                case "10":
                    return 2;
                case "11":
                    return 3;
            }
            return 0;
        }

        //four ascii characters (0..1) to byte representation
        [DebuggerStepThrough]
        public static byte BinaryToNibble(string input)
        {
            byte high = BinaryToPair(input.Substring(0, 2));
            high = (byte)(high << 2);
            byte low = BinaryToPair(input.Substring(2, 2));
            return (byte)(high | low);
        }

        //eight ascii characters (0..1) to byte representation
        [DebuggerStepThrough]
        public static byte BinaryToByte(string input)
        {
            byte high = BinaryToNibble(input.Substring(0, 4));
            high = (byte)(high << 4);
            byte low = BinaryToNibble(input.Substring(4, 4));
            return (byte)(high | low);
        }

        /// <summary>
        /// Gets if the specified string is ASCII string.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>Returns true if specified string is ASCII string, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        [DebuggerStepThrough]
        public static bool IsAscii(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            foreach (char c in value)
            {
                if ((int)c > 127)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Qoutes string and escapes chars.
        /// </summary>
        /// <param name="text">Text to quote.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string QuoteString(string text)
        {
            // String is already quoted-string.
            if (text != null && text.StartsWith("\"") && text.EndsWith("\""))
            {
                return text;
            }

            StringBuilder retVal = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\\')
                {
                    retVal.Append("\\\\");
                }
                else if (c == '\"')
                {
                    retVal.Append("\\\"");
                }
                else
                {
                    retVal.Append(c);
                }
            }

            return "\"" + retVal.ToString() + "\"";
        }

        /// <summary>
        /// Unquotes and unescapes escaped chars specified text. For example "xxx" will become to 'xxx', "escaped quote \"", will become to escaped 'quote "'.
        /// </summary>
        /// <param name="text">Text to unquote.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string UnQuoteString(string text)
        {
            int startPosInText = 0;
            int endPosInText = text.Length;

            //--- Trim. We can't use standard string.Trim(), it's slow. ----//
            for (int i = 0; i < endPosInText; i++)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    startPosInText++;
                }
                else
                {
                    break;
                }
            }
            for (int i = endPosInText - 1; i > 0; i--)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    endPosInText--;
                }
                else
                {
                    break;
                }
            }
            //--------------------------------------------------------------//

            // All text trimmed
            if ((endPosInText - startPosInText) <= 0)
            {
                return "";
            }

            // Remove starting and ending quotes.         
            if (text[startPosInText] == '\"')
            {
                startPosInText++;
            }
            if (text[endPosInText - 1] == '\"')
            {
                endPosInText--;
            }

            // Just '"'
            if (endPosInText == startPosInText - 1)
            {
                return "";
            }

            char[] chars = new char[endPosInText - startPosInText];

            int posInChars = 0;
            bool charIsEscaped = false;
            for (int i = startPosInText; i < endPosInText; i++)
            {
                char c = text[i];

                // Escaping char
                if (!charIsEscaped && c == '\\')
                {
                    charIsEscaped = true;
                }
                // Escaped char
                else if (charIsEscaped)
                {
                    // TODO: replace \n,\r,\t,\v ???
                    chars[posInChars] = c;
                    posInChars++;
                    charIsEscaped = false;
                }
                // Normal char
                else
                {
                    chars[posInChars] = c;
                    posInChars++;
                    charIsEscaped = false;
                }
            }

            return new string(chars, 0, posInChars);
        }

        /// <summary>
        /// Escapes specified chars in the specified string.
        /// </summary>
        /// <param name="text">Text to escape.</param>
        /// <param name="charsToEscape">Chars to escape.</param>
        [DebuggerStepThrough]
        public static string EscapeString(string text, char[] charsToEscape)
        {
            // Create worst scenario buffer, assume all chars must be escaped
            char[] buffer = new char[text.Length * 2];
            int nChars = 0;
            foreach (char c in text)
            {
                foreach (char escapeChar in charsToEscape)
                {
                    if (c == escapeChar)
                    {
                        buffer[nChars] = '\\';
                        nChars++;
                        break;
                    }
                }

                buffer[nChars] = c;
                nChars++;
            }

            return new string(buffer, 0, nChars);
        }

        /// <summary>
        /// Unescapes all escaped chars.
        /// </summary>
        /// <param name="text">Text to unescape.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string UnEscapeString(string text)
        {
            // Create worst scenarion buffer, non of the chars escaped.
            char[] buffer = new char[text.Length];
            int nChars = 0;
            bool escapedCahr = false;
            foreach (char c in text)
            {
                if (!escapedCahr && c == '\\')
                {
                    escapedCahr = true;
                }
                else
                {
                    buffer[nChars] = c;
                    nChars++;
                    escapedCahr = false;
                }
            }

            return new string(buffer, 0, nChars);
        }

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string[] SplitQuotedString(string text, char splitChar)
        {
            return SplitQuotedString(text, splitChar, false);
        }

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <param name="unquote">If true, splitted parst will be unqouted if they are qouted.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string[] SplitQuotedString(string text, char splitChar, bool unquote)
        {
            return SplitQuotedString(text, splitChar, unquote, int.MaxValue);
        }

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <param name="unquote">If true, splitted parst will be unqouted if they are qouted.</param>
        /// <param name="count">Maximum number of substrings to return.</param>
        /// <returns>Returns splitted string.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        [DebuggerStepThrough]
        public static string[] SplitQuotedString(string text, char splitChar, bool unquote, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            List<string> splitParts = new List<string>();  // Holds splitted parts
            int startPos = 0;
            bool inQuotedString = false;               // Holds flag if position is quoted string or not
            char lastChar = '0';

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // We have exceeded maximum allowed splitted parts.
                if ((splitParts.Count + 1) >= count)
                {
                    break;
                }

                // We have quoted string start/end.
                if (lastChar != '\\' && c == '\"')
                {
                    inQuotedString = !inQuotedString;
                }
                // We have escaped or normal char.
                //else{

                // We igonre split char in quoted-string.
                if (!inQuotedString)
                {
                    // We have split char, do split.
                    if (c == splitChar)
                    {
                        if (unquote)
                        {
                            splitParts.Add(UnQuoteString(text.Substring(startPos, i - startPos)));
                        }
                        else
                        {
                            splitParts.Add(text.Substring(startPos, i - startPos));
                        }

                        // Store new split part start position.
                        startPos = i + 1;
                    }
                }
                //else{

                lastChar = c;
            }

            // Add last split part to splitted parts list
            if (unquote)
            {
                splitParts.Add(UnQuoteString(text.Substring(startPos, text.Length - startPos)));
            }
            else
            {
                splitParts.Add(text.Substring(startPos, text.Length - startPos));
            }

            return splitParts.ToArray();
        }

        /// <summary>
        /// Gets first index of specified char. The specified char in quoted string is skipped.
        /// Returns -1 if specified char doesn't exist.
        /// </summary>
        /// <param name="text">Text in what to check.</param>
        /// <param name="indexChar">Char what index to get.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int QuotedIndexOf(string text, char indexChar)
        {
            int retVal = -1;
            bool inQuotedString = false; // Holds flag if position is quoted string or not			
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\"')
                {
                    // Start/end quoted string area
                    inQuotedString = !inQuotedString;
                }

                // Current char is what index we want and it isn't in quoted string, return it's index
                if (!inQuotedString && c == indexChar)
                {
                    return i;
                }
            }

            return retVal;
        }
    }
}
