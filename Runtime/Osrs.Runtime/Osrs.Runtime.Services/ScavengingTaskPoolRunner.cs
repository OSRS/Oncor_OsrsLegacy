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

namespace Osrs.Runtime.Services
{
    /// <summary>
    /// Allows a task pool to be run in a continuous loop.
    /// This will "fan out" in that each runner in the pool will be non-blocking and run the action function repeatedly up to the concurrency of how many workers are in the pool.
    /// </summary>
    public class ScavengingTaskPoolRunner
    {
        private readonly Action run;
        private TaskPool pool;

        private ScavengingTaskPoolRunner(Action run)
        {
            this.run = run;
        }

        /// <summary>
        /// The work function to provide the pool as the action to run
        /// </summary>
        internal void Run()
        {
            this.run();
            try
            {
                TaskPoolRunner r = null;
                do
                {
                    r = this.pool.Next();
                    if (r != null)
                        r.Run(); //remember this is an async call
                }
                while (r == null);
            }
            catch
            { }
        }

        public static TaskPool CreateScavengingTaskPool(Action toRun, TaskPoolOptions options)
        {
            MethodContract.NotNull(toRun, nameof(toRun));
            ScavengingTaskPoolRunner runner = new ScavengingTaskPoolRunner(toRun);
            TaskPool pool = new TaskPool(runner.Run, options);
            runner.pool = pool;
            return pool;
        }
    }
}
