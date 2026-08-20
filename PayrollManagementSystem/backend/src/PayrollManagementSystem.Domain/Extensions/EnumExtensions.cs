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
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"Not found: {description}");
        }

        public static bool TryGetValueFromDescription<T>(string description, out T result) where T : struct, Enum
        {
            try
            {
                result = GetValueFromDescription<T>(description);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}
