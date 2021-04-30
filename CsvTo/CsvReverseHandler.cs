using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvTo
{
    internal class CsvReverseHandler: IEnumerable<string>
    {
        string _filePath = null;
        Stream _fileStream;
        internal Parser Parser;

        public CsvReverseHandler(string filePath, string delimiter = ",", string escape = "\"")
        {
            _filePath = filePath;
            Parser = new Parser(delimiter, escape);
        }
        public CsvReverseHandler(Stream fileStream, string delimiter = ",", string escape = "\"")
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
        string FirstLineFromStream(Stream fileStraem)
        {
            using (var reader = new StreamReader(fileStraem))
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
            var sb = new StringBuilder();
            var lines = ReadLine(filePath);
            foreach (var l in lines)
            {
                var ecount = Parser.EscapeCount(l);
                // this is a  new csv line
                if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                    || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                    || (ecount % 2 != 0 && sb.Length > 0))  // "a,bb,c\r\n  
                {
                    sb.Insert(0, l + Environment.NewLine);
                    yield return sb.ToString().TrimEnd();
                    sb.Clear(); //clear sb for next loop
                }
                // this is not a new csv line need to concat
                else // a" \r\n
                {
                    sb.Insert(0, l + Environment.NewLine);
                }
            }
        }
        IEnumerator<string> StreamHandler(Stream fileStream)
        {
            var sb = new StringBuilder();
            var lines = ReadLine(fileStream);
            foreach (var l in lines)
            {
                var ecount = Parser.EscapeCount(l);
                // this is a  new csv line
                if ((ecount == 0 && sb.Length == 0)  // aa,bb,cc\r\n
                    || (ecount != 0 && ecount % 2 == 0 && sb.Length == 0)  // "a","""",c\r\n
                    || (ecount % 2 != 0 && sb.Length > 0))  // "a,bb,c\r\n  
                {
                    sb.Insert(0, l + Environment.NewLine);
                    yield return sb.ToString().TrimEnd();
                    sb.Clear(); //clear sb for next loop
                }
                // this is not a new csv line need to concat
                else // a" \r\n
                {
                    sb.Insert(0, l + Environment.NewLine);
                }
            }
        }
        IEnumerable<string> ReadLine(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
                var currentPosition = lastPosition;

                var bufSize = 1024;
                var buf = new char[1024];
                StringBuilder sb = new StringBuilder();
                do
                {
                    if (currentPosition >= bufSize)
                    {
                        reader.DiscardBufferedData();
                        currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
                        reader.Read(buf, 0, bufSize);
                        currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
                        for (int i = buf.Length - 1; i >= 0; i--)
                        {
                            if (buf[i] == '\r' || buf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else
                            {
                                sb.Insert(0, buf[i]);
                            }
                        }
                    }
                    else
                    {
                        reader.DiscardBufferedData();
                        var maxRead = currentPosition;
                        var newBuf = new char[maxRead];
                        currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
                        reader.Read(newBuf, 0, (int)maxRead);
                        for (int i = newBuf.Length - 1; i >= 0; i--)
                        {
                            if (newBuf[i] == '\r' || newBuf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else if (i == 0)
                            {
                                sb.Insert(0, newBuf[i]);
                                yield return sb.ToString();
                                sb.Clear();
                            }
                            else
                            {
                                sb.Insert(0, newBuf[i]);
                            }
                        }
                        currentPosition = -1;
                    }
                } while (currentPosition > 0);
            }
        }
        IEnumerable<string> ReadLine(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
                var currentPosition = lastPosition;

                var bufSize = 1024;
                var buf = new char[1024];
                StringBuilder sb = new StringBuilder();
                do
                {
                    if (currentPosition >= bufSize)
                    {
                        reader.DiscardBufferedData();
                        currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
                        reader.Read(buf, 0, bufSize);
                        currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
                        for (int i = buf.Length - 1; i >= 0; i--)
                        {
                            if (buf[i] == '\r' || buf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else
                            {
                                sb.Insert(0, buf[i]);
                            }
                        }
                    }
                    else
                    {
                        reader.DiscardBufferedData();
                        var maxRead = currentPosition;
                        var newBuf = new char[maxRead];
                        currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
                        reader.Read(newBuf, 0, (int)maxRead);
                        for (int i = newBuf.Length - 1; i >= 0; i--)
                        {
                            if (newBuf[i] == '\r' || newBuf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else if (i == 0)
                            {
                                sb.Insert(0, newBuf[i]);
                                yield return sb.ToString();
                                sb.Clear();
                            }
                            else
                            {
                                sb.Insert(0, newBuf[i]);
                            }
                        }
                        currentPosition = -1;
                    }
                } while (currentPosition > 0);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}