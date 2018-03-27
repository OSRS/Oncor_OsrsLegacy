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
    public enum ContextScopeElement
    {
        /// <summary>
        /// The scope of the context is unknown (the default value for any scoped context)
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The scope of the context is the single current request
        /// </summary>
        Request,
        /// <summary>
        /// The scope of the context is the current user
        /// </summary>
        User,
        /// <summary>
        /// The scope of the context is the executing code module (meaning is dependent upon implementation)
        /// </summary>
        Module,
        /// <summary>
        /// The scope of the context is the currently executing process
        /// </summary>
        Process,
        /// <summary>
        /// The scope of the context is the current container / node (multiple containers may share a machine)
        /// </summary>
        Container,
        /// <summary>
        /// The scope of the context is the current machine / vm (multiple containers may share a machine)
        /// </summary>
        Machine,
        /// <summary>
        /// The scope of the context is the current service (a service may server multiple applications, but is not part of any application)
        /// </summary>
        Service,
        /// <summary>
        /// The scope of the context is the application for which the context originates (an application may use multiple services, but the application owns none of the services)
        /// </summary>
        Application,
        /// <summary>
        /// The scope of the context is the system (a system is a hierarchy of other systems, applications and services)
        /// </summary>
        System,
        /// <summary>
        /// The scope of the context is a machine group, which is a logical grouping of machines and the containers they run
        /// </summary>
        MachineGroup,
        /// <summary>
        /// The scope of the context is a physical site in which machines are hosted (e.g. a data center), note that a machine group is generally within a hosting site, but may span hosting sites
        /// </summary>
        HostingSite,
        /// <summary>
        /// The scope of the context is the enterprise, which is the collection of all other scopes, in that any child scope can only be part of a single enterprise
        /// </summary>
        Enterprise,
        /// <summary>
        /// The scope of the context is the special "global" scope, meaning the scope is applicable to any an all things (e.g. unbounded scope)
        /// </summary>
        Global
    }

    /// <summary>
    /// A context is a collection of information relating to the running executable code base.
    /// Contexts are used for identifying ownership of an entity, such as configuration parameters.
    /// Contexts are used for security, such as tracking the current user executing a thread of code.
    /// </summary>
    public interface IContext : IEquatable<IContext>
    { }

    /// <summary>
    /// A context is a collection of information relating to the running executable code base.
    /// Contexts are used for identifying ownership of an entity, such as configuration parameters.
    /// Contexts are used for security, such as tracking the current user executing a thread of code.
    /// </summary>
    public interface IContext<T> : IContext, IEquatable<IContext<T>>
        where T:IEquatable<T>
    {
        T Identity
        {
            get;
        }
    }

    /// <summary>
    /// A context is a collection of information relating to the running executable code base.
    /// Contexts are used for identifying ownership of an entity, such as configuration parameters.
    /// Contexts are used for security, such as tracking the current user executing a thread of code.
    /// </summary>
    public abstract class GenericContext<T> : IContext<T>, IEquatable<GenericContext<T>>
        where T : IEquatable<T>
    {
        private readonly T identity;
        public T Identity
        {
            get { return this.identity; }
        }

        protected GenericContext(T id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            this.identity = id;
        }

        public bool Equals(IContext other)
        {
            return this.Equals(other as GenericContext<T>);
        }

        public bool Equals(IContext<T> other)
        {
            return this.Equals(other as GenericContext<T>);
        }

        public bool Equals(GenericContext<T> other)
        {
            return other != null && this.identity.Equals(other.Identity);
        }
    }

    /// <summary>
    /// A context is a collection of information relating to the running executable code base.
    /// Contexts are used for identifying ownership of an entity, such as configuration parameters.
    /// Contexts are used for security, such as tracking the current user executing a thread of code.
    /// </summary>
    public class ContextBase : GenericContext<Guid>
    {
        protected internal ContextBase(Guid id) : base(Guid.NewGuid())
        { }
    }
}
