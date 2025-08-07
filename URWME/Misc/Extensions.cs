using System.Collections;
//using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace URWME // Unreal World MemoryManager
{
    public static class Extensions
    {
        public static string ReplaceModifiedString(this string s)
        {
            return s;//.Replace('Σ', 'ä').Replace('Ø', '¥').Replace('─', 'Ä').Replace('÷', 'ö');
        }

        public static string ToOrdinal(this int i)
        {
            if (i <= 0) return i.ToString();

            switch (i % 100)
            {
                case 11:
                case 12:
                case 13:
                    return i + "th";
            }

            switch (i % 10)
            {
                case 1:
                    return i + "st";
                case 2:
                    return i + "nd";
                case 3:
                    return i + "rd";
                default:
                    return i + "th";
            }
        }

        public static string ToJson(this object obj)
        {
            // Return an empty object if the input is null
            if (obj == null)
                return "{}";

            // Use the built-in JsonSerializer to handle the object
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

    }

}
