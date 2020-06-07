using System;

namespace CsvTo
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvColumnIndexAttribute : Attribute
    {
        public int ColumnIndex { get; private set; }
        public CsvColumnIndexAttribute(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }
    }
}
