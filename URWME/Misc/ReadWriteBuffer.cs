using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URWME
{
    public class ReadWriteBuffer
    {
        private byte[] buffer;
        private int bufferLength;

        public byte[] CurrentBuffer { get { return buffer; } set { buffer = value; bufferLength = value.Length; } }

        public ReadWriteBuffer(byte[] buffer)
        {
            CurrentBuffer = buffer;
        }

        // Write a value to the buffer at a specific offset
        public void Write<T>(int offset, T value)
        {
            byte[] data = ConvertToBytes(value);

            if (offset + data.Length > bufferLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset and data length exceed buffer length.");
            }

            Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
        }

        // Read a value from the buffer at a specific offset
        public T Read<T>(int offset, int size = 0)
        {
            if (size == 0) { size = GetSizeOfType<T>(); }

            if (offset + size > bufferLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset and type size exceed buffer length.");
            }

            byte[] data = new byte[size];
            Buffer.BlockCopy(buffer, offset, data, 0, size);
            return ConvertFromBytes<T>(data);
        }

        // Convert a value to a byte array
        private byte[] ConvertToBytes<T>(T value)
        {
            if (typeof(T) == typeof(bool))
                return BitConverter.GetBytes((bool)(object)value);
            if (typeof(T) == typeof(byte))
                return new byte[] { (byte)(object)value };
            if (typeof(T) == typeof(short))
                return BitConverter.GetBytes((short)(object)value);
            if (typeof(T) == typeof(ushort))
                return BitConverter.GetBytes((ushort)(object)value);
            if (typeof(T) == typeof(int))
                return BitConverter.GetBytes((int)(object)value);
            if (typeof(T) == typeof(uint))
                return BitConverter.GetBytes((uint)(object)value);
            if (typeof(T) == typeof(long))
                return BitConverter.GetBytes((long)(object)value);
            if (typeof(T) == typeof(ulong))
                return BitConverter.GetBytes((ulong)(object)value);
            if (typeof(T) == typeof(float))
                return BitConverter.GetBytes((float)(object)value);
            if (typeof(T) == typeof(double))
                return BitConverter.GetBytes((double)(object)value);
            if (typeof(T) == typeof(string))
                return Encoding.UTF8.GetBytes((string)(object)value);

            throw new InvalidOperationException("Unsupported type");
        }

        // Convert a byte array to a value
        private T ConvertFromBytes<T>(byte[] data)
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)BitConverter.ToBoolean(data, 0);
            if (typeof(T) == typeof(byte))
                return (T)(object)data[0];
            if (typeof(T) == typeof(short))
                return (T)(object)BitConverter.ToInt16(data, 0);
            if (typeof(T) == typeof(ushort))
                return (T)(object)BitConverter.ToUInt16(data, 0);
            if (typeof(T) == typeof(int))
                return (T)(object)BitConverter.ToInt32(data, 0);
            if (typeof(T) == typeof(uint))
                return (T)(object)BitConverter.ToUInt32(data, 0);
            if (typeof(T) == typeof(long))
                return (T)(object)BitConverter.ToInt64(data, 0);
            if (typeof(T) == typeof(ulong))
                return (T)(object)BitConverter.ToUInt64(data, 0);
            if (typeof(T) == typeof(float))
                return (T)(object)BitConverter.ToSingle(data, 0);
            if (typeof(T) == typeof(double))
                return (T)(object)BitConverter.ToDouble(data, 0);
            if (typeof(T) == typeof(string))
                return (T)(object)DefaultData.URWEncoding.GetString(data).Split('\0')[0];

            throw new InvalidOperationException("Unsupported type");
        }

        // Get the size of the type in bytes
        private int GetSizeOfType<T>()
        {
            if (typeof(T) == typeof(bool))
                return sizeof(bool);
            if (typeof(T) == typeof(byte))
                return sizeof(byte);
            if (typeof(T) == typeof(short))
                return sizeof(short);
            if (typeof(T) == typeof(ushort))
                return sizeof(ushort);
            if (typeof(T) == typeof(int))
                return sizeof(int);
            if (typeof(T) == typeof(uint))
                return sizeof(uint);
            if (typeof(T) == typeof(long))
                return sizeof(long);
            if (typeof(T) == typeof(ulong))
                return sizeof(ulong);
            if (typeof(T) == typeof(float))
                return sizeof(float);
            if (typeof(T) == typeof(double))
                return sizeof(double);
            if (typeof(T) == typeof(string))
                return 0; // Variable size, handled separately

            throw new InvalidOperationException("Unsupported type");
        }

        // Read a string from the buffer with a specified length
        public string ReadString(int offset, int length)
        {
            if (offset + length > bufferLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset and length exceed buffer length.");
            }

            byte[] data = new byte[length];
            Buffer.BlockCopy(buffer, offset, data, 0, length);
            return Encoding.UTF8.GetString(data).Split('\0')[0];
        }

        // Write a string to the buffer at a specific offset
        public void WriteString(int offset, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value + '\0');

            if (offset + data.Length > bufferLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset and data length exceed buffer length.");
            }

            Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
        }
    }
}
