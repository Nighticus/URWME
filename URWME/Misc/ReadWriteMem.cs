using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace URWME // Unreal World MemoryManager
{
    public class ReadWriteMem
    {
        [DllImport("Kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, int BaseAddress, byte[] Buffer, int Size, ref int NumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr BaseAddress, byte[] Buffer, int Size, ref int NumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        private Process Proc;
        public Process TargetProcess
        {
            get { return Proc; }
            set { Proc = value; }
        }
        //public IntPtr WindowHandle = IntPtr.Zero;

        public int ProcBaseAddress = 0;
        public const int VersionOffset = 0x0000;

        public ReadWriteMem(Process T)
        {
            TargetProcess = T;
            ProcBaseAddress = T.MainModule.BaseAddress.ToInt32();
            //WindowHandle = T.Handle;
        }

        public bool Write(int BaseAddress, dynamic obj, byte pb = 1)
        {
            byte[] Buffer;
            if (obj is byte b)
                Buffer = new byte[] { b };
            else if (obj is byte[] ba)
                Buffer = ba;
            else if (obj is bool bo)
                Buffer = BitConverter.GetBytes(bo);
            else if (obj is short s)
                Buffer = BitConverter.GetBytes(s);
            else if (obj is int i)
                Buffer = BitConverter.GetBytes(i);
            else if (obj is float f)
                Buffer = BitConverter.GetBytes(f);
            else if (obj is double d)
                Buffer = BitConverter.GetBytes(d);
            else if (obj is long l)
                Buffer = BitConverter.GetBytes(l);
            else if (obj is Half h)
                Buffer = BitConverter.GetBytes(h);
            else
                throw new ArgumentException("Unsupported type", nameof(obj));

            int BytesWritten = 0;
            IntPtr targetAddr = (IntPtr)((pb * ProcBaseAddress) + BaseAddress + VersionOffset);

            // Check if we need to handle special case for 2-byte buffer with trailing zero
            byte[] actualBuffer;
            int actualSize;
            if (Buffer.Length == 2 && Buffer[1] == 0)
            {
                actualBuffer = new byte[] { (byte)obj };
                actualSize = 1;
            }
            else
            {
                actualBuffer = Buffer;
                actualSize = Buffer.Length;
            }

            // Try normal write first
            bool writeSuccess = WriteProcessMemory(Proc.Handle, targetAddr, actualBuffer, actualSize, ref BytesWritten);

            // If write failed (0 bytes written), try with VirtualProtectEx
            if (BytesWritten == 0)
            {
                uint oldProtect;
                if (VirtualProtectEx(Proc.Handle, targetAddr, actualSize, 0x04, out oldProtect)) // PAGE_READWRITE
                {
                    // Try write again with changed protection
                    WriteProcessMemory(Proc.Handle, targetAddr, actualBuffer, actualSize, ref BytesWritten);

                    // Restore original protection
                    VirtualProtectEx(Proc.Handle, targetAddr, actualSize, oldProtect, out uint temp);
                }
            }

            return BytesWritten > 0;
        }

        public bool Write(int BaseAddress, byte[] Buffer, byte pb = 1)
        {
            int BytesWritten = 0;
            WriteProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, Buffer.Length, ref BytesWritten);
            switch (BytesWritten)
            {
                case 0: return false;
                default: return true;
            }
        }

        public bool Write(int BaseAddress, string Text, byte pb = 1)
        {
            byte[] Buffer = DefaultData.URWEncoding.GetBytes(Text.ReplaceModifiedString());
            int BytesWritten = 0;
            WriteProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, Buffer.Length, ref BytesWritten);
            switch (BytesWritten)
            {
                case 0: return false;
                default: return true;
            }
        }

        public T Read<T>(int BaseAddress, byte pb = 1)
        {
            byte[] Buffer;
            int BytesRead = 0;

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    Buffer = new byte[1];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 1, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToBoolean(Buffer, 0), typeof(T));
                case TypeCode.Byte:
                    Buffer = new byte[1];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 1, ref BytesRead);
                    return (T)Convert.ChangeType(Buffer[0], typeof(T));
                case TypeCode.Int16:
                    Buffer = new byte[2];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 2, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToUInt16(Buffer, 0), typeof(T));
                case TypeCode.Int32:
                    Buffer = new byte[4];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 4, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToUInt32(Buffer, 0), typeof(T));
                case TypeCode.UInt32:
                    Buffer = new byte[4];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 4, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToUInt32(Buffer, 0), typeof(T));
                case TypeCode.Single:
                    Buffer = new byte[4];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 4, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToSingle(Buffer, 0), typeof(T));
                case TypeCode.Double:
                    Buffer = new byte[8];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, 8, ref BytesRead);
                    return (T)Convert.ChangeType(BitConverter.ToDouble(Buffer, 0), typeof(T));
                default:
                    return (T)Convert.ChangeType(false, typeof(T));
            }
        }

        public T Read<T>(int BaseAddress, int Length, byte pb = 1)
        {
            byte[] Buffer;
            int BytesRead = 0;

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Object:
                    Buffer = new byte[Length];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, Length, ref BytesRead);
                    return (T)Convert.ChangeType(Buffer, typeof(T));
                case TypeCode.String:
                    Buffer = new byte[Length];
                    ReadProcessMemory(Proc.Handle, (pb * ProcBaseAddress) + BaseAddress + VersionOffset, Buffer, Length, ref BytesRead);
                    return (T)Convert.ChangeType(DefaultData.URWEncoding.GetString(Buffer, 0, Buffer.Length).Split(new char[] { '\0' })[0].ReplaceModifiedString(), typeof(T));
                default:
                    return (T)Convert.ChangeType(false, typeof(T));
            }
        }

        public int FindSignature(string pattern, bool firstMatchOnly = true, bool isHex = true)
        {
            byte[] patternBytes;
            bool[] mask;
            ParsePattern(pattern, out patternBytes, out mask, isHex);

            IntPtr currentAddress = IntPtr.Zero;
            MEMORY_BASIC_INFORMATION m;
            byte[] buffer;
            int bytesRead = 0;

            while (VirtualQueryEx(Proc.Handle, currentAddress, out m, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))) != false)
            {
                if ((m.State & 0x1000) != 0 && (m.Protect & 0x100) == 0) // MEM_COMMIT and not PAGE_NOACCESS
                {
                    buffer = new byte[(int)m.RegionSize];
                    ReadProcessMemory(Proc.Handle, (int)m.BaseAddress, buffer, buffer.Length, ref bytesRead);

                    var matches = ScanPattern(buffer, patternBytes, mask);
                    if (matches.Count > 0)
                        return (int)m.BaseAddress + matches[0]; // First match only
                }
                currentAddress = new IntPtr(m.BaseAddress.ToInt64() + (long)m.RegionSize);
            }
            return -1;
        }

        public async Task<int> FindSignatureAsyncOld(string pattern, bool isHex = true)
        {
            return await Task.Run(() => FindSignature(pattern, isHex: isHex));
        }

        public async Task<int> FindSignatureAsync(string pattern, bool isHex = true)
        {
            var sw = Stopwatch.StartNew();   // start timing

            int result = await Task.Run(() => FindSignature(pattern, isHex: isHex));

            sw.Stop();   // stop timing

            // log with timestamp + elapsed time + result
            Debug.WriteLine($"0x{result.ToString("X")} in {sw.ElapsedMilliseconds} ms");

            return result;
        }


        private List<int> ScanPattern(byte[] buffer, byte[] pattern, bool[] mask)
        {
            List<int> results = new List<int>();

            for (int i = 0; i < buffer.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (mask[j] && buffer[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    results.Add(i);
            }
            return results;
        }
        private void ParsePattern(string pattern, out byte[] bytes, out bool[] mask, bool isHex)
        {
            string[] splits = pattern.Split(' ');
            bytes = new byte[splits.Length];
            mask = new bool[splits.Length];

            for (int i = 0; i < splits.Length; i++)
            {
                if (splits[i] == "??" || splits[i] == "?")
                {
                    bytes[i] = 0x00;
                    mask[i] = false; // false means wildcard
                }
                else
                {
                    if (isHex)
                    { bytes[i] = Convert.ToByte(splits[i], 16); }
                    else
                    { bytes[i] = Convert.ToByte(splits[i], 10); }
                    mask[i] = true;  // true means must match
                }
            }
        }
    }

}
