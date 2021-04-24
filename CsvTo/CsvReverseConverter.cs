using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvReverseConverter
    {
        string _filePath = null;
        Stream _fileStream;
        bool _hasHeader;
        string _delimiter;
        string _escape;

        public CsvReverseConverter(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public CsvReverseConverter(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
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

        DataTable ToDataTableFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            DataTable dt = new DataTable();
            var er = handler.GetEnumerator();
            if (_hasHeader)
            {
                var tmpQueue = new Queue<string[]>();
                if (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    dt.Columns.AddRange(elements.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                    }

                }
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                            dt.Rows.Add(tmpQueue.Dequeue());
                    }
                }
                var header = tmpQueue.Dequeue();
                for (int i = 0; i < header.Length; i++)
                {
                    dt.Columns[$"column{i}"].ColumnName = header[i];
                }
            }
            else
            {
                if (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    dt.Columns.AddRange(elements.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                        dt.Rows.Add(elements);
                }
            }
            return dt;
        }
        DataTable ToDataTableFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream, _delimiter, _escape);
            DataTable dt = new DataTable();
            var er = handler.GetEnumerator();
            if (_hasHeader)
            {
                var tmpQueue = new Queue<string[]>();
                if (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    dt.Columns.AddRange(elements.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                    }

                }
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                            dt.Rows.Add(tmpQueue.Dequeue());
                    }
                }
                var header = tmpQueue.Dequeue();
                for (int i = 0; i < header.Length; i++)
                {
                    dt.Columns[$"column{i}"].ColumnName = header[i];
                }
            }
            else
            {
                if (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    dt.Columns.AddRange(elements.Select((f, i) => new DataColumn($"column{i}")).ToArray());
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                        dt.Rows.Add(elements);
                }
            }
            return dt;
        }

        IEnumerable<string[]> ToCollectionFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            if (!_hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = handler.Parser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        yield return elements;
                    }
                }
            }
            else
            {
                var tmpQueue = new Queue<string[]>();
                var er = handler.GetEnumerator();
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                            yield return tmpQueue.Dequeue();
                    }
                }
            }
        }
        IEnumerable<string[]> ToCollectionFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream,_delimiter, _escape);
            if (!_hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = handler.Parser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        yield return elements;
                    }
                }
            }
            else
            {
                var tmpQueue = new Queue<string[]>();
                var er = handler.GetEnumerator();
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                            yield return tmpQueue.Dequeue();
                    }
                }
            }
        }
    }
}
