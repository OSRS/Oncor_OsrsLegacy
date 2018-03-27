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
using System.IO;

namespace Osrs.Net.Http
{
    public class MimeTypes
    {
        private string defaultMime = "text/plain";
        public string DefaultMimeType
        {
            get { return this.defaultMime; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.defaultMime = "text/plain";
                else
                    this.defaultMime = value;
            }
        }

        private readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>();
        public string this[string extension]
        {
            get
            {
                if (string.IsNullOrEmpty(extension))
                    return string.Empty;
                string ext = extension.ToLowerInvariant();
                if (this.mimeTypes.ContainsKey(ext))
                    return this.mimeTypes[ext];
                return this.defaultMime;
            }
        }

        public MimeTypes()
        { }

        public string GetFor(string filename)
        {
            string ext = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(ext))
                return this.defaultMime;
            ext = ext.ToLowerInvariant();
            if (this.mimeTypes.ContainsKey(ext))
                return this.mimeTypes[ext];
            return this.defaultMime;
        }

        public void Add(string extension, string mimeType)
        {
            if (string.IsNullOrEmpty(extension))
                return;
            if (this.Exists(extension))
                return;
            this.mimeTypes[extension.ToLowerInvariant()] = mimeType;
        }

        public void Remove(string extension)
        {
            if (this.mimeTypes.ContainsKey(extension))
                this.mimeTypes.Remove(extension);
        }

        public bool Exists(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;
            return this.mimeTypes.ContainsKey(extension.ToLowerInvariant());
        }

        public static MimeTypes GetAllWellKnown()
        {
            MimeTypes tmp = new MimeTypes();
            AddAllWellKnown(tmp);
            return tmp;
        }

        public static void AddAllWellKnown(MimeTypes types)
        {
            if (types == null)
                return;

            types.Add(".*", "application/octet-stream");
            types.Add(".323", "text/h323");
            types.Add(".aaf", "application/octet-stream");
            types.Add(".aca", "application/octet-stream");
            types.Add(".accdb", "application/msaccess");
            types.Add(".accde", "application/msaccess");
            types.Add(".accdt", "application/msaccess");
            types.Add(".acx", "application/internet-property-stream");
            types.Add(".afm", "application/octet-stream");
            types.Add(".ai", "application/postscript");
            types.Add(".aif", "audio/x-aiff");
            types.Add(".aifc", "audio/aiff");
            types.Add(".aiff", "audio/aiff");
            types.Add(".application", "application/x-ms-application");
            types.Add(".art", "image/x-jg");
            types.Add(".asd", "application/octet-stream");
            types.Add(".asf", "video/x-ms-asf");
            types.Add(".asi", "application/octet-stream");
            //types.Add(".asm", "text/plain");
            types.Add(".asr", "video/x-ms-asf");
            types.Add(".asx", "video/x-ms-asf");
            types.Add(".atom", "application/atom+xml");
            types.Add(".au", "audio/basic");
            types.Add(".avi", "video/x-msvideo");
            types.Add(".axs", "application/olescript");
            //types.Add(".bas", "text/plain");
            types.Add(".bcpio", "application/x-bcpio");
            types.Add(".bin", "application/octet-stream");
            types.Add(".bmp", "image/bmp");
            //types.Add(".c", "text/plain");
            types.Add(".cab", "application/octet-stream");
            types.Add(".calx", "application/vnd.ms-office.calx");
            types.Add(".cat", "application/vnd.ms-pki.seccat");
            types.Add(".cdf", "application/x-cdf");
            types.Add(".chm", "application/octet-stream");
            types.Add(".class", "application/x-java-applet");
            types.Add(".clp", "application/x-msclip");
            types.Add(".cmx", "image/x-cmx");
            //types.Add(".cnf", "text/plain");
            types.Add(".cod", "image/cis-cod");
            types.Add(".cpio", "application/x-cpio");
            //types.Add(".cpp", "text/plain");
            types.Add(".crd", "application/x-mscardfile");
            types.Add(".crl", "application/pkix-crl");
            types.Add(".crt", "application/x-x509-ca-cert");
            types.Add(".csh", "application/x-csh");
            types.Add(".css", "text/css");
            types.Add(".csv", "application/octet-stream");
            types.Add(".cur", "application/octet-stream");
            types.Add(".dcr", "application/x-director");
            types.Add(".deploy", "application/octet-stream");
            types.Add(".der", "application/x-x509-ca-cert");
            types.Add(".dib", "image/bmp");
            types.Add(".dir", "application/x-director");
            types.Add(".disco", "text/xml");
            types.Add(".dll", "application/x-msdownload");
            types.Add(".dll.config", "text/xml");
            types.Add(".dlm", "text/dlm");
            types.Add(".doc", "application/msword");
            types.Add(".docm", "application/vnd.ms-word.document.macroEnabled.12");
            types.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            types.Add(".dot", "application/msword");
            types.Add(".dotm", "application/vnd.ms-word.template.macroEnabled.12");
            types.Add(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
            types.Add(".dsp", "application/octet-stream");
            types.Add(".dtd", "text/xml");
            types.Add(".dvi", "application/x-dvi");
            types.Add(".dwf", "drawing/x-dwf");
            types.Add(".dwp", "application/octet-stream");
            types.Add(".dxr", "application/x-director");
            types.Add(".eml", "message/rfc822");
            types.Add(".emz", "application/octet-stream");
            types.Add(".eot", "application/vnd.ms-fontobject");
            types.Add(".eps", "application/postscript");
            types.Add(".etx", "text/x-setext");
            types.Add(".evy", "application/envoy");
            types.Add(".exe", "application/octet-stream");
            types.Add(".exe.config", "text/xml");
            types.Add(".fdf", "application/vnd.fdf");
            types.Add(".fif", "application/fractals");
            types.Add(".fla", "application/octet-stream");
            types.Add(".flr", "x-world/x-vrml");
            types.Add(".flv", "video/x-flv");
            types.Add(".gif", "image/gif");
            types.Add(".gtar", "application/x-gtar");
            types.Add(".gz", "application/x-gzip");
            //types.Add(".h", "text/plain");
            types.Add(".hdf", "application/x-hdf");
            types.Add(".hdml", "text/x-hdml");
            types.Add(".hhc", "application/x-oleobject");
            types.Add(".hhk", "application/octet-stream");
            types.Add(".hhp", "application/octet-stream");
            types.Add(".hlp", "application/winhlp");
            types.Add(".hqx", "application/mac-binhex40");
            types.Add(".hta", "application/hta");
            types.Add(".htc", "text/x-component");
            types.Add(".htm", "text/html");
            types.Add(".html", "text/html");
            types.Add(".htt", "text/webviewhtml");
            types.Add(".hxt", "text/html");
            types.Add(".ico", "image/x-icon");
            types.Add(".ics", "application/octet-stream");
            types.Add(".ief", "image/ief");
            types.Add(".iii", "application/x-iphone");
            types.Add(".inf", "application/octet-stream");
            types.Add(".ins", "application/x-internet-signup");
            types.Add(".isp", "application/x-internet-signup");
            types.Add(".IVF", "video/x-ivf");
            types.Add(".jar", "application/java-archive");
            types.Add(".java", "application/octet-stream");
            types.Add(".jck", "application/liquidmotion");
            types.Add(".jcz", "application/liquidmotion");
            types.Add(".jfif", "image/pjpeg");
            types.Add(".jpb", "application/octet-stream");
            types.Add(".jpe", "image/jpeg");
            types.Add(".jpeg", "image/jpeg");
            types.Add(".jpg", "image/jpeg");
            types.Add(".js", "application/x-javascript");
            types.Add(".jsx", "text/jscript");
            types.Add(".latex", "application/x-latex");
            types.Add(".lit", "application/x-ms-reader");
            types.Add(".lpk", "application/octet-stream");
            types.Add(".lsf", "video/x-la-asf");
            types.Add(".lsx", "video/x-la-asf");
            types.Add(".lzh", "application/octet-stream");
            types.Add(".m13", "application/x-msmediaview");
            types.Add(".m14", "application/x-msmediaview");
            types.Add(".m1v", "video/mpeg");
            types.Add(".m3u", "audio/x-mpegurl");
            types.Add(".man", "application/x-troff-man");
            types.Add(".manifest", "application/x-ms-manifest");
            //types.Add(".map", "text/plain");
            types.Add(".mdb", "application/x-msaccess");
            types.Add(".mdp", "application/octet-stream");
            types.Add(".me", "application/x-troff-me");
            types.Add(".mht", "message/rfc822");
            types.Add(".mhtml", "message/rfc822");
            types.Add(".mid", "audio/mid");
            types.Add(".midi", "audio/mid");
            types.Add(".mix", "application/octet-stream");
            types.Add(".mmf", "application/x-smaf");
            types.Add(".mno", "text/xml");
            types.Add(".mny", "application/x-msmoney");
            types.Add(".mov", "video/quicktime");
            types.Add(".movie", "video/x-sgi-movie");
            types.Add(".mp2", "video/mpeg");
            types.Add(".mp3", "audio/mpeg");
            types.Add(".mpa", "video/mpeg");
            types.Add(".mpe", "video/mpeg");
            types.Add(".mpeg", "video/mpeg");
            types.Add(".mpg", "video/mpeg");
            types.Add(".mpp", "application/vnd.ms-project");
            types.Add(".mpv2", "video/mpeg");
            types.Add(".ms", "application/x-troff-ms");
            types.Add(".msi", "application/octet-stream");
            types.Add(".mso", "application/octet-stream");
            types.Add(".mvb", "application/x-msmediaview");
            types.Add(".mvc", "application/x-miva-compiled");
            types.Add(".nc", "application/x-netcdf");
            types.Add(".nsc", "video/x-ms-asf");
            types.Add(".nws", "message/rfc822");
            types.Add(".ocx", "application/octet-stream");
            types.Add(".oda", "application/oda");
            types.Add(".odc", "text/x-ms-odc");
            types.Add(".ods", "application/oleobject");
            types.Add(".one", "application/onenote");
            types.Add(".onea", "application/onenote");
            types.Add(".onetoc", "application/onenote");
            types.Add(".onetoc2", "application/onenote");
            types.Add(".onetmp", "application/onenote");
            types.Add(".onepkg", "application/onenote");
            types.Add(".osdx", "application/opensearchdescription+xml");
            types.Add(".otf", "application/x-font-opentype");
            types.Add(".p10", "application/pkcs10");
            types.Add(".p12", "application/x-pkcs12");
            types.Add(".p7b", "application/x-pkcs7-certificates");
            types.Add(".p7c", "application/pkcs7-mime");
            types.Add(".p7m", "application/pkcs7-mime");
            types.Add(".p7r", "application/x-pkcs7-certreqresp");
            types.Add(".p7s", "application/pkcs7-signature");
            types.Add(".pbm", "image/x-portable-bitmap");
            types.Add(".pcx", "application/octet-stream");
            types.Add(".pcz", "application/octet-stream");
            types.Add(".pdf", "application/pdf");
            types.Add(".pfb", "application/octet-stream");
            types.Add(".pfm", "application/octet-stream");
            types.Add(".pfx", "application/x-pkcs12");
            types.Add(".pgm", "image/x-portable-graymap");
            types.Add(".pko", "application/vnd.ms-pki.pko");
            types.Add(".pma", "application/x-perfmon");
            types.Add(".pmc", "application/x-perfmon");
            types.Add(".pml", "application/x-perfmon");
            types.Add(".pmr", "application/x-perfmon");
            types.Add(".pmw", "application/x-perfmon");
            types.Add(".png", "image/png");
            types.Add(".pnm", "image/x-portable-anymap");
            types.Add(".pnz", "image/png");
            types.Add(".pot", "application/vnd.ms-powerpoint");
            types.Add(".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12");
            types.Add(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
            types.Add(".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12");
            types.Add(".ppm", "image/x-portable-pixmap");
            types.Add(".pps", "application/vnd.ms-powerpoint");
            types.Add(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12");
            types.Add(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            types.Add(".ppt", "application/vnd.ms-powerpoint");
            types.Add(".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12");
            types.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            types.Add(".prf", "application/pics-rules");
            types.Add(".prm", "application/octet-stream");
            types.Add(".prx", "application/octet-stream");
            types.Add(".ps", "application/postscript");
            types.Add(".psd", "application/octet-stream");
            types.Add(".psm", "application/octet-stream");
            types.Add(".psp", "application/octet-stream");
            types.Add(".pub", "application/x-mspublisher");
            types.Add(".qt", "video/quicktime");
            types.Add(".qtl", "application/x-quicktimeplayer");
            types.Add(".qxd", "application/octet-stream");
            types.Add(".ra", "audio/x-pn-realaudio");
            types.Add(".ram", "audio/x-pn-realaudio");
            types.Add(".rar", "application/octet-stream");
            types.Add(".ras", "image/x-cmu-raster");
            types.Add(".rf", "image/vnd.rn-realflash");
            types.Add(".rgb", "image/x-rgb");
            types.Add(".rm", "application/vnd.rn-realmedia");
            types.Add(".rmi", "audio/mid");
            types.Add(".roff", "application/x-troff");
            types.Add(".rpm", "audio/x-pn-realaudio-plugin");
            types.Add(".rtf", "application/rtf");
            types.Add(".rtx", "text/richtext");
            types.Add(".scd", "application/x-msschedule");
            types.Add(".sct", "text/scriptlet");
            types.Add(".sea", "application/octet-stream");
            types.Add(".setpay", "application/set-payment-initiation");
            types.Add(".setreg", "application/set-registration-initiation");
            types.Add(".sfnt", "application/font-sfnt");
            types.Add(".sgml", "text/sgml");
            types.Add(".sh", "application/x-sh");
            types.Add(".shar", "application/x-shar");
            types.Add(".sit", "application/x-stuffit");
            types.Add(".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12");
            types.Add(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
            types.Add(".smd", "audio/x-smd");
            types.Add(".smi", "application/octet-stream");
            types.Add(".smx", "audio/x-smd");
            types.Add(".smz", "audio/x-smd");
            types.Add(".snd", "audio/basic");
            types.Add(".snp", "application/octet-stream");
            types.Add(".spc", "application/x-pkcs7-certificates");
            types.Add(".spl", "application/futuresplash");
            types.Add(".src", "application/x-wais-source");
            types.Add(".ssm", "application/streamingmedia");
            types.Add(".sst", "application/vnd.ms-pki.certstore");
            types.Add(".stl", "application/vnd.ms-pki.stl");
            types.Add(".sv4cpio", "application/x-sv4cpio");
            types.Add(".sv4crc", "application/x-sv4crc");
            types.Add(".svg", "image/svg+xml");
            types.Add(".swf", "application/x-shockwave-flash");
            types.Add(".t", "application/x-troff");
            types.Add(".tar", "application/x-tar");
            types.Add(".tcl", "application/x-tcl");
            types.Add(".tex", "application/x-tex");
            types.Add(".texi", "application/x-texinfo");
            types.Add(".texinfo", "application/x-texinfo");
            types.Add(".tgz", "application/x-compressed");
            types.Add(".thmx", "application/vnd.ms-officetheme");
            types.Add(".thn", "application/octet-stream");
            types.Add(".tif", "image/tiff");
            types.Add(".tiff", "image/tiff");
            types.Add(".toc", "application/octet-stream");
            types.Add(".tr", "application/x-troff");
            types.Add(".trm", "application/x-msterminal");
            types.Add(".tsv", "text/tab-separated-values");
            types.Add(".ttf", "application/x-font-ttf");
            //types.Add(".txt", "text/plain");
            types.Add(".u32", "application/octet-stream");
            types.Add(".uls", "text/iuls");
            types.Add(".ustar", "application/x-ustar");
            types.Add(".vbs", "text/vbscript");
            types.Add(".vcf", "text/x-vcard");
            //types.Add(".vcs", "text/plain");
            types.Add(".vdx", "application/vnd.ms-visio.viewer");
            types.Add(".vml", "text/xml");
            types.Add(".vsd", "application/vnd.visio");
            types.Add(".vss", "application/vnd.visio");
            types.Add(".vst", "application/vnd.visio");
            types.Add(".vsto", "application/x-ms-vsto");
            types.Add(".vsw", "application/vnd.visio");
            types.Add(".vsx", "application/vnd.visio");
            types.Add(".vtx", "application/vnd.visio");
            types.Add(".wav", "audio/wav");
            types.Add(".wax", "audio/x-ms-wax");
            types.Add(".wbmp", "image/vnd.wap.wbmp");
            types.Add(".wcm", "application/vnd.ms-works");
            types.Add(".wdb", "application/vnd.ms-works");
            types.Add(".wks", "application/vnd.ms-works");
            types.Add(".wm", "video/x-ms-wm");
            types.Add(".wma", "audio/x-ms-wma");
            types.Add(".wmd", "application/x-ms-wmd");
            types.Add(".wmf", "application/x-msmetafile");
            types.Add(".wml", "text/vnd.wap.wml");
            types.Add(".wmlc", "application/vnd.wap.wmlc");
            types.Add(".wmls", "text/vnd.wap.wmlscript");
            types.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
            types.Add(".wmp", "video/x-ms-wmp");
            types.Add(".wmv", "video/x-ms-wmv");
            types.Add(".wmx", "video/x-ms-wmx");
            types.Add(".wmz", "application/x-ms-wmz");
            types.Add(".woff", "application/font-woff");
            types.Add(".woff2", "application/font-woff2");
            types.Add(".wps", "application/vnd.ms-works");
            types.Add(".wri", "application/x-mswrite");
            types.Add(".wrl", "x-world/x-vrml");
            types.Add(".wrz", "x-world/x-vrml");
            types.Add(".wsdl", "text/xml");
            types.Add(".wvx", "video/x-ms-wvx");
            types.Add(".x", "application/directx");
            types.Add(".xaf", "x-world/x-vrml");
            types.Add(".xaml", "application/xaml+xml");
            types.Add(".xap", "application/x-silverlight-app");
            types.Add(".xbap", "application/x-ms-xbap");
            types.Add(".xbm", "image/x-xbitmap");
            //types.Add(".xdr", "text/plain");
            types.Add(".xla", "application/vnd.ms-excel");
            types.Add(".xlam", "application/vnd.ms-excel.addin.macroEnabled.12");
            types.Add(".xlc", "application/vnd.ms-excel");
            types.Add(".xlm", "application/vnd.ms-excel");
            types.Add(".xls", "application/vnd.ms-excel");
            types.Add(".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12");
            types.Add(".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12");
            types.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            types.Add(".xlt", "application/vnd.ms-excel");
            types.Add(".xltm", "application/vnd.ms-excel.template.macroEnabled.12");
            types.Add(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
            types.Add(".xlw", "application/vnd.ms-excel");
            types.Add(".xml", "text/xml");
            types.Add(".xof", "x-world/x-vrml");
            types.Add(".xpm", "image/x-xpixmap");
            types.Add(".xps", "application/vnd.ms-xpsdocument");
            types.Add(".xsd", "text/xml");
            types.Add(".xsf", "text/xml");
            types.Add(".xsl", "text/xml");
            types.Add(".xslt", "text/xml");
            types.Add(".xsn", "application/octet-stream");
            types.Add(".xtp", "application/octet-stream");
            types.Add(".xwd", "image/x-xwindowdump");
            types.Add(".z", "application/x-compress");
            types.Add(".zip", "application/x-zip-compressed");
        }
    }
}
