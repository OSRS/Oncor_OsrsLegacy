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
using System.Collections.Generic;
using System.IO;
using Osrs.Threading;
using Osrs.Runtime;

namespace Osrs.Net.Http.Handlers
{
    public class SimpleFileHandler : HttpHandlerBase
    {
        private static readonly FileExtensions defaultExtensions = new FileExtensions();
        public static FileExtensions DefaultExtensions
        {
            get { return defaultExtensions; }
        }

        private readonly DirectoryMapper mapper;

        private readonly MimeTypes mimeTypes;
        public MimeTypes MimeTypes
        {
            get { return this.mimeTypes; }
        }

        private readonly FileExtensions allowedExtensions;
        public FileExtensions LocalExtensionsAllowed
        {
            get { return this.allowedExtensions; }
        }

        private readonly FileExtensions disallowedExtensions;
        public FileExtensions LocalExtensionsDisallowed
        {
            get { return this.disallowedExtensions; }
        }

        private readonly List<string> defaultFiles = new List<string>();
        public List<string> DefaultFiles
        {
            get { return this.defaultFiles; } //dangerously mutable!
        }

        private string MapPath(string path)
        {
            string tmp = null;
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Length>1 || path[0]!='/')
                {
                    string part = path.Replace('/', Path.DirectorySeparatorChar);
                    tmp = this.mapper.MapPath(part);
                    if (!string.IsNullOrEmpty(tmp)) //found a direct match
                        return tmp;

                    foreach (string s in this.defaultFiles)
                    {
                        tmp = this.mapper.MapPath(Path.Combine(part, s));
                        if (!string.IsNullOrEmpty(tmp))
                            return tmp;
                    }
                    return null; //we have no file that matches
                }
            }

            //if we're here, it's either an empty or bare '/' path
            foreach (string s in this.defaultFiles)
            {
                tmp = this.mapper.MapPath(s);
                if (!string.IsNullOrEmpty(tmp))
                    return tmp;
            }
            return null; //we have no file that matches
        }

        private void Push(HttpResponse response, string mappedPath, Stream to)
        {
            FileStream from = null;
            MemoryStream ms = new MemoryStream();
            try
            {
                from = File.OpenRead(mappedPath);
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCodes.Status500InternalServerError;
                return;
            }

            response.StatusCode = HttpStatusCodes.Status200OK;
            response.ContentType = this.mimeTypes.GetFor(mappedPath);

            try
            {
                int t = from.ReadByte();
                while (-1 < t)
                {
                    ms.WriteByte((byte)t);
                    t = from.ReadByte();
                }
            }
            catch //problem here is we really can't change the statue code now.
            {
            }

            response.ContentLength = ms.Length;
            ms.Seek(0, SeekOrigin.Begin);
            response.Body.Write(ms.ToArray(), 0, (int)ms.Length);

            from.Dispose();
        }

        public SimpleFileHandler(string localPath) : this(localPath, null, MimeTypes.GetAllWellKnown(), null, null)
        { }

        public SimpleFileHandler(string localPath, string defaultFile) : this(localPath, new string[] { defaultFile }, MimeTypes.GetAllWellKnown(), null, null)
        { }

        public SimpleFileHandler(string localPath, IEnumerable<string> defaultFiles, MimeTypes mimeTypes, FileExtensions allowed, FileExtensions disallowed)
            : base()
        {
            MethodContract.NotNull(localPath, nameof(localPath));
            MethodContract.NotNull(defaultFiles, nameof(defaultFiles));
            MethodContract.NotNull(mimeTypes, nameof(mimeTypes));

            this.mapper = new DirectoryMapper(localPath);
            this.mimeTypes = mimeTypes;
            if (defaultFiles != null)
            {
                foreach (string curFil in defaultFiles)
                {
                    this.defaultFiles.Add(curFil);
                }
            }

            if (allowed != null)
                this.allowedExtensions = allowed;
            else
                this.allowedExtensions = new FileExtensions();

            if (disallowed != null)
                this.disallowedExtensions = disallowed;
            else
                this.disallowedExtensions = new FileExtensions();
        }

        public override void Handle(HttpContext context, CancellationToken cancel)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string matchPath = UriHelper.GetRequestUrl(context);
            int qIndex = matchPath.IndexOf('?');
            if (qIndex > -1)
                matchPath = matchPath.Substring(0, qIndex);

            if (!this.disallowedExtensions.MatchesFile(matchPath))
            {
                string mappedPath = this.MapPath(matchPath);

                if (!string.IsNullOrEmpty(mappedPath))
                {
                    if (this.allowedExtensions.MatchesFile(mappedPath))
                    {
                        this.Push(response, mappedPath, response.Body);
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCodes.Status403Forbidden;
                    }
                }
                else
                {
                    response.StatusCode = HttpStatusCodes.Status404NotFound;
                }
            }
            else
                response.StatusCode = HttpStatusCodes.Status403Forbidden;
        }
    }
}
