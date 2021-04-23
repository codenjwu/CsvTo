using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvToDatatableReverse
    {
        public DataTable ConvertFromFile(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvReverseHandler handler = new CsvReverseHandler(filePath, delimiter, escape);
            DataTable dt = new DataTable();
            var er = handler.GetEnumerator();
            if (hasHeader)
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
        public DataTable ConvertFromStream(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvReverseHandler handler = new CsvReverseHandler(fileStream, delimiter, escape);
            DataTable dt = new DataTable();
            var er = handler.GetEnumerator();
            if (hasHeader)
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
    }
}
