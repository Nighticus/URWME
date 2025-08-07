using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace URWME
{
    public class CommandEngine
    {
        private readonly Dictionary<string, object> _instances; // Maps instance names to objects.

        public CommandEngine(Dictionary<string, object> instances)
        {
            _instances = instances;
        }

        public string ExecuteCommand(string command)
        {
            try
            {
                // Extract the instance name (e.g., "Player" from "Player.Name = 'bob'")
                var instanceMatch = Regex.Match(command, @"^(\w+)\.");
                if (!instanceMatch.Success)
                    throw new Exception("Invalid command format. Expected instance name at the beginning.");

                string instanceName = instanceMatch.Groups[1].Value;
                if (!_instances.ContainsKey(instanceName))
                    throw new Exception($"Instance '{instanceName}' not found.");

                // Get the target instance (e.g., the Player object)
                object targetInstance = _instances[instanceName];

                // Remove the instance name prefix from the command (e.g., "Name = 'bob'" instead of "Player.Name = 'bob'")
                string instanceCommand = command.Substring(instanceName.Length + 1);

                // Check for assignment commands (e.g., Player.Name = "bob")
                var assignmentMatch = Regex.Match(instanceCommand, @"^(.*) = (.*)$");
                if (assignmentMatch.Success)
                {
                    string left = assignmentMatch.Groups[1].Value.Trim();
                    string right = assignmentMatch.Groups[2].Value.Trim();

                    object value = ParseValue(right);
                    SetPropertyOrField(targetInstance, left, value);
                    return $"set {instanceName}.{left} to {value}";
                }

                // Otherwise, assume it's a retrieval or method invocation (e.g., Player.Name)
                object result = GetPropertyOrMethod(targetInstance, instanceCommand);
                return $"{result.ToString()}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private object GetPropertyOrMethod(object obj, string expression)
        {
            string[] parts = expression.Split('.');
            object currentObject = obj;

            foreach (string part in parts)
            {
                if (part.Contains("[") && part.Contains("]")) // Check for indexing, e.g., Attributes["Strength"]
                {
                    currentObject = GetIndexedProperty(currentObject, part);
                }
                else if (part.Contains("(") && part.Contains(")")) // Check for method calls
                {
                    currentObject = InvokeMethod(currentObject, part);
                }
                else // Regular property or field access
                {
                    currentObject = GetPropertyOrField(currentObject, part);
                }
            }

            return currentObject;
        }

        private void SetPropertyOrField(object obj, string expression, object value)
        {
            string[] parts = expression.Split('.');
            object currentObject = obj;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                string part = parts[i];
                currentObject = part.Contains("[") && part.Contains("]")
                    ? GetIndexedProperty(currentObject, part)
                    : GetPropertyOrField(currentObject, part);
            }

            string finalPart = parts[^1];
            if (finalPart.Contains("[") && finalPart.Contains("]"))
            {
                SetIndexedProperty(currentObject, finalPart, value);
            }
            else
            {
                SetSimplePropertyOrField(currentObject, finalPart, value);
            }
        }

        private object GetPropertyOrField(object obj, string name)
        {
            PropertyInfo prop = obj.GetType().GetProperty(name);
            if (prop != null) return prop.GetValue(obj);

            FieldInfo field = obj.GetType().GetField(name);
            if (field != null) return field.GetValue(obj);

            throw new Exception($"Property or field '{name}' not found on {obj.GetType().Name}");
        }

        private void SetSimplePropertyOrField(object obj, string name, object value)
        {
            PropertyInfo prop = obj.GetType().GetProperty(name);
            if (prop != null)
            {
                value = ConvertValue(value, prop.PropertyType); // Convert to correct type
                prop.SetValue(obj, value);
                return;
            }

            FieldInfo field = obj.GetType().GetField(name);
            if (field != null)
            {
                value = ConvertValue(value, field.FieldType); // Convert to correct type
                field.SetValue(obj, value);
                return;
            }

            throw new Exception($"Property or field '{name}' not found on {obj.GetType().Name}");
        }

        private object ConvertValue(object value, Type targetType)
        {
            // If the value is already of the target type, return it directly
            if (value.GetType() == targetType)
                return value;

            // Handle nullable types by getting the underlying type
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                // Use Convert.ChangeType for conversion, which supports most base types
                return Convert.ChangeType(value, underlyingType);
            }
            catch (InvalidCastException)
            {
                throw new Exception($"Cannot convert '{value}' to {underlyingType.Name}");
            }
        }

        private object GetIndexedProperty(object obj, string indexedExpression)
        {
            var match = Regex.Match(indexedExpression, @"(\w+)\[(.+?)\]");
            if (!match.Success)
                throw new Exception($"Invalid indexed expression '{indexedExpression}'");

            string propertyName = match.Groups[1].Value;
            string indexValue = match.Groups[2].Value;

            // Retrieve the collection property (e.g., Skills)
            object collection = GetPropertyOrField(obj, propertyName);
            Type collectionType = collection.GetType();

            // Try to find the correct indexer (Item[int] or Item[string])
            PropertyInfo indexer;
            object index;
            if (int.TryParse(indexValue, out int intIndex))
            {
                indexer = collectionType.GetProperty("Item", new[] { typeof(int) });
                index = intIndex;
            }
            else
            {
                indexer = collectionType.GetProperty("Item", new[] { typeof(string) });
                index = indexValue;
            }

            if (indexer == null)
                throw new Exception($"Indexer with the appropriate parameter type not found for '{propertyName}'.");

            return indexer.GetValue(collection, new[] { index });
        }

        private void SetIndexedProperty(object obj, string indexedExpression, object value)
        {
            var match = Regex.Match(indexedExpression, @"(\w+)\[(.+?)\]");
            if (!match.Success)
                throw new Exception($"Invalid indexed expression '{indexedExpression}'");

            string propertyName = match.Groups[1].Value;
            string indexValue = match.Groups[2].Value;

            object collection = GetPropertyOrField(obj, propertyName);

            if (collection is IList list)
            {
                int index = int.Parse(indexValue);
                list[index] = value;
            }
            else if (collection is IDictionary dictionary)
            {
                object key = ParseValue(indexValue);
                dictionary[key] = value;
            }
            else
            {
                throw new Exception($"Property '{propertyName}' is not an indexable type");
            }
        }

        private object InvokeMethod(object obj, string methodCall)
        {
            var match = Regex.Match(methodCall, @"(\w+)\((.*)\)");
            if (!match.Success)
                throw new Exception($"Invalid method call '{methodCall}'");

            string methodName = match.Groups[1].Value;
            string[] parameterStrings = match.Groups[2].Value.Split(',')
                .Select(p => p.Trim()).ToArray();

            MethodInfo method = obj.GetType().GetMethod(methodName);
            if (method == null)
                throw new Exception($"Method '{methodName}' not found on {obj.GetType().Name}");

            object[] parameters = parameterStrings.Select(ParseValue).ToArray();
            if (method.GetParameters().Length <= 0) { return method.Invoke(obj, null); }
            return method.Invoke(obj, parameters);
        }

        private object ParseValue(string value)
        {
            if (int.TryParse(value, out int intValue)) return intValue;
            if (value.StartsWith("\"") && value.EndsWith("\""))
                return value.Trim('"');
            return value;
        }
    }
}