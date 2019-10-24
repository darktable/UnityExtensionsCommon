using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 在字段之上绘制一条横线
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class LineAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public float spaceBefore;
        public float spaceAfter;
        public float height;
#endif

        public LineAttribute(float spaceBefore = 6, float spaceAfter = 8, float height = 1)
        {
#if UNITY_EDITOR
            this.spaceBefore = spaceBefore;
            this.spaceAfter = spaceAfter;
            this.height = height;
#endif
        }

    } // class LineAttribute

} // namespace UnityExtensions