using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 让 int 或 float 字段的值限制在指定范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class MinMaxAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public float min;
        public float max;
#endif

        /// <summary>
        /// 让 int 或 float 字段的值限制在指定范围
        /// </summary>
        public MinMaxAttribute(float min, float max)
        {
#if UNITY_EDITOR
            this.min = min;
            this.max = max;
#endif
        }

    } // class MinMaxAttribute

} // namespace UnityExtensions