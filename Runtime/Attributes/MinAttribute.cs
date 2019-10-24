using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 让 int 或 float 字段的值限制在指定范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class MinAttribute : PropertyAttribute
    {
        public float min;

        /// <summary>
        /// 让 int 或 float 字段的值限制在指定范围
        /// </summary>
        public MinAttribute(float min)
        {
            this.min = min;
        }

    } // class MinAttribute

} // namespace UnityExtensions