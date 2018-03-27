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
using System.Security.Cryptography;

namespace Osrs.Security
{
    public class SaltPair
    {
        public readonly string Salt;
        public readonly string SaltedPayload;

        internal SaltPair(string salt, string salted)
        {
            this.Salt = salt;
            this.SaltedPayload = salted;
        }
    }

    public enum SaltCreationModel
    {
        Default,
        NonRepeatable,
        NonRepeatableStrong,
        Repeatable
    }

    public enum SaltEmbeddingModel
    {
        Default,
        Beginning,
        Ending,
        Middle,
        Randomized
    }

    public class SaltShaker
    {
        private readonly SaltCreationModel model;
        public SaltCreationModel CreationModel
        {
            get { return this.model; }
        }

        private readonly SaltEmbeddingModel embed;
        public SaltEmbeddingModel EmbeddingModel
        {
            get { return this.embed; }
        }

        private int seed;

        private readonly char minChar = char.MinValue;
        public char MinChar
        {
            get { return (char)this.minChar; }
        }

        private readonly char maxChar = char.MaxValue;
        public char MaxChar
        {
            get { return (char)this.maxChar; }
        }

        private readonly int saltLength;
        public int SaltLength
        {
            get { return this.saltLength; }
        }

        private HashSet<int> suppressedChars;
        public ISet<int> SupressedChars
        {
            get
            {
                if (this.suppressedChars == null)
                    this.suppressedChars = new HashSet<int>();
                return this.suppressedChars;
            }
        }

        public SaltShaker(int saltLength) : this(char.MinValue, char.MaxValue, saltLength, SaltCreationModel.NonRepeatableStrong, SaltEmbeddingModel.Middle, 0)
        { }

        public SaltShaker(char minChar, char maxChar, int saltLength) : this(minChar, maxChar, saltLength, SaltCreationModel.NonRepeatableStrong, SaltEmbeddingModel.Middle, 0)
        { }

        public SaltShaker(int saltLength, int seed) : this(char.MinValue, char.MaxValue, saltLength, SaltCreationModel.Repeatable, SaltEmbeddingModel.Randomized, seed)
        { }

        public SaltShaker(char minChar, char maxChar, int saltLength, int seed) : this(minChar, maxChar, saltLength, SaltCreationModel.Repeatable, SaltEmbeddingModel.Randomized, seed)
        { }

        public SaltShaker(char minChar, char maxChar, int saltLength, SaltCreationModel creationModel, SaltEmbeddingModel embedModel, int seed)
        {
            MethodContract.Assert(minChar < maxChar, nameof(minChar) + " must be less than " + nameof(maxChar));
            MethodContract.Assert(saltLength > 0, nameof(saltLength));
            this.seed = seed;
            this.model = creationModel;
            this.embed = embedModel;
            this.minChar = minChar;
            this.maxChar = maxChar;
            this.saltLength = saltLength;
        }

        /// <summary>
        /// Generates a salt value that is purely randomized and therefore must be persisted as it cannot be reliably duplicated
        /// </summary>
        /// <returns></returns>
        public string GenerateSalt()
        {
            if (this.model == SaltCreationModel.NonRepeatableStrong)
                return SaltShaker.GenerateSalt(this.saltLength, this.minChar, this.maxChar, RandomNumberGenerator.Create(), this.suppressedChars);
            else if (this.model == SaltCreationModel.NonRepeatable)
                return SaltShaker.GenerateSalt(this.saltLength, this.minChar, this.maxChar, new Random(), this.suppressedChars);
            else
                return SaltShaker.GenerateSalt(this.saltLength, this.minChar, this.maxChar, new Random(this.seed), this.suppressedChars);
        }

        /// <summary>
        /// Generates a salt value based upon the value of the payload such that the same payload will generate the same salt reliably
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string GenerateSalt(string payload)
        {
            return SaltShaker.GenerateSalt(this.saltLength, this.minChar, this.maxChar, payload, this.suppressedChars);
        }

        /// <summary>
        /// Embeds the provided payload into the provided salt.
        /// The embedding model is determined by the SaltEntropyModel used when creating this salt shaker
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string Embed(string payload, string salt)
        {
            return SaltShaker.EmbedSalt(this.embed, payload, salt);
        }

        /// <summary>
        /// Creates a salt and embeds the provided payload in it in a single operation.
        /// The mechanism used to generate and embed the salt is determined by the SaltEntropyModel used when creating this salt shaker
        /// The salt value produced is returned with the embedded value in the SaltPair.  If the model produces a non-reprocable salt, both values must be retained.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public SaltPair Salt(string payload)
        {
            string salt;
            if (this.model == SaltCreationModel.Default || this.model == SaltCreationModel.Repeatable)
                salt = this.GenerateSalt(payload);
            else
                salt = this.GenerateSalt();

            return new SaltPair(salt, EmbedSalt(this.embed, payload, salt));
        }

        public static string EmbedSalt(SaltEmbeddingModel model, string payload, string salt)
        {
            if (!string.IsNullOrEmpty(payload))
            {
                if (!string.IsNullOrEmpty(salt))
                {
                    if (salt.Length > payload.Length)
                    {
                        if (model == SaltEmbeddingModel.Default || model == SaltEmbeddingModel.Randomized)
                        {
                            int hash = Math.Abs(payload.GetHashCode()); //calc the entropy
                            while (hash > salt.Length)
                            {
                                hash = hash - salt.Length;
                            }
                            //ok, hash is still positive, but now less than salt.length so we can embed

                            char[] front = payload.ToCharArray();
                            char[] back = salt.ToCharArray();
                            if (hash + front.Length < back.Length) //direct embed
                            {
                                for (int i = 0; i < front.Length; i++)
                                {
                                    back[hash + i] = front[i];
                                }
                            }
                            else //need to do a wrap around like a circular array
                            {
                                int wrapIndex = back.Length - hash;
                                for (int i = 0; i < front.Length; i++)
                                {
                                    if (hash + i < back.Length)
                                        back[hash + i] = front[i];
                                    else
                                        back[i - wrapIndex] = front[i];
                                }
                            }
                            return new string(back);
                        }
                        else if (model == SaltEmbeddingModel.Middle) //goes in the middle
                        {
                            char[] front = payload.ToCharArray();
                            char[] back = salt.ToCharArray();
                            int start = (back.Length - front.Length) / 2;
                            for (int i = 0; i < front.Length; i++)
                            {
                                back[start + i] = front[i];
                            }
                            return new string(back);
                        }
                        else if (model == SaltEmbeddingModel.Beginning) //easiest one to do
                        {
                            char[] data = salt.ToCharArray();
                            for (int i = 0; i < payload.Length; i++)
                                data[i] = payload[i];
                            return new string(data);
                        }
                        else //end
                        {
                            char[] data = salt.ToCharArray();
                            int indexBase = data.Length - payload.Length; //first spot to replace
                            for (int i = 0; i < payload.Length; i++)
                                data[indexBase + i] = payload[i];
                            return new string(data);
                        }
                    }
                    else if (salt.Length == payload.Length)
                        return payload; //no extra chars
                    else
                        return payload.Substring(0, salt.Length); //truncate to salt length
                }
                return payload; //nothing to embed in
            }
            return salt;
        }

        public static string GenerateSalt(int saltLength, string payload)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, payload, null);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, string payload)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, payload, null);
        }

        public static string GenerateSalt(int saltLength, string payload, ISet<int> suppressedChars)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, payload, suppressedChars);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, string payload, ISet<int> suppressedChars)
        {
            //generate a seed for the RNG - to add entropy, we "wiggle" the values since many passwords are in range 0-127 (ASCII chars)
            byte[] raw = new byte[] { 0, 0, 0, 0 }; //32-bits
            int index = 0;
            for (int i = 0; i < payload.Length; i++)
            {
                raw[index] = (byte)(raw[index] ^ (255 & (int)payload[i])); //masked to low byte
                index++;
                if (index == 4) //out of bounds for raw length
                    index = 0;
            }

            return GenerateSalt(saltLength, minChar, maxChar, new Random(BitConverter.ToInt32(raw, 0)), suppressedChars);
        }

        public static string GenerateSalt(int saltLength, int seed)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, seed, null);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, int seed)
        {
            return GenerateSalt(saltLength, minChar, maxChar, new Random(seed), null);
        }

        public static string GenerateSalt(int saltLength, int seed, ISet<int> suppressedChars)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, new Random(seed), suppressedChars);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, int seed, ISet<int> suppressedChars)
        {
            return GenerateSalt(saltLength, minChar, maxChar, new Random(seed), suppressedChars);
        }

        public static string GenerateSalt(int saltLength, Random generator)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, generator, null);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, Random generator)
        {
            return GenerateSalt(saltLength, minChar, maxChar, generator, null);
        }

        public static string GenerateSalt(int saltLength, Random generator, ISet<int> suppressedChars)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, generator, suppressedChars);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, Random generator, ISet<int> suppressedChars)
        {
            MethodContract.Assert(minChar < maxChar, nameof(minChar) + " must be less than " + nameof(maxChar));
            int min = (int)minChar;
            int max = (int)maxChar;

            char[] sb = new char[saltLength]; //buckets to fill
            HashSet<int> visited = new HashSet<int>(); //buckets visited

            int count = 0;
            int lastBucket = 0;
            int bucket = GetBucketIndex(visited, saltLength, lastBucket, generator.Next(1, (saltLength - count) + 1));
            int cur;
            while (count < saltLength)
            {
                cur = generator.Next(min, max + 1);
                if (suppressedChars == null || !suppressedChars.Contains(cur))
                {
                    sb[bucket] = (char)cur;
                    lastBucket = bucket;
                    bucket = GetBucketIndex(visited, saltLength, lastBucket, generator.Next(1, (saltLength - count) + 1));
                    count++;
                }
            }

            return new string(sb);
        }

        public static string GenerateSalt(int saltLength, RandomNumberGenerator generator)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, generator, null);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, RandomNumberGenerator generator)
        {
            return GenerateSalt(saltLength, minChar, maxChar, generator, null);
        }

        public static string GenerateSalt(int saltLength, RandomNumberGenerator generator, ISet<int> suppressedChars)
        {
            return GenerateSalt(saltLength, char.MinValue, char.MaxValue, generator, suppressedChars);
        }

        public static string GenerateSalt(int saltLength, char minChar, char maxChar, RandomNumberGenerator generator, ISet<int> suppressedChars)
        {
            MethodContract.Assert(minChar < maxChar, nameof(minChar) + " must be less than " + nameof(maxChar));
            int min = (int)minChar;
            int max = (int)maxChar;

            char[] sb = new char[saltLength]; //buckets to fill

            //for speed here, we'll do it with a stride rather than random bucket fill
            byte[] raw = new byte[4];
            int count = (saltLength/2)<10 ? saltLength/2 : 10; //start as maximum stride we'd want
            int lastBucket = (saltLength/2)<5 ? saltLength/2 : 5; //start as minimum stride we'd want
            int stride;
            if (count == lastBucket)
                stride = count;
            else
                stride = GetValue(generator, raw, lastBucket, count);
            count = 0;
            lastBucket = 0; //this is actually the base bucket for the stride return to 0

            int bucket = 0;
            int cur;
            while (count < saltLength)
            {
                cur = GetValue(generator, raw, min, max);
                if (suppressedChars == null || !suppressedChars.Contains(cur))
                {
                    sb[bucket] = (char)cur;
                    bucket = bucket + stride;
                    if (bucket>=sb.Length)
                    {
                        lastBucket++; //increment the base bucket, ends up at stride
                        bucket = lastBucket;
                    }
                    count++;
                }
            }

            return new string(sb);
        }

        private static int GetBucketIndex(HashSet<int> visited, int bucketCount, int lastBucket, int marchLength)
        {
            //if there are fewer buckets left than marchLength, we can just reduce the marchlength
            int remainingBuckets = bucketCount - visited.Count;
            if (remainingBuckets > 0)
            {
                if (marchLength > 1)
                {
                    if (remainingBuckets < marchLength)
                    {
                        do //TODO -- convert to direct rather than loop
                        {
                            marchLength -= remainingBuckets;
                        }
                        while (remainingBuckets < marchLength);
                    }

                    int curIndex = lastBucket + 1;
                    while (curIndex < bucketCount)
                    {
                        if (!visited.Contains(curIndex)) //it's a good bucket
                        {
                            visited.Add(curIndex);
                            return curIndex;
                        }
                        curIndex++;
                    }
                    //we got to the end, so start over at 0
                    curIndex = 0;
                    while (curIndex < lastBucket)
                    {
                        if (!visited.Contains(curIndex)) //it's a good bucket
                        {
                            visited.Add(curIndex);
                            return curIndex;
                        }
                        curIndex++;
                    }
                    throw new IndexOutOfRangeException("not possible - we vomit"); //shouldn't be possible
                }
                else
                {
                    remainingBuckets = lastBucket++;
                    while (visited.Contains(remainingBuckets))
                    {
                        remainingBuckets++;
                    }
                    if (remainingBuckets>=bucketCount) //over the end, start over at 0
                    {
                        remainingBuckets = 0;
                        while (visited.Contains(remainingBuckets))
                        {
                            remainingBuckets++;
                        }
                    }
                    visited.Add(remainingBuckets);
                    return remainingBuckets;
                }
            }
            return -1;
        }

        private static int GetValue(RandomNumberGenerator generator, byte[] raw, int min, int max)
        {
            if (min < max)
            {
                generator.GetBytes(raw);
                int bucket = BitConverter.ToInt32(raw, 0);
                if (bucket < min)
                {
                    bucket = Math.Abs(bucket);
                    if (bucket < min)
                    {
                        do //TODO -- convert to direct rather than loop
                        {
                            bucket = bucket + min;
                        }
                        while (bucket < min);
                    }
                }

                if (bucket > max) //allow for flowing over
                {
                    int range = max - min;
                    do //TODO -- convert to direct rather than loop
                    {
                        bucket = bucket - range;
                    }
                    while (bucket > max);
                }

                return bucket;
            }
            return min;
        }
    }
}
