using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BitPool.Extensions
{
    namespace Generic
    {
        public static class GenericExtensionMethods
        {
            public static bool IsBetween<T> (this T value, T low, T high) where T : IComparable<T> {
                return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
            }
        }
    }

    namespace Enumerations
    {
        public static class EnumExtensions
        {
            public static T FromString<T> (this T type, string value) where T : struct, IConvertible {
                if (!typeof(T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");
                T outputType;
                try {
                    outputType = (T) System.Enum.Parse(typeof(T), value);
                } catch (ArgumentException) {
                    throw new ArgumentException("Enumeration member is unknown / invalid.");
                }
                return outputType;
            }


            /// <summary>
            /// Reads an enumeration value encoded as a string.
            /// </summary>
            /// <typeparam name='T'>
            /// Must be an enumeration type.
            /// </typeparam>
            public static void ToEnum<T> (this string stringValue, out T value) where T : struct, IConvertible {
                if (!typeof(T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");
                try {
                    value = (T) System.Enum.Parse(typeof(T), stringValue);
                } catch (ArgumentException) {
                    throw new ArgumentException("Enumeration member is unknown / invalid.");
                }
            }
        }
    }

    namespace Streams
    {
        public static class StreamExtensions
        {
            public static void WritePrimitive (this Stream stream, bool value) {
                stream.WriteByte(value ? (byte) 1 : (byte) 0);
            }

            public static void ReadPrimitive (this Stream stream, out bool value) {
                var b = stream.ReadByte();
                value = b != 0;
            }

            public static void WritePrimitive (this Stream stream, byte value) {
                stream.WriteByte(value);
            }

            public static void ReadPrimitive (this Stream stream, out byte value) {
                value = (byte) stream.ReadByte();
            }

            public static void WritePrimitive (this Stream stream, sbyte value) {
                stream.WriteByte((byte) value);
            }

            public static void ReadPrimitive (this Stream stream, out sbyte value) {
                value = (sbyte) stream.ReadByte();
            }

            public static void WritePrimitive (this Stream stream, char value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive (this Stream stream, out char value) {
                value = (char) ReadVarint32(stream);
            }

            public static void WritePrimitive (this Stream stream, ushort value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive (this Stream stream, out ushort value) {
                value = (ushort) ReadVarint32(stream);
            }

            public static void WritePrimitive (this Stream stream, short value) {
                WriteVarint32(stream, EncodeZigZag32(value));
            }

            public static void ReadPrimitive (this Stream stream, out short value) {
                value = (short) DecodeZigZag32(ReadVarint32(stream));
            }

            public static void WritePrimitive (this Stream stream, uint value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive (this Stream stream, out uint value) {
                value = ReadVarint32(stream);
            }

            public static void WritePrimitive (this Stream stream, int value) {
                WriteVarint32(stream, EncodeZigZag32(value));
            }

            public static void ReadPrimitive (this Stream stream, out int value) {
                value = DecodeZigZag32(ReadVarint32(stream));
            }

            public static void WritePrimitive (this Stream stream, ulong value) {
                WriteVarint64(stream, value);
            }

            public static void ReadPrimitive (this Stream stream, out ulong value) {
                value = ReadVarint64(stream);
            }

            public static void WritePrimitive (this Stream stream, long value) {
                WriteVarint64(stream, EncodeZigZag64(value));
            }

            public static void ReadPrimitive (this Stream stream, out long value) {
                value = DecodeZigZag64(ReadVarint64(stream));
            }

            public static unsafe void WritePrimitive (this Stream stream, float value) {
                uint v = *(uint*) (&value);
                WriteVarint32(stream, v);
            }

            public static unsafe void ReadPrimitive (this Stream stream, out float value) {
                uint v = ReadVarint32(stream);
                value = *(float*) (&v);
            }

            public static unsafe void WritePrimitive (this Stream stream, double value) {
                ulong v = *(ulong*) (&value);
                WriteVarint64(stream, v);
            }

            public static unsafe void ReadPrimitive (this Stream stream, out double value) {
                ulong v = ReadVarint64(stream);
                value = *(double*) (&v);
            }

            public static void WritePrimitive (this Stream stream, DateTime value) {
                long v = value.ToBinary();
                WritePrimitive(stream, v);
            }

            public static void ReadPrimitive (this Stream stream, out DateTime value) {
                long v;
                ReadPrimitive(stream, out v);
                value = DateTime.FromBinary(v);
            }

            public static void WritePrimitive (this Stream stream, string value) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                var encoding = new UTF8Encoding(false, true);

                int len = encoding.GetByteCount(value);

                WritePrimitive(stream, (uint) len + 1);

                var buf = new byte[len];

                encoding.GetBytes(value, 0, value.Length, buf, 0);

                stream.Write(buf, 0, len);
            }

            public static void ReadPrimitive (this Stream stream, out string value) {
                uint len;
                ReadPrimitive(stream, out len);

                if (len == 0) {
                    value = null;
                    return;
                } else if (len == 1) {
                    value = string.Empty;
                    return;
                }

                len -= 1;

                var encoding = new UTF8Encoding(false, true);

                var buf = new byte[len];

                int l = 0;

                while (l < len) {
                    int r = stream.Read(buf, l, (int) len - l);
                    if (r == 0)
                        throw new EndOfStreamException();
                    l += r;
                }

                value = encoding.GetString(buf);
            }



            public static void WritePrimitive (this Stream stream, byte[] value) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                WritePrimitive(stream, (uint) value.Length + 1);
                stream.Write(value, 0, value.Length);
            }

            public static void WritePrimitive (this Stream stream, byte[] value, int offset, int count) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                WritePrimitive(stream, (uint) count + 1);
                stream.Write(value, offset, count);
            }

            public static void WritePrimitiveMeta (this Stream stream, byte[] value, bool negative) {
                stream.WritePrimitiveMeta(value, 0, value.Length, negative);
            }

            /// <summary>
            /// Writes a length-encoded byte array with additional boolean property stored as integer sign.
            /// </summary>
            /// <param name="stream">Stream to write to.</param>
            /// <param name="value">Source byte array.</param>
            /// <param name="offset">Offset at which to start writing bytes from the source array.</param>
            /// <param name="count">Number of bytes to be written.</param>
            /// <param name="negative">If set to <c>true</c> length-specifying integer will be stored with negative sign.</param>
            public static void WritePrimitiveMeta (this Stream stream, byte[] value, int offset, int count, bool negative) {
                if (value == null) {
                    WritePrimitive(stream, 0);
                    return;
                }

                WritePrimitive(stream, negative ? -(count + 1) : count + 1);
                stream.Write(value, offset, count);
            }

            private static readonly byte[] s_emptyByteArray = new byte[0];

            public static void ReadPrimitive (this Stream stream, out byte[] value) {
                uint len;
                ReadPrimitive(stream, out len);

                if (len == 0) {
                    value = null;
                    return;
                } else if (len == 1) {
                    value = s_emptyByteArray;
                    return;
                }

                len -= 1;

                value = new byte[len];
                int l = 0;

                while (l < len) {
                    int r = stream.Read(value, l, (int) len - l);
                    if (r == 0) throw new EndOfStreamException();
                    l += r;
                }
            }

            /// <summary>
            /// Reads a length-encoded byte array with additional boolean property stored as integer sign.
            /// </summary>
            /// <param name="stream">Stream to be read from.</param>
            /// <param name="value">Output byte array.</param>
            /// <param name="negative">Stored boolean state. Will be <c>true</c> if stored integer has negative sign.</param>
            public static void ReadPrimitiveMeta (this Stream stream, out byte[] value, out bool negative) {
                int len;
                ReadPrimitive(stream, out len);

                negative = Math.Sign(len) < 0;
                len = Math.Abs(len);

                if (len == 0) {
                    value = null;
                    return;
                } else if (len == 1) {
                    value = s_emptyByteArray;
                    return;
                }

                len -= 1;

                value = new byte[len];
                int l = 0;

                while (l < len) {
                    int r = stream.Read(value, l, len - l);
                    if (r == 0) throw new EndOfStreamException();
                    l += r;
                }
            }

            /// <summary>
            /// Reads an enumeration value from a stream that was encoded as a string.
            /// </summary>
            /// <typeparam name='T'>
            /// Must be an enumeration type.
            /// </typeparam>
            public static void ReadPrimitive<T> (this Stream stream, out T value) where T : struct, IConvertible {

                if (!typeof(T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");
                try {
                    string stringValue;
                    ReadPrimitive(stream, out stringValue);
                    value = (T) Enum.Parse(typeof(T), stringValue);
                } catch (ArgumentException) {
                    throw new ArgumentException("Enumeration member is unknown or otherwise invalid.");
                }
            }

            /// <summary>
            /// Writes an enumeration value into a stream, encoded as a string .
            /// </summary>
            /// <typeparam name='T'>
            /// Must be an enumeration type.
            /// </typeparam>
            public static void WritePrimitive<T> (this Stream stream, T value) where T : struct, IConvertible {
                if (!typeof(T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");

                WritePrimitive(stream, Enum.GetName(typeof(T), value));
            }

            private static uint EncodeZigZag32 (int n) {
                return (uint) ((n << 1) ^ (n >> 31));
            }

            private static ulong EncodeZigZag64 (long n) {
                return (ulong) ((n << 1) ^ (n >> 63));
            }

            private static int DecodeZigZag32 (uint n) {
                return (int) (n >> 1) ^ -(int) (n & 1);
            }

            private static long DecodeZigZag64 (ulong n) {
                return (long) (n >> 1) ^ -(long) (n & 1);
            }

            private static uint ReadVarint32 (Stream stream) {
                int result = 0;
                int offset = 0;

                for (; offset < 32; offset += 7) {
                    int b = stream.ReadByte();
                    if (b == -1)
                        throw new EndOfStreamException();

                    result |= (b & 0x7f) << offset;

                    if ((b & 0x80) == 0)
                        return (uint) result;
                }

                throw new InvalidDataException();
            }

            private static void WriteVarint32 (Stream stream, uint value) {
                for (; value >= 0x80u; value >>= 7)
                    stream.WriteByte((byte) (value | 0x80u));

                stream.WriteByte((byte) value);
            }

            private static ulong ReadVarint64 (Stream stream) {
                long result = 0;
                int offset = 0;

                for (; offset < 64; offset += 7) {
                    int b = stream.ReadByte();
                    if (b == -1)
                        throw new EndOfStreamException();

                    result |= ((long) (b & 0x7f)) << offset;

                    if ((b & 0x80) == 0)
                        return (ulong) result;
                }

                throw new InvalidDataException();
            }

            private static void WriteVarint64 (Stream stream, ulong value) {
                for (; value >= 0x80u; value >>= 7)
                    stream.WriteByte((byte) (value | 0x80u));

                stream.WriteByte((byte) value);
            }
        }
    }
}
