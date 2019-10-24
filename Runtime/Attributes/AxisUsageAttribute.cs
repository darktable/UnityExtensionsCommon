using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// Axis 的使用方式
    /// </summary>
    public enum AxisUsage
    {
        Direction6 = 0,
        Direction6Mask = 1,
        Axis3 = 2,
        Axis3Mask = 3,
        Plane3 = 4,
    }


    /// <summary>
    /// 指定 Axis 用途
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AxisUsageAttribute : PropertyAttribute
    {
        public AxisUsage usage;

        /// <summary>
        /// 指定 Axis 用途
        /// </summary>
        public AxisUsageAttribute(AxisUsage usage)
        {
            this.usage = usage;
        }

    } // class AxisUsageAttribute

} // namespace UnityExtensions