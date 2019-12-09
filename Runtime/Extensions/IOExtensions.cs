using System;
using System.IO;
using System.Collections.Generic;

namespace UnityExtensions
{
    public static class IOExtensions
    {
        public delegate bool TryParseFunc<T>(string text, out T value);

        struct BinaryReaderWriter<T>
        {
            public static Func<BinaryReader, T> read;
            public static Action<BinaryWriter, T> write;
        }

        struct TextReaderWriter<T>
        {
            public static Action<TextWriter, T> write;
            public static TryParseFunc<T> tryParse;
        }

        static IOExtensions()
        {
            BinaryReaderWriter<int>.read = r => r.ReadInt32();
            BinaryReaderWriter<int>.write = (w, v) => w.Write(v);
            TextReaderWriter<int>.write = (w, v) => w.Write(v);
            TextReaderWriter<int>.tryParse = int.TryParse;

            BinaryReaderWriter<float>.read = r => r.ReadSingle();
            BinaryReaderWriter<float>.write = (w, v) => w.Write(v);
            TextReaderWriter<float>.write = (w, v) => w.Write(v);
            TextReaderWriter<float>.tryParse = float.TryParse;

            BinaryReaderWriter<ulong>.read = r => r.ReadUInt64();
            BinaryReaderWriter<ulong>.write = (w, v) => w.Write(v);
            TextReaderWriter<ulong>.write = (w, v) => w.Write(v);
            TextReaderWriter<ulong>.tryParse = ulong.TryParse;

            BinaryReaderWriter<uint>.read = r => r.ReadUInt32();
            BinaryReaderWriter<uint>.write = (w, v) => w.Write(v);
            TextReaderWriter<uint>.write = (w, v) => w.Write(v);
            TextReaderWriter<uint>.tryParse = uint.TryParse;

            BinaryReaderWriter<ushort>.read = r => r.ReadUInt16();
            BinaryReaderWriter<ushort>.write = (w, v) => w.Write(v);
            TextReaderWriter<ushort>.write = (w, v) => w.Write(v);
            TextReaderWriter<ushort>.tryParse = ushort.TryParse;

            BinaryReaderWriter<string>.read = r => r.ReadString();
            BinaryReaderWriter<string>.write = (w, v) => w.Write(v);
            TextReaderWriter<string>.write = (w, v) => w.Write(v);
            TextReaderWriter<string>.tryParse = (string t, out string v) => { v = t; return t != null; };

            BinaryReaderWriter<sbyte>.read = r => r.ReadSByte();
            BinaryReaderWriter<sbyte>.write = (w, v) => w.Write(v);
            TextReaderWriter<sbyte>.write = (w, v) => w.Write(v);
            TextReaderWriter<sbyte>.tryParse = sbyte.TryParse;

            BinaryReaderWriter<long>.read = r => r.ReadInt64();
            BinaryReaderWriter<long>.write = (w, v) => w.Write(v);
            TextReaderWriter<long>.write = (w, v) => w.Write(v);
            TextReaderWriter<long>.tryParse = long.TryParse;

            BinaryReaderWriter<DateTime>.read = r => DateTime.FromBinary(r.ReadInt64());
            BinaryReaderWriter<DateTime>.write = (w, v) => w.Write(v.ToBinary());
            TextReaderWriter<DateTime>.write = (w, v) => w.Write(v.ToLocalTime().ToString());
            TextReaderWriter<DateTime>.tryParse = DateTime.TryParse;

            BinaryReaderWriter<short>.read = r => r.ReadInt16();
            BinaryReaderWriter<short>.write = (w, v) => w.Write(v);
            TextReaderWriter<short>.write = (w, v) => w.Write(v);
            TextReaderWriter<short>.tryParse = short.TryParse;

            BinaryReaderWriter<decimal>.read = r => r.ReadDecimal();
            BinaryReaderWriter<decimal>.write = (w, v) => w.Write(v);
            TextReaderWriter<decimal>.write = (w, v) => w.Write(v);
            TextReaderWriter<decimal>.tryParse = decimal.TryParse;

            BinaryReaderWriter<byte>.read = r => r.ReadByte();
            BinaryReaderWriter<byte>.write = (w, v) => w.Write(v);
            TextReaderWriter<byte>.write = (w, v) => w.Write(v);
            TextReaderWriter<byte>.tryParse = byte.TryParse;

            BinaryReaderWriter<bool>.read = r => r.ReadBoolean();
            BinaryReaderWriter<bool>.write = (w, v) => w.Write(v);
            TextReaderWriter<bool>.write = (w, v) => w.Write(v);
            TextReaderWriter<bool>.tryParse = bool.TryParse;

            BinaryReaderWriter<double>.read = r => r.ReadDouble();
            BinaryReaderWriter<double>.write = (w, v) => w.Write(v);
            TextReaderWriter<double>.write = (w, v) => w.Write(v);
            TextReaderWriter<double>.tryParse = double.TryParse;

            BinaryReaderWriter<char>.read = r => r.ReadChar();
            BinaryReaderWriter<char>.write = (w, v) => w.Write(v);
            TextReaderWriter<char>.write = (w, v) => w.Write(v);
            TextReaderWriter<char>.tryParse = char.TryParse;
        }

        /// <summary>
        /// Register a custom type reading function to BinaryReader.
        /// </summary>
        public static void Register<T>(Func<BinaryReader, T> read)
        {
            BinaryReaderWriter<T>.read = read;
        }

        /// <summary>
        /// Register a custom type writing function to BinaryWriter.
        /// </summary>
        public static void Register<T>(Action<BinaryWriter, T> write)
        {
            BinaryReaderWriter<T>.write = write;
        }

        /// <summary>
        /// Register a custom type writing function to TextWriter.
        /// </summary>
        public static void Register<T>(Action<TextWriter, T> write)
        {
            TextReaderWriter<T>.write = write;
        }

        /// <summary>
        /// Register a custom type parsing function.
        /// </summary>
        public static void Register<T>(TryParseFunc<T> tryParse)
        {
            TextReaderWriter<T>.tryParse = tryParse;
        }

        /// <summary>
        /// Read a specific type data from the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// </summary>
        public static T Read<T>(this BinaryReader reader)
        {
            return BinaryReaderWriter<T>.read(reader);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T>(this BinaryWriter writer, T value)
        {
            BinaryReaderWriter<T>.write(writer, value);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// Note: DateTime is always converted to Local time when writing.
        /// </summary>
        public static void Write<T>(this TextWriter writer, T value)
        {
            TextReaderWriter<T>.write(writer, value);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// Note: DateTime is always converted to Local time when writing.
        /// </summary>
        public static void WriteLine<T>(this TextWriter writer, T value)
        {
            TextReaderWriter<T>.write(writer, value);
            writer.WriteLine();
        }

        /// <summary>
        /// Try parse a specific type data.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// </summary>
        public static bool TryParse<T>(this string text, out T value)
        {
            return TextReaderWriter<T>.tryParse(text, out value);
        }

        /// <summary>
        /// Read a specific type data list from the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// </summary>
        public static void Read<T>(this BinaryReader reader, IList<T> buffer, int index, int count)
        {
            while (count-- > 0)
            {
                buffer[index++] = BinaryReaderWriter<T>.read(reader);
            }
        }

        /// <summary>
        /// Write a specific type data list to the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T>(this BinaryWriter writer, IList<T> buffer, int index, int count)
        {
            while (count-- > 0)
            {
                BinaryReaderWriter<T>.write(writer, buffer[index++]);
            }
        }

        /// <summary>
        /// Write a specific type data list to the stream.
        /// Default support numeric types, DateTime and string, you can use Register to register custom types.
        /// Note: DateTime is always converted to Local time when writing.
        /// </summary>
        public static void Write<T>(this TextWriter writer, IList<T> buffer, string separator, int index, int count)
        {
            while (count-- > 0)
            {
                TextReaderWriter<T>.write(writer, buffer[index++]);
                if (count > 0) writer.Write(separator);
            }
        }

        /// <summary>
        /// Write an Int32 to the stream.
        /// </summary>
        public static void WriteBinary(this Stream stream, int value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
            stream.WriteByte((byte)((value >> 16) & 0xFF));
            stream.WriteByte((byte)((value >> 24) & 0xFF));
        }

        /// <summary>
        /// Read an Int32 from the stream.
        /// </summary>
        public static int ReadBinaryInt32(this Stream stream)
        {
            int b0 = stream.ReadByte();
            int b1 = stream.ReadByte();
            int b2 = stream.ReadByte();
            int b3 = stream.ReadByte();
            return b0 | (b1 << 8) | (b2 << 16) | (b3 << 24);
        }

    } // class IOExtensions

} // namespace UnityExtensions