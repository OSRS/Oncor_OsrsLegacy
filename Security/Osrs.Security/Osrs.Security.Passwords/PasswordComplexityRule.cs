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

using Osrs.Data.Validation;
using Osrs.Numerics;
using System.Collections.Generic;

namespace Osrs.Security.Passwords
{
    public abstract class PasswordComplexityRule : IValidator<string>
    {
        public abstract bool IsValid(string item);

        public bool IsValidObject(object item)
        {
            return this.IsValid(item as string);
        }
    }

    public class MultiRulePasswordComplexityRule : PasswordComplexityRule
    {
        private readonly HashSet<PasswordComplexityRule> rules = new HashSet<PasswordComplexityRule>();
        public ISet<PasswordComplexityRule> Rules
        {
            get { return this.rules; }
        }

        public override bool IsValid(string item)
        {
            foreach(PasswordComplexityRule curRule in this.rules)
            {
                if (curRule != null && !curRule.IsValid(item))
                    return false;
            }
            return true;
        }
    }

    public class LegalCharsPasswordComplexityRule : PasswordComplexityRule
    {
        private HashSet<char> legalChars;
        public ISet<char> LegalChars
        {
            get
            {
                if (legalChars == null)
                    legalChars = new HashSet<char>();
                return legalChars;
            }
        }

        private HashSet<char> illegalChars;
        public ISet<char> IllegalChars
        {
            get
            {
                if (illegalChars == null)
                    illegalChars = new HashSet<char>();
                return illegalChars;
            }
        }

        private HashSet<ValueRange<char>> rangesRestricted;
        public ISet<ValueRange<char>> IllegalCharRanges
        {
            get
            {
                if (rangesRestricted == null)
                    rangesRestricted = new HashSet<ValueRange<char>>();
                return rangesRestricted;
            }
        }

        public override bool IsValid(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                if (legalChars!=null) //all are illegal unless called out as legal
                {
                    foreach(char cur in item)
                    {
                        if (!legalChars.Contains(cur))
                            return false; //short-circuit
                    }
                }
                if (illegalChars!=null)
                {
                    foreach(char cur in item)
                    {
                        if (this.illegalChars.Contains(cur))
                            return false; //short-circuit
                    }
                }
                if (rangesRestricted!=null)
                {
                    foreach(char cur in item)
                    {
                        foreach(ValueRange<char> range in rangesRestricted)
                        {
                            if (range!=null)
                            {
                                if (!range.IsValid(cur))
                                    return false; //short-circuit
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }

    public class CharCountPasswordComplexityRule : PasswordComplexityRule
    {
        public ushort MinLower
        {
            get;
            set;
        } = 0;

        public ushort MinUpper
        {
            get;
            set;
        } = 0;

        public ushort MinNumber
        {
            get;
            set;
        } = 0;

        public override bool IsValid(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                int ctLow = 0;
                int ctUp = 0;
                int ctNum = 0;
                foreach(char cur in item)
                {
                    if (char.IsLower(cur))
                        ctLow++;
                    else if (char.IsUpper(cur))
                        ctUp++;
                    else if (char.IsNumber(cur))
                        ctNum++;
                }
                if (ctLow < MinLower || ctUp < MinUpper || ctNum < MinNumber) //value of 0 for any will pass as true if no min set
                    return false;

                return true;
            }
            return false;
        }
    }

    public class CountCharRangePasswordComplexityRule : PasswordComplexityRule
    {
        private HashSet<ValueRange<char>> legalRanges = new HashSet<ValueRange<char>>();
        public ISet<ValueRange<char>> LegalRanges
        {
            get { return this.legalRanges; }
        }

        public ushort MinCount
        {
            get;
            set;
        } = 1;

        public override bool IsValid(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                int ctNum = 0;
                foreach (char cur in item)
                {
                    foreach(ValueRange<char> curRange in this.legalRanges)
                    {
                        if (curRange!=null && curRange.IsValid(cur))
                            ctNum++;
                    }
                }
                if (ctNum < MinCount) //value of 0 for any will pass as true if no min set
                    return false;

                return true;
            }
            return false;
        }
    }

    public class LengthPasswordComplexityRule : PasswordComplexityRule
    {
        private ValueRange<uint> lengths = new ValueRange<uint>(8, 2048);
        public ValueRange<uint> Lengths
        {
            get { return this.lengths; }
            set
            {
                if (value != null)
                    this.lengths = value;
            }
        }

        public override bool IsValid(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                return this.Lengths.IsValid((uint)item.Length);
            }
            return false;
        }
    }
}
