using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }
        public StringValueAttribute(string value) => Value = value;
    }
}
