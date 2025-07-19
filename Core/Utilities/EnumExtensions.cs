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
    }
}
