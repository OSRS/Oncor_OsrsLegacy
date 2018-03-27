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

using Osrs.Numerics.Spatial.Geometry;
using System;
using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Operations
{
    public static class GeometryUtils
    {
        public static Envelope2<T> EnvelopeFor<T>(IEnumerable<IGeometry2<T>> shapes)
            where T : IEquatable<T>, IComparable<T>
        {
            if (shapes == null)
                return null;
            IEnumerator<IGeometry2<T>> en = shapes.GetEnumerator();
            Envelope2<T> curEnv = null;
            if (en.MoveNext())
            {
                IGeometry2<T> cur = en.Current;
                Envelope2<T> env = null;
                if (cur != null)
                    env = cur.Envelope;
                curEnv = env;
                while (en.MoveNext())
                {
                    cur = en.Current;
                    if (cur != null)
                        env = cur.Envelope;
                    curEnv = Envelope2<T>.Bound(curEnv, env);
                }
                return curEnv;
            }
            return null;
        }

        public static Envelope2<T> EnvelopeFor<T>(IGeometry2<T> a, IGeometry2<T> b)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null)
            {
                if (b == null)
                    return null;
                return a.Envelope;
            }
            else if (b == null)
                return a.Envelope;

            return Envelope2<T>.Bound(a.Envelope, b.Envelope);
        }

        public static Envelope2<T> EnvelopeFor<T>(IGeometry2<T> a, IGeometry2<T> b, IGeometry2<T> c)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null)
            {
                if (b == null)
                {
                    if (c == null)
                        return null;
                    return c.Envelope;
                }
            }
            else if (b == null)
            {
                if (c == null)
                    return a.Envelope;
                return EnvelopeFor<T>(a, c);
            }
            else if (c == null)
            {
                return EnvelopeFor<T>(a, b);
            }

            Envelope2<T> envCur = a.Envelope;
            envCur = Envelope2<T>.Bound(envCur, b.Envelope);
            return Envelope2<T>.Bound(envCur, c.Envelope);
        }

        public static Envelope2<T> EnvelopeFor<T>(Envelope2<T> env, IGeometry2<T> p)
        where T : IEquatable<T>, IComparable<T>
        {
            if (env == null)
            {
                if (p == null)
                    return null;
                return p.Envelope;
            }
            return Envelope2<T>.Bound(env, p.Envelope);
        }
    }
}
