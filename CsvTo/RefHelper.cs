using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace CsvTo
{
    internal class RefHelper
    {
        internal static bool IsNotComplexType(PropertyDescriptor property)
            => property.PropertyType == typeof(string) || property.PropertyType.IsValueType;

        internal static Type GetPropType(PropertyDescriptor property)
            => Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        // get properties that is not complex type and does not have a csvignore attribute
        internal static Dictionary<string, (int index, Type ty)> GetProperties(Type type)
        {
            Dictionary<string, (int index, Type ty)> props = new Dictionary<string, (int index, Type ty)>();
            var ps = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor p in ps)
            {
                if (IsNotComplexType(p)) // filter out complex type
                {
                    var igAttr = p.PropertyType.GetCustomAttributes(typeof(CsvIgnoreAttribute), true).FirstOrDefault() as CsvIgnoreAttribute; // get csvignore attribute

                    if (igAttr == null)
                    {
                        var clAttr = p.PropertyType.GetCustomAttributes(typeof(CsvColumnAttribute), true).FirstOrDefault() as CsvColumnAttribute; // get csvcolumn attribute
                        props.Add(clAttr != null ? clAttr.Column.ToUpper() : p.Name.ToUpper(), (0, GetPropType(p)));
                    }
                }
            }
            return props;
        }

        internal static object ConvertFromString(Type type, string value)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            try
            {
               return  typeConverter.ConvertFromString(value);
            }
            catch (Exception)
            {
                return GetDefaultValue(type);
            } 
        }

        internal static T GetDefaultValue<T>()
        {
            Expression<Func<T>> e = Expression.Lambda<Func<T>>(
                Expression.Default(typeof(T))
            );

            return e.Compile()();
        }

        internal static object GetDefaultValue(Type type)
        {
            if (type == null) throw new ArgumentNullException("No type specified");

            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )
            );

            return e.Compile()();
        }
    }
}
