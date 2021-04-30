using System;
using System.Collections.Generic;
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

        internal DataTable ToDataTableHandler(CsvHandler csvHandler, Dictionary<string, (int index, Type ty)> props)
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
                        if (!props.TryGetValue(firstLine[i].Trim().ToUpper(), out (int index, Type ty) value))
                            ignoreIndex.Add(i);
                        else
                        {
                            props[firstLine[i]] = (i, props[firstLine[i]].ty);
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
                        try
                        {
                            data[dt.Columns[item.Key].Ordinal] = RefHelper.ConvertFromString(item.Value.ty, elements[item.Value.index]);
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            data[dt.Columns[item.Key].Ordinal] = RefHelper.GetDefaultValue(item.Value.ty);
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
        internal IEnumerable<T> ToCollectionHandler<T>(CsvHandler csvHandler, Dictionary<string, (int index, Type ty)> props, PropertyInfo[] ps)
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
                        if (!props.TryGetValue(firstLine[i].Trim().ToUpper(), out (int index, Type ty) value))
                            ignoreIndex.Add(i);
                        else
                        {
                            props[firstLine[i]] = (i, props[firstLine[i]].ty);
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
                            var p = ps.FirstOrDefault(p => p.Name.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                            p.SetValue(obj, RefHelper.ConvertFromString(p.PropertyType, pValue));
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            var pValue = elements[item.Key];
                            var p = ps.FirstOrDefault(p => p.Name.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                            p.SetValue(obj, RefHelper.GetDefaultValue(props[item.Value].ty));
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
