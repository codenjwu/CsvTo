using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvToDatatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public DataTable ConvertFromFile(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvHandler csvHandler = new CsvHandler(filePath, delimiter, escape);
            DataTable dt = new DataTable();
            var er = csvHandler.GetEnumerator();
            if (hasHeader)
            {
                if (er.MoveNext())
                {
                    var firstLine = csvHandler.Parser.Split(er.Current);
                    if (!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Columns.AddRange(firstLine.Select((f, i) => new DataColumn(f)).ToArray());
                    }
                    else
                        throw new FormatException("csv header should not be empty");
                }
            }
            else
            {
                if (er.MoveNext())
                {
                    var firstLine = csvHandler.Parser.Split(er.Current);
                    dt.Columns.AddRange(firstLine.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if(!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
                        dt.Rows.Add(firstLine);
                }
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    dt.Rows.Add(elements);
            }
            return dt;
        }
        public DataTable ConvertFromStream(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvHandler csvHandler = new CsvHandler(fileStream, delimiter, escape);
            DataTable dt = new DataTable();
            var er = csvHandler.GetEnumerator();
            if (hasHeader)
            {
                if (er.MoveNext())
                {
                    var firstLine = csvHandler.Parser.Split(er.Current);
                    if (!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Columns.AddRange(firstLine.Select((f, i) => new DataColumn(f)).ToArray());
                    }
                    else
                        throw new FormatException("csv header should not be empty");
                }
            }
            else
            {
                if (er.MoveNext())
                {
                    var firstLine = csvHandler.Parser.Split(er.Current);
                    dt.Columns.AddRange(firstLine.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if (!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
                        dt.Rows.Add(firstLine);
                }
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    dt.Rows.Add(elements);
            }
            return dt;
        }
    }
}
