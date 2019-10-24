using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 让 int 或 float 字段的值限制在指定范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class MaxAttribute : PropertyAttribute
    {
        public float max;

        /// <summary>
        /// 让 int 或 float 字段的值限制在指定范围
        /// </summary>
        public MaxAttribute(float max)
        {
            this.max = max;
        }

    } // class MaxAttribute

} // namespace UnityExtensions