using System;

namespace CsvTo
{
    internal static class TypeConverter
    {
        internal static object ConvertType(string value, Type ty)
        {
            return string.IsNullOrWhiteSpace(value) ? GetDefaultValue(ty) : Convert.ChangeType(value, ty);
        }
        internal static object GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            return null;
        }
    }
}
