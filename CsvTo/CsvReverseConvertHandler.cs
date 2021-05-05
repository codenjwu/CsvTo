using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CsvTo
{
    public class CsvReverseConvertHandler
    {
        internal DataTable ToDataTableHandler(CsvReverseHandler handler, bool hasHeader)
        {
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
        internal IEnumerable<string[]> ToCollectionHandler(CsvReverseHandler handler, bool hasHeader)
        {
            if (!hasHeader)
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
        internal DataTable ToDataTableHandler(CsvReverseHandler handler, Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> props)
        {
            DataTable dt = new DataTable();
            var columns = handler.FirstLine();
            var ignoreIndex = new List<int>();

            var firstLine = handler.Parser.Split(columns);
            if (!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
            {
                for (int i = 0; i < firstLine.Length; i++)
                {
                    firstLine[i] = firstLine[i].Trim().ToUpper();
                    if (!props.TryGetValue(firstLine[i], out (int index, Type ty, PropertyDescriptor pd) value))
                        ignoreIndex.Add(i);
                    else
                    {
                        props[firstLine[i]] = (i, props[firstLine[i]].ty, props[firstLine[i]].pd);
                        dt.Columns.Add(new DataColumn(firstLine[i], value.ty));
                    }
                }
            }
            else
                throw new FormatException("csv header should not be empty");

            var er = handler.GetEnumerator();

            var tmpQueue = new Queue<object[]>();

            while (er.MoveNext())
            {
                var ele = handler.Parser.Split(er.Current);
                if (!ele.All(e => string.IsNullOrWhiteSpace(e)))
                {
                    var data = new object[dt.Columns.Count];
                    foreach (var item in props)
                    {
                        if (dt.Columns.IndexOf(item.Key) >= 0)
                            try
                            {
                                var val = ele[item.Value.index];
                                if (string.IsNullOrWhiteSpace(val) && item.Value.ty != typeof(string))
                                    data[dt.Columns[item.Key].Ordinal] = DBNull.Value;
                                else
                                    data[dt.Columns[item.Key].Ordinal] = RefHelper.ConvertFromString(item.Value.ty, val);
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                var df = RefHelper.GetDefaultValue(item.Value.pd.PropertyType);
                                data[dt.Columns[item.Key].Ordinal] = df != null ? df : DBNull.Value;
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                    }
                    tmpQueue.Enqueue(data);
                    if (tmpQueue.Count > 1)
                        dt.Rows.Add(tmpQueue.Dequeue());
                }
            }
            var header = tmpQueue.Dequeue();

            return dt;
        }
        internal IEnumerable<T> ToCollectionHandler<T>(CsvReverseHandler handler, Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> props)
        {

            var columns = handler.FirstLine();
            var ignoreIndex = new List<int>();
            var indexTypeMapping = new Dictionary<int, string>();
            var firstLine = handler.Parser.Split(columns);
            if (!firstLine.All(e => string.IsNullOrWhiteSpace(e)))
            {
                for (int i = 0; i < firstLine.Length; i++)
                {
                    firstLine[i] = firstLine[i].Trim().ToUpper();
                    if (!props.TryGetValue(firstLine[i], out (int index, Type ty, PropertyDescriptor pd) value))
                        ignoreIndex.Add(i);
                    else
                    {
                        props[firstLine[i]] = (i, props[firstLine[i]].ty,props[firstLine[i]].pd);
                        indexTypeMapping.Add(i, firstLine[i]);
                    }
                }
            }
            else
                throw new FormatException("csv header should not be empty");

            var tmpQueue = new Queue<T>();
            var er = handler.GetEnumerator();
            while (er.MoveNext())
            {
                var obj = Activator.CreateInstance<T>();
                var elements = handler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                {
                    foreach (var item in indexTypeMapping)
                    {
                        try
                        {
                            var pValue = elements[item.Key];
                            var p = props.FirstOrDefault(pt => pt.Key.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                            p.Value.pd.SetValue(obj, RefHelper.ConvertFromString(p.Value.ty, pValue));
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            var pValue = elements[item.Key];
                            var p = props.FirstOrDefault(pt => pt.Key.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                            p.Value.pd.SetValue(obj, RefHelper.GetDefaultValue(p.Value.ty));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    tmpQueue.Enqueue(obj);
                    if (tmpQueue.Count > 1)
                        yield return tmpQueue.Dequeue();
                }
            }
        }
    }
}
