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
namespace Osrs.Text
{
    /// <summary>
    /// Writer interface for outputting text for logging or console
    /// </summary>
    public interface ITextWriter
    {
        void Write(string s);
        void WriteLine(string s);
        void WriteLine();
    }

    /// <summary>
    /// Writer interface for inputting text for console
    /// </summary>
    public interface ITextReader
    {
        bool HasMore
        {
            get;
        }

        char Read();
        string ReadWord();
        string ReadLine();
    }
}
