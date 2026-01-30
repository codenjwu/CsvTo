using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
        Encoding _encoding;
        
        public CsvConverter(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"", Encoding encoding = null)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
            _encoding = encoding ?? Encoding.UTF8;
        }
        public CsvConverter(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"", Encoding encoding = null)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
            _encoding = encoding ?? Encoding.UTF8;
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
            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape, _encoding);
            return new CsvConvertHandler().ToDataTableHandler(csvHandler,_hasHeader);
        }
        DataTable ToDataTableFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape, _encoding);
            return new CsvConvertHandler().ToDataTableHandler(csvHandler, _hasHeader);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        IEnumerable<string[]> ToCollectionFromFile()
        {
            CsvHandler csvHandler = new CsvHandler(_filePath, _delimiter, _escape, _encoding);
            return new CsvConvertHandler().ToCollectionHandler(csvHandler,_hasHeader);
        }
        IEnumerable<string[]> ToCollectionFromStream()
        {
            CsvHandler csvHandler = new CsvHandler(_fileStream, _delimiter, _escape, _encoding);
            return new CsvConvertHandler().ToCollectionHandler(csvHandler, _hasHeader);
        }
        
    }
}
