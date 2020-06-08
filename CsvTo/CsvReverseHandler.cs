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
        private string _filePath;
        private Stream _fileStream;

        public CsvReverseHandler(string filePath)
        {
            _filePath = filePath;
        }
        public CsvReverseHandler(Stream fileStream)
        {
            _fileStream = fileStream;
        }
        public IEnumerator<string> GetEnumerator()
        {
            //yield return stk.Pop(); 
            if(string.IsNullOrWhiteSpace(_filePath))
             return ReverseHandler(_filePath);
            return ReverseHandler(_fileStream);
        }
        //Stack<string> stk = new Stack<string>();
        private IEnumerator<string> ReverseHandler(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
                var currentPosition = lastPosition;

                var bufSize = 64;
                var buf = new char[64];
                StringBuilder sb = new StringBuilder();
                do
                {
                    if (currentPosition < bufSize)
                    {
                        reader.DiscardBufferedData();
                        var maxRead = currentPosition;
                        var newBuf = new char[maxRead];
                        currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
                        reader.ReadAsync(newBuf, 0, (int)maxRead).GetAwaiter().GetResult();
                        for (int i = newBuf.Length - 1; i >= 0; i--)
                        {
                            if (newBuf[i] == '\r' || newBuf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    //stk.Push(sb.ToString());
                                    //sb.Clear();
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else if (i == 0)
                            {
                                sb.Insert(0, newBuf[i]);
                                //stk.Push(sb.ToString());
                                //sb.Clear();
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
                    else
                    {
                        reader.DiscardBufferedData();
                        currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
                        reader.ReadAsync(buf, 0, bufSize).GetAwaiter().GetResult();
                        currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
                        for (int i = buf.Length - 1; i >= 0; i--)
                        {
                            if (buf[i] == '\r' || buf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    //stk.Push(sb.ToString());
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
                } while (currentPosition > 0);
            }
        }
        private IEnumerator<string> ReverseHandler(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                var lastPosition = reader.BaseStream.Seek(0, SeekOrigin.End);
                var currentPosition = lastPosition;

                var bufSize = 64;
                var buf = new char[64];
                StringBuilder sb = new StringBuilder();
                do
                {
                    if (currentPosition < bufSize)
                    {
                        reader.DiscardBufferedData();
                        var maxRead = currentPosition;
                        var newBuf = new char[maxRead];
                        currentPosition = reader.BaseStream.Seek(0 - currentPosition, SeekOrigin.Current);
                        reader.ReadAsync(newBuf, 0, (int)maxRead).GetAwaiter().GetResult();
                        for (int i = newBuf.Length - 1; i >= 0; i--)
                        {
                            if (newBuf[i] == '\r' || newBuf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    //stk.Push(sb.ToString());
                                    //sb.Clear();
                                    yield return sb.ToString();
                                    sb.Clear();
                                }
                            }
                            else if (i == 0)
                            {
                                sb.Insert(0, newBuf[i]);
                                //stk.Push(sb.ToString());
                                //sb.Clear();
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
                    else
                    {
                        reader.DiscardBufferedData();
                        currentPosition = reader.BaseStream.Seek(0 - bufSize, SeekOrigin.Current);
                        reader.ReadAsync(buf, 0, bufSize).GetAwaiter().GetResult();
                        currentPosition = reader.BaseStream.Seek(currentPosition - lastPosition, SeekOrigin.End);
                        for (int i = buf.Length - 1; i >= 0; i--)
                        {
                            if (buf[i] == '\r' || buf[i] == '\n')
                            {
                                if (sb.Length != 0)
                                {
                                    //stk.Push(sb.ToString());
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
                } while (currentPosition > 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}