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
using Osrs.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Osrs.IO
{
    public sealed class StreamWriterTextWriter : ITextWriter
    {
        private readonly StreamWriter writer;

        [DebuggerStepThrough]
        public StreamWriterTextWriter(Stream writer)
        {
            if (writer == null)
                throw new ArgumentNullException();
            if (!writer.CanWrite)
                throw new ArgumentException();
            this.writer = new StreamWriter(writer);
        }

        [DebuggerStepThrough]
        public StreamWriterTextWriter(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException();
            this.writer = writer;
        }

        [DebuggerStepThrough]
        public void Write(string s)
        {
            this.writer.Write(s);
        }

        [DebuggerStepThrough]
        public void WriteLine(string s)
        {
            this.writer.WriteLine(s);
        }

        [DebuggerStepThrough]
        public void WriteLine()
        {
            this.writer.WriteLine();
        }

        [DebuggerStepThrough]
        public static implicit operator StreamWriterTextWriter(StreamWriter writer)
        {
            if (writer != null)
                return new StreamWriterTextWriter(writer);
            return null;
        }

        [DebuggerStepThrough]
        public static implicit operator StreamWriterTextWriter(Stream writer)
        {
            if (writer != null)
                return new StreamWriterTextWriter(writer);
            return null;
        }
    }

    public sealed class StreamReaderTextReader : ITextReader
    {
        private readonly StreamReader reader;

        [DebuggerStepThrough]
        public StreamReaderTextReader(StreamReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();
            this.reader = reader;
        }

        [DebuggerStepThrough]
        public StreamReaderTextReader(Stream reader)
        {
            if (reader == null)
                throw new ArgumentNullException();
            if (!reader.CanRead)
                throw new ArgumentException();

            this.reader = new StreamReader(reader);
        }

        [DebuggerStepThrough]
        public char Read()
        {
            return (char)this.reader.Read();
        }

        [DebuggerStepThrough]
        public string ReadWord()
        {
            StringBuilder sb = new StringBuilder();
            int cur = this.reader.Read();
            while (!this.reader.EndOfStream && !char.IsWhiteSpace((char)cur))
            {
                sb.Append((char)cur);
                cur = this.reader.Read();
            }
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public string ReadLine()
        {
            return this.reader.ReadLine();
        }

        public bool HasMore
        {
            [DebuggerStepThrough]
            get { return !this.reader.EndOfStream; }
        }

        [DebuggerStepThrough]
        public static implicit operator StreamReaderTextReader(StreamReader reader)
        {
            if (reader != null)
                return new StreamReaderTextReader(reader);
            return null;
        }

        [DebuggerStepThrough]
        public static implicit operator StreamReaderTextReader(Stream reader)
        {
            if (reader != null)
                return new StreamReaderTextReader(reader);
            return null;
        }
    }
}
