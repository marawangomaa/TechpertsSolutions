using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var field = type.GetField(enumVal.ToString());
            if (field == null)
                return enumVal.ToString();
                
            var attribute = field.GetCustomAttributes(typeof(StringValueAttribute), false)
                                 .FirstOrDefault() as StringValueAttribute;
            return attribute?.Value ?? enumVal.ToString();
        }

        // Parses an enum value from its StringValue attribute or enum name
        public static TEnum ParseFromStringValue<TEnum>(string value) where TEnum : struct, Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(StringValueAttribute)) as StringValueAttribute;
                if (attribute != null && attribute.Value == value)
                    return (TEnum)field.GetValue(null);
                if (field.Name == value)
                    return (TEnum)field.GetValue(null);
            }
            throw new ArgumentException($"No matching enum value for '{value}' in {typeof(TEnum).Name}");
        }
    }
}
