using System.Collections;
using System.IO;
using System.Text;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.Results
{
    public class CsvFileResult : FileContentResult
    {
        public CsvFileResult(IEnumerable data, string fileName) : base(GetData(data), "text/csv")
        {
            Guard.ArgumentNotEmpty(fileName, nameof(fileName));

            FileDownloadName = fileName;
        }

        private static byte[] GetData(IEnumerable data)
        {
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                foreach (var item in data)
                {
                    var properties = item.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        writer.Write(prop.GetValue(item).ToString() ?? "");
                        writer.Write(", ");
                    }

                    writer.WriteLine();
                }
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}