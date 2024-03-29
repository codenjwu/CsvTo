﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvConverter<T> where T : class, new()
    {
        string _filePath = null;
        Stream _fileStream;
        bool _hasHeader;
        string _delimiter;
        string _escape;
        static Type _type;
        // cache property that is not complex type and does not have a csvignore attribute
        static Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> _props;
        static Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> _dtprops;
        static CsvConverter()
        {
            _type = typeof(T);
            _props = RefHelper.GetProperties(_type);
            _dtprops = RefHelper.GetPropertiesForDataTable(_type);
        }
        public CsvConverter(string filePath, bool hasHeader = true, string delimiter = ",", string escape = "\"")
        {
            if (!hasHeader)
                throw new FormatException("Generic converter requests a header for csv file!");
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public CsvConverter(Stream fileStream, bool hasHeader = true, string delimiter = ",", string escape = "\"")
        {
            if (!hasHeader)
                throw new FormatException("Generic converter requests a header for csv file!");
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
        public IEnumerable<T> ToCollection()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToCollectionFromFile();
            return ToCollectionFromStream();
        }

        DataTable ToDataTableFromFile()
        {
            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape);

            return new CsvConvertHandler().ToDataTableHandler(csvHandler,_dtprops);
        }
        DataTable ToDataTableFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape);

            return new CsvConvertHandler().ToDataTableHandler(csvHandler, _dtprops);
        }

        IEnumerable<T> ToCollectionFromFile()
        {
            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape);
            return new CsvConvertHandler().ToCollectionHandler<T>(csvHandler, _props);
        }
        IEnumerable<T> ToCollectionFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape);
            return new CsvConvertHandler().ToCollectionHandler<T>(csvHandler, _props);
        }
    }
}
