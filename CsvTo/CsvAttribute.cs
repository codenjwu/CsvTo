using System;

namespace CsvTo
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvColumnAttribute : Attribute
    {
        public string Column { get; private set; }
        public CsvColumnAttribute(string Column)
        {
            this.Column = Column;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvIgnoreAttribute : Attribute
    {
    }
}
