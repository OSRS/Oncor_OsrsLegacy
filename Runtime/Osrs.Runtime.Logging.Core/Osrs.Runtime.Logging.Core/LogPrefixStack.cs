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

using System.Collections.Generic;

namespace Osrs.Runtime.Logging
{
    public class LogPrefixStack
    {
        private readonly Stack<string> stack = new Stack<string>();

        public void Clear()
        {
            this.stack.Clear();
            this.Value = string.Empty;
        }

        public void Push(string value)
        {
            if (value != null)
            {
                this.stack.Push(value);
                if (this.stack.Count>0)
                    this.Value = this.Value + ":" + value;
                else
                    this.Value = value; //first element
            }
            else
            {
                this.stack.Push(string.Empty);
                if (this.stack.Count > 0)
                    this.Value = this.Value + ":";
                //else -- empty value and empty stack, so we're ok to do nothing
            }
        }

        public void Pop()
        {
            if (this.stack.Count > 0)
            {
                string tmp = this.stack.Pop();
                if (this.stack.Count == 0) //last element
                    this.Value = string.Empty;
                else
                    this.Value = this.Value.Substring(0, this.Value.Length - (tmp.Length + 1)); //make sure to eat the leading ":"
            }
        }

        public string Value
        {
            get;
            private set;
        } = string.Empty;

        public string Format(string message)
        {
            return Format(this.Value, message);
        }

        public static string Format(string prefix, string message)
        {
            return string.Format("[{0}] {1}", prefix, message);
        }
    }
}
