using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvConverter
    {
        string _filePath=null;
        Stream _fileStream;
        bool _hasHeader;
        string _delimiter;
        string _escape;
        public CsvConverter(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public CsvConverter(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public DataTable ToDataTable()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToDataTableFromFile();
            return ToDataTableFromStream();
        }
        public IEnumerable<string[]> ToCollection()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToCollectionFromFile();
            return ToCollectionFromStream();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        DataTable ToDataTableFromFile()
        {

            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape);
            DataTable dt = new DataTable();
            var er = csvHandler.GetEnumerator();
            if (_hasHeader)
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
        DataTable ToDataTableFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape);
            DataTable dt = new DataTable();
            var er = csvHandler.GetEnumerator();
            if (_hasHeader)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        IEnumerable<string[]> ToCollectionFromFile()
        {
            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape);
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            if (_hasHeader)
            {
                er.MoveNext();
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    yield return elements;
            }
        }
        IEnumerable<string[]> ToCollectionFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape);
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            if (_hasHeader)
            {
                er.MoveNext();
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    yield return elements;
            }
        }
    }
}
