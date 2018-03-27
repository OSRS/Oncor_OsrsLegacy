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
using System.Collections.Generic;

namespace Osrs.Data
{
    public delegate void ChangedEventHandler(object changed);
    public delegate void ChangedEventHandler<T>(T changed);

    public class ChangeCallbackHandler
    {
        private HashSet<ChangedEventHandler> handlers = new HashSet<ChangedEventHandler>();

        public void AddHandler(ChangedEventHandler handler)
        {
            if (handler != null)
                this.handlers.Add(handler);
        }

        public virtual void ChangeOccurred(object changed)
        {
            foreach(ChangedEventHandler curHandler in this.handlers)
            {
                curHandler(changed);
            }
        }
    }

    public class ChangeCallbackHandler<T>
    {
        private HashSet<ChangedEventHandler<T>> handlers = new HashSet<ChangedEventHandler<T>>();

        public void AddHandler(ChangedEventHandler<T> handler)
        {
            if (handler != null)
                this.handlers.Add(handler);
        }

        public virtual void ChangeOccurred(T changed)
        {
            foreach (ChangedEventHandler<T> curHandler in this.handlers)
            {
                curHandler(changed);
            }
        }
    }
}
