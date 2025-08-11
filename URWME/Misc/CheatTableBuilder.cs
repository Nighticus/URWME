using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace URWME
{
    public static class CheatTableBuilder
    {
        public static void GenerateCheatTableFile(
            string groupName = "3.86",
            Type addressClassType = null,
            string processName = "urw.exe",
            string outputFileName = null)
        {
            nint WindowHandle = DefaultData.URW.MainWindowHandle;

            if (addressClassType == null)
            {
                addressClassType = typeof(Address); // Default fallback
                Console.WriteLine($"Warning: No address class type provided. Using '{addressClassType.Name}'.");
            }

            var props = addressClassType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            if (!props.Any())
            {
                Console.WriteLine($"No public static properties found in {addressClassType.Name}.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            int idCounter = 1;

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<CheatTable CheatEngineTableVersion=\"46\">");
            sb.AppendLine("  <CheatEntries>");
            sb.AppendLine("    <CheatEntry>");
            sb.AppendLine($"      <ID>{idCounter++}</ID>");
            sb.AppendLine($"      <Description>\"{groupName}\"</Description>");
            sb.AppendLine("      <Options moHideChildren=\"1\"/>");
            sb.AppendLine("      <GroupHeader>1</GroupHeader>");
            sb.AppendLine("      <CheatEntries>");

            foreach (var prop in props)
            {
                if (prop.PropertyType != typeof(int) && prop.PropertyType != typeof(IntPtr))
                {
                    Console.WriteLine($"Skipping {prop.Name}: unsupported type {prop.PropertyType.Name}");
                    continue;
                }

                string name = prop.Name;
                object value = prop.GetValue(null);
                if (value == null) continue;

                long addressValue = prop.PropertyType == typeof(int)
                    ? (int)value
                    : ((IntPtr)value).ToInt64();

                var attr = prop.GetCustomAttribute<CheatTypeAttribute>();
                string variableType = attr?.VariableType ?? "4 Bytes";
                string showAsSigned = attr != null && attr.ShowAsSigned ? "1" : "0";
                int byteLength = attr?.ByteLength ?? 0;
                int length = attr?.Length ?? 0;

                sb.AppendLine("        <CheatEntry>");
                sb.AppendLine($"          <ID>{idCounter++}</ID>");
                sb.AppendLine($"          <Description>\"{name}\"</Description>");
                sb.AppendLine($"          <VariableType>{variableType}</VariableType>");
                if (variableType == "Array of byte" && byteLength > 0)
                {
                    sb.AppendLine($"          <ByteLength>{byteLength}</ByteLength>");

                    string dummyValue = string.Join(" ", Enumerable.Repeat("00", byteLength));
                    string realAddress = (addressValue + WindowHandle).ToString("X8");
                    sb.AppendLine($"          <LastState Value=\"{dummyValue}\" RealAddress=\"{realAddress}\"/>");
                }
                if (variableType == "String" && length > 0)
                {
                    sb.AppendLine($"          <Length>{length}</Length>");
                }

                sb.AppendLine($"          <Address>{processName}+{addressValue:X}</Address>");
                sb.AppendLine($"          <ShowAsSigned>{showAsSigned}</ShowAsSigned>");
                sb.AppendLine("        </CheatEntry>");
            }


            sb.AppendLine("      </CheatEntries>");
            sb.AppendLine("    </CheatEntry>");
            sb.AppendLine("  </CheatEntries>");
            sb.AppendLine("  <UserdefinedSymbols/>");
            sb.AppendLine("  <Comments></Comments>");
            sb.AppendLine("</CheatTable>");

            // File output
            if (string.IsNullOrWhiteSpace(outputFileName))
                outputFileName = $"CheatTable_{groupName.Replace(" ", "_")}.ct";

            try
            {
                File.WriteAllText(outputFileName, sb.ToString(), new UTF8Encoding(false));
                Console.WriteLine($"Cheat Table saved to: {Path.GetFullPath(outputFileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class CheatTypeAttribute : System.Attribute
    {
        public string VariableType { get; }
        public bool ShowAsSigned { get; }
        public int ByteLength { get; }
        public int Length { get; }

        public CheatTypeAttribute(string variableType, bool showAsSigned = false, int byteLength = 0, int length = 0)
        {
            VariableType = variableType;
            ShowAsSigned = showAsSigned;
            ByteLength = byteLength;
            Length = length;
        }
    }

}
