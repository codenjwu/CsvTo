using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CsvTo
{
    internal class CsvConvertHandler
    {
        internal DataTable ToDataTableHandler(CsvHandler csvHandler, bool hasHeader)
        {
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
        internal IEnumerable<string[]> ToCollectionHandler(CsvHandler csvHandler, bool hasHeader)
        {
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            if (hasHeader)
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

        internal DataTable ToDataTableHandler(CsvHandler csvHandler, Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> props)
        {
            DataTable dt = new DataTable();
            var er = csvHandler.GetEnumerator();
            var ignoreIndex = new List<int>();
            if (er.MoveNext())
            {
                var firstLine = csvHandler.Parser.Split(er.Current);
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
            }

            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                {
                    var data = new object[dt.Columns.Count];
                    foreach (var item in props)
                    {
                        if (dt.Columns.IndexOf(item.Key) >= 0)
                            try
                            {
                                var val = elements[item.Value.index];
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
                    dt.Rows.Add(data);
                }
            }
            return dt;
        }
        internal IEnumerable<T> ToCollectionHandler<T>(CsvHandler csvHandler, Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> props)
        {
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            var ignoreIndex = new List<int>();
            var indexTypeMapping = new Dictionary<int, string>();
            if (er.MoveNext())
            {
                var firstLine = csvHandler.Parser.Split(er.Current);
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
                            indexTypeMapping.Add(i, firstLine[i]);
                        }
                    }
                }
                else
                    throw new FormatException("csv header should not be empty");
            }
            while (er.MoveNext())
            {
                var obj = Activator.CreateInstance<T>();
                var elements = csvHandler.Parser.Split(er.Current);
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
                    yield return obj;
                }
            }
        }
    }
}
