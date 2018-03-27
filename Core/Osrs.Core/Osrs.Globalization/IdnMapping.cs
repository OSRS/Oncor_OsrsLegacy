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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the IDN functions and implementation.
//
// This allows encoding of non-ASCII domain names in a "punycode" form,
// for example:
//
//     \u5B89\u5BA4\u5948\u7F8E\u6075-with-SUPER-MONKEYS
//
// is encoded as:
//
//     xn---with-SUPER-MONKEYS-pc58ag80a8qai00g7n9n
//
// Additional options are provided to allow unassigned IDN characters and
// to validate according to the Std3ASCII Rules (like DNS names).
//
// There are also rules regarding bidirectionality of text and the length
// of segments.
//
// For additional rules see also:
//  RFC 3490 - Internationalizing Domain Names in Applications (IDNA)
//  RFC 3491 - Nameprep: A Stringprep Profile for Internationalized Domain Names (IDN)
//  RFC 3492 - Punycode: A Bootstring encoding of Unicode for Internationalized Domain Names in Applications (IDNA)

//
// IdnMapping.cs
//
// Author:
//	Atsushi Enomoto  <atsushi@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Globalization;
using System.Text;

namespace Osrs.Globalization
{
    public sealed class IdnMapping
    {
        private const string acePrefix = "xn--";
        private static readonly Puny puny = new Puny();
        private bool allowUnassigned;
        private bool useStd3AsciiRules;

        public IdnMapping()
        {
        }

        public bool AllowUnassigned
        {
            get { return allowUnassigned; }
            set { allowUnassigned = value; }
        }

        public bool UseStd3AsciiRules
        {
            get { return useStd3AsciiRules; }
            set { useStd3AsciiRules = value; }
        }

        // Gets ASCII (Punycode) version of the string
        public string GetAscii(string unicode)
        {
            return GetAscii(unicode, 0);
        }

        public string GetAscii(string unicode, int index)
        {
            if (unicode == null)
                throw new ArgumentNullException(nameof(unicode));
            return GetAscii(unicode, index, unicode.Length - index);
        }

        public string GetAscii(string unicode, int index, int count)
        {
            if (unicode == null)
                throw new ArgumentNullException(nameof(unicode));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException((index < 0) ? nameof(index) : nameof(count), "ArgumentOutOfRange_NeedNonNegNum");
            if (index > unicode.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
            if (index > unicode.Length - count)
                throw new ArgumentOutOfRangeException(nameof(unicode), "ArgumentOutOfRange_IndexCountBuffer");

            if (count == 0)
            {
                throw new ArgumentException("Argument_IdnBadLabelSize", nameof(unicode));
            }
            if (unicode[index + count - 1] == 0)
            {
                throw new ArgumentException(string.Format("Argument_InvalidCharSequence: {0}", index + count - 1), nameof(unicode));
            }
            return GetAsciiCore(unicode, index, count);
        }

        // Gets Unicode version of the string.  Normalized and limited to IDNA characters.
        public string GetUnicode(string ascii)
        {
            return GetUnicode(ascii, 0);
        }

        public string GetUnicode(string ascii, int index)
        {
            if (ascii == null)
                throw new ArgumentNullException(nameof(ascii));
            return GetUnicode(ascii, index, ascii.Length - index);
        }

        public string GetUnicode(string ascii, int index, int count)
        {
            if (ascii == null)
                throw new ArgumentNullException(nameof(ascii));
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException((index < 0) ? nameof(index) : nameof(count), "ArgumentOutOfRange_NeedNonNegNum");
            if (index > ascii.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
            if (index > ascii.Length - count)
                throw new ArgumentOutOfRangeException(nameof(ascii), "ArgumentOutOfRange_IndexCountBuffer");

            // This is a case (i.e. explicitly null-terminated input) where behavior in .NET and Win32 intentionally differ.
            // The .NET APIs should (and did in v4.0 and earlier) throw an ArgumentException on input that includes a terminating null.
            // The Win32 APIs fail on an embedded null, but not on a terminating null.
            if (count > 0 && ascii[index + count - 1] == (char)0)
                throw new ArgumentException("Argument_IdnBadPunycode", nameof(ascii));

            return GetUnicodeCore(ascii, index, count);
        }

        public override bool Equals(object obj)
        {
            IdnMapping that = obj as IdnMapping;
            return
                that != null &&
                allowUnassigned == that.allowUnassigned &&
                useStd3AsciiRules == that.useStd3AsciiRules;
        }

        public override int GetHashCode()
        {
            return (allowUnassigned ? 100 : 200) + (useStd3AsciiRules ? 1000 : 2000);
        }

        private string GetAsciiCore(string url, int start, int count)
        {
            string section = url.Substring(start, count);
            string[] labels = section.Split('.', '\u3002', '\uFF0E', '\uFF61');

            int partX = 0;
            for(int i=0;i<labels.Length; partX += labels[i].Length, i++)
            {
                if (labels[i].Length == 0 && i + 1 == labels.Length)
                    continue; //ignore a trailing empty string added by split on a ..
                labels[i] = ToAscii(labels[i], partX);
            }

            return string.Join(".", labels);
        }

        private string ToAscii(string part, int start)
        {
            for(int i=0;i<part.Length;i++)
            {
                if (part[i] < '\x20' || part[i] == '\x7F')
                    throw new ArgumentException(string.Format("Illegal character found at {0}", start+i));
                if (part[i]>=0x80)
                {
                    part = NamePrep(part, start);
                    break;
                }
            }

            if (this.useStd3AsciiRules)
                VerifyStd3Ascii(part, start);

            for(int i=0;i<part.Length;i++)
            {
                if (part[i] >= 0x80)
                {
                    if (part.StartsWith(acePrefix, StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException(string.Format("Input string cannot start with ACE (xn--) at {0}", start + i));
                    part = puny.Encode(part, start);
                    part = acePrefix + part;
                    break;
                }
            }

            VerifyLength(part, start);
            return part;
        }

        private string GetUnicodeCore(string url, int start, int count)
        {
            string section = url.Substring(start, count);
            string[] labels = section.Split('.', '\u3002', '\uFF0E', '\uFF61');

            int partX = 0;
            for (int i = 0; i < labels.Length; partX += labels[i].Length, i++)
            {
                if (labels[i].Length == 0 && i + 1 == labels.Length)
                    continue; //ignore a trailing empty string added by split on a ..
                labels[i] = ToUnicode(labels[i], partX);
            }

            return string.Join(".", labels);
        }

        private string ToUnicode(string part, int start)
        {
            for (int i = 0; i < part.Length; i++)
            {
                if (part[i] >= 0x80)
                {
                    part = NamePrep(part, start);
                    break;
                }
            }

            if (!part.StartsWith(acePrefix, StringComparison.OrdinalIgnoreCase))
                return part;

            part = part.ToLowerInvariant();

            string body = part.Substring(4); //cut off aceprefix

            part = puny.Decode(part, start);

            string revBody = part;

            part = ToAscii(part, start);

            if (string.Compare(body, part, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ArgumentException(string.Format("Verification of ToUnicode() failed at part {0}", start));

            return revBody;
        }

        private string NamePrep(string part, int start)
        {
            part = part.Normalize(NormalizationForm.FormKC);

            for (int i = 0; i < part.Length; i++)
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(part, i))
                {
                    case UnicodeCategory.SpaceSeparator:
                        if (part[i] < '\x80')
                            continue; // valid
                        break;
                    case UnicodeCategory.Control:
                        if (part[i] != '\x0' && part[i] < '\x80')
                            continue; // valid
                        break;
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.Surrogate:
                        break;
                    default:
                        char c = part[i];
                        if ('\uFDDF' <= c && c <= '\uFDEF' ||
                            ((int)c & 0xFFFF) == 0xFFFE || '\uFFF9' <= c && c <= '\uFFFD' || '\u2FF0' <= c && c <= '\u2FFB' || '\u202A' <= c && c <= '\u202E' || '\u206A' <= c && c <= '\u206F')
                            break;
                        switch (c)
                        {
                            case '\u0340':
                            case '\u0341':
                            case '\u200E':
                            case '\u200F':
                            case '\u2028':
                            case '\u2029':
                                break;
                            default:
                                continue;
                        }
                        break;
                }
                throw new ArgumentException(string.Format("Not allowed character was in the input string, at {0}", start + i));
            }

            if (!this.allowUnassigned)
            {
                for(int i=0;i<part.Length;i++)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(part, i) == UnicodeCategory.OtherNotAssigned)
                        throw new ArgumentException(string.Format("Use of unassigned Unicode character is prohibited at {0}", start + i));
                }
            }

            return part;
        }

        private void VerifyStd3Ascii(string part, int start)
        {
            if (part.Length > 0 && part[0] == '-')
                throw new ArgumentException(String.Format("'-' is not allowed at head of a sequence in STD3 mode, found at {0}", start));
            if (part.Length > 0 && part[part.Length - 1] == '-')
                throw new ArgumentException(String.Format("'-' is not allowed at tail of a sequence in STD3 mode, found at {0}", start + part.Length - 1));

            for (int i = 0; i < part.Length; i++)
            {
                char c = part[i];
                if (c == '-')
                    continue;
                if (c <= '\x2F' || '\x3A' <= c && c <= '\x40' || '\x5B' <= c && c <= '\x60' || '\x7B' <= c && c <= '\x7F')
                    throw new ArgumentException(String.Format("Not allowed character in STD3 mode, found at {0}", start + i));
            }
        }

        private void VerifyLength(string part, int start)
        {
            if (part.Length == 0)
                throw new ArgumentException(String.Format("A label in the input string resulted in an invalid zero-length string, at {0}", start));
            if (part.Length > 63)
                throw new ArgumentException(String.Format("A label in the input string exceeded the length in ASCII representation, at {0}", start));
        }
    }

    internal sealed class Puny
    {
        private readonly char delimiter;
        private readonly int base_num;
        private readonly int tmin;
        private readonly int tmax;
        private readonly int skew;
        private readonly int damp;
        private readonly int initial_bias;
        private readonly int initial_n;

        internal Puny() : this('-', 36, 1, 26, 38, 700, 72, 0x80)
        { }

        internal Puny(char delimiter, int baseNum, int tmin, int tmax, int skew, int damp, int initialBias, int initialN)
        {
            this.delimiter = delimiter;
            base_num = baseNum;
            this.tmin = tmin;
            this.tmax = tmax;
            this.skew = skew;
            this.damp = damp;
            initial_bias = initialBias;
            initial_n = initialN;
        }

        internal string Encode(string s, int offset)
        {
            int n = initial_n;
            int delta = 0;
            int bias = initial_bias;
            int b = 0, h = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
                if (s[i] < '\x80')
                    sb.Append(s[i]);
            b = h = sb.Length;
            if (b > 0)
                sb.Append(delimiter);

            while (h < s.Length)
            {
                int m = int.MaxValue;
                for (int i = 0; i < s.Length; i++)
                    if (s[i] >= n && s[i] < m)
                        m = s[i];
                checked { delta += (m - n) * (h + 1); }
                n = m;
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if (c < n || c < '\x80')
                        checked { delta++; }
                    if (c == n)
                    {
                        int q = delta;
                        for (int k = base_num; ; k += base_num)
                        {
                            int t =
                                k <= bias + tmin ? tmin :
                                k >= bias + tmax ? tmax :
                                k - bias;
                            if (q < t)
                                break;
                            sb.Append(EncodeDigit(t + (q - t) % (base_num - t)));
                            q = (q - t) / (base_num - t);
                        }
                        sb.Append(EncodeDigit(q));
                        bias = Adapt(delta, h + 1, h == b);
                        delta = 0;
                        h++;
                    }
                }
                delta++;
                n++;
            }

            return sb.ToString();
        }

        // 41..5A (A-Z) = 0-25
        // 61..7A (a-z) = 0-25
        // 30..39 (0-9) = 26-35
        private char EncodeDigit(int d)
        {
            return (char)(d < 26 ? d + 'a' : d - 26 + '0');
        }

        private int DecodeDigit(char c)
        {
            return c - '0' < 10 ? c - 22 :
                c - 'A' < 26 ? c - 'A' :
                c - 'a' < 26 ? c - 'a' : base_num;
        }

        private int Adapt(int delta, int numPoints, bool firstTime)
        {
            if (firstTime)
                delta = delta / damp;
            else
                delta = delta / 2;
            delta = delta + (delta / numPoints);
            int k = 0;
            while (delta > ((base_num - tmin) * tmax) / 2)
            {
                delta = delta / (base_num - tmin);
                k += base_num;
            }
            return k + (((base_num - tmin + 1) * delta) / (delta + skew));
        }

        internal string Decode(string s, int offset)
        {
            int n = initial_n;
            int i = 0;
            int bias = initial_bias;
            int b = 0;
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < s.Length; j++)
            {
                if (s[j] == delimiter)
                    b = j;
            }
            if (b < 0)
                return s;
            sb.Append(s, 0, b);

            for (int z = b > 0 ? b + 1 : 0; z < s.Length;)
            {
                int old_i = i;
                int w = 1;
                for (int k = base_num; ; k += base_num)
                {
                    int digit = DecodeDigit(s[z++]);
                    i = i + digit * w;
                    int t = k <= bias + tmin ? tmin :
                        k >= bias + tmax ? tmax :
                        k - bias;
                    if (digit < t)
                        break;
                    w = w * (base_num - t);
                }
                bias = Adapt(i - old_i, sb.Length + 1, old_i == 0);
                n = n + i / (sb.Length + 1);
                i = i % (sb.Length + 1);
                if (n < '\x80')
                    throw new ArgumentException(String.Format("Invalid Bootstring decode result, at {0}", offset + z));
                sb.Insert(i, (char)n);
                i++;
            }

            return sb.ToString();
        }
    }
}
