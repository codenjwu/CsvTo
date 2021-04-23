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
        internal IEnumerator<string> FileHandler(string filePath)
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
        internal IEnumerator<string> StreamHandler(Stream fileStream)
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
        private IEnumerable<string> ReadLine(string path)
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
        private IEnumerable<string> ReadLine(Stream fileStream)
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
        //private string _filePath;
        //private Stream _fileStream;

        //public CsvReverseHandler(string filePath)
        //{
        //    _filePath = filePath;
        //}
        //public CsvReverseHandler(Stream fileStream)
        //{
        //    _fileStream = fileStream;
        //}
        //public IEnumerator<string> GetEnumerator()
        //{
        //    //yield return stk.Pop(); 
        //    if(!string.IsNullOrWhiteSpace(_filePath))
        //        return ReverseHandler(_filePath);
        //    return ReverseHandler(_fileStream);
        //}
        ////Stack<string> stk = new Stack<string>();
        //private IEnumerator<string> ReverseHandler(string filePath)
        //{
        //    using (var reader = new StreamReader(filePath))
        //    {
        //        var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
        //        var currentPosition = lastPosition;

        //        var bufSize = 64;
        //        var buf = new char[64];
        //        StringBuilder sb = new StringBuilder();
        //        do
        //        {
        //            if (currentPosition < bufSize)
        //            {
        //                reader.DiscardBufferedData();
        //                var maxRead = currentPosition;
        //                var newBuf = new char[maxRead];
        //                currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
        //                reader.ReadAsync(newBuf, 0, (int)maxRead).GetAwaiter().GetResult();
        //                for (int i = newBuf.Length - 1; i >= 0; i--)
        //                {
        //                    if (newBuf[i] == '\r' || newBuf[i] == '\n')
        //                    {
        //                        if (sb.Length != 0)
        //                        {
        //                            //stk.Push(sb.ToString());
        //                            //sb.Clear();
        //                            yield return sb.ToString();
        //                            sb.Clear();
        //                        }
        //                    }
        //                    else if (i == 0)
        //                    {
        //                        sb.Insert(0, newBuf[i]);
        //                        //stk.Push(sb.ToString());
        //                        //sb.Clear();
        //                        yield return sb.ToString();
        //                        sb.Clear();
        //                    }
        //                    else
        //                    {
        //                        sb.Insert(0, newBuf[i]);
        //                    }
        //                }
        //                currentPosition = -1;
        //            }
        //            else
        //            {
        //                reader.DiscardBufferedData();
        //                currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
        //                reader.ReadAsync(buf, 0, bufSize).GetAwaiter().GetResult();
        //                currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
        //                for (int i = buf.Length - 1; i >= 0; i--)
        //                {
        //                    if (buf[i] == '\r' || buf[i] == '\n')
        //                    {
        //                        if (sb.Length != 0)
        //                        {
        //                            //stk.Push(sb.ToString());
        //                            yield return sb.ToString();
        //                            sb.Clear();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sb.Insert(0, buf[i]);
        //                    }
        //                }
        //            }
        //        } while (currentPosition > 0);
        //    }
        //}
        //private IEnumerator<string> ReverseHandler(Stream fileStream)
        //{
        //    using (var reader = new StreamReader(fileStream))
        //    {
        //        var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
        //        var currentPosition = lastPosition;

        //        var bufSize = 64;
        //        var buf = new char[64];
        //        StringBuilder sb = new StringBuilder();
        //        do
        //        {
        //            if (currentPosition < bufSize)
        //            {
        //                reader.DiscardBufferedData();
        //                var maxRead = currentPosition;
        //                var newBuf = new char[maxRead];
        //                currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
        //                reader.ReadAsync(newBuf, 0, (int)maxRead).GetAwaiter().GetResult();
        //                for (int i = newBuf.Length - 1; i >= 0; i--)
        //                {
        //                    if (newBuf[i] == '\r' || newBuf[i] == '\n')
        //                    {
        //                        if (sb.Length != 0)
        //                        {
        //                            //stk.Push(sb.ToString());
        //                            //sb.Clear();
        //                            yield return sb.ToString();
        //                            sb.Clear();
        //                        }
        //                    }
        //                    else if (i == 0)
        //                    {
        //                        sb.Insert(0, newBuf[i]);
        //                        //stk.Push(sb.ToString());
        //                        //sb.Clear();
        //                        yield return sb.ToString();
        //                        sb.Clear();
        //                    }
        //                    else
        //                    {
        //                        sb.Insert(0, newBuf[i]);
        //                    }
        //                }
        //                currentPosition = -1;
        //            }
        //            else
        //            {
        //                reader.DiscardBufferedData();
        //                currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
        //                reader.ReadAsync(buf, 0, bufSize).GetAwaiter().GetResult();
        //                currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
        //                for (int i = buf.Length - 1; i >= 0; i--)
        //                {
        //                    if (buf[i] == '\r' || buf[i] == '\n')
        //                    {
        //                        if (sb.Length != 0)
        //                        {
        //                            //stk.Push(sb.ToString());
        //                            yield return sb.ToString();
        //                            sb.Clear();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sb.Insert(0, buf[i]);
        //                    }
        //                }
        //            }
        //        } while (currentPosition > 0);
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }
}