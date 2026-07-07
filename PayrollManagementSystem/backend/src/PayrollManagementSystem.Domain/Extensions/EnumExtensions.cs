using System;
using System.ComponentModel;
using System.Reflection;

namespace PayrollManagementSystem.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
