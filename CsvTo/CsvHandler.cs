using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvTo
{
    internal class CsvHandler : IEnumerable<string>
    {
        string _filePath = null;
        Stream _fileStream;
        internal Parser Parser;

        public CsvHandler(string filePath, string delimiter = ",", string escape = "\"")
        {
            _filePath = filePath;
            Parser = new Parser(delimiter, escape);
        }

        public CsvHandler(Stream fileStream, string delimiter = ",", string escape = "\"")
        {
            _fileStream = fileStream;
            Parser = new Parser(delimiter, escape);
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return FileHandler(_filePath);
            return StreamHandler(_fileStream);
        }

        internal string FirstLine()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return FirstLineFromFile(_filePath);
            return FirstLineFromStream(_fileStream);
        }

        string FirstLineFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var l = reader.ReadLine();
                    var ecount = Parser.EscapeCount(l);
                    // this is a  new csv line
                    if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                        || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                        || (ecount % 2 != 0 && sb.Length > 0))  //a,bb,c"\r\n  
                    {
                        sb.Append(l);
                        break; // break after reading the first line
                    }
                    // this is not a new csv line need to concat
                    else // "a \r\n
                    {
                        sb.Append(l + Environment.NewLine);
                    }
                }
                return sb.ToString();
            }
        }
        string FirstLineFromStream(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                var sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var l = reader.ReadLine();
                    var ecount = Parser.EscapeCount(l);
                    // this is a  new csv line
                    if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                        || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                        || (ecount % 2 != 0 && sb.Length > 0))  //a,bb,c"\r\n  
                    {
                        sb.Append(l);
                        break; // break after reading the first line
                    }
                    // this is not a new csv line need to concat
                    else // "a \r\n
                    {
                        sb.Append(l + Environment.NewLine);
                    }
                }
                return sb.ToString();
            }
        }

        IEnumerator<string> FileHandler(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var l = reader.ReadLine();
                    var ecount = Parser.EscapeCount(l);
                    // this is a  new csv line
                    if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                        || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                        || (ecount % 2 != 0 && sb.Length > 0))  //a,bb,c"\r\n  
                    {
                        sb.Append(l);
                        yield return sb.ToString();
                        sb.Clear(); //clear sb for next loop
                    }
                    // this is not a new csv line need to concat
                    else // "a \r\n
                    {
                        sb.Append(l + Environment.NewLine);
                    }
                }
            }
        }
        IEnumerator<string> StreamHandler(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                var sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var l = reader.ReadLine();
                    var ecount = Parser.EscapeCount(l);
                    // this is a  new csv line
                    if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                        || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                        || (ecount % 2 != 0 && sb.Length > 0))  //a,bb,c"\r\n  
                    {
                        sb.Append(l);
                        yield return sb.ToString();
                        sb.Clear(); //clear sb for next loop
                    }
                    // this is not a new csv line need to concat
                    else // "a \r\n
                    {
                        sb.Append(l + Environment.NewLine);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}