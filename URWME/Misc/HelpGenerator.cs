using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace URWME
{
    public static class HelpGenerator
    {
        public static string GenerateHelp<T>()
        {
            var type = typeof(T);
            var sb = new StringBuilder();

            sb.AppendLine($"Help for {type.Name}:");
            sb.AppendLine();

            // Properties
            var properties = type.GetProperties();
            if (properties.Length > 0)
            {
                sb.AppendLine("Properties:");
                foreach (var property in properties)
                {
                    var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "No description";
                    sb.AppendLine($"  - {property.Name} ({property.PropertyType.Name}): {description}");
                }
                sb.AppendLine();
            }

            // Fields
            var fields = type.GetFields();
            if (fields.Length > 0)
            {
                sb.AppendLine("Fields:");
                foreach (var field in fields)
                {
                    var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "No description";
                    sb.AppendLine($"  - {field.Name} ({field.FieldType.Name}): {description}");
                }
                sb.AppendLine();
            }

            // Methods
            var methods = type.GetMethods()
                              .Where(m => !m.IsSpecialName) // Exclude methods like .ctor, .cctor, etc.
                              .ToArray();
            if (methods.Length > 0)
            {
                sb.AppendLine("Methods:");
                foreach (var method in methods)
                {
                    var description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "No description";
                    var parameterList = string.Join(", ", method.GetParameters()
                                                                 .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    var returnType = method.ReturnType.Name;
                    sb.AppendLine($"  - {method.Name}({parameterList}) : {returnType} - {description}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class DescriptionAttribute : System.Attribute
    {
        public string Description { get; }
        public DescriptionAttribute(string description) => Description = description;
    }
}
