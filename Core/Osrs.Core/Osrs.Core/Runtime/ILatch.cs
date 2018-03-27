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
namespace Osrs.Runtime
{
    public interface IParameterizedLatch<T>
    {
        bool IsLatched
        {
            get;
        }

        void Latch(T item);
    }

    /// <summary>
    /// Provides a mechanism to "lock out" changes to an object at some point after creation.
    /// 
    /// This provides the ability to allow properties or methods to be used for a short period after instantiation of an object, then deterministically locked to prevent further editing.
    /// </summary>
    public interface ILatch
    {
        bool IsLatched
        {
            get;
        }

        void Latch();
    }

    /// <summary>
    /// Unlockable latch for situations where latching is transient.
    /// 
    /// Common scenario is to provide private access to the latch to allow a second object control of latching such as:
    /// class A{
    ///     ILatch latch;
    ///     static A Create(ILatch latchreference)
    ///     {
    ///         this.latch=latchReference;
    ///     }
    /// }
    /// 
    /// class B{
    ///     ILatch aLatch=new SimpleLatch();
    ///     A GetA()
    ///     {
    ///         return A.Create(this.aLatch);
    ///     }
    ///     
    ///     void LatchA()
    ///     {
    ///         this.aLatch.Latch();  //this will lock ALL objects sharing the latch
    ///     }
    /// }
    /// 
    /// </summary>
    public sealed class SimpleLatch : ILatch
    {
        private bool latched = false;
        public bool IsLatched
        {
            get { return this.latched; }
        }

        public void Latch()
        {
            this.latched = true;
        }
    }

    /// <summary>
    /// Unlockable latch for situations where latching is transient.
    /// 
    /// Common scenario is to provide private access to the latch to allow a second object control of latching such as:
    /// class A{
    ///     ILatch latch;
    ///     static A Create(ILatch latchreference)
    ///     {
    ///         this.latch=latchReference;
    ///     }
    /// }
    /// 
    /// class B{
    ///     ILatch aLatch=new SimpleLatch();
    ///     A GetA()
    ///     {
    ///         return A.Create(this.aLatch);
    ///     }
    ///     
    ///     void LatchA()
    ///     {
    ///         this.aLatch.Latch();  //this will lock ALL objects sharing the latch
    ///     }
    ///     
    ///     void UnLatchA()
    ///     {
    ///         this.aLatch.UnLatch();  //this will unlock ALL objects sharing the latch
    ///     }
    /// }
    /// 
    /// </summary>
    public sealed class SimpleReversableLatch : ILatch
    {
        private bool latched = false;
        public bool IsLatched
        {
            get { return this.latched; }
        }

        public void Latch()
        {
            this.latched = true;
        }

        public void UnLatch()
        {
            this.latched = false;
        }
    }
}
