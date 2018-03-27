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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Osrs.Text
{
    public static class ParseUtils
    {
        [DebuggerStepThrough]
        public static string[] ParseColon(this string parseString)
        {
            return parseString.Split(new char[] { ':' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblColon(this string parseString)
        {
            return parseString.Split(new string[] { "::" }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParseSemiColon(this string parseString)
        {
            return parseString.Split(new char[] { ';' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblSemiColon(this string parseString)
        {
            return parseString.Split(new string[] { ";;" }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParseComma(this string parseString)
        {
            return parseString.Split(new char[] { ',' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblComma(this string parseString)
        {
            return parseString.Split(new string[] { ",," }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParseDash(this string parseString)
        {
            return parseString.Split(new char[] { '-' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblDash(this string parseString)
        {
            return parseString.Split(new string[] { "--" }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParseDot(this string parseString)
        {
            return parseString.Split(new char[] { '.' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblDot(this string parseString)
        {
            return parseString.Split(new string[] { ".." }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParsePipe(this string parseString)
        {
            return parseString.Split(new char[] { '|' });
        }

        [DebuggerStepThrough]
        public static string[] ParseDblPipe(this string parseString)
        {
            return parseString.Split(new string[] { "||" }, StringSplitOptions.None);
        }

        [DebuggerStepThrough]
        public static string[] ParseQuote(this string parseString)
        {
            if (parseString == null || string.Empty.Equals(parseString))
            {
                return null;
            }
            char[] chars = parseString.ToCharArray();
            char qc = '"';
            bool bsCount = false;
            bool inQuote = false;
            List<string> items = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (char cur in chars)
            {
                if (inQuote)
                {
                    if (cur == '\'' || cur == '"')
                    {
                        if (cur == qc && !bsCount)
                        {
                            inQuote = false;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                else
                {
                    if (cur == '\'' || cur == '"')
                    {
                        if (!bsCount)
                        {
                            inQuote = true;
                            qc = cur;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                if (cur == '\\')
                {
                    bsCount = !bsCount;
                }
                else
                {
                    bsCount = false;
                }
            }
            if (inQuote)
            {
                return null;
            }
            items.Add(sb.ToString());
            return items.ToArray();
        }

        [DebuggerStepThrough]
        public static string[] ParseSingleQuote(this string parseString)
        {
            if (parseString == null || string.Empty.Equals(parseString))
            {
                return null;
            }
            char[] chars = parseString.ToCharArray();
            bool bsCount = false;
            bool inQuote = false;
            List<string> items = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (char cur in chars)
            {
                if (inQuote)
                {
                    if (cur == '\'')
                    {
                        if (!bsCount)
                        {
                            inQuote = false;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                else
                {
                    if (cur == '\'')
                    {
                        if (!bsCount)
                        {
                            inQuote = true;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                if (cur == '\\')
                {
                    bsCount = !bsCount;
                }
                else
                {
                    bsCount = false;
                }
            }
            if (inQuote)
            {
                return null;
            }
            items.Add(sb.ToString());
            return items.ToArray();
        }

        [DebuggerStepThrough]
        public static string[] ParseDoubleQuote(this string parseString)
        {
            if (parseString == null || string.Empty.Equals(parseString))
            {
                return null;
            }
            char[] chars = parseString.ToCharArray();
            bool bsCount = false;
            bool inQuote = false;
            List<string> items = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (char cur in chars)
            {
                if (inQuote)
                {
                    if (cur == '"')
                    {
                        if (!bsCount)
                        {
                            inQuote = false;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                else
                {
                    if (cur == '"')
                    {
                        if (!bsCount)
                        {
                            inQuote = true;
                            sb.Append(cur);
                            items.Add(sb.ToString());
                            sb = new StringBuilder();
                            continue;
                        }
                    }
                    sb.Append(cur);
                }
                if (cur == '\\')
                {
                    bsCount = !bsCount;
                }
                else
                {
                    bsCount = false;
                }
            }
            if (inQuote)
            {
                return null;
            }
            items.Add(sb.ToString());
            return items.ToArray();
        }

        [DebuggerStepThrough]
        public static string UnQuote(this string parseString)
        {
            if (parseString == null || string.Empty.Equals(parseString))
            {
                return parseString;
            }
            if (parseString[0].Equals('\'') && parseString[parseString.Length - 1].Equals('\''))
            {
                return parseString.Substring(1, parseString.Length - 2);
            }
            if (parseString[0].Equals('"') && parseString[parseString.Length - 1].Equals('"'))
            {
                return parseString.Substring(1, parseString.Length - 2);
            }
            return null;
        }
    }
}
