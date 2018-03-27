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

using Osrs.Data;
using Osrs.Runtime;
using System;

namespace Osrs.WellKnown.Projects
{
    public sealed class ProjectStatus
    {
        public CompoundIdentity ProjectIdentity
        {
            get;
        }

        public Guid Identity
        {
            get;
        }

        public CompoundIdentity StatusTypeIdentity
        {
            get;
        }

        public DateTime StatusDate
        {
            get;
        }

        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.text = value;
            }
        }

        public ProjectStatus(CompoundIdentity projectId, Guid entryId, CompoundIdentity projectStatusTypeId, string statusText)
            : this(projectId, entryId, projectStatusTypeId, statusText, DateTime.UtcNow)
        { }

        public ProjectStatus(CompoundIdentity projectId, Guid entryId, CompoundIdentity projectStatusTypeId, string statusText, DateTime statusDate)
        {
            MethodContract.NotNullOrEmpty(projectId, nameof(projectId));
            MethodContract.NotNullOrEmpty(projectStatusTypeId, nameof(projectStatusTypeId));
            MethodContract.NotNullOrEmpty(statusText, nameof(statusText));
            MethodContract.Assert(!Guid.Empty.Equals(entryId), nameof(entryId));

            this.ProjectIdentity = projectId;
            this.StatusTypeIdentity = projectStatusTypeId;
            this.Identity = entryId;
            this.text = statusText;
            this.StatusDate = statusDate;
        }
    }
}
