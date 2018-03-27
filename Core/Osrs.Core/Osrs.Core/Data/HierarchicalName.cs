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
using System.Text;

namespace Osrs.Data
{
    public class HierarchicalName
    {
        private static readonly char[] splits = new char[] { '/' };
        protected readonly string[] nameParts;
        protected readonly string terminalName;

        public bool HasTerminalName
        {
            get { return !string.IsNullOrEmpty(this.terminalName); }
        }

        public virtual HierarchicalName BaseName
        {
            get
            {
                return new HierarchicalName(this.nameParts, null);
            }
        }

        public string[] BaseNameParts
        {
            get
            {
                string[] tmp = new string[this.nameParts.Length];
                Array.Copy(this.nameParts, tmp, this.nameParts.Length);
                return tmp;
            }
        }

        public string[] AllNameParts
        {
            get
            {
                string[] tmp = new string[this.nameParts.Length + 1];
                Array.Copy(this.nameParts, tmp, this.nameParts.Length);
                tmp[tmp.Length - 1] = this.terminalName;
                return tmp;
            }
        }

        public string TerminalName
        {
            get { return this.terminalName; }
        }

        protected HierarchicalName(string[] nameParts, string datasetName)
        {
            this.nameParts = nameParts;
            this.terminalName = datasetName;
        }

        protected HierarchicalName(string[] nameParts)
        {
            string[] subParts = new string[nameParts.Length - 1];
            Array.Copy(nameParts, subParts, subParts.Length);
            this.nameParts = subParts;
            this.terminalName = nameParts[nameParts.Length - 1];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in this.nameParts)
            {
                sb.Append(s);
                sb.Append('/');
            }
            if (this.HasTerminalName)
                sb.Append(this.terminalName);
            else
                sb.Length = sb.Length - 1;
            return sb.ToString();
        }

        public static HierarchicalName Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            char[] chars = name.ToCharArray();
            StringBuilder sb = new StringBuilder();
            List<string> parts = new List<string>();
            for (int i = 0; i < chars.Length; i++)
            {
                char curChar = chars[i];
                if (char.IsLetterOrDigit(curChar) || curChar == '.' || curChar == '_' || curChar == ' ')
                    sb.Append(curChar);
                else if (curChar == '/' || curChar == '\\')
                {
                    string tmp = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(tmp))
                        parts.Add(tmp);
                }
                else
                    return null; //bad character, we fail
            }
            if (sb.Length > 0)
            {
                string tmp = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parts.Add(tmp);
            }
            HierarchicalName dn = new HierarchicalName(new string[parts.Count - 1], parts[parts.Count - 1]);
            for (int i = 0; i < dn.nameParts.Length; i++)
                dn.nameParts[i] = parts[i];
            return dn;
        }

        public static HierarchicalName Create(IEnumerable<string> nameParts)
        {
            if (nameParts == null)
                return null;
            List<string> tmpL = new List<string>();
            foreach (string s in nameParts)
                tmpL.Add(s.Trim());
            if (tmpL.Count < 1)
                return null;
            for (int j = 0; j < tmpL.Count; j++)
            {
                string s = tmpL[j];
                if (string.IsNullOrEmpty(s))
                    return null;
                StringBuilder sb = new StringBuilder();
                char[] chars = s.Trim().ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    char curChar = chars[i];
                    if (char.IsLetterOrDigit(curChar) || curChar == '.' || curChar == '_' || curChar == ' ')
                        sb.Append(curChar);
                    else if (curChar == '/' || curChar == '\\')
                    {
                        string tmp = sb.ToString().Trim();
                        if (!string.IsNullOrEmpty(tmp))
                            break;
                    }
                    else
                        return null; //bad character, we fail
                }
                if (chars.Length != sb.Length)
                    return null;  //we prematurely ended
                tmpL[j] = sb.ToString();
            }
            HierarchicalName dn = new HierarchicalName(new string[tmpL.Count - 1], tmpL[tmpL.Count - 1]);
            for (int i = 0; i < dn.nameParts.Length; i++)
                dn.nameParts[i] = tmpL[i];
            return dn;
        }

        protected static string[] ToParts(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            char[] chars = name.ToCharArray();
            StringBuilder sb = new StringBuilder();
            List<string> parts = new List<string>();
            for (int i = 0; i < chars.Length; i++)
            {
                char curChar = chars[i];
                if (char.IsLetterOrDigit(curChar) || curChar == '.' || curChar == '_' || curChar == ' ')
                    sb.Append(curChar);
                else if (curChar == '/' || curChar == '\\')
                {
                    string tmp = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(tmp))
                        parts.Add(tmp);
                }
                else
                    return null; //bad character, we fail
            }
            if (sb.Length > 0)
            {
                string tmp = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parts.Add(tmp);
            }
            return parts.ToArray();
        }

        protected static string[] ToParts(IEnumerable<string> nameParts)
        {
            if (nameParts == null)
                return null;
            List<string> tmpL = new List<string>();
            foreach (string s in nameParts)
                tmpL.Add(s.Trim());
            if (tmpL.Count < 1)
                return null;
            for (int j = 0; j < tmpL.Count; j++)
            {
                string s = tmpL[j];
                if (string.IsNullOrEmpty(s))
                    return null;
                StringBuilder sb = new StringBuilder();
                char[] chars = s.Trim().ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    char curChar = chars[i];
                    if (char.IsLetterOrDigit(curChar) || curChar == '.' || curChar == '_' || curChar == ' ')
                        sb.Append(curChar);
                    else if (curChar == '/' || curChar == '\\')
                    {
                        string tmp = sb.ToString().Trim();
                        if (!string.IsNullOrEmpty(tmp))
                            break;
                    }
                    else
                        return null; //bad character, we fail
                }
                if (chars.Length != sb.Length)
                    return null;  //we prematurely ended
                tmpL[j] = sb.ToString();
            }
            return tmpL.ToArray();
        }

        public static HierarchicalName Create(HierarchicalName baseName, string datasetName)
        {
            if (baseName == null || string.IsNullOrEmpty(datasetName))
                return null;
            StringBuilder sb = new StringBuilder();
            char[] chars = datasetName.Trim().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char curChar = chars[i];
                if (char.IsLetterOrDigit(curChar) || curChar == '.' || curChar == '_' || curChar == ' ')
                    sb.Append(curChar);
                else if (curChar == '/' || curChar == '\\')
                {
                    string tmp = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(tmp))
                        break;
                }
                else
                    return null; //bad character, we fail
            }
            if (chars.Length != sb.Length)
                return null;  //we prematurely ended
            return new HierarchicalName(baseName.nameParts, sb.ToString());
        }
    }
}
