using System;
using System.IO;
using System.Collections.Generic;

namespace UnityExtensions
{
    public static class ReaderWriterExtensions
    {
        public delegate bool TryParseFunc<T>(string text, out T value);

        struct ReaderWriter<T>
        {
            public static Func<BinaryReader, T> read;
            public static Action<BinaryWriter, T> write;
            public static Action<TextWriter, T> writeText;
            public static TryParseFunc<T> tryParse;
        }

        static ReaderWriterExtensions()
        {
            ReaderWriter<int>.read = r => r.ReadInt32();
            ReaderWriter<int>.write = (w, v) => w.Write(v);
            ReaderWriter<int>.writeText = (w, v) => w.Write(v);
            ReaderWriter<int>.tryParse = int.TryParse;

            ReaderWriter<float>.read = r => r.ReadSingle();
            ReaderWriter<float>.write = (w, v) => w.Write(v);
            ReaderWriter<float>.writeText = (w, v) => w.Write(v);
            ReaderWriter<float>.tryParse = float.TryParse;

            ReaderWriter<ulong>.read = r => r.ReadUInt64();
            ReaderWriter<ulong>.write = (w, v) => w.Write(v);
            ReaderWriter<ulong>.writeText = (w, v) => w.Write(v);
            ReaderWriter<ulong>.tryParse = ulong.TryParse;

            ReaderWriter<uint>.read = r => r.ReadUInt32();
            ReaderWriter<uint>.write = (w, v) => w.Write(v);
            ReaderWriter<uint>.writeText = (w, v) => w.Write(v);
            ReaderWriter<uint>.tryParse = uint.TryParse;

            ReaderWriter<ushort>.read = r => r.ReadUInt16();
            ReaderWriter<ushort>.write = (w, v) => w.Write(v);
            ReaderWriter<ushort>.writeText = (w, v) => w.Write(v);
            ReaderWriter<ushort>.tryParse = ushort.TryParse;

            ReaderWriter<string>.read = r => r.ReadString();
            ReaderWriter<string>.write = (w, v) => w.Write(v);
            ReaderWriter<string>.writeText = (w, v) => w.Write(v);
            ReaderWriter<string>.tryParse = (string t, out string v) => { v = t; return t != null; };

            ReaderWriter<sbyte>.read = r => r.ReadSByte();
            ReaderWriter<sbyte>.write = (w, v) => w.Write(v);
            ReaderWriter<sbyte>.writeText = (w, v) => w.Write(v);
            ReaderWriter<sbyte>.tryParse = sbyte.TryParse;

            ReaderWriter<long>.read = r => r.ReadInt64();
            ReaderWriter<long>.write = (w, v) => w.Write(v);
            ReaderWriter<long>.writeText = (w, v) => w.Write(v);
            ReaderWriter<long>.tryParse = long.TryParse;

            ReaderWriter<short>.read = r => r.ReadInt16();
            ReaderWriter<short>.write = (w, v) => w.Write(v);
            ReaderWriter<short>.writeText = (w, v) => w.Write(v);
            ReaderWriter<short>.tryParse = short.TryParse;

            ReaderWriter<decimal>.read = r => r.ReadDecimal();
            ReaderWriter<decimal>.write = (w, v) => w.Write(v);
            ReaderWriter<decimal>.writeText = (w, v) => w.Write(v);
            ReaderWriter<decimal>.tryParse = decimal.TryParse;

            ReaderWriter<byte>.read = r => r.ReadByte();
            ReaderWriter<byte>.write = (w, v) => w.Write(v);
            ReaderWriter<byte>.writeText = (w, v) => w.Write(v);
            ReaderWriter<byte>.tryParse = byte.TryParse;

            ReaderWriter<bool>.read = r => r.ReadBoolean();
            ReaderWriter<bool>.write = (w, v) => w.Write(v);
            ReaderWriter<bool>.writeText = (w, v) => w.Write(v);
            ReaderWriter<bool>.tryParse = bool.TryParse;

            ReaderWriter<double>.read = r => r.ReadDouble();
            ReaderWriter<double>.write = (w, v) => w.Write(v);
            ReaderWriter<double>.writeText = (w, v) => w.Write(v);
            ReaderWriter<double>.tryParse = double.TryParse;

            ReaderWriter<char>.read = r => r.ReadChar();
            ReaderWriter<char>.write = (w, v) => w.Write(v);
            ReaderWriter<char>.writeText = (w, v) => w.Write(v);
            ReaderWriter<char>.tryParse = char.TryParse;
        }

        /// <summary>
        /// Register a custom type reading function to BinaryReader.
        /// </summary>
        public static void Register<T>(Func<BinaryReader, T> read)
        {
            ReaderWriter<T>.read = read;
        }

        /// <summary>
        /// Register a custom type writing function to BinaryWriter.
        /// </summary>
        public static void Register<T>(Action<BinaryWriter, T> write)
        {
            ReaderWriter<T>.write = write;
        }

        /// <summary>
        /// Register a custom type writing function to TextWriter.
        /// </summary>
        public static void Register<T>(Action<TextWriter, T> write)
        {
            ReaderWriter<T>.writeText = write;
        }

        /// <summary>
        /// Register a custom type parsing function.
        /// </summary>
        public static void Register<T>(TryParseFunc<T> tryParse)
        {
            ReaderWriter<T>.tryParse = tryParse;
        }

        /// <summary>
        /// Read a specific type data from the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static T Read<T>(this BinaryReader reader)
        {
            return ReaderWriter<T>.read(reader);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T>(this BinaryWriter writer, T value)
        {
            ReaderWriter<T>.write(writer, value);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T>(this TextWriter writer, T value)
        {
            ReaderWriter<T>.writeText(writer, value);
        }

        /// <summary>
        /// Write a specific type data to the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void WriteLine<T>(this TextWriter writer, T value)
        {
            ReaderWriter<T>.writeText(writer, value);
            writer.WriteLine();
        }

        /// <summary>
        /// Try parse a specific type data.
        /// </summary>
        public static bool TryParse<T>(this string text, out T value)
        {
            return ReaderWriter<T>.tryParse(text, out value);
        }

        /// <summary>
        /// Read a specific type data list from the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void Read<T, TList>(this BinaryReader reader, TList buffer, int index, int count) where TList : IList<T>
        {
            while (count-- > 0)
            {
                buffer[index++] = ReaderWriter<T>.read(reader);
            }
        }

        /// <summary>
        /// Write a specific type data list to the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T, TList>(this BinaryWriter writer, TList buffer, int index, int count) where TList : IList<T>
        {
            while (count-- > 0)
            {
                ReaderWriter<T>.write(writer, buffer[index++]);
            }
        }

        /// <summary>
        /// Write a specific type data list to the stream.
        /// Default support numeric types and string, you can use Register to register custom types.
        /// </summary>
        public static void Write<T, TList>(this TextWriter writer, TList buffer, string separator, int index, int count) where TList : IList<T>
        {
            while (count-- > 0)
            {
                ReaderWriter<T>.writeText(writer, buffer[index++]);
                if (count > 0) writer.Write(separator);
            }
        }

    } // class ReaderWriterExtensions

} // namespace UnityExtensions