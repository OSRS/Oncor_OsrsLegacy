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
    public sealed class ProjectInformation
    {
        public CompoundIdentity ProjectIdentity
        {
            get;
        }

        public Guid Identity
        {
            get;
        }

        private string infoText;
        public string InformationText
        {
            get { return this.infoText; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.infoText = value;
            }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get { return this.startDate; }
            set
            {
                if (value.HasValue)
                {
                    if (this.endDate.HasValue && value > this.endDate)
                        return; //don't allow the change if it inverts timeline
                    this.startDate = value;
                }
                else
                    this.startDate = value; //set to null
            }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return this.endDate; }
            set
            {
                if (value.HasValue)
                {
                    if (this.startDate.HasValue && value < this.startDate)
                        return; //don't allow the change if it inverts timeline
                    this.endDate = value;
                }
                else
                    this.endDate = value; //set to null
            }
        }

        public ProjectInformation(CompoundIdentity projectId, Guid entryId, string infoText, DateTime? startDate, DateTime? endDate)
        {
            MethodContract.NotNullOrEmpty(projectId, nameof(projectId));
            MethodContract.NotNullOrEmpty(infoText, nameof(infoText));
            MethodContract.Assert(!Guid.Empty.Equals(entryId), nameof(entryId));

            if (startDate.HasValue && endDate.HasValue)
                MethodContract.Assert(startDate <= endDate, "startDate must be <= endDate");

            this.ProjectIdentity = projectId;
            this.Identity = entryId;
            this.infoText = infoText;
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
