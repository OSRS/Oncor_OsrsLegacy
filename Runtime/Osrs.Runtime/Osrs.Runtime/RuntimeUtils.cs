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

using System.Diagnostics;

namespace Osrs.Runtime
{
    public static class RuntimeUtils
    {
        [DebuggerStepThrough]
        public static bool Failed(RunState state)
        {
            return state == RunState.FailedBootstrapping ||
                state == RunState.FailedInitializing ||
                state == RunState.FailedInstalling ||
                state == RunState.FailedRunning ||
                state == RunState.FailedStarting ||
                state == RunState.FailedStopping ||
                state == RunState.FailedUnbootstrapping ||
                state == RunState.FailedUninitializing ||
                state == RunState.FailedUninstalling;
        }

        [DebuggerStepThrough]
        public static bool Bootstrappable(RunState state)
        {
            return Bootstrappable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Bootstrappable(RunState state, bool allowFailed)
        {
            return state == RunState.Created || (allowFailed ? state == RunState.FailedBootstrapping : false);
        }

        [DebuggerStepThrough]
        public static bool Initializable(RunState state)
        {
            return Initializable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Initializable(RunState state, bool allowFailed)
        {
            return state == RunState.Created || state == RunState.Bootstrapped || (allowFailed ? state == RunState.FailedInitializing : false);
        }

        [DebuggerStepThrough]
        public static bool Startable(RunState state)
        {
            return Startable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Startable(RunState state, bool allowFailed)
        {
            return state == RunState.Created || state == RunState.Stopped || state == RunState.Initialized ||
                (allowFailed ? (state == RunState.FailedStarting || state == RunState.FailedRunning) : false);
        }

        [DebuggerStepThrough]
        public static bool Stoppable(RunState state)
        {
            return Stoppable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Stoppable(RunState state, bool allowFailed)
        {
            return state == RunState.Running || state == RunState.Paused ||
                (allowFailed ? (state == RunState.FailedStopping || state == RunState.FailedRunning) : false);
        }

        [DebuggerStepThrough]
        public static bool Pausable(RunState state)
        {
            return Pausable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Pausable(RunState state, bool allowFailed)
        {
            return state == RunState.Running ||
                (allowFailed ? (state == RunState.FailedStopping || state == RunState.FailedRunning) : false);
        }

        [DebuggerStepThrough]
        public static bool Resumable(RunState state)
        {
            return Resumable(state, false);
        }

        [DebuggerStepThrough]
        public static bool Resumable(RunState state, bool allowFailed)
        {
            return state == RunState.Paused ||
                (allowFailed ? (state == RunState.FailedStarting || state == RunState.FailedRunning) : false);
        }

        [DebuggerStepThrough]
        public static void WaitForBootstrappableState(IStatefulExecution state)
        {
            WaitForBootstrappableState(state, false);
        }

        [DebuggerStepThrough]
        public static void WaitForBootstrappableState(IStatefulExecution state, bool allowFailed)
        {
            MethodContract.NotNull(state, nameof(state));

            System.Threading.SpinWait waiter = new System.Threading.SpinWait();
            while (true)
            {
                if (Bootstrappable(state.State, allowFailed))
                    return;
                if (allowFailed && Failed(state.State))
                    return; //try to allow a recovery here
                waiter.SpinOnce();
            }
        }

        [DebuggerStepThrough]
        public static void WaitForInitializableState(IStatefulExecution state)
        {
            WaitForInitializableState(state, false);
        }

        [DebuggerStepThrough]
        public static void WaitForInitializableState(IStatefulExecution state, bool allowFailed)
        {
            MethodContract.NotNull(state, nameof(state));

            System.Threading.SpinWait waiter = new System.Threading.SpinWait();
            while (true)
            {
                if (Initializable(state.State, allowFailed))
                    return;
                if (allowFailed && Failed(state.State))
                    return; //try to allow a recovery here
                waiter.SpinOnce();
            }
        }
    }
}
