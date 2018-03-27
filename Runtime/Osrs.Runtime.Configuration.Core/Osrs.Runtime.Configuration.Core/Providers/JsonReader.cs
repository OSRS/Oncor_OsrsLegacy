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
using System.IO;
using System.Text;

namespace Osrs.Runtime.Configuration.Providers
{
    internal sealed class JsonReader
    {
        internal JObject Read(string fileName)
        {
            if (!File.Exists(fileName))
                return null;
            return this.Read(File.OpenText(fileName));
        }

        internal JObject Read(StreamReader rdr)
        {
            if (rdr != null)
            {
                if (rdr.BaseStream.CanRead)
                {
                    JsonReader.SpinWhiteSpace(rdr); //prior to start of object
                    JObject o = JObject.Read(rdr);
                    rdr.Dispose();
                    return o;
                }
                try
                {
                    rdr.Dispose();
                }
                catch
                { }
            }
            return null;
        }

        internal JsonReader()
        {}

        internal static void SpinWhiteSpace(StreamReader rdr)
        {
            int cur = rdr.Peek();
            while (cur > 0 && char.IsWhiteSpace((char)cur))
            {
                rdr.Read();
                cur = rdr.Peek();
            }
        }

        internal static string ReadString(StreamReader rdr)
        {
            if (rdr.Peek() == '"')
            {
                rdr.Read();
                StringBuilder sb = new StringBuilder();
                bool notEscaped = true;
                while(true)
                {
                    int curInt = rdr.Read();
                    if (curInt >= 0)
                    {
                        char cur = (char)curInt;
                        if (notEscaped)
                        {
                            if (cur == '\\')//escapes
                                notEscaped = false;
                            else if (cur == '"') //ends string
                                return sb.ToString();
                            else
                                sb.Append(cur);
                        }
                        else //we're escaped
                        {
                            if (cur == '\\')
                                sb.Append(cur);
                            else if (cur == '"')
                                sb.Append(cur);
                            else if (cur == '/')
                                sb.Append(cur);
                            else if (cur == 'b')
                                sb.Append('\b');
                            else if (cur == 'f')
                                sb.Append('\f');
                            else if (cur == 'n')
                                sb.Append('\n');
                            else if (cur == 'r')
                                sb.Append('\r');
                            else if (cur == 't')
                                sb.Append('\t');
                            else if (cur == 'u') //character code
                            {
                                char[] values = new char[4];
                                if (rdr.Read(values, 0, 4) != 4)
                                    break; //we didn't read 4 characters
                                int chr = 0;
                                for (int i = 0; i < values.Length; i++)
                                {
                                    char c = values[i];
                                    int bse = bases[i];
                                    switch (c)
                                    {
                                        case '0':
                                            continue;
                                        case '1':
                                            chr += bse * 1;
                                            break;
                                        case '2':
                                            chr += bse * 2;
                                            break;
                                        case '3':
                                            chr += bse * 3;
                                            break;
                                        case '4':
                                            chr += bse * 4;
                                            break;
                                        case '5':
                                            chr += bse * 5;
                                            break;
                                        case '6':
                                            chr += bse * 6;
                                            break;
                                        case '7':
                                            chr += bse * 7;
                                            break;
                                        case '8':
                                            chr += bse * 8;
                                            break;
                                        case '9':
                                            chr += bse * 9;
                                            break;
                                        case 'a':
                                            chr += bse * 10;
                                            break;
                                        case 'b':
                                            chr += bse * 11;
                                            break;
                                        case 'c':
                                            chr += bse * 12;
                                            break;
                                        case 'd':
                                            chr += bse * 13;
                                            break;
                                        case 'e':
                                            chr += bse * 14;
                                            break;
                                        case 'f':
                                            chr += bse * 15;
                                            break;
                                        default:
                                            return null; //got a bad char
                                    }
                                }
                            }
                            else //illegal escape
                                return null;
                            notEscaped = true;
                        }
                    }
                    else //eof or other issue
                        break;
                }
            }
            return null;
        }
        private static readonly int[] bases = new int[] { 4096, 256, 16, 1 };

        internal static bool ReadNull(StreamReader rdr)
        {
            int cur = rdr.Read();
            if (cur == (int)'n')
            {
                cur = rdr.Read();
                if (cur==(int)'u')
                {
                    cur = rdr.Read();
                    if (cur == (int)'l')
                    {
                        cur = rdr.Read();
                        if (cur == (int)'l')
                            return true;
                    }
                }
            }
            return false;
        }
    }

    internal abstract class JEntity
    {}

    internal sealed class JObject : JEntity
    {
        private Dictionary<string, JEntity> properties = new Dictionary<string, JEntity>();
        public JEntity this[string key]
        {
            get 
            {
                if (this.properties.ContainsKey(key))
                    return this.properties[key];
                return null;
            }
        }

        public IEnumerable<string> Keys
        { get { return this.properties.Keys; } }

        internal static JObject Read(StreamReader rdr)
        {
            //JsonReader.SpinWhiteSpace(rdr); //prior to start of object
            if (rdr.Peek() == (int)'{')
            {
                rdr.Read(); //just eat the opener
                JObject tmp = new JObject();
                JsonReader.SpinWhiteSpace(rdr); //after start of object, prior to first data char

                if (rdr.Peek() == (int)'}')
                {
                    rdr.Read(); //eat the }
                    return tmp; //empty object
                }
                //we have to have properties
                while(true)
                {
                    if (ReadProperty(rdr, tmp))
                    {
                        JsonReader.SpinWhiteSpace(rdr); //after property, prior to next data char
                        if (rdr.Peek() == (int)'}') //end object, return finished
                        {
                            rdr.Read(); //eat the }
                            return tmp;
                        }
                        else if (rdr.Peek() == (int)',') //keep going, we MUST have more properties
                        {
                            rdr.Read(); //eat the comma
                            JsonReader.SpinWhiteSpace(rdr); //after comma prior to next data char
                        }
                        else
                            return null; //something funny happened, like early EoF or illegal character
                    }
                    else //failed to read a property
                        return null;
                }
            }
            return null;
        }

        private static bool ReadProperty(StreamReader rdr, JObject tmp)
        {
            string key = JsonReader.ReadString(rdr);
            if (key != null && !tmp.properties.ContainsKey(key))
            {
                JsonReader.SpinWhiteSpace(rdr);
                if (rdr.Read() == (int)':')
                {
                    JsonReader.SpinWhiteSpace(rdr);
                    JEntity value = null;
                    bool wasNull = false;
                    int c = rdr.Peek();
                    if (c == (int)'"')
                        value = JPrimitive.Read(rdr); //its a string!
                    else if (c == (int)'[')
                        value = JArray.Read(rdr);
                    else if (c == (int)'{')
                        value = JObject.Read(rdr);
                    else if (c == (int)'-' || (c >= 48 && c <= 57)) //- or ascii for 0-9 -- should be a number
                        value = JPrimitive.Read(rdr);
                    else if (c == (int)'t') //should be "true"
                        value = JPrimitive.Read(rdr);
                    else if (c == (int)'f') //should be "false"
                        value = JPrimitive.Read(rdr);
                    else if (c == (int)'n') //should be "null"
                    {
                        wasNull = true;
                        if (!JsonReader.ReadNull(rdr))
                            return false; //looked like a null, but wasn't
                    }
                    else
                        return false;  //unrecognized starting char

                    if (value != null || wasNull)
                    {
                        tmp.properties.Add(key, value);
                        return true;
                    }
                    //property wasn't "null", but we didn't get anything read - that's an error
                }
            }

            return false; //didn't get a key back
        }
    }

    internal sealed class JArray : JEntity
    {
        private readonly List<JEntity> elements = new List<JEntity>();
        public int Count
        { get { return this.elements.Count; } }

        public JEntity this[int index]
        { get { return this.elements[index]; } }

        internal static JArray Read(StreamReader rdr)
        {
            JsonReader.SpinWhiteSpace(rdr); //prior to start of object
            if (rdr.Read() == (int)'[')
            {
                JArray tmp = new JArray();
                JsonReader.SpinWhiteSpace(rdr); //after start of object, prior to first data char

                if (rdr.Peek() == (int)']')
                    return tmp; //empty array
                //we have to have properties
                while (true)
                {
                    if (ReadElement(rdr, tmp))
                    {
                        JsonReader.SpinWhiteSpace(rdr); //after property, prior to next data char
                        if (rdr.Peek() == (int)']') //end array, return finished
                        {
                            rdr.Read(); //eat the end array
                            return tmp;
                        }
                        else if (rdr.Peek() == (int)',') //keep going, we MUST have more elements
                        {
                            rdr.Read(); //eat the comma
                            JsonReader.SpinWhiteSpace(rdr); //after comma prior to next data char
                        }
                        else
                            return null; //something funny happened, like early EoF or illegal character
                    }
                    else
                        return null; //failed to read the element
                }
            }
            return null;
        }

        private static bool ReadElement(StreamReader rdr, JArray tmp)
        {
            JsonReader.SpinWhiteSpace(rdr);
            JEntity element = null;
            int cur = rdr.Peek();
            if (cur == (int)'"')
                element = JPrimitive.Read(rdr); //its a string!
            else if (cur == (int)'{')
                element = JObject.Read(rdr);
            else if (cur==(int)'[')
                element = JArray.Read(rdr);
            else if (cur == (int)'-' || (cur >= 48 && cur <= 57)) //- or ascii for 0-9 -- should be a number
                element = JPrimitive.Read(rdr);
            else if (cur == (int)'t') //should be "true"
                element = JPrimitive.Read(rdr);
            else if (cur == (int)'f') //should be "false"
                element = JPrimitive.Read(rdr);
            else if (cur == (int)'n') //should be "null"
            {
                if (JsonReader.ReadNull(rdr))
                {
                    tmp.elements.Add(null);
                    return true; //one off case here due to a literal null element
                }
                //looked like a null, but wasn't - so we fall through fail
            }

            if (element != null)
            {
                tmp.elements.Add(element);
                return true;
            }

            return false;
        }
    }

    internal sealed class JPrimitive : JEntity
    {
        private readonly string value;
        private readonly JType parsedType;
        public JType Type
        {
            get { return this.parsedType; }
        }

        public override string ToString()
        {
            return this.value;
        }

        public int ToInt()
        {
            if (this.parsedType == JType.JNumber)
                return int.Parse(this.value);
            throw new InvalidCastException();
        }
        public long ToLong()
        {
            if (this.parsedType == JType.JNumber)
                return long.Parse(this.value);
            throw new InvalidCastException();
        }
        public float ToFloat()
        {
            if (this.parsedType == JType.JNumber)
                return float.Parse(this.value);
            throw new InvalidCastException();
        }
        public double ToDouble()
        {
            if (this.parsedType == JType.JNumber)
                return double.Parse(this.value);
            throw new InvalidCastException();
        }
        public bool ToBool()
        {
            if (this.parsedType == JType.JBool)
                return bool.Parse(this.value);
            throw new InvalidCastException();
        }

        private JPrimitive(string value, JType parsedType)
        {
            this.value = value;
            this.parsedType = parsedType;
        }

        internal static JPrimitive Read(StreamReader rdr)
        {
            string value=null;
            JType parsedType = JType.JString;
            int cur = rdr.Peek();
            if (cur ==(int)'"')
                value = JsonReader.ReadString(rdr);
            else if (cur == (int)'-' || (cur >= 48 && cur <= 57)) //- or ascii for 0-9 -- should be a number
            {
                StringBuilder sb = new StringBuilder();

                cur = rdr.Peek();
                if (cur == (int)'-')
                {
                    sb.Append((char)cur);
                    cur = rdr.Read();
                }

                if (ParseIntPart(rdr, sb))
                {
                    cur = rdr.Peek();
                    if (cur == (int)'.') //do the decimal part
                    {
                        sb.Append('.');
                        rdr.Read();
                        if (!ParseIntPart(rdr, sb))
                            return null; //failed the parse decimal part
                        cur = rdr.Peek();
                    }

                    if (cur == (int)'e' || cur == (int)'E') //do the exponent part with or without a decimal point
                    {
                        sb.Append('e');
                        rdr.Read();
                        cur = rdr.Peek();
                        if (cur == (int)'-' || cur == (int)'+')
                        {
                            sb.Append((char)cur);
                            rdr.Read();
                        }
                        //ok, now it better be all digits
                        if (ParseIntPart(rdr, sb))
                            value = sb.ToString();
                    }
                    else
                        value = sb.ToString();
                }
                //else value = null; --note this is implicit, hence commented out

                parsedType = JType.JNumber;
            }
            else if (cur == (int)'t') //should be "true"
            {
                char[] txt = new char[4];
                if (4 == rdr.Read(txt, 0, 4))
                {
                    if (txt[1]=='r' && txt[2]=='u' && txt[3]=='e')
                        value = "true";
                }
                parsedType = JType.JBool;
            }
            else if (cur == (int)'f') //should be "false"
            {
                char[] txt = new char[5];
                if (5 == rdr.Read(txt, 0, 5))
                {
                    if (txt[1] == 'a' && txt[2] == 'l' && txt[3] == 's' && txt[4] == 'e')
                        value = "false";
                }
                parsedType = JType.JBool;
            }

            if (value == null)
                return null;
            return new JPrimitive(value, parsedType);
        }
    
        private static bool ParseIntPart(StreamReader rdr, StringBuilder sb)
        {
            int cur = rdr.Peek();
            if (cur >= 48 && cur <= 57) //note we don't handle the sign part for this method
            {
                while (cur >= 48 && cur <= 57)
                {
                    sb.Append((char)cur);
                    rdr.Read();
                    cur = rdr.Peek();
                }
                return true;
            }
            return false;
        }
    }

    internal enum JType
    {
        JString,
        JBool,
        JNumber
    }
}
