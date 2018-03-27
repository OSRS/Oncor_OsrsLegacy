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

namespace Osrs.Runtime
{
    /// <summary>
    /// Compares a pair of <see cref="SemanticVersion"/> for compatibility.
    /// For a specific pair of items, such as a Serializer for a Type, a specific compare rule may be needed.
    /// In this case, a custom SemanticVersionComparer is created to validate the version compatibility.
    /// </summary>
    public class SemanticVersionComparer
    {
        private static readonly SemanticVersionComparer instance = new SemanticVersionComparer();
        public static SemanticVersionComparer Default
        {
            get { return instance; }
        }

        public SemanticVersionComparer()
        { }

        public virtual bool Compatible(SemanticVersion ideal, SemanticVersion actual)
        {
            if (ideal.Major == actual.Major)
            {
                return ideal.Minor >= actual.Minor;
            }
            return false;
        }
    }

    /// <summary>
    /// A SemanticVersion is a set of unsigned short integers that represent the incremental version of an item.
    /// The definition of a semantic version expects and implies certain things.
    /// 
    /// A semantic version is composed of 4 required parts and one optional part as follows:
    /// (ushort) Major, (ushort) Minor, (ushort) Patch, (ushort) Increment, (string) Tag (optional)
    /// Example (string format): 10.1.11.232-alpha  <major>.<minor>.<patch>.<increment>-<tag>
    /// Since each "part" of the version is an unsigned, 16-bit (short) integer, numeric comparison rules apply to each part.
    /// E.g.  11.1.2.1 is "greater" than 10.9999.9999.9999
    /// Each "part" can use the entire value range of 0->65535 (inclusive)
    /// A version of 0.0.0.0 is the implicit "zero" | "first" | "initial" version.
    /// The tag string can be up to 1024 characters in length, and may contain any alpha-numeric characters (only, so no punctuation or whitespace)
    /// 
    /// The implied rules for comparing semantic versions for compatibility are:
    /// If comparing SemanticVersion a  to  SemanticVersion b:
    /// Major versions are breaking versions, so no compatibility is implied (rule: a.Major!=b.Major implies incompatible)
    /// Minor versions are backward compatible, so a is compatible with b if a&lt;b (rule: a.Major!=b.Major || a.Minor<b.Minor implies incompatible)
    /// Patch versions are bug/performance fixes, so full compatiblity is expected (rule:  a.Patch==b.Patch || a.Patch!=b.Patch still implies compatibility)
    /// Increment versions are build numbers or sub-patches, so full compatiblity is expected (rule:  a.Increment==b.Increment || a.Increment!=b.Increment still implies compatibility)
    /// Tags are for notation purposes and should not affect compatibility
    /// 
    /// For specific purposes, SemanticVersions may be used to determine compatibility between entities where these rules differ.
    /// In those cases, use a custom <see cref="SemanticVersionComparer"/>
    /// </summary>
    public sealed class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IEmpty<SemanticVersion>
    {
        private static readonly SemanticVersion empty = new SemanticVersion(0, 0, 0, 0);
        public static SemanticVersion Empty
        {
            get { return empty; }
        }

        private readonly ushort major;
        public ushort Major
        {
            get { return this.major; }
        }

        private readonly ushort minor;
        public ushort Minor
        {
            get { return this.minor; }
        }

        private readonly ushort patch;
        public ushort Patch
        {
            get { return this.patch; }
        }

        private readonly ushort increment;
        public ushort Increment
        {
            get { return this.increment; }
        }

        private readonly string tag;
        public string Tag
        {
            get { return this.tag; }
        }

        public SemanticVersion(ushort major, ushort minor, ushort patch, ushort increment) : this(major, minor, patch, increment, null)
        { }

        public SemanticVersion(ushort major, ushort minor, ushort patch, ushort increment, string tag)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.increment = increment;
            if (tag != null && tag.Length > 1024)
                tag = tag.Substring(0, 1024);
            this.tag = tag;
        }

        public static SemanticVersion Create(System.Version v)
        {
            return new SemanticVersion((ushort)v.Major, (ushort)v.Minor, (ushort)v.Revision, (ushort)v.Build);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.tag))
            {
                return this.major.ToString() + '.' + this.minor.ToString() + '.' + this.patch.ToString() + '.' + this.increment.ToString();
            }
            return this.major.ToString() + '.' + this.minor.ToString() + '.' + this.patch.ToString() + '.' + this.increment.ToString() + '-' + this.tag;
        }

        public static SemanticVersion Parse(string versionString)
        {
            int posA = 0; //major-minor
            int posB = 0; //minor-patch
            int posC = 0; //patch-increment
            int posD = 0; //increment-tag .. optional

            int pos = 0;
            for (int i = 0; i < versionString.Length; i++)
            {
                char cur = versionString[i];
                if (cur == '.')
                {
                    if (pos == 0)
                        posA = i;
                    else if (pos == 1)
                        posB = i;
                    else if (pos == 2)
                        posC = i;
                    else //too many .
                        throw new ArgumentException();

                    pos++;
                }
                else if (cur == '-') //tag
                {
                    if (pos == 3)
                    {
                        posD = i;
                        pos++;
                    }
                    else //illegal -
                        throw new ArgumentException();
                }
                else
                {
                    if (pos < 3)
                    {
                        if (!char.IsDigit(cur))
                            throw new ArgumentException();
                    }
                    else //in the tag
                    {
                        if (!char.IsLetterOrDigit(cur))
                            throw new ArgumentException();
                    }
                }
            }
            if (posD == 0)
                posD = versionString.Length; //no tag at all

            ushort a = ushort.Parse(versionString.Substring(0, posA));
            ushort b = ushort.Parse(versionString.Substring(posA, posB - posA));
            ushort c = ushort.Parse(versionString.Substring(posB, posC - posB));
            ushort d = ushort.Parse(versionString.Substring(posC, posD - posC));

            if (posD < versionString.Length)
                return new SemanticVersion(a, b, c, d, versionString.Substring(posD));
            return new SemanticVersion(a, b, c, d);
        }

        public static bool TryParse(string versionString, out SemanticVersion ver)
        {
            try
            {
                ver = Parse(versionString);
            }
            catch
            {
                ver = null;
                return false;
            }
            return true;
        }

        public bool Equals(SemanticVersion other)
        {
            if (object.ReferenceEquals(other, null))
                return false;
            return this.major == other.major && this.minor == other.minor && this.patch == other.patch && this.increment == other.increment && this.tag == other.tag;
        }

        public int CompareTo(SemanticVersion other)
        {
            int cur = this.major.CompareTo(other.major);
            if (cur != 0)
                return cur;
            cur = this.minor.CompareTo(other.minor);
            if (cur != 0)
                return cur;
            cur = this.patch.CompareTo(other.patch);
            if (cur != 0)
                return cur;
            return this.increment.CompareTo(other.increment);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj.As<SemanticVersion>());
        }

        public override int GetHashCode()
        {
            if (this.tag == null)
                return (((int)this.major) << 16 | ((int)this.minor)) ^
                    (((int)this.patch) << 16 | ((int)this.increment));
            return this.tag.GetHashCode() ^ (((int)this.major) << 16 | ((int)this.minor)) ^
                    (((int)this.patch) << 16 | ((int)this.increment));
        }

        public static bool operator ==(SemanticVersion a, SemanticVersion b)
        {
            if (object.ReferenceEquals(a, null))
                return object.ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(SemanticVersion a, SemanticVersion b)
        {
            if (object.ReferenceEquals(a, null))
                return !object.ReferenceEquals(b, null);
            return !a.Equals(b);
        }

        public bool IsEmpty
        {
            get { return this.Equals(empty); }
        }
    }

    public class SemanticTypeVersionComparer
    {
        private static readonly SemanticTypeVersionComparer instance = new SemanticTypeVersionComparer();
        public static SemanticTypeVersionComparer Default
        {
            get { return instance; }
        }

        public SemanticTypeVersionComparer()
        { }

        public virtual bool Compatible(SemanticTypeVersion ideal, SemanticTypeVersion actual)
        {
            if (ideal.Major == actual.Major)
            {
                return ideal.Minor >= actual.Minor;
            }
            return false;
        }
    }

    public sealed class SemanticTypeVersion : IEquatable<SemanticTypeVersion>, IComparable<SemanticTypeVersion>, IEmpty<SemanticTypeVersion>
    {
        private static readonly SemanticTypeVersion empty = new SemanticTypeVersion(0, 0);
        public static SemanticTypeVersion Empty
        {
            get { return empty; }
        }

        private readonly ushort major;
        public ushort Major
        {
            get { return this.major; }
        }

        private readonly ushort minor;
        public ushort Minor
        {
            get { return this.minor; }
        }

        public SemanticTypeVersion(ushort major, ushort minor)
        {
            this.major = major;
            this.minor = minor;
        }

        public override string ToString()
        {
            return this.major.ToString() + '.' + this.minor.ToString();
        }

        public static SemanticTypeVersion Parse(string versionString)
        {
            int posA = 0; //major-minor
            int posB = 0; //minor-patch

            int pos = 0;
            for (int i = 0; i < versionString.Length; i++)
            {
                char cur = versionString[i];
                if (cur == '.')
                {
                    if (pos == 0)
                        posA = i;
                    else if (pos == 1)
                        posB = i;
                    else //too many .
                        throw new ArgumentException();

                    pos++;
                }
                else
                {
                    if (!char.IsDigit(cur))
                        throw new ArgumentException();
                }
            }
            ushort a = ushort.Parse(versionString.Substring(0, posA));
            ushort b = ushort.Parse(versionString.Substring(posA, posB - posA));

            return new SemanticTypeVersion(a, b);
        }

        public static bool TryParse(string versionString, out SemanticTypeVersion ver)
        {
            try
            {
                ver = Parse(versionString);
            }
            catch
            {
                ver = null;
                return false;
            }
            return true;
        }

        public bool Equals(SemanticTypeVersion other)
        {
            if (object.ReferenceEquals(other, null))
                return false;
            return this.major == other.major && this.minor == other.minor;
        }

        public int CompareTo(SemanticTypeVersion other)
        {
            int cur = this.major.CompareTo(other.major);
            if (cur != 0)
                return cur;
            return this.minor.CompareTo(other.minor);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj.As<SemanticTypeVersion>());
        }

        public override int GetHashCode()
        {
            return (((int)this.major) << 16 | ((int)this.minor));
        }

        public static bool operator ==(SemanticTypeVersion a, SemanticTypeVersion b)
        {
            if (object.ReferenceEquals(a, null))
                return object.ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(SemanticTypeVersion a, SemanticTypeVersion b)
        {
            if (object.ReferenceEquals(a, null))
                return !object.ReferenceEquals(b, null);
            return !a.Equals(b);
        }

        public bool IsEmpty
        {
            get { return this.Equals(empty); }
        }
    }
}
