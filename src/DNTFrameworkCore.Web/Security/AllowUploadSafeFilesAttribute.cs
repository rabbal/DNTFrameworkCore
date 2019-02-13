using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Security
{
       /// <summary>
    /// Disallows uploading dangerous files such as .aspx, web.config and .asp files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AllowUploadSafeFilesAttribute : ValidationAttribute
    {
        private readonly ISet<string> _extentionsToFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            ".aspx", ".asax", ".asp", ".ashx", ".asmx", ".axd", ".master", ".svc", ".php" ,
            ".php3" , ".php4", ".ph3", ".ph4", ".php4", ".ph5", ".sphp", ".cfm", ".ps", ".stm",
            ".htaccess", ".htpasswd", ".php5", ".phtml", ".cgi", ".pl", ".plx", ".py", ".rb", ".sh", ".jsp",
            ".cshtml", ".vbhtml", ".swf" , ".xap", ".asptxt"
        };

        private readonly ISet<string> _namesToFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
           "web.config" , "htaccess" , "htpasswd", "web~1.con", "desktop.ini"
        };

        /// <summary>
        /// Disallows uploading dangerous files such as .aspx, web.config and .asp files.
        /// </summary>
        /// <param name="extentionsToFilter">Disallowed file extensions such as .asp</param>
        /// <param name="namesToFilter">Disallowed names such as web.config</param>
        public AllowUploadSafeFilesAttribute(
            string[] extentionsToFilter = null,
            string[] namesToFilter = null)
        {
            if (extentionsToFilter != null)
            {
                foreach (var item in extentionsToFilter)
                {
                    _extentionsToFilter.Add(item);
                }
            }

            if (namesToFilter != null)
            {
                foreach (var item in namesToFilter)
                {
                    _namesToFilter.Add(item);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true; // returning false, makes this field required.
            }

            if (value is IFormFile file)
            {
                return IsValidFile(file);
            }

            if (!(value is IList<IFormFile> files))
            {
                return false;
            }

            foreach (var postedFile in files)
            {
                if (!IsValidFile(postedFile)) return false;
            }

            return true;
        }

        private bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return true; // returning false, makes this field required.
            }

            var fileName = file.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            fileName = fileName.ToLowerInvariant();
            var name = Path.GetFileName(fileName);
            var ext = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return !_extentionsToFilter.Contains(ext) &&
                   !_namesToFilter.Contains(name) &&
                   !_namesToFilter.Contains(ext) &&
                   //for "file.asp;.jpg" files --> run as an ASP file
                   _extentionsToFilter.All(item => !name.Contains(item));
        }
    }
}