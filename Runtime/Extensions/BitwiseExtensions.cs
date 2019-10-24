﻿
namespace UnityExtensions
{
    /// <summary>
    /// 位操作扩展方法
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 判断 value 是否包含 target 的所有非零二进制位
        /// </summary>
        public static bool ContainBits(this int value, int target) { return (value & target) == target; }


        /// <summary>
        /// 判断两个值是否在某些二进制位都是非零的
        /// </summary>
        public static bool IntersectBits(this int value1, int value2) { return (value1 & value2) != 0; }


        /// <summary>
        /// 从 value 中移除所有 target 的非零位
        /// </summary>
        public static void RemoveBits(this ref int value, int target) { value = value & ~target; }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~7
        /// </summary>
        public static void SetBit0(this ref sbyte value, int bit) { value = (sbyte)(value & (~(1 << bit))); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~7
        /// </summary>
        public static void SetBit0(this ref byte value, int bit) { value = (byte)(value & (~(1u << bit))); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~15
        /// </summary>
        public static void SetBit0(this ref short value, int bit) { value = (short)(value & (~(1 << bit))); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~15
        /// </summary>
        public static void SetBit0(this ref ushort value, int bit) { value = (ushort)(value & (~(1u << bit))); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~31
        /// </summary>
        public static void SetBit0(this ref int value, int bit) { value = value & (~(1 << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~31
        /// </summary>
        public static void SetBit0(this ref uint value, int bit) { value = value & (~(1u << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~63
        /// </summary>
        public static void SetBit0(this ref long value, int bit) { value = value & (~(1L << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 0, bit = 0~63
        /// </summary>
        public static void SetBit0(this ref ulong value, int bit) { value = value & (~(1UL << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~7
        /// </summary>
        public static void SetBit1(this ref sbyte value, int bit) { value = (sbyte)((byte)value | (1u << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~7
        /// </summary>
        public static void SetBit1(this ref byte value, int bit) { value = (byte)(value | (1u << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~15
        /// </summary>
        public static void SetBit1(this ref short value, int bit) { value = (short)((ushort)value | (1u << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~15
        /// </summary>
        public static void SetBit1(this ref ushort value, int bit) { value = (ushort)(value | (1u << bit)); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~31
        /// </summary>
        public static void SetBit1(this ref int value, int bit) { value = value | (1 << bit); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~31
        /// </summary>
        public static void SetBit1(this ref uint value, int bit) { value = value | (1u << bit); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~63
        /// </summary>
        public static void SetBit1(this ref long value, int bit) { value = value | (1L << bit); }


        /// <summary>
        /// 将指定二进制位设置为 1, bit = 0~63
        /// </summary>
        public static void SetBit1(this ref ulong value, int bit) { value = value | (1UL << bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~7
        /// </summary>
        public static void SetBit(this ref sbyte value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~7
        /// </summary>
        public static void SetBit(this ref byte value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~15
        /// </summary>
        public static void SetBit(this ref short value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~15
        /// </summary>
        public static void SetBit(this ref ushort value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~31
        /// </summary>
        public static void SetBit(this ref int value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~31
        /// </summary>
        public static void SetBit(this ref uint value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~63
        /// </summary>
        public static void SetBit(this ref long value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位设置为 1 或 0, bit = 0~63
        /// </summary>
        public static void SetBit(this ref ulong value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~7
        /// </summary>
        public static void ReverseBit(this ref sbyte value, int bit) { value = (sbyte)(value ^ (1 << bit)); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~7
        /// </summary>
        public static void ReverseBit(this ref byte value, int bit) { value = (byte)(value ^ (1u << bit)); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~15
        /// </summary>
        public static void ReverseBit(this ref short value, int bit) { value = (short)(value ^ (1 << bit)); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~15
        /// </summary>
        public static void ReverseBit(this ref ushort value, int bit) { value = (ushort)(value ^ (1u << bit)); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~31
        /// </summary>
        public static void ReverseBit(this ref int value, int bit) { value = value ^ (1 << bit); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~31
        /// </summary>
        public static void ReverseBit(this ref uint value, int bit) { value = value ^ (1u << bit); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~63
        /// </summary>
        public static void ReverseBit(this ref long value, int bit) { value = value ^ (1L << bit); }


        /// <summary>
        /// 将指定二进制位反转, bit = 0~63
        /// </summary>
        public static void ReverseBit(this ref ulong value, int bit) { value = value ^ (1UL << bit); }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~7
        /// </summary>
        public static bool GetBit(this sbyte value, int bit) { return (value & (1 << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~7
        /// </summary>
        public static bool GetBit(this byte value, int bit) { return (value & (1u << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~15
        /// </summary>
        public static bool GetBit(this short value, int bit) { return (value & (1 << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~15
        /// </summary>
        public static bool GetBit(this ushort value, int bit) { return (value & (1u << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~31
        /// </summary>
        public static bool GetBit(this int value, int bit) { return (value & (1 << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~31
        /// </summary>
        public static bool GetBit(this uint value, int bit) { return (value & (1u << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~63
        /// </summary>
        public static bool GetBit(this long value, int bit) { return (value & (1L << bit)) != 0; }


        /// <summary>
        /// 获得指定二进制位的值, bit = 0~63
        /// </summary>
        public static bool GetBit(this ulong value, int bit) { return (value & (1UL << bit)) != 0; }

    } // class Extensions

} // namespace UnityExtensions