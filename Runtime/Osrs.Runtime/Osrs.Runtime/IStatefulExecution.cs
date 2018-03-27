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
    public enum RunState
    {
        Unknown = int.MinValue,
        Downloading = int.MinValue + 1,
        FailedUninstalling = int.MinValue + 100,
        Uninstalling = int.MinValue + 500,
        FailedInstalling = -500,
        Installing = -100,
        Created = 0,
        Bootstrapping = 1,
        FailedBootstrapping = 2,
        Bootstrapped = 3,
        Initializing = 4,
        FailedInitializing = 5,
        Initialized = 6,
        Starting = 7,
        FailedStarting = 8,
        Running = 9,
        FailedRunning = 10,
        Pausing = 11,
        Paused = 12,
        Resuming = 13,
        Stopping = 14,
        FailedStopping = 15,
        Stopped = 16,
        Uninitializing = 17,
        FailedUninitializing = 18,
        Uninitialized = 19,
        UnBootstrapping = 20,
        FailedUnbootstrapping = 21,
        UnBootstrapped = 22,
        /// <summary>
        /// Same as concept of "ran to completion" - implies should not be started again but did not fail
        /// </summary>
        Shutdown = 23
    }

    public interface IStatefulExecution
    {
        RunState State
        {
            get;
        }
    }
}
