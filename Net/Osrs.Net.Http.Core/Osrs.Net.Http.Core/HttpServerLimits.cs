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

namespace Osrs.Net.Http
{
    public class HttpServerLimits
    {
        private int maxOutstandingAccepts = 5 * Environment.ProcessorCount; //same as Katana
        public int MaxOutstandingAccepts
        {
            get
            {
                return this.maxOutstandingAccepts;
            }
            set
            {
                if (value > 0)
                    this.maxOutstandingAccepts = value;
                else if (value == 0)
                    this.maxOutstandingRequests = 5 * Environment.ProcessorCount; //same as Katana
            }
        }

        private int maxOutstandingRequests = Int32.MaxValue; //same as Katana
        public int MaxOutstandingRequests
        {
            get
            {
                return this.maxOutstandingRequests;
            }
            set
            {
                if (value > 0)
                    this.maxOutstandingRequests = value;
                else if (value == 0)
                    this.maxOutstandingRequests = Int32.MaxValue; //same as Katana
            }
        }
    }
}
