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
    public interface ISimpleService : IStatefulExecution
    {
        void Start();
        void Stop();
    }

    public interface IService : ISimpleService
    {
        void Pause();
        void Resume();
    }

    public interface IInitializable
    {
        void Initialize();
    }

    public interface IIntializable<T>
    {
        void Initialize(T configInfo);
    }

    public interface IModule : IService, IInitializable
    { }

    public interface IModule<T> : IModule, IIntializable<T>
    { }

    public interface IBootstrappable : IStatefulExecution
    {
        void Bootstrap();
    }

    public interface IBootstrappable<T>
    {
        void Bootstrap(T configInfo);
    }

    public interface ISystemModule : IModule, IBootstrappable
    { }

    public interface ISystemModule<T> : ISystemModule, IBootstrappable<T>
    { }

    public interface IShutdown
    {
        void Shutdown();
    }

    public interface IRunnable
    {
        void Run();
    }
}
