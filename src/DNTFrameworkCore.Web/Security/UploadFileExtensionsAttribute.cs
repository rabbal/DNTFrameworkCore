using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Security
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2555
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class UploadFileExtensionsAttribute : ValidationAttribute
    {
        private readonly IList<string> _allowedExtensions;

        /// <summary>
        /// Allowing only selected file extensions are safe to be uploaded.
        /// </summary>
        /// <param name="fileExtensions">Allowed files extensions to be uploaded</param>
        public UploadFileExtensionsAttribute(string fileExtensions)
        {
            _allowedExtensions = fileExtensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
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

            var fileExtension = Path.GetExtension(file.FileName);
            return !string.IsNullOrWhiteSpace(fileExtension) &&
                   _allowedExtensions.Any(ext => fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}