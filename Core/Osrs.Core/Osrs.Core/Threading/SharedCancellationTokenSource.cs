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
namespace Osrs.Threading
{
    public enum SharedFireMode
    {
        SystemCallsOsrs,
        OsrsCallsSystem,
        Bidirectional
    }

    public sealed class SharedCancellationTokenSource
    {
        private System.Threading.CancellationTokenSource systemSource;
        public System.Threading.CancellationTokenSource SystemSource
        {
            get { return this.systemSource; }
        }

        private Osrs.Threading.CancellationTokenSource osrsSource;
        private Osrs.Threading.CancellationTokenSource OsrsSource
        {
            get { return this.osrsSource; }
        }

        public SharedFireMode SharedFireMode
        {
            get;
            set;
        }

        private bool triggered = false;

        private void CallbackOsrs()
        {
            if (!(this.triggered || systemSource.IsCancellationRequested) && this.SharedFireMode != SharedFireMode.SystemCallsOsrs)
                systemSource.Cancel();
            this.triggered = true; //in any case, we only fire once
        }

        private void CallbackSystem()
        {
            if (!(this.triggered || osrsSource.IsCancellationRequested) && this.SharedFireMode != SharedFireMode.OsrsCallsSystem)
                osrsSource.Cancel();
            this.triggered = true; //in any case, we only fire once
        }

        private void Init()
        {
            if (systemSource != null && osrsSource != null)
            {
                systemSource.Token.Register(CallbackSystem);
                osrsSource.RegisterCallback(CallbackOsrs);
            }
        }

        public SharedCancellationTokenSource(System.Threading.CancellationTokenSource systemSource) : this(systemSource, SharedFireMode.Bidirectional)
        { }

        public SharedCancellationTokenSource(System.Threading.CancellationTokenSource systemSource, SharedFireMode fireMode)
        {
            if (systemSource != null)
            {
                this.systemSource = systemSource;
                this.osrsSource = new CancellationTokenSource();
            }
            this.SharedFireMode = fireMode;
        }

        public SharedCancellationTokenSource(Osrs.Threading.CancellationTokenSource osrsSource) : this(osrsSource, SharedFireMode.Bidirectional)
        { }

        public SharedCancellationTokenSource(Osrs.Threading.CancellationTokenSource osrsSource, SharedFireMode fireMode)
        {
            if (osrsSource != null)
            {
                this.osrsSource = osrsSource;
                this.systemSource = new System.Threading.CancellationTokenSource();
            }
            this.SharedFireMode = fireMode;
        }

        public SharedCancellationTokenSource(Osrs.Threading.CancellationTokenSource osrsSource, System.Threading.CancellationTokenSource systemSource)
            : this(osrsSource, systemSource, SharedFireMode.Bidirectional)
        { }

        public SharedCancellationTokenSource(Osrs.Threading.CancellationTokenSource osrsSource, System.Threading.CancellationTokenSource systemSource, SharedFireMode fireMode)
        {
            if (systemSource != null && osrsSource != null)
            {
                this.systemSource = systemSource;
                this.osrsSource = osrsSource;
            }
            this.SharedFireMode = fireMode;
        }
    }
}
